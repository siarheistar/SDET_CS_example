# Docker Testing Guide - SDET C# Framework

Полное руководство по запуску тестов в Docker и просмотру Allure отчётов.

## Быстрый старт

### 1. Сборка Docker образа

```bash
# Собрать образ
docker-compose build

# Или через Docker напрямую
docker build -f docker/Dockerfile -t sdet-cs-framework:latest .
```

### 2. Запуск тестов

```bash
# Запустить все тесты
docker-compose up all-tests

# Запустить только Unit и API тесты (без браузера)
docker-compose up unit-api-tests

# Запустить в фоне
docker-compose up -d all-tests

# Посмотреть логи
docker-compose logs -f all-tests
```

### 3. Остановка контейнеров

```bash
# Остановить и удалить контейнеры
docker-compose down

# Остановить и удалить всё (включая volumes)
docker-compose down -v
```

## Подробное описание

### Доступные сервисы в docker-compose.yml

#### `all-tests` - Все тесты
Запускает все тесты (Unit, API, UI, BDD)

```bash
docker-compose up all-tests
```

**Примечание:** UI тесты упадут без display/browser, но это нормально для CI/CD.

#### `unit-api-tests` - Unit и API тесты
Запускает только Unit и API тесты (без UI)

```bash
docker-compose up unit-api-tests
```

**Результат:**
- ✅ Unit тесты должны пройти
- ❌ API тесты упадут если нет запущенного API сервера на localhost:5001

#### `build-only` - Проверка сборки
Только проверяет что проект собирается без ошибок

```bash
docker-compose up build-only
```

## Запуск тестов с фильтрами

### Через docker run

```bash
# Все Unit тесты
docker run --rm sdet-cs-framework:latest \
  dotnet test --filter "TestCategory=Unit"

# Все API тесты
docker run --rm sdet-cs-framework:latest \
  dotnet test --filter "TestCategory=API"

# Smoke тесты
docker run --rm sdet-cs-framework:latest \
  dotnet test --filter "TestCategory=Smoke"

# Конкретный тест по имени
docker run --rm sdet-cs-framework:latest \
  dotnet test --filter "FullyQualifiedName~UserValidatorTests"
```

### Кастомная команда в docker-compose

Создайте свой сервис в `docker-compose.yml`:

```yaml
services:
  smoke-tests:
    image: sdet-cs-framework:latest
    command: >
      dotnet test
      --filter "TestCategory=Smoke"
      --logger:"console;verbosity=normal"
    volumes:
      - ./test-reports:/app/test-reports
      - ./allure-results:/app/allure-results
```

Запустить:
```bash
docker-compose up smoke-tests
```

## Получение отчётов

### 1. TRX отчёты (XML формат)

Отчёты автоматически сохраняются в `./test-reports/`

```bash
# Запустить тесты с volume mount
docker-compose up all-tests

# Посмотреть отчёт
ls -la test-reports/
cat test-reports/test-results.trx
```

### 2. Allure отчёты

#### Шаг 1: Запустить тесты и собрать результаты

```bash
# Запустить тесты (результаты сохранятся в ./allure-results/)
docker-compose up all-tests
```

#### Шаг 2: Установить Allure (если ещё не установлен)

**macOS:**
```bash
brew install allure
```

**Linux:**
```bash
# Скачать и установить
wget https://github.com/allure-framework/allure2/releases/download/2.25.0/allure-2.25.0.tgz
tar -zxvf allure-2.25.0.tgz
sudo mv allure-2.25.0 /opt/
sudo ln -s /opt/allure-2.25.0/bin/allure /usr/local/bin/allure
```

**Windows:**
```bash
scoop install allure
```

#### Шаг 3: Сгенерировать и открыть Allure отчёт

```bash
# Сгенерировать отчёт и открыть в браузере
allure serve allure-results

# Или сгенерировать статический HTML отчёт
allure generate allure-results -o allure-report --clean

# Открыть отчёт
allure open allure-report
```

### 3. Allure отчёт внутри Docker контейнера

Создайте отдельный сервис для генерации Allure:

Добавьте в `docker-compose.yml`:

```yaml
services:
  allure-report:
    image: sdet-cs-framework:latest
    command: >
      bash -c "
      allure generate /app/allure-results -o /app/allure-report --clean &&
      allure open /app/allure-report
      "
    volumes:
      - ./allure-results:/app/allure-results
      - ./allure-report:/app/allure-report
    ports:
      - "4040:4040"
```

Запустить:
```bash
docker-compose up allure-report
```

Открыть в браузере: http://localhost:4040

## Примеры использования

### Пример 1: Локальная разработка

