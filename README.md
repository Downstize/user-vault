# 🛡️ UserVault API

**Сервис управления пользователями с авторизацией, JWT и CRUD-операциями.**  
Проект реализован на **.NET 9**, с использованием **PostgreSQL**, **JWT**, **Docker** и **Swagger**.

---

## 📌 Функциональность

- **Авторизация** по логину и паролю с выдачей JWT-токена.
- **Роли**: `Admin` и `User`, с разграничением доступа к API.
- **CRUD-операции над пользователями**:
  - создание
  - изменение имени, логина, пароля, даты рождения и пола
  - мягкое удаление и восстановление
  - запросы пользователей по фильтрам
- **Swagger UI** для взаимодействия с API.
- **Хранение пользователей в PostgreSQL**.

---

## 🛠️ Технологии

- **.NET 9 (ASP.NET Core Web API)**
- **Entity Framework Core**
- **PostgreSQL**
- **JWT (аутентификация и авторизация)**
- **Docker + Docker Compose**
- **Serilog (логгирование)**
- **Swagger/OpenAPI**

---

## 📂 Структура проекта
```
📦 UserVault
┣ 📂 Controllers      # AuthController и UsersController
┣ 📂 Services         # JWT-сервис и бизнес-логика пользователей
┣ 📂 Models           # Сущность User и enum
┣ 📂 Dtos             # DTO-модели
┣ 📂 Data             # AppDbContext (EF Core)
┣ 📜 Program.cs       # Конфигурация приложения
┣ 📜 appsettings.json # Настройки БД и JWT
┣ 📜 Dockerfile       # Docker-образ приложения
┣ 📜 docker-compose.yml # Docker Compose с БД
````

---

## 🚀 Запуск проекта

1️⃣ Клонирование репозитория и запуск:
```bash
git clone https://github.com/your-user/uservault.git
cd uservault
docker-compose up --build
````

2️⃣ Swagger будет доступен по адресу:

```
http://localhost:5470/swagger
```

---

## 🔐 Авторизация и роли

После запуска:

* Предсоздан пользователь:

  ```
  Login: admin
  Password: admin123
  ```

* Получить JWT токен через `/auth/login`:

  ```json
  {
    "login": "admin",
    "password": "admin123"
  }
  ```

* Вставить токен в Swagger через кнопку "Authorize (Замочек в правом верхнем углу)" как:

  ```
  token
  ```

---

## 📌 Основные эндпоинты

| Метод    | Путь                                 | Доступ     | Описание                     |
| -------- | ------------------------------------ | ---------- | ---------------------------- |
| `POST`   | `/auth/login`                        | Все        | Авторизация                  |
| `POST`   | `/api/users/create`                  | Admin      | Создать пользователя         |
| `PUT`    | `/api/users/update/name/{login}`     | Admin/User | Изменить имя                 |
| `PUT`    | `/api/users/update/password/{login}` | Admin/User | Изменить пароль              |
| `PUT`    | `/api/users/update/login/{login}`    | Admin/User | Изменить логин               |
| `DELETE` | `/api/users/{login}`                 | Admin      | Мягкое удаление              |
| `PUT`    | `/api/users/restore/{login}`         | Admin      | Восстановление               |
| `GET`    | `/api/users/all`                     | Admin      | Все активные пользователи    |
| `GET`    | `/api/users/by-login/{login}`        | Admin      | Данные по логину             |
| `GET`    | `/api/users/validate`                | User       | Самостоятельная проверка     |
| `GET`    | `/api/users/older-than/{age}`        | Admin      | Пользователи старше возраста |

---

## 📌 Валидация полей

| Поле       | Ограничения                              |
| ---------- | ---------------------------------------- |
| `Login`    | Только латинские буквы и цифры           |
| `Password` | Только латинские буквы и цифры           |
| `Name`     | Только русские и латинские буквы         |
| `Gender`   | 0 — женщина, 1 — мужчина, 2 — неизвестно |

---

## 🔒 Безопасность

* Все защищённые маршруты используют JWT.
* Только авторизованные пользователи могут вызывать методы.
* Роль `Admin` необходима для критичных операций (создание, удаление, восстановление, просмотр всех).

---

## ✉️ Контакты

Разработчик: **Вячеслав / Venceslao**  
GitHub:  
[https://github.com/Downstize/user-vault.git](https://github.com/Downstize/user-vault.git)  
Email:  
`swankydid@gmail.com`
