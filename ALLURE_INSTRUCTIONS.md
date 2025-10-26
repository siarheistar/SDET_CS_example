# 📊 Как посмотреть Allure отчёты

## ⚠️ Важно!
Allure Docker Service работает, но для просмотра отчётов нужны результаты в формате Allure JSON.
Сейчас тесты генерируют только TRX отчёты.

## ✅ Решение 1: Установить Allure CLI (РЕКОМЕНДУЕТСЯ)

### macOS:
```bash
brew install allure
```

### Linux:
```bash
wget https://github.com/allure-framework/allure2/releases/download/2.25.0/allure-2.25.0.tgz
tar -zxvf allure-2.25.0.tgz
sudo mv allure-2.25.0 /opt/
sudo ln -s /opt/allure-2.25.0/bin/allure /usr/local/bin/allure
```

### Использование:
```bash
# 1. Запустить тесты
docker-compose up unit-api-tests

# 2. Посмотреть TRX отчёт
cat test-reports/test-results.trx

# 3. (Опционально) Если есть Allure JSON:
allure serve allure-results
```

## ✅ Решение 2: Просмотр TRX отчётов

TRX - это стандартный формат отчётов .NET:

```bash
# Запустить тесты
docker-compose up unit-api-tests

# Посмотреть результаты
cat test-reports/test-results.trx | grep -E "outcome|testName"

# Или открыть в редакторе
code test-reports/test-results.trx
```

## ✅ Решение 3: Консольный вывод

Самый простой способ - смотреть результаты прямо в консоли:

```bash
docker-compose up unit-api-tests
```

**Вывод покажет:**
```
✅ Passed: 10
❌ Failed: 5
Total time: 0.6 seconds
```

## 📊 Текущие результаты тестов

После `docker-compose up unit-api-tests` ты увидишь:

```
Passed Tests (10):
  ✅ ValidateEmail_WithInvalidEmail_ReturnsFalse("invalid")
  ✅ ValidateEmail_WithInvalidEmail_ReturnsFalse("user@")
  ✅ ValidateEmail_WithInvalidEmail_ReturnsFalse("")
  ✅ ValidateEmail_WithValidEmail_ReturnsTrue("test@example.com")
  ✅ ValidateEmail_WithValidEmail_ReturnsTrue("user.name@domain.co.uk")
  ✅ ValidateEmail_WithValidEmail_ReturnsTrue("user+tag@example.com")
  ✅ ValidateUsername_WithInvalidUsername_ReturnsFalse("ab")
  ✅ ValidateUsername_WithInvalidUsername_ReturnsFalse("a")
  ✅ ValidateUsername_WithInvalidUsername_ReturnsFalse("")
  ✅ ValidateUsername_WithValidUsername_ReturnsTrue

Failed Tests (5):
  ❌ API тесты - Connection refused (нет API сервера)
```

## 🔧 Чтобы включить полноценные Allure отчёты

Нужно обновить тесты чтобы они генерировали Allure JSON:

1. Убедиться что `Allure.NUnit` правильно настроен
2. Добавить `[AllureNUnit]` атрибут к тест-классам
3. Результаты появятся в `allure-results/`

Пока что проще всего смотреть результаты:
- В консоли при запуске
- В TRX файлах
- В логах Docker: `docker-compose logs unit-api-tests`

---

**Краткая инструкция:**

```bash
# Запустить тесты и увидеть результаты
docker-compose up unit-api-tests

# Посмотреть детали в TRX
cat test-reports/test-results.trx

# Посмотреть логи
docker-compose logs unit-api-tests
```

**Готово!** ✅
