
Це API для управління задачами користувачів, з можливістю реєстрації, 
логіну, створення, оновлення, отримання та видалення задач. 
Для доступу до API використовується автентифікація на основі JWT, 

База даних MS SQL, що використовується для зберігання тасків та користувачів задеплоїна на AWS

Публічна API задеплоїна на heroku - https://luna-api-test-d5a8b4257ac5.herokuapp.com/
Та оновлюється автоматично після кожної зміни(push) на GitHub

Можна тестувати в Postman, передаючи в тілі запиту(Body) JSON із відповідними параметрами.
Також в Headers вказати ключ `Content-Type` зі значенням `application/json`.Це потрібно для передачі JSON у тілі запиту.

Токен має бути переданий у заголовку `Authorization` у форматі `Bearer <token>`для роботи із тасками, 
бо тільки авторизований користувач може працювати із ними


## Ендпоінти API для реєстрації/логінізації

### 1. Реєстрація користувача
- **Метод:** `POST`
- **Ендпоінт:** `/api/users/register`
- **Опис:** Реєструє нового користувача.
- **Тіло запиту:**
    ```json
    {
      "username": "string",
      "email": "string",
      "password": "string"
    }
    ```
- **Відповідь:**
  - 200 OK: Реєстрація успішна.
  - 400 Bad Request: Якщо електронна пошта або ім’я користувача вже існують.

### 2. Логін користувача
- **Метод:** `POST`
- **Ендпоінт:** `/api/users/login`
- **Опис:** Аутентифікує користувача та повертає токен JWT.
- **Тіло запиту:**
    ```json
    {
      "username": "string",
      "email": "string",
      "password": "string"
    }
    ```
- **Відповідь:**
  - 200 OK: Повертає токен JWT.
  - 401 Unauthorized: Якщо аутентифікація не вдалася.

## Ендпоінти API для тасків

### 3. Отримати всі задачі
- **Метод:** `GET`
- **Ендпоінт:** `/api/task`
- **Опис:** Отримує всі задачі для автентифікованого користувача.
- **Параметри запиту:**
    - `status`: Фільтрація за статусом задачі (наприклад, Pending, Completed).
    - `priority`: Фільтрація за пріоритетом задачі (наприклад, Low, High).
    - `dueDate`: Фільтрація задач, що мають дату виконання до зазначеної.
    - `page`: Номер сторінки для пагінації (за замовчуванням 1).
    - `pageSize`: Кількість задач на сторінці (за замовчуванням 10).
- **Відповідь:**
  - 200 OK: Повертає список задач.

### 4. Отримати конкретну задачу
- **Метод:** `GET`
- **Ендпоінт:** `/api/task/{id}`
- **Опис:** Отримує конкретну задачу за її ID для автентифікованого користувача.
- **Параметри URL:**
    - `id`: Guid задачі.
- **Відповідь:**
  - 200 OK: Повертає дані задачі.
  - 404 Not Found: Якщо задача не існує або не належить автентифікованому користувачу.

### 5. Створити задачу
- **Метод:** `POST`
- **Ендпоінт:** `/api/task`
- **Опис:** Створює нову задачу для автентифікованого користувача.
- **Тіло запиту:**
    ```json
    {
      "title": "string",
      "description": "string",
      "dueDate": "YYYY-MM-DD",
      "priority": "Low | Medium | High"
    }
    ```
- **Відповідь:**
  - 201 Created: Повертає створену задачу разом з її ID.
  - 400 Bad Request: Якщо тіло запиту є недійсним.

### 6. Оновити задачу
- **Метод:** `PUT`
- **Ендпоінт:** `/api/task/{id}`
- **Опис:** Оновлює існуючу задачу для автентифікованого користувача.
- **Параметри URL:**
    - `id`: Guid задачі для оновлення.
- **Тіло запиту:**
    ```json
    {
      "title": "string",
      "description": "string",
      "dueDate": "YYYY-MM-DD",
      "status": "Pending | InProgress | Completed",
      "priority": "Low | Medium | High"
    }
    ```
- **Відповідь:**
  - 204 No Content: Задача успішно оновлена.
  - 404 Not Found: Якщо задача не існує або не належить автентифікованому користувачу.

### 7. Видалити задачу
- **Метод:** `DELETE`
- **Ендпоінт:** `/api/task/{id}`
- **Опис:** Видаляє конкретну задачу для автентифікованого користувача.
- **Параметри URL:**
    - `id`: Guid задачі для видалення.
- **Відповідь:**
  - 204 No Content: Задача успішно видалена.
  - 404 Not Found: Якщо задача не існує або не належить автентифікованому користувачу.

---

