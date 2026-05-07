import os
import shutil
from datetime import datetime
from pathlib import Path

from sentence_transformers import InputExample, SentenceTransformer, losses
from torch.utils.data import DataLoader


BASE_MODEL_NAME = "sentence-transformers/all-mpnet-base-v2"

MODELS_DIR = os.path.expanduser("~/Desktop/ml/volume/models")
BASE_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet")
CLEAN_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet_clean")
TMP_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet_tmp")
BACKUP_PREFIX = "base_mpnet_old_"

BATCH_SIZE = 8
EPOCHS = 4
LR = 1e-5
TRIPLET_MARGIN = 0.25
WARMUP_RATIO = 0.1
FORCE_CLEAN_BASE = os.getenv("FORCE_CLEAN_BASE", "0") == "1"


POSITIVE_PAIRS = [
    ("c#", "asp.net"),
    ("c# backend", "asp.net backend developer"),
    ("c# backend", "asp.net core backend developer"),
    ("c# backend", "asp.net backend developr"),
    ("csharp backend", "asp.net backend developer"),
    ("backend c#", "asp.net backend developer"),
    ("backend разработчик c#", "разработчик asp.net core"),
    ("c# web api", "asp.net core web api"),
    ("c# api developer", "asp.net api developer"),
    ("junior c# backend", "junior asp.net developer"),
    ("разработчик на c#", "asp.net engineer"),
    ("c# серверная разработка", "asp.net серверная разработка"),
    ("c# mvc developer", "asp.net mvc developer"),
    ("c# fullstack backend", "asp.net fullstack developer"),
    ("dotnet backend", "asp.net microservices"),
    ("backend .net", "asp.net core backend"),
    ("dotnet web developer", "asp.net web developer"),
    (".net backend", "asp.net core rest api"),
    ("c# microservices", "asp.net core microservices"),
    ("c# rest api", "asp.net rest api"),
    ("c# developer", "asp.net developer"),
    ("middle c# developer", "middle asp.net developer"),
    ("senior c# backend", "senior asp.net backend developer"),
    ("ооп c# backend", "asp.net core backend engineer"),
    ("asp.net", "c#"),
    ("asp.net backend", "c# backend developer"),
    ("asp.net backend developr", "c# backend"),
    ("asp.net core", "c# developer"),
    ("asp.net mvc", "c# mvc developer"),
    ("asp.net web api", "c# web api"),
    ("asp.net engineer", "разработчик на c#"),
    ("asp.net microservices", "c# microservices"),
    ("asp.net rest api", "c# rest api"),
    ("junior asp.net developer", "junior c# backend"),
    ("middle asp.net developer", "middle c# developer"),
    ("senior asp.net backend", "senior c# backend"),
    ("asp.net core backend", "backend .net"),
    ("asp.net backend developer", "c# backend"),
    ("asp.net fullstack", "c# fullstack backend"),
    ("asp.net серверная разработка", "c# серверная разработка"),
]

NEGATIVES = [
    "react frontend developer",
    "ui ux designer",
    "ios swift developer",
    "qa manual tester",
    "android kotlin developer",
    "1c бухгалтер",
    "product manager",
    "figma designer",
    "smm специалист",
    "hr manager",
    "php wordpress developer",
    "drupal developer",
    "digital marketing specialist",
    "network engineer cisco",
    "python django developer",
    "java spring backend",
    "data analyst sql",
    "sales manager b2b",
    "business analyst",
    "devops kubernetes aws",
]

HARD_NEGATIVES = [
    "c++ game developer",
    "c engineer",
    "embedded c developer",
    "draft backend developer",
    "java spring backend developer",
    "python django backend developer",
    "php laravel backend developer",
    "node.js backend developer",
    "golang backend developer",
    "drupal developer",
    "wordpress developer",
    "php cms developer",
]

