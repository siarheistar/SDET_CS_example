# SDET C# Test Automation Framework

## 🎯 Overview

Comprehensive C# test automation framework built with:
- **.NET 8.0** - Latest .NET framework
- **Playwright** - Modern browser automation
- **SpecFlow** - BDD/Gherkin support
- **NUnit** - Test framework
- **Allure** - Beautiful test reporting
- **Docker** - Containerized test execution
- **GitHub Actions** - CI/CD pipelines
- **OOP & SOLID Principles** - Enterprise-grade architecture

---

## 📁 Project Structure

```
SDET_CS_example/
├── src/
│   ├── Application/              # Web API application (test target)
│   ├── Services/                 # Business logic services
│   ├── Interfaces/               # SOLID interface definitions
│   ├── Repositories/             # Data access layer
│   ├── Validators/               # Input validation logic
│   ├── Utils/                    # Utility classes
│   └── SDET.Core.csproj          # Core project file
│
├── tests/
│   ├── Unit/                     # Unit tests (NUnit)
│   ├── Api/                      # API tests (RestSharp)
│   ├── Ui/                       # UI tests (Playwright)
│   ├── Bdd/                      # BDD tests (SpecFlow)
│   │   ├── Features/             # Gherkin feature files
│   │   └── StepDefinitions/      # Step definition classes
│   ├── Integration/              # Integration tests
│   ├── Pages/                    # Page Object Model
│   ├── Base/                     # Base test classes
│   ├── Services/                 # Test services
│   ├── Factories/                # Factory pattern implementation
│   ├── Interfaces/               # Test interfaces
│   ├── Fixtures/                 # Test fixtures
│   ├── Configurations/           # Test configuration files
│   ├── TestData/                 # Test data (JSON/YAML)
│   └── SDET.Tests.csproj         # Test project file
│
├── docker/
│   └── Dockerfile                # Docker image for test execution
│
├── .github/workflows/            # CI/CD pipelines
│   ├── test-automation.yml       # Main testing pipeline
│   └── docker-pipeline.yml       # Docker-based testing
│
├── postman/                      # Postman collections
│   ├── collections/              # API test collections
│   └── environments/             # Environment configurations
│
├── scripts/                      # Helper scripts
│   ├── run-tests.sh              # Execute tests locally
│   ├── docker-build.sh           # Build Docker image
│   └── install-browsers.sh       # Install Playwright browsers
│
├── docs/                         # Documentation
│   ├── ARCHITECTURE.md           # Architecture overview
│   ├── SETUP.md                  # Setup instructions
│   ├── TEST_GUIDE.md             # Testing guide
│   └── API_DOCS.md               # API documentation
│
├── allure-results/               # Allure test results
├── allure-report/                # Generated Allure reports
├── test-reports/                 # Test execution reports
├── logs/                         # Application and test logs
├── screenshots/                  # Test failure screenshots
│
└── SDET_CS_Framework.sln         # Visual Studio solution file
```

