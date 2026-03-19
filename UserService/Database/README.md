# Database Contract for UserService

## Общая информация

- **Схема:** public
- **Таблица:** `users`
- **Назначение:** хранение данных пользователей для UserService

### Структура таблицы `public.users`

| Колонка  | Тип          | Ограничения                      |
|----------|--------------|----------------------------------|
| id       | serial       | PRIMARY KEY                      |
| login    | varchar(64)  | NOT NULL, UNIQUE                 |
| password | varchar(128) | NOT NULL                         |
| name     | varchar(64)  | NOT NULL                         |
| surname  | varchar(64)  | NOT NULL                         |
| age      | int          | NOT NULL, CHECK (14 ≤ age ≤ 120) |

### Индексы

- `unique(login)` — уникальность логина
- `ix_users_name_surname (name, surname)` — для ускорения поиска по имени и фамилии

---