CORE_BIDIRECTIONAL_PAIRS = [
    ("c#", "asp.net"),
    ("c# backend", "asp.net backend"),
    ("c# backend developer", "asp.net developer"),
    ("c# web api", "asp.net web api"),
    ("c# mvc", "asp.net mvc"),
    ("c# backend", "asp.net core backend"),
    ("backend разработчик c#", "разработчик asp.net core"),
    ("asp.net", "c#"),
    ("asp.net backend", "c# backend"),
    ("asp.net developer", "c# developer"),
    ("asp.net web api", "c# web api"),
    ("asp.net mvc", "c# mvc"),
    ("asp.net core backend", "c# backend"),
    ("разработчик asp.net core", "backend разработчик c#"),
]

STABILITY_PAIRS = [
    ("python backend developer", "django backend developer"),
    ("java backend developer", "spring backend developer"),
    ("react frontend developer", "frontend react engineer"),
    ("c++ game developer", "game developer c++"),
    ("php laravel backend", "laravel backend developer"),
]


def build_demo_triplets() -> list[InputExample]:
    triplets: list[InputExample] = []
    seen: set[tuple[str, str, str]] = set()

    def add_triplet(anchor: str, positive: str, negative: str) -> None:
        key = (anchor.strip(), positive.strip(), negative.strip())
        if key in seen or positive.strip() == negative.strip():
            return
        seen.add(key)
        triplets.append(InputExample(texts=[anchor, positive, negative]))

    for anchor, positive in CORE_BIDIRECTIONAL_PAIRS:
        for negative in HARD_NEGATIVES:
            add_triplet(anchor, positive, negative)

    for idx, (anchor, positive) in enumerate(POSITIVE_PAIRS):
        first_negative = NEGATIVES[idx % len(NEGATIVES)]
        second_negative = NEGATIVES[(idx + 7) % len(NEGATIVES)]
        hard_negative = HARD_NEGATIVES[idx % len(HARD_NEGATIVES)]

        add_triplet(anchor, positive, first_negative)
        add_triplet(anchor, positive, second_negative)
        add_triplet(anchor, positive, hard_negative)

    for idx, (anchor, positive) in enumerate(STABILITY_PAIRS):
        negative = NEGATIVES[idx % len(NEGATIVES)]
        add_triplet(anchor, positive, negative)

    return triplets


DEMO_TRIPLETS = build_demo_triplets()


def deploy_model(model: SentenceTransformer) -> None:
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")

    print("Saving trained model to temporary directory...")

    if os.path.exists(TMP_MODEL_DIR):
        shutil.rmtree(TMP_MODEL_DIR)

    model.save(TMP_MODEL_DIR)

    if os.path.exists(BASE_MODEL_DIR):
        backup_dir = os.path.join(MODELS_DIR, f"{BACKUP_PREFIX}{timestamp}")
        print(f"Backing up current model to: {backup_dir}")
        os.rename(BASE_MODEL_DIR, backup_dir)

    print("Promoting new demo model to base_mpnet...")
    os.rename(TMP_MODEL_DIR, BASE_MODEL_DIR)
    print("Demo model deployed.")


def resolve_model_source() -> str:
    if not FORCE_CLEAN_BASE and os.path.exists(BASE_MODEL_DIR):
        return BASE_MODEL_DIR

    if os.path.exists(CLEAN_MODEL_DIR):
        return CLEAN_MODEL_DIR

    return BASE_MODEL_NAME


def train_demo_model() -> None:
    model_source = resolve_model_source()
    print(f"Loading model from: {model_source}")
    model = SentenceTransformer(model_source)

    train_loader = DataLoader(
        DEMO_TRIPLETS,
        shuffle=True,
        batch_size=BATCH_SIZE
    )

    warmup_steps = max(1, int(len(train_loader) * EPOCHS * WARMUP_RATIO))
    train_loss = losses.TripletLoss(
        model=model,
        distance_metric=losses.TripletDistanceMetric.COSINE,
        triplet_margin=TRIPLET_MARGIN
    )

    print(f"Triplets count: {len(DEMO_TRIPLETS)}")
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

    os.makedirs(MODELS_DIR, exist_ok=True)
    deploy_model(model)

    print("Done.")
    print("Important: indexed document vectors in Elasticsearch are NOT regenerated by this script.")
    print("To see the effect in search, restart the embeddings container and reindex vacancies/resumes.")


if __name__ == "__main__":
    Path(MODELS_DIR).mkdir(parents=True, exist_ok=True)
    train_demo_model()
