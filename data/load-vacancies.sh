#!/usr/bin/env bash

JWT="$1"
FILE="./data/vacancies-data.json"
URL="http://localhost:5005/api/v1/Company/CreateVacancy"

if [ -z "$JWT" ]; then
  echo "Ошибка: передайте JWT токен как аргумент."
  echo "Пример: ./load-vacancies.sh <jwt>"
  exit 1
fi

if [ ! -f "$FILE" ]; then
  echo "Ошибка: файл $FILE не найден!"
  exit 1
fi

COUNT=$(jq length "$FILE")

echo "Найдено $COUNT вакансий. Начинаю загрузку..."

for i in $(seq 0 $(($COUNT - 1))); do
  VACANCY=$(jq -c ".[$i]" "$FILE")

  echo "Отправка вакансии #$((i+1))..."

  curl -s -X POST "$URL" \
    -H "accept: */*" \
    -H "Authorization: Bearer $JWT" \
    -H "Content-Type: application/json" \
    -d "$VACANCY" > /dev/null

  echo "✓ Загружено"
done

echo "Готово!"