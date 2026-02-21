import psycopg2
from elasticsearch import Elasticsearch
import json

es = Elasticsearch("http://localhost:9200",
                   basic_auth=("elastic", "elasticpass"))

conn = psycopg2.connect(
    dbname="searchdb",
    user="searchuser",
    password="searchpass",
    host="localhost",
    port=5440
)

cur = conn.cursor()

cur.execute("""
SELECT s.query,
       i.documentid,
       i.clicked,
       i.position,
       i.dwelltimems
FROM searchimpressions i
JOIN searchsessions s ON s.id = i.sessionid
WHERE s.createdat > NOW() - INTERVAL '30 days'
""")

rows = cur.fetchall()

dataset = []

for query, doc_id, clicked, position, dwell in rows:

    doc = es.get(index="vacancies", id=doc_id)
    text = doc["_source"]["description"]

    if clicked and dwell and dwell > 3000:
        dataset.append({
            "query": query,
            "positive": text
        })

with open("train.jsonl", "w") as f:
    for row in dataset:
        f.write(json.dumps(row) + "\n")