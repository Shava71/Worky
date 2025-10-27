# Worky
## Проект "Веб-сервис для бюро по трудойстройству", позволяющий работодателям и соискателям выкладывать вакансии и резюме, откликаться, а так же вести общение через текстовый и голосовой-видео чаты внутри единой платформы
### Проект реализован с использованием **микросервисов** и отдельного фронт-сервера на **React**
---

### Описание веток (развитие проекта):
* main - общая ветка (merge from **microservicesbackend** branch)
* backend - подход нарушения всех принципов программирования (SOLID, KISS, DRY), написание всех бизнес-процессов напрямую в контроллерах
* backendRef - переписанный проект на слоистую архитектуру (DAL - BLL - PLL)
* microservicesbackend - переписанный проект на микросервисы
* frontend - фронт-сервер на React
---

### Описание использованных фреймворков / инструментов для MicroServicesBackend
- C#
- .NET Core
- ASP.NET
- REST API
- PostgreSQL
- Redis
- ELK (ElasticSearch)
- Kafka
- MassTransit 
- BackGroundServices
- HttClient (typed client by IHttpClientFactory) + Polly
- EF (+ Code First), Dapper
- SignalR
- JWT Bearer
- xUnit
- YARP gateway
- Swagger
---

### Краткое описание сервисов:

Легенда:
- [x] - В состоянии "pushed"
- [ ] - в состоянии написания

**Микросервисы:**
- [x] ApiGateway - api-шлюз через применения asp.net YARP, необходим для собственной реализации обратного простого и масштабируемого прокси-сервера
- [x] AuthService - сервис, отвечающий за регистрацию и авторизацию пользователя за счёт JWT-token
- [x] FilterService - сервис, отвечающий за хранение и администрирование фильтров, доступных вакансиям и резюме
- [x] WorkerService - сервис, отвечающий за функциональность роли "Соискатель" и резюме
- [ ] CompanyService - сервис, отвечающий за функциональность роли "Работодатель", вакансии и договора (подписки)
- [ ] ChatService - сервис, отвечающий за сигнальный сервер для реализации общения между пользователями платформы (SignalR)
- [ ] SearchSerive - сервис, отвечающий за индексирование данных (CQRS)
---
<details><summary>Описание ApiGateway</summary>
  
</details>
<details><summary>Описание AuthService</summary>
  
</details>
<details><summary>Описание Filterervice</summary>
  
</details>
<details><summary>Описание WorkerService</summary>
  
</details>
<details><summary>Описание CompanyService</summary>
  
</details>
<details><summary>Описание ChatService</summary>
  
</details>
<details><summary>Описание SearchSerive</summary>
  
</details>
---

## Секция схем:

---

## **docker-compose** реализован в родительской директории. Запуск контейнеров и их установка

Установка контейнеров и запуск:

``` shell
docker compose up -d --build
```

Последующие запуски контейнеров:

``` shell
docker compose up -d
```


