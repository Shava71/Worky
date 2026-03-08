# Адрес Elasticsearch
ES_URL=http://localhost:9200

# Файлы маппинга
RESUMES_MAPPING= 	./elasticmapping/resume-mapping.json
VACANCIES_MAPPING=	./elasticmapping/vacancy-mapping.json

ELASTIC_USER=elastic
ELASTIC_PASS=elasticpass

KIBANA_USER=kibana_system
KIBANA_PASS=kibanapass

# Установит пароль для Kibana
set-kibana-pass:
	@docker exec -it elasticsearch bin/elasticsearch-reset-password \
  	-u kibana_system \
  	-i

# Создание маппингов
create-resumes:
	@echo "Creating index 'resumes'..."
	@curl -u $(ELASTIC_USER):$(ELASTIC_PASS) -X PUT "http://localhost:9200/resumes" \
		-H "Content-Type: application/json" \
		-d @./elasticmapping/resume-mapping.json && echo "resumes created successfully"

create-vacancies:
	@echo "Creating index 'vacancies'..."
	@curl -u $(ELASTIC_USER):$(ELASTIC_PASS) -X PUT "http://localhost:9200/vacancies" \
		-H "Content-Type: application/json" \
		-d @./elasticmapping/vacancy-mapping.json && echo "vacancies created successfully"

create-mapping: create-resumes create-vacancies

load-vacancies:
	./data/load-vacancies.sh $(JWT)

# Удаление индексов
delete-resumes:
	@echo "Deleting index 'resumes'..."
	curl -X DELETE "$(ES_URL)/resumes"
	@echo "\n resumes deleted"

delete-vacancies:
	@echo "Deleting index 'vacancies'..."
	curl -X DELETE "$(ES_URL)/vacancies"
	@echo "\n vacancies deleted"


# Полный recreate
recreate-resumes: delete-resumes create-resumes
	@echo "resumes recreated"

recreate-vacancies: delete-vacancies create-vacancies
	@echo "vacancies recreated"

recreate-all: delete-resumes delete-vacancies create-resumes create-vacancies
	@echo "All indices recreated"


# Проверка
check:
	@echo "📌 Checking indices..."
	curl "$(ES_URL)/_cat/indices?v"
	
load-test:
	k6 run searchvacancy_test_k6.js

# ================================
# CONFIG
# ================================

MODEL_NAME=sentence-transformers/all-mpnet-base-v2
BASE_DIR=$(HOME)/Desktop/ml/volume/models
BASE_MODEL_DIR=$(BASE_DIR)/base_mpnet
CURRENT_LINK=$(BASE_DIR)/current

PYTHON=python3

# ================================
# HELP
# ================================

help:
	@echo "Available commands:"
	@echo "  make init-model      -> Download base MPNet model"
	@echo "  make switch-base     -> Point current -> base_mpnet"
	@echo "  make reset-current   -> Recreate current symlink"
	@echo "  make clean-models    -> Remove all models"

# ================================
# 1️⃣ DOWNLOAD BASE MODEL
# ================================

init-model:
	@echo "Creating model directory..."
	mkdir -p $(BASE_MODEL_DIR)

	@echo "Downloading model from HuggingFace..."
	python3 Services/SearchService/SearchService.ML/download_model.py

	@echo "Done."

# ================================
# 2️⃣ SWITCH CURRENT -> BASE
# ================================

switch-base:
	@echo "Updating current symlink..."
	rm -rf $(CURRENT_LINK)
	ln -s $(BASE_MODEL_DIR) $(CURRENT_LINK)
	@echo "current -> base_mpnet"

switch-base-win:
	powershell -Command " \
	if (Test-Path '$(CURRENT_LINK)') { Remove-Item '$(CURRENT_LINK)' -Force } ; \
	New-Item -ItemType SymbolicLink \
	-Path '$(CURRENT_LINK)' \
	-Target '$(BASE_MODEL_DIR)' \
	"

# ================================
# 3️⃣ RESET CURRENT (if broken)
# ================================

reset-current:
	rm -rf $(CURRENT_LINK)
	ln -s $(BASE_MODEL_DIR) $(CURRENT_LINK)

# ================================
# 4️⃣ CLEAN MODELS
# ================================

clean-models:
	rm -rf $(BASE_DIR)
	@echo "All models removed."

start-python:
	source .venv/bin/activate
	
build-before-compose:
	docker compose build apigateway
	docker compose build authservice
	docker compose build companyservice
	docker compose build feedbackservice
	docker compose build filterservice
	docker compose build searchservice
	docker compose build workerservice