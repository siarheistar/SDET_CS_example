# Feature: User Registration API (Backend)
# This feature tests user registration functionality from the API perspective
# Tags: @api, @backend, @smoke, @regression

Feature: User Registration API
    As an API client
    I want to register users via the API
    So that I can integrate user registration into other applications

    Background:
        Given the API is available at "/api/users"

    @smoke @api @positive
    Scenario: Register user via API with valid data
        Given I have valid user registration data
        When I send a POST request to "/api/users/register"
        Then the response status code should be 201
        And the response should contain "id"
        And the response should contain "username"
        And the response should contain "email"

    @api @positive
    Scenario: Register user and verify response schema
        Given I have the following user data:
            | Field    | Value                  |
            | Username | api_user_001           |
            | Email    | api.user@example.com   |
            | Password | ApiUserPass123!        |
        When I send a POST request to "/api/users/register"
        Then the response status code should be 201
        And the response should match the user schema

    @api @negative @validation
    Scenario: API registration fails with missing username
        Given I have user data without username
        When I send a POST request to "/api/users/register"
        Then the response status code should be 400
        And the response should contain error message "username is required"

    @api @negative @validation
    Scenario: API registration fails with invalid email
        Given I have user data with invalid email "not-an-email"
        When I send a POST request to "/api/users/register"
        Then the response status code should be 400
        And the response should contain error message "invalid email"

    @api @negative @validation
    Scenario Outline: API validation with various invalid inputs
        Given I have user data with "<field>" as "<value>"
        When I send a POST request to "/api/users/register"
        Then the response status code should be <status_code>
        And the response should contain error message "<error_message>"

        Examples:
            | field    | value       | status_code | error_message          |
            | username | ab          | 400         | at least 3 characters  |
            | email    | invalid     | 400         | invalid email          |
            | password | 12          | 400         | at least               |
            | username | (empty)     | 400         | required               |
            | email    | (empty)     | 400         | required               |

    @api @performance
    Scenario: API registration completes within acceptable time
        Given I have valid user registration data
        When I send a POST request to "/api/users/register"
        Then the response status code should be 201
        And the response time should be less than 2000 milliseconds

    @api @security
    Scenario: API registration does not expose password in response
        Given I have valid user registration data
        When I send a POST request to "/api/users/register"
        Then the response status code should be 201
        And the response should not contain "password"
        And the response should not contain the password value

    @api @idempotency
    Scenario: Duplicate username registration fails
        Given I register a user with username "duplicate_user"
        When I send a POST request to register the same username again
        Then the response status code should be 409
        And the response should contain error message "already exists"

    @api @batch @performance
    Scenario: Register multiple users via API
        When I send 10 concurrent registration requests
        Then all requests should complete successfully
        And all response status codes should be 201
        And the total time should be less than 5000 milliseconds

    @api @contenttype
    Scenario: API accepts JSON content type
        Given I have valid user registration data
        And I set the Content-Type header to "application/json"
        When I send a POST request to "/api/users/register"
        Then the response status code should be 201

    @api @contenttype @negative
    Scenario: API rejects invalid content type
        Given I have valid user registration data
        And I set the Content-Type header to "text/plain"
        When I send a POST request to "/api/users/register"
        Then the response status code should be 415
