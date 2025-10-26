# Testing Guide

Quick reference for running tests in the SDET Framework.

## Quick Start - Run Tests Now!

```bash
# Option 1: Use the helper script (recommended)
./run-tests.sh

# Option 2: Use dotnet CLI directly
dotnet test --filter "TestCategory=Unit"
```

## Test Types

### Unit Tests (Default - Always Works)
- **No dependencies** - Always run successfully
- **Fast** - Completes in < 1 second
- **11 tests** - Email and username validation

```bash
./run-tests.sh unit
# OR
dotnet test --filter "TestCategory=Unit"
```

### API Tests (Requires API Server)
- Requires API server running at `http://localhost:5000`
- Tests REST API endpoints

```bash
./run-tests.sh api
# OR
dotnet test --filter "TestCategory=API"
```

### UI Tests (Requires Playwright)
- Requires Playwright browser installed
- Tests web UI functionality

```bash
./run-tests.sh ui
# OR
dotnet test --filter "TestCategory=UI"
```

### Smoke Tests
- Quick smoke tests only

```bash
./run-tests.sh smoke
```

## Script Options

```bash
./run-tests.sh          # Unit tests (default)
./run-tests.sh unit     # Unit tests
./run-tests.sh api      # API tests
./run-tests.sh ui       # UI tests
./run-tests.sh smoke    # Smoke tests
./run-tests.sh all      # ALL tests (requires everything)
./run-tests.sh help     # Show help
```

## Advanced .NET CLI Commands

### Run with detailed output
```bash
dotnet test --filter "TestCategory=Unit" --logger:"console;verbosity=detailed"
```

### Run specific test by name
```bash
dotnet test --filter "FullyQualifiedName~UserValidatorTests"
```

### Run and collect coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Run with Allure reporting
```bash
dotnet test --filter "TestCategory=Unit"
allure serve allure-results
```

## Test Results

After running tests, check:
- **Console output** - Immediate results
- **TRX file** - `test-reports/test-results.trx`
- **Allure results** - `allure-results/` directory
- **GitHub Actions** - https://github.com/siarheistar/SDET_CS_example/actions
- **Live Report** - https://siarheistar.github.io/SDET_CS_example/

## Current Test Status

✅ **Unit Tests**: 11/11 passing
⚠️ **API Tests**: Requires API server (disabled by default)
⚠️ **UI Tests**: Requires Playwright (disabled by default)
⚠️ **BDD Tests**: Requires UI/API setup (disabled by default)

## Troubleshooting

### "Page not initialized" error
You tried to run UI tests without Playwright. Run only Unit tests:
```bash
./run-tests.sh unit
```

### "Connection refused" error
API tests can't find the API server. Either skip them or start the API server first.

### All tests run but many fail
You ran `dotnet test` without a filter. Use the script or add a filter:
```bash
./run-tests.sh          # Recommended
dotnet test --filter "TestCategory=Unit"  # Alternative
```

## Docker Option

If you don't want to install .NET 8.0:

```bash
docker-compose up unit-api-tests
```

## CI/CD

Tests run automatically on every push to `main` branch via GitHub Actions.

View results:
- GitHub Actions: https://github.com/siarheistar/SDET_CS_example/actions
- Allure Report: https://siarheistar.github.io/SDET_CS_example/
