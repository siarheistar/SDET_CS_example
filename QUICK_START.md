# Quick Start - Запуск тестов за 3 минуты

## 🚀 Быстрый запуск

### 1️⃣ Запустить тесты

```bash
# Запустить Unit тесты (самые быстрые, не требуют API/браузер)
docker-compose up unit-api-tests
```

**Результат:** Увидишь прогон тестов в консоли:
```
✅ Passed: 10
❌ Failed: 5 (API тесты - нет сервера)
```

### 2️⃣ Посмотреть Allure отчёт

```bash
# Запустить Allure сервер
docker-compose up -d allure-server

# Открыть в браузере
open http://localhost:5050
```

**Или через командную строку:**
```bash
# Если установлен Allure CLI
allure serve allure-results
```

### 3️⃣ Остановить всё

```bash
docker-compose down
```

---

## 📊 Команды

| Команда | Описание |
|---------|----------|
| `docker-compose up unit-api-tests` | Unit + API тесты |
| `docker-compose up all-tests` | Все тесты |
| `docker-compose up -d allure-server` | Allure UI |
| `docker-compose down` | Остановить |

---

## 🔥 Allure отчёт

```bash
# Запустить Allure сервер
docker-compose up -d allure-server

# Открыть http://localhost:5050
```

Подробнее: [DOCKER_TESTING_GUIDE.md](./DOCKER_TESTING_GUIDE.md)

**Готово!** 🎉

---

## 🎯 БЫСТРАЯ КОМАНДА ДЛЯ ДЕМО МЕНЕДЖЕРУ:

```bash
# 1. Генерировать и открыть Allure отчёт
allure serve allure-results -p 9999

# Или если уже сгенерирован:
allure open allure-report -p 9999
```

**Откроется:** http://localhost:9999

**⚠️ ВАЖНО:** Используй порт 9999 (порт 8080 занят Jenkins!)
