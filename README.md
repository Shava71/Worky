# Worky

## Стэк технологий:
- ASP.NET
- React
- PostgreSQL
- Redis
- S3 minio
- Kafka + Zookeeper
- ElasticSearch + Kibana

## Иструкция по запуску
1. Клонирование репозитория:
```git clone https://github.com/Shava71/Worky.git```;
2. Загрузка **ML-NET** языковой модели: ```make init-model```;
3. Разворачивание **docker-compose**: ```docker compose up -d```;
4. Загрузка маппингов для **ElasticSearch*: ```make create-mapping```.

