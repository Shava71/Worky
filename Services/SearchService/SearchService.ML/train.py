import os
import shutil
import random
import psycopg2
import requests
from datetime import datetime
from typing import List, Tuple

from requests.auth import HTTPBasicAuth
from sentence_transformers import SentenceTransformer, InputExample, losses
from torch.utils.data import DataLoader, random_split


# ==============================
# CONFIG
# ==============================

BASE_MODEL_NAME = "sentence-transformers/all-mpnet-base-v2"

MODELS_DIR = os.path.expanduser("~/Desktop/ml/volume/models")
BASE_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet")
CLEAN_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet_clean")
TMP_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet_tmp")
BACKUP_PREFIX = "base_mpnet_old_"

ELASTIC_URL = "http://localhost:9200"
ELASTIC_USER = "elastic"
ELASTIC_PASSWORD = "elasticpass"

INDICES = {
    "vacancy": "vacancies",
    "resume": "resumes"
}

DB_CONFIG = {
    "host": "localhost",
    "port": "5440",
    "database": "searchdb",
    "user": "searchuser",
    "password": "searchpass"
}

BATCH_SIZE = 16
EPOCHS = 2
LR = 1e-5
VALIDATION_SPLIT = 0.1
NEGATIVES_PER_POSITIVE = 2
MIN_DWELL_MS = 5000
TRIPLET_MARGIN = 0.25
WARMUP_RATIO = 0.1
MIN_TRIPLETS_TO_TRAIN = 20
FORCE_CLEAN_BASE = os.getenv("FORCE_CLEAN_BASE", "0") == "1"


# ==============================
# DATABASE LOADER
# ==============================

def load_training_data():
    conn = psycopg2.connect(**DB_CONFIG)
    cur = conn.cursor()

    query = """
        SELECT s."Query",
               i."DocumentId",
               i."DocumentType",
               i."Clicked",
               i."Position",
               i."DwellTimeMs"
        FROM "SearchImpressions" i
        JOIN "SearchSessions" s ON s."Id" = i."SessionId"
        WHERE s."Query" IS NOT NULL AND s."Query" <> ''
    """

    cur.execute(query)
    rows = cur.fetchall()

    cur.close()
    conn.close()
    return rows


# ==============================
# FETCH DOCUMENT TEXT
# ==============================

def fetch_document_text(document_id: str, document_type: str) -> str | None:
    index = INDICES.get(document_type)
    if not index:
        print(f"[ERROR] Unknown document type: {document_type}")
        return None

    url = f"{ELASTIC_URL}/{index}/_doc/{document_id}"
    print(f"[REQUEST] {url}")

    try:
        response = requests.get(url, 
                                auth=HTTPBasicAuth(ELASTIC_USER, ELASTIC_PASSWORD),
                                proxies={"http": None, "https": None})
    except Exception as e:
        print(f"[ERROR] Request failed: {e}")
        return None

    print(f"[RESPONSE] status={response.status_code}")

    if response.status_code != 200:
        print(f"[ERROR] Document not found in ES: {document_id}")
        print(response.text)
        return None

    source = response.json().get("_source", {})

    if document_type == "vacancy":
        text = f"{source.get('post', '')} {source.get('description', '')}"

    elif document_type == "resume":
        text = f"{source.get('post', '')} {source.get('skill', '')}"

    else:
        return None

    print(f"[TEXT] length={len(text)}")

    return text


# ==============================
# BUILD TRIPLETS
# ==============================

def build_triplets(rows) -> List[InputExample]:
    """
    anchor   = query
    positive = clicked + dwell > MIN_DWELL_MS
    negative = не кликнуто или маленький dwell
    """

    positives: List[Tuple[str, str]] = []
    negatives_by_query = {}

    print("Building positives/negatives...")

    for row in rows:
        query, doc_id, doc_type, clicked, position, dwell = row

        text = fetch_document_text(str(doc_id), doc_type)
        if not text:
            print(f"[SKIP] Document not found or empty text: id={doc_id}, type={doc_type}")
            continue
        else:
            print(f"[OK] Fetched text for document: id={doc_id}, type={doc_type}, length={len(text)}")

        if clicked and dwell and dwell > MIN_DWELL_MS:
            positives.append((query, text))
            print(f"[POSITIVE] query='{query}', doc_id={doc_id}, dwell={dwell}")
        else:
            negatives_by_query.setdefault(query, []).append((position, text))
            print(f"[NEGATIVE] query='{query}', doc_id={doc_id}, clicked={clicked}, dwell={dwell}")

    print(f"Total positives: {len(positives)}")
    print(f"Total unique queries with negatives: {len(negatives_by_query)}")

    triplets: List[InputExample] = []
    seen = set()

    # открываем log-файл
    log_file_path = os.path.join(os.getcwd(), "triplets.log")
    with open(log_file_path, "w", encoding="utf-8") as log_f:

        for anchor_query, positive_text in positives:
            negatives = negatives_by_query.get(anchor_query, [])
            print(f"Processing anchor query='{anchor_query}', negatives found={len(negatives)}")

            # берём первые 5 по позиции
            negatives_sorted = sorted(negatives, key=lambda x: x[0])[:5]

            if not negatives_sorted:
                print(f"[SKIP ANCHOR] No negatives for query='{anchor_query}'")
                continue

            sampled = random.sample(
                negatives_sorted,
                min(NEGATIVES_PER_POSITIVE, len(negatives_sorted))
            )

            for _, neg_text in sampled:
                if not neg_text or neg_text == positive_text:
                    continue

                key = (anchor_query.strip(), positive_text.strip(), neg_text.strip())
                if key in seen:
                    continue

                seen.add(key)
                triplets.append(InputExample(texts=[anchor_query, positive_text, neg_text]))

                # записываем в log
                log_f.write("Anchor: {}\n".format(anchor_query))
                log_f.write("Positive: {}\n".format(positive_text))
                log_f.write("Negative: {}\n".format(neg_text))
                log_f.write("-" * 80 + "\n")

    print(f"Triplets built: {len(triplets)}")
    print(f"Triplets logged to {log_file_path}")

    return triplets