---

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Docker** (Optional) - [Download](https://www.docker.com/products/docker-desktop)
- **Node.js** (for Newman/Postman) - [Download](https://nodejs.org/)
- **Allure CLI** (Optional) - [Install Guide](https://docs.qameta.io/allure/)

### Installation

```bash
# Clone the repository
cd /Users/sergei/Projects/SDET_CS_example

# Restore NuGet packages
dotnet restore

# Install Playwright browsers
pwsh tests/bin/Debug/net8.0/playwright.ps1 install

# Verify installation
dotnet test --list-tests
```

### Running Tests

#### Run All Tests
```bash
dotnet test
```

#### Run Specific Test Categories
```bash
# Unit tests only
dotnet test --filter TestCategory=Unit

# API tests only
dotnet test --filter TestCategory=API

# UI tests only
dotnet test --filter TestCategory=UI

# BDD tests only
dotnet test --filter TestCategory=BDD

# Smoke tests only
dotnet test --filter TestCategory=Smoke
```

#### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

#### Run with Allure Reporting
```bash
# Run tests
dotnet test

# Generate Allure report
allure serve allure-results
```

---

## 🐳 Docker Execution

### Build Docker Image
```bash
docker build -f docker/Dockerfile -t sdet-cs-framework .
```

### Run Tests in Docker
```bash
# Run all tests
docker run sdet-cs-framework

# Run specific test suite
docker run sdet-cs-framework dotnet test --filter TestCategory=Smoke
```

### Docker Compose (Multiple Services)
```bash
# Start all services
docker-compose up

# Run specific service
docker-compose up test-unit
docker-compose up test-api
docker-compose up test-ui
```

---

## 🧪 Test Types

### 1. Unit Tests (`tests/Unit/`)
- Test individual components in isolation
- Mock external dependencies
- Fast execution
- Framework: NUnit
- Example: `UserValidatorTests.cs`

### 2. API Tests (`tests/Api/`)
- RESTful API endpoint testing
- Request/Response validation
- Status code verification
- Schema validation
- Framework: RestSharp + NUnit
- Example: `UserApiTests.cs`

### 3. UI Tests (`tests/Ui/`)
- Browser automation testing
- Cross-browser support (Chrome, Firefox, WebKit)
- Page Object Model pattern
- Screenshot capture on failure
- Framework: Playwright + NUnit
- Example: `UserRegistrationTests.cs`

### 4. BDD Tests (`tests/Bdd/`)
- Behavior-Driven Development
- Gherkin feature files
- Business-readable scenarios
- Framework: SpecFlow + NUnit
- Example: `UserRegistration.feature`

### 5. Integration Tests (`tests/Integration/`)
- End-to-end workflow testing
- Component interaction validation
- Database integration
- Example: `OrderWorkflowTests.cs`

---

## 📊 Reporting

### Allure Reports
Beautiful, interactive HTML test reports with:
- Test execution history
- Test categorization
- Screenshots and attachments
- Test duration metrics
- Failure analysis

```bash
# Generate and view report
allure serve allure-results
```

Access at: `http://localhost:5052` (when running via Docker)

### NUnit Reports
Standard XML test results:
- Located in `test-reports/`
- Compatible with CI/CD tools
- Includes test metrics

---

## 🏗️ Architecture & Design Patterns

### SOLID Principles

#### **S - Single Responsibility**
Each class has one reason to change:
- `IPageObject` - UI abstraction only
- `IApiClient` - API communication only
- `ITestDataService` - Test data generation only

#### **O - Open/Closed**
Open for extension, closed for modification:
- `BasePage` with virtual methods
- Extensible test base classes
- Plugin-based reporting

#### **L - Liskov Substitution**
Subtypes can replace base types:
- All page objects inherit from `IPageObject`
- All test classes can use base class fixtures

#### **I - Interface Segregation**
Focused, minimal interfaces:
- `IUITest`, `IAPITest`, `IIntegrationTest`
- No monolithic interfaces

#### **D - Dependency Inversion**
Depend on abstractions:
- Inject `IWebDriver`, not concrete driver
- Use `IConfiguration` interface
- Factory pattern for object creation

### Design Patterns

#### **1. Page Object Model (POM)**
```csharp
// Interface
public interface IPageObject
{
    void NavigateTo();
    bool IsLoaded();
}

// Base Implementation
public abstract class BasePage : IPageObject
{
    protected IPage Page { get; }
    protected abstract string PageUrl { get; }

    public virtual void NavigateTo() => Page.GotoAsync(PageUrl);
    public virtual bool IsLoaded() => Page.Url.Contains(PageUrl);
}

// Concrete Page
public class HomePage : BasePage
{
    protected override string PageUrl => "/home";

    public void RegisterUser(string username, string email, string password)
    {
        // Implementation
    }
}
```

#### **2. Factory Pattern**
```csharp
public interface ITestDataFactory
{
    UserTestData CreateUser();
    ProductTestData CreateProduct();
}

public class TestDataFactory : ITestDataFactory
{
    private readonly Faker _faker;

    public UserTestData CreateUser() => new UserTestData
    {
        Username = _faker.Internet.UserName(),
        Email = _faker.Internet.Email(),
        Password = _faker.Internet.Password()
    };
}
```

#### **3. Repository Pattern**
```csharp
public interface ITestRepository<T>
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> CreateAsync(T entity);
}
```

---

## 📝 BDD Feature Files

### Example: User Registration Feature

```gherkin
Feature: User Registration
    As a new user
    I want to register an account
    So that I can access the application

    @smoke @ui @regression
    Scenario: Successful user registration with valid data
        Given I am on the registration page
        When I enter username "john_doe"
        And I enter email "john@example.com"
        And I enter password "SecurePass123!"
        And I click the register button
        Then I should see a success message
        And the user should be created in the database

    @api @regression
    Scenario: Register user via API
        Given I have valid user registration data
        When I send a POST request to "/api/users/register"
        Then the response status code should be 201
        And the response should contain user ID

    @negative @validation
    Scenario Outline: Registration validation errors
        Given I am on the registration page
        When I enter username "<username>"
        And I enter email "<email>"
        And I enter password "<password>"
        And I click the register button
        Then I should see error message "<error>"

        Examples:
            | username | email           | password | error                              |
            | ab       | valid@email.com | Pass123! | Username must be at least 3 characters |
            | john_doe | invalid-email   | Pass123! | Please enter a valid email address     |
            | john_doe | valid@email.com | 123      | Password must be at least 8 characters |
```

---

## 🔧 Configuration

### Test Configuration (`tests/Configurations/testsettings.json`)

```json
{
  "BaseUrl": "http://localhost:5001",
  "ApiBaseUrl": "http://localhost:5001/api",
  "Browser": {
    "Type": "chromium",
    "Headless": true,
    "SlowMo": 0,
    "Timeout": 30000
  },
  "Allure": {
    "ResultsDirectory": "allure-results",
    "ReportDirectory": "allure-report"
  },
  "Logging": {
    "LogLevel": "Information",
    "LogFile": "logs/test-execution.log"
  }
}
```

---

## 🔌 Postman Integration

### Running Postman Collections with Newman

```bash
# Install Newman
npm install -g newman newman-reporter-allure

# Run collection
newman run postman/collections/UserAPI.postman_collection.json \
    --environment postman/environments/local.postman_environment.json \
    --reporters cli,allure \
    --reporter-allure-export allure-results
```

---

## 🎯 CI/CD Integration

### GitHub Actions

Automated testing on:
- **Push** to main/develop branches
- **Pull Request** creation
- **Scheduled** daily regression runs
- **Manual** workflow dispatch

#### Test Pipeline Jobs:
1. **Code Quality** - Linting, formatting checks
2. **Unit Tests** - Fast feedback
3. **API Tests** - Backend validation
4. **UI Tests** - Frontend validation
5. **BDD Tests** - Business scenario validation
6. **Integration Tests** - End-to-end workflows
7. **Report Generation** - Allure reports
8. **Deployment** - GitHub Pages for reports

---

## 📈 Test Coverage

### Coverage Reports

```bash
# Generate coverage report
dotnet test /p:CollectCoverage=true \
    /p:CoverletOutput=coverage/ \
    /p:CoverletOutputFormat=opencover

# View HTML report
open coverage/index.html
```

### Coverage Thresholds
- **Minimum**: 80% line coverage
- **Target**: 90%+ line coverage
- **Critical Paths**: 100% coverage

---

## 🛠️ Development

### Adding New Tests

#### 1. Unit Test
```csharp
[TestFixture]
[Category("Unit")]
public class UserServiceTests
{
    [Test]
    public void CreateUser_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var service = new UserService();

        // Act
        var result = service.CreateUser("john", "john@test.com");

        // Assert
        result.Should().NotBeNull();
    }
}
```

#### 2. API Test
```csharp
[TestFixture]
[Category("API")]
public class UserApiTests : BaseApiTest
{
    [Test]
    public async Task RegisterUser_WithValidData_Returns201()
    {
        // Arrange
        var userData = TestDataFactory.CreateUser();

        // Act
        var response = await ApiClient.PostAsync("/users/register", userData);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
```

#### 3. UI Test
```csharp
[TestFixture]
[Category("UI")]
public class RegistrationTests : BaseUITest
{
    [Test]
    public async Task RegisterUser_WithValidData_ShowsSuccessMessage()
    {
        // Arrange
        var homePage = new HomePage(Page);

        // Act
        await homePage.NavigateToAsync();
        await homePage.RegisterUserAsync("john", "john@test.com", "Pass123!");

        // Assert
        var message = await homePage.GetSuccessMessageAsync();
        message.Should().Contain("successfully");
    }
}
```

---

## 🐛 Troubleshooting

### Common Issues

**Issue**: Playwright browsers not installed
```bash
# Solution
pwsh tests/bin/Debug/net8.0/playwright.ps1 install
```

**Issue**: Port already in use
```bash
# Solution: Change port in testsettings.json or stop conflicting service
lsof -ti:5001 | xargs kill
```

**Issue**: Test failures in Docker
```bash
# Solution: Check logs
docker logs <container-id>

# View screenshots
docker cp <container-id>:/app/screenshots ./screenshots
```

---

## 📚 Additional Resources

### Documentation
- [Architecture Guide](docs/ARCHITECTURE.md)
- [Setup Instructions](docs/SETUP.md)
- [Test Writing Guide](docs/TEST_GUIDE.md)
- [API Documentation](docs/API_DOCS.md)

### External Links
- [Playwright for .NET](https://playwright.dev/dotnet/)
- [SpecFlow Documentation](https://specflow.org/documentation/)
- [NUnit Documentation](https://docs.nunit.org/)
- [Allure Framework](https://docs.qameta.io/allure/)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License.

---

## 👥 Authors

- **SDET Team** - Test Automation Engineers

---

## ✅ Project Status

- ✅ Project structure created
- ✅ NuGet packages configured
- ✅ Test frameworks integrated
- ✅ Page Object Model implemented
- ✅ BDD feature files created
- ✅ Docker configuration ready
- ✅ CI/CD pipelines configured
- ✅ Documentation complete

**Status**: ✅ Ready for Development and Testing

---

**Last Updated**: 2025-10-18