```bash
# Собрать образ
docker-compose build

# Запустить Unit тесты для быстрой проверки
docker-compose up unit-api-tests

# Посмотреть результаты
cat test-reports/test-results.trx
```

### Пример 2: CI/CD Pipeline

```bash
# Сборка
docker-compose build all-tests

# Запуск всех тестов
docker-compose up --exit-code-from all-tests all-tests

# Сохранить артефакты
cp -r test-reports/ $CI_ARTIFACTS_DIR/
cp -r allure-results/ $CI_ARTIFACTS_DIR/

# Сгенерировать Allure отчёт
allure generate allure-results -o allure-report
```

### Пример 3: Полный цикл с отчётами

```bash
# 1. Очистить старые результаты
rm -rf test-reports/* allure-results/* allure-report/*

# 2. Запустить тесты
docker-compose up all-tests

# 3. Сгенерировать Allure отчёт
allure generate allure-results -o allure-report --clean

# 4. Открыть отчёт в браузере
allure open allure-report
```

## Структура директорий с отчётами

```
SDET_CS_example/
├── test-reports/          # TRX отчёты от dotnet test
│   └── test-results.trx
├── allure-results/        # JSON результаты для Allure
│   ├── *-result.json
│   └── *-container.json
├── allure-report/         # HTML отчёт Allure (генерируется)
│   └── index.html
├── logs/                  # Логи тестов
│   └── test-*.log
├── screenshots/           # Скриншоты UI тестов
└── videos/               # Видео записи UI тестов
```

## Troubleshooting

### Проблема: Тесты не находят Allure результаты

**Решение:**
```bash
# Проверить что volume примонтирован
docker-compose config

# Убедиться что директория существует
mkdir -p allure-results

# Дать права на запись
chmod 777 allure-results
```

### Проблема: API тесты падают с "Connection refused"

**Причина:** API сервер не запущен

**Решение 1:** Запустить API сервер в отдельном контейнере

Добавьте в `docker-compose.yml`:
```yaml
services:
  api-server:
    build:
      context: .
      dockerfile: docker/Dockerfile
      target: test-runtime
    command: dotnet run --project /app/src/Application/SDET.Application.csproj
    ports:
      - "5001:5001"
    environment:
      - ASPNETCORE_URLS=http://+:5001

  api-tests:
    image: sdet-cs-framework:latest
    depends_on:
      - api-server
    environment:
      - API_BASE_URL=http://api-server:5001
    command: dotnet test --filter "TestCategory=API"
```

**Решение 2:** Пропустить API тесты
```bash
docker run --rm sdet-cs-framework:latest \
  dotnet test --filter "TestCategory!=API"
```

### Проблема: UI тесты падают

**Причина:** Нет display для браузера

**Решение:** Запускать только Unit/API тесты
```bash
docker-compose up unit-api-tests
```

## Дополнительные команды

### Просмотр логов контейнера

```bash
# Все логи
docker-compose logs all-tests

# Последние 100 строк
docker-compose logs --tail=100 all-tests

# В реальном времени
docker-compose logs -f all-tests
```

### Интерактивный режим (debugging)

```bash
# Войти в контейнер
docker run -it --rm sdet-cs-framework:latest /bin/bash

# Внутри контейнера
cd /app/tests
dotnet test --filter "FullyQualifiedName~ValidateEmail"
ls -la /app/allure-results/
```

### Очистка

```bash
# Удалить все контейнеры
docker-compose down

# Удалить образы
docker rmi sdet-cs-framework:latest

# Очистить всё Docker
docker system prune -a
```

## GitHub Actions / CI интеграция

Пример `.github/workflows/tests.yml`:

```yaml
name: Run Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Build Docker image
      run: docker-compose build all-tests

    - name: Run tests
      run: docker-compose up --exit-code-from all-tests all-tests

    - name: Generate Allure Report
      if: always()
      run: |
        sudo apt-get install -y allure
        allure generate allure-results -o allure-report --clean

    - name: Upload test results
      if: always()
      uses: actions/upload-artifact@v3
      with:
        name: test-results
        path: |
          test-reports/
          allure-report/

    - name: Publish Allure Report
      if: always()
      uses: simple-elf/allure-report-action@master
      with:
        allure_results: allure-results
        allure_report: allure-report
```

## Полезные ссылки

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Allure Report Documentation](https://docs.qameta.io/allure/)
- [NUnit Documentation](https://docs.nunit.org/)
- [.NET Test Documentation](https://docs.microsoft.com/en-us/dotnet/core/testing/)

---

**Версия:** 1.0
**Дата:** 2025-10-21
**Автор:** SDET Team
