from huggingface_hub import snapshot_download
import os
import shutil


MODELS_DIR = os.path.expanduser("~/Desktop/ml/volume/models")
BASE_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet")
CLEAN_MODEL_DIR = os.path.join(MODELS_DIR, "base_mpnet_clean")
MODEL_NAME = "sentence-transformers/all-mpnet-base-v2"


def download_model():
    os.makedirs(CLEAN_MODEL_DIR, exist_ok=True)

    snapshot_download(
        repo_id=MODEL_NAME,
        local_dir=CLEAN_MODEL_DIR,
        local_dir_use_symlinks=False
    )

    if not os.path.exists(BASE_MODEL_DIR):
        shutil.copytree(CLEAN_MODEL_DIR, BASE_MODEL_DIR)
        print(f"Runtime model initialized at {BASE_MODEL_DIR}")

    print(f"Clean base model downloaded to {CLEAN_MODEL_DIR}")


if __name__ == "__main__":
    download_model()
