import os
import psycopg2
import torch
import random
from datetime import datetime
from typing import List, Tuple

import requests
from sentence_transformers import SentenceTransformer, InputExample, losses
from sentence_transformers.evaluation import TripletEvaluator
from torch.utils.data import DataLoader, random_split


# CONFIG
BASE_MODEL = "sentence-transformers/all-mpnet-base-v2"

MODELS_DIR = os.path.expanduser("~/Desktop/ml/volume/models")
VERSION = datetime.now().strftime("mpnet-v%Y%m%d-%H%M%S")
OUTPUT_DIR = os.path.join(MODELS_DIR, VERSION)

ELASTIC_URL = "http://elasticsearch:9200"

INDICES = {
    "vacancy": "vacancies",
    "resume": "resumes"
}

DB_CONFIG = {
    "host": "postgres",
    "database": "searchdb",
    "user": "searchuser",
    "password": "searchpass"
}

BATCH_SIZE = 32
EPOCHS = 1
LR = 2e-5
VALIDATION_SPLIT = 0.1
NEGATIVES_PER_POSITIVE = 3

MIN_DWELL_MS = 5000


# DATABASE LOADER
def load_training_data():
    """
    Загружаем:
    - query
    - documentid
    - documenttype (vacancy / resume)
    - clicked
    - position
    - dwelltimems
    """

    conn = psycopg2.connect(**DB_CONFIG)
    cur = conn.cursor()

    query = """
        SELECT s.query,
               i.documentid,
               i.documenttype,
               i.clicked,
               i.position,
               i.dwelltimems
        FROM searchimpressions i
        JOIN searchsessions s ON s.id = i.sessionid
        WHERE s.query IS NOT NULL
    """

    cur.execute(query)
    rows = cur.fetchall()

    cur.close()
    conn.close()

    return rows



# ELASTIC FETCHER
def fetch_document_text(document_id: str, document_type: str) -> str | None:
    """
    Универсальный fetch для vacancy и resume
    """

    index = INDICES.get(document_type)
    if not index:
        return None

    response = requests.get(
        f"{ELASTIC_URL}/{index}/_doc/{document_id}"
    )

    if response.status_code != 200:
        return None

    source = response.json().get("_source", {})

    if document_type == "vacancy":
        return f"{source.get('post', '')} {source.get('description', '')}"

    if document_type == "resume":
        return f"{source.get('post', '')} {source.get('skill', '')}"

    return None


# TRIPLET BUILDER
def build_triplets(rows) -> List[InputExample]:
    """
    Строим triplet:
    anchor = query
    positive = clicked + dwell > threshold
    negative = не кликнуто или маленький dwell
    """

    positives: List[Tuple[str, str]] = []
    negatives: List[Tuple[str, str]] = []

    print("Building positives/negatives...")

    for row in rows:
        query, doc_id, doc_type, clicked, position, dwell = row

        text = fetch_document_text(str(doc_id), doc_type)

        if not text:
            continue

        if clicked and dwell and dwell > MIN_DWELL_MS:
            positives.append((query, text))
        else:
            negatives.append((query, text))

    print(f"Positives: {len(positives)}")
    print(f"Negatives: {len(negatives)}")

    if not positives or not negatives:
        return []

    triplets: List[InputExample] = []

    for anchor_query, positive_text in positives:

        # выбираем случайные negative
        sampled_negatives = random.sample(
            negatives,
            min(NEGATIVES_PER_POSITIVE, len(negatives))
        )

        for neg_query, neg_text in sampled_negatives:
            if anchor_query == neg_query:
                triplets.append(
                    InputExample(texts=[
                        anchor_query,
                        positive_text,
                        neg_text
                    ])
                )

    return triplets


# TRAIN
def train():
    print("Loading base model...")
    model = SentenceTransformer(BASE_MODEL)

    print("Loading training data...")
    rows = load_training_data()

    if not rows:
        print("No training data found.")
        return

    triplets = build_triplets(rows)

    if len(triplets) == 0:
        print("No triplets generated.")
        return

    print(f"Total triplets: {len(triplets)}")

    train_size = int((1 - VALIDATION_SPLIT) * len(triplets))
    val_size = len(triplets) - train_size

    train_data, val_data = random_split(triplets, [train_size, val_size])

    train_loader = DataLoader(
        train_data,
        shuffle=True,
        batch_size=BATCH_SIZE
    )

    val_loader = DataLoader(
        val_data,
        shuffle=False,
        batch_size=BATCH_SIZE
    )

    train_loss = losses.TripletLoss(model)

    print("Training started...")

    model.fit(
        train_objectives=[(train_loader, train_loss)],
        epochs=EPOCHS,
        optimizer_params={"lr": LR},
        use_amp=True,
        show_progress_bar=True
    )

    print("Saving model...")
    os.makedirs(MODELS_DIR, exist_ok=True)
    model.save(OUTPUT_DIR)

    update_symlink()
    reindex_elasticsearch()

    print("Training complete.")



# SYMLINK UPDATE
def update_symlink():
    current_path = os.path.join(MODELS_DIR, "current")

    if os.path.islink(current_path) or os.path.exists(current_path):
        os.remove(current_path)

    os.symlink(OUTPUT_DIR, current_path)

    print(f"Symlink updated → {OUTPUT_DIR}")


# REINDEX BOTH INDICES
def reindex_elasticsearch():
    print("Triggering reindex for vacancies and resumes...")

    requests.post("http://localhost:5006/api/reindex/vacancies")
    requests.post("http://localhost:5006/api/reindex/resumes")

    print("Reindex triggered.")



if __name__ == "__main__":
    train()