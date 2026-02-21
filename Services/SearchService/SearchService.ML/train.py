import os
import psycopg2
import torch
import shutil
from datetime import datetime
from sentence_transformers import SentenceTransformer, InputExample, losses
from torch.utils.data import DataLoader, random_split
from sentence_transformers.evaluation import EmbeddingSimilarityEvaluator
import requests

# ==========================
# CONFIG
# ==========================

BASE_MODEL = "sentence-transformers/all-mpnet-base-v2"

DESKTOP_MODELS_DIR = os.path.expanduser("~/Desktop/ml/volume/models")
VERSION = datetime.now().strftime("mpnet-v%Y%m%d-%H%M%S")
OUTPUT_DIR = os.path.join(DESKTOP_MODELS_DIR, VERSION)

DB_CONFIG = {
    "host": "postgres",
    "database": "searchdb",
    "user": "searchuser",
    "password": "searchpass"
}

BATCH_SIZE = 32
EPOCHS = 1
LR = 2e-5

# ==========================
# DATASET EXPORTER
# ==========================

def load_training_data():
    conn = psycopg2.connect(**DB_CONFIG)
    cur = conn.cursor()

    query = """
        SELECT s.query,
               i.documentid,
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

# ==========================
# FETCH DOCUMENT TEXT
# ==========================

def fetch_document_text(document_id):
    # Запрос к Elasticsearch
    es = requests.get(
        f"http://elasticsearch:9200/vacancies/_doc/{document_id}"
    )
    if es.status_code != 200:
        return None

    source = es.json()["_source"]
    return source.get("post", "") + " " + source.get("description", "")

# ==========================
# BUILD TRIPLET DATASET
# ==========================

def build_triplets(rows):
    positives = []
    negatives = []

    for row in rows:
        query, doc_id, clicked, position, dwell = row

        text = fetch_document_text(doc_id)
        if not text:
            continue

        if clicked and dwell and dwell > 5000:
            positives.append((query, text))
        else:
            negatives.append((query, text))

    triplets = []

    for anchor, positive in positives:
        for neg_query, neg_text in negatives[:3]:
            if anchor == neg_query:
                triplets.append(
                    InputExample(texts=[anchor, positive, neg_text])
                )

    return triplets

# ==========================
# TRAIN
# ==========================

def train():
    print("Loading model...")
    model = SentenceTransformer(BASE_MODEL)

    print("Loading data...")
    rows = load_training_data()
    triplets = build_triplets(rows)

    print(f"Triplets: {len(triplets)}")

    dataset = DataLoader(triplets, shuffle=True, batch_size=BATCH_SIZE)

    train_size = int(0.9 * len(triplets))
    val_size = len(triplets) - train_size

    train_data, val_data = random_split(triplets, [train_size, val_size])

    train_loader = DataLoader(train_data, shuffle=True, batch_size=BATCH_SIZE)

    loss = losses.TripletLoss(model)

    model.fit(
        train_objectives=[(train_loader, loss)],
        epochs=EPOCHS,
        optimizer_params={"lr": LR},
        use_amp=True,  # mixed precision
        show_progress_bar=True
    )

    print("Saving model...")
    model.save(OUTPUT_DIR)

    update_symlink()
    reindex_elasticsearch()

# ==========================
# SYMLINK SWITCH
# ==========================

def update_symlink():
    current_path = os.path.join(DESKTOP_MODELS_DIR, "current")

    if os.path.exists(current_path):
        os.remove(current_path)

    os.symlink(OUTPUT_DIR, current_path)

    print("Symlink updated to new model")

# ==========================
# REINDEX ELASTICSEARCH
# ==========================

def reindex_elasticsearch():
    print("Triggering reindex...")

    requests.post("http://localhost:5006/api/reindex")

# ==========================

if __name__ == "__main__":
    train()