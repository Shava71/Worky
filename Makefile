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
	@echo "🔄 resumes recreated"

recreate-vacancies: delete-vacancies create-vacancies
	@echo "🔄 vacancies recreated"

recreate-all: delete-resumes delete-vacancies create-resumes create-vacancies
	@echo "🚀🔥 All indices recreated"


# Проверка
check:
	@echo "📌 Checking indices..."
	curl "$(ES_URL)/_cat/indices?v"