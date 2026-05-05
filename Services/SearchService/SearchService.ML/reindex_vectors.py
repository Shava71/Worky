import argparse
import json
import os
import re
from typing import Dict, Iterable, List, Tuple

import requests
from requests.auth import HTTPBasicAuth
from sentence_transformers import SentenceTransformer


BASE_MODEL_NAME = "sentence-transformers/all-mpnet-base-v2"
MODELS_DIR = os.path.expanduser("~/Desktop/ml/volume/models")
BASE_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet")

ELASTIC_URL = "http://localhost:9200"
ELASTIC_USER = "elastic"
ELASTIC_PASSWORD = "elasticpass"

SCROLL_TTL = "2m"
DEFAULT_BATCH_SIZE = 100

INDEX_ALIASES = {
    "vacancies": "vacancies",
    "vacancy": "vacancies",
    "resumes": "resumes",
    "resume": "resumes",
}


def get_model() -> SentenceTransformer:
    model_source = BASE_MODEL_DIR if os.path.exists(BASE_MODEL_DIR) else BASE_MODEL_NAME
    print(f"Loading embedding model from: {model_source}")
    return SentenceTransformer(model_source)


def get_auth() -> HTTPBasicAuth:
    return HTTPBasicAuth(ELASTIC_USER, ELASTIC_PASSWORD)


def normalize_search_text(text: str) -> str:
    if not text:
        return ""

    value = text.strip().lower().replace("ё", "е")

    replacements = [
        (r"\bc#\b", "c# csharp dotnet asp.net aspnet"),
        (r"\basp\.net core\b", "asp.net core aspnet core asp.net aspnet dotnet csharp"),
        (r"\basp\.net\b", "asp.net aspnet dotnet csharp"),
        (r"(?<!asp)\.net\b", ".net dotnet csharp"),
        (r"\bстажер\b", "стажер стажировка junior internship trainee"),
        (r"\bстажировка\b", "стажировка стажер junior internship trainee"),
        (r"\bjunior\b", "junior стажер стажировка trainee"),
    ]

    for pattern, replacement in replacements:
        value = re.sub(pattern, replacement, value, flags=re.IGNORECASE)

    return re.sub(r"\s+", " ", value).strip()


def build_text(index_name: str, source: Dict) -> str:
    if index_name == "vacancies":
        post = source.get("post", "") or ""
        description = source.get("description", "") or ""
        text = f"{post} {description}".strip()
        return normalize_search_text(text or post)

    if index_name == "resumes":
        post = source.get("post", "") or ""
        skill = source.get("skill", "") or ""
        text = f"{post} {skill}".strip()
        return normalize_search_text(text or post)

    raise ValueError(f"Unsupported index: {index_name}")


def iter_documents(index_name: str, batch_size: int) -> Iterable[List[Dict]]:
    search_url = f"{ELASTIC_URL}/{index_name}/_search?scroll={SCROLL_TTL}"
    payload = {
        "size": batch_size,
        "query": {"match_all": {}}
    }

    response = requests.post(
        search_url,
        auth=get_auth(),
        json=payload,
        timeout=60,
        proxies={"http": None, "https": None},
    )
    response.raise_for_status()
    data = response.json()

    scroll_id = data.get("_scroll_id")
    hits = data.get("hits", {}).get("hits", [])

    try:
        while hits:
            yield hits

            scroll_response = requests.post(
                f"{ELASTIC_URL}/_search/scroll",
                auth=get_auth(),
                json={"scroll": SCROLL_TTL, "scroll_id": scroll_id},
                timeout=60,
                proxies={"http": None, "https": None},
            )
            scroll_response.raise_for_status()
            scroll_data = scroll_response.json()

            scroll_id = scroll_data.get("_scroll_id")
            hits = scroll_data.get("hits", {}).get("hits", [])
    finally:
        if scroll_id:
            requests.delete(
                f"{ELASTIC_URL}/_search/scroll",
                auth=get_auth(),
                json={"scroll_id": [scroll_id]},
                timeout=30,
                proxies={"http": None, "https": None},
            )


def encode_texts(model: SentenceTransformer, items: List[Tuple[str, Dict]], index_name: str) -> List[Dict]:
    texts = [build_text(index_name, source) for _, source in items]
    embeddings = model.encode(
        texts,
        batch_size=len(texts),
        show_progress_bar=False,
        convert_to_numpy=True,
    )

    documents = []
    for (doc_id, source), embedding in zip(items, embeddings):
        updated_source = dict(source)
        updated_source["vector"] = [float(x) for x in embedding.tolist()]
        documents.append({"id": doc_id, "source": updated_source})

    return documents


def bulk_index(index_name: str, documents: List[Dict]) -> None:
    lines = []
    for doc in documents:
        lines.append(json.dumps({"index": {"_index": index_name, "_id": doc["id"]}}, ensure_ascii=False))
        lines.append(json.dumps(doc["source"], ensure_ascii=False))

    payload = "\n".join(lines) + "\n"

    response = requests.post(
        f"{ELASTIC_URL}/_bulk",
        auth=get_auth(),
        data=payload.encode("utf-8"),
        headers={"Content-Type": "application/x-ndjson"},
        timeout=120,
        proxies={"http": None, "https": None},
    )
    response.raise_for_status()
    data = response.json()

    if data.get("errors"):
        raise RuntimeError(f"Bulk indexing failed for index '{index_name}'.")


def refresh_index(index_name: str) -> None:
    response = requests.post(
        f"{ELASTIC_URL}/{index_name}/_refresh",
        auth=get_auth(),
        timeout=30,
        proxies={"http": None, "https": None},
    )
    response.raise_for_status()


def reindex_index(index_name: str, model: SentenceTransformer, batch_size: int) -> None:
    total = 0

    print(f"Reindexing vectors in '{index_name}'...")

    for hits in iter_documents(index_name, batch_size):
        items = [(hit["_id"], hit["_source"]) for hit in hits]
        updated_documents = encode_texts(model, items, index_name)
        bulk_index(index_name, updated_documents)
        total += len(updated_documents)
        print(f"Updated {total} documents in '{index_name}'")

    refresh_index(index_name)
    print(f"Index '{index_name}' refreshed. Total updated: {total}")


def parse_args():
    parser = argparse.ArgumentParser(
        description="Recalculate embedding vectors for Elasticsearch indices."
    )
    parser.add_argument(
        "--index",
        default="all",
        choices=["all", "vacancies", "vacancy", "resumes", "resume"],
        help="Which index to reindex."
    )
    parser.add_argument(
        "--batch-size",
        type=int,
        default=DEFAULT_BATCH_SIZE,
        help="How many documents to process per batch."
    )
    return parser.parse_args()


def main():
    args = parse_args()
    model = get_model()

    if args.index == "all":
        targets = ["vacancies", "resumes"]
    else:
        targets = [INDEX_ALIASES[args.index]]

    for index_name in targets:
        reindex_index(index_name, model, args.batch_size)

    print("Vector reindex completed.")
    print("If the embeddings container is running, restart it to ensure query vectors use the same updated model.")


if __name__ == "__main__":
    main()
