from huggingface_hub import snapshot_download
import os


BASE_MODEL_DIR = os.path.expanduser("~/Desktop/ml/volume/models/base_mpnet")
MODEL_NAME = "sentence-transformers/all-mpnet-base-v2"


def download_model():
    os.makedirs(BASE_MODEL_DIR, exist_ok=True)

    snapshot_download(
        repo_id=MODEL_NAME,
        local_dir=BASE_MODEL_DIR,
        local_dir_use_symlinks=False
    )

    print(f"Model downloaded to {BASE_MODEL_DIR}")


if __name__ == "__main__":
    download_model()