# ==============================
# DEPLOY MODEL (ATOMIC ROTATION)
# ==============================

def deploy_model(model: SentenceTransformer):
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")

    print("Saving trained model to temp directory...")

    if os.path.exists(TMP_MODEL_DIR):
        shutil.rmtree(TMP_MODEL_DIR)

    model.save(TMP_MODEL_DIR)

    if os.path.exists(BASE_MODEL_DIR):
        backup_dir = os.path.join(
            MODELS_DIR,
            f"{BACKUP_PREFIX}{timestamp}"
        )

        print(f"Backing up old model → {backup_dir}")
        os.rename(BASE_MODEL_DIR, backup_dir)

    print("Promoting temp model to base_mpnet...")
    os.rename(TMP_MODEL_DIR, BASE_MODEL_DIR)

    print("Model deployed successfully.")


# ==============================
# REINDEX
# ==============================

def reindex_elasticsearch():
    print("Triggering reindex for vacancies and resumes...")

    # requests.post("http://localhost:5006/api/reindex/vacancies")
    # requests.post("http://localhost:5006/api/reindex/resumes")

    print("Reindex triggered.")


def resolve_model_source() -> str:
    if not FORCE_CLEAN_BASE and os.path.exists(BASE_MODEL_DIR):
        return BASE_MODEL_DIR

    if os.path.exists(CLEAN_MODEL_DIR):
        return CLEAN_MODEL_DIR

    return BASE_MODEL_NAME


# ==============================
# TRAIN
# ==============================

def train():
    model_source = resolve_model_source()
    print(f"Loading base model from: {model_source}")
    model = SentenceTransformer(model_source)

    print("Loading training data...")
    rows = load_training_data()

    if not rows:
        print("No training data found.")
        return

    triplets = build_triplets(rows)

    if not triplets:
        print("No triplets generated.")
        return

    if len(triplets) < MIN_TRIPLETS_TO_TRAIN:
        print(
            f"Not enough triplets to train safely: {len(triplets)} < {MIN_TRIPLETS_TO_TRAIN}. "
            "Skipping deploy to avoid degrading semantic search."
        )
        return

    train_size = int((1 - VALIDATION_SPLIT) * len(triplets))
    val_size = len(triplets) - train_size

    train_data, _ = random_split(triplets, [train_size, val_size])

    train_loader = DataLoader(
        train_data,
        shuffle=True,
        batch_size=BATCH_SIZE
    )

    warmup_steps = max(1, int(len(train_loader) * EPOCHS * WARMUP_RATIO))
    train_loss = losses.TripletLoss(
        model=model,
        distance_metric=losses.TripletDistanceMetric.COSINE,
        triplet_margin=TRIPLET_MARGIN
    )

    print("Training started...")
    print(f"Triplets count: {len(triplets)}")
    print(f"Batch size: {BATCH_SIZE}")
    print(f"Epochs: {EPOCHS}")
    print(f"Learning rate: {LR}")
    print(f"Warmup steps: {warmup_steps}")
    print(f"Triplet margin: {TRIPLET_MARGIN}")

    model.fit(
        train_objectives=[(train_loader, train_loss)],
        epochs=EPOCHS,
        optimizer_params={"lr": LR},
        warmup_steps=warmup_steps,
        use_amp=True,
        show_progress_bar=True
    )

    print("Deploying model...")
    os.makedirs(MODELS_DIR, exist_ok=True)
    deploy_model(model)

    reindex_elasticsearch()

    print("Training complete.")


# ==============================

if __name__ == "__main__":
    train()
    
