# Feature: User Registration (Frontend)
# This feature tests user registration functionality from the UI perspective
# Tags: @ui, @frontend, @smoke, @regression

Feature: User Registration
    As a new user
    I want to register an account
    So that I can access the application features

    Background:
        Given I am on the home page
        And the registration form is visible

    @smoke @ui @positive
    Scenario: Successful user registration with valid data
        When I register with the following details:
            | Field    | Value                  |
            | Username | john_doe               |
            | Email    | john.doe@example.com   |
            | Password | SecurePassword123!     |
        Then I should see a success message
        And the success message should contain "successfully"

    @ui @positive
    Scenario: Register multiple users successfully
        When I register user "alice_smith" with email "alice@example.com" and password "AlicePass123!"
        Then I should see a success message
        When I register user "bob_jones" with email "bob@example.com" and password "BobPass456!"
        Then I should see a success message

    @ui @negative @validation
    Scenario: Registration fails with username too short
        When I register with username "ab" and email "test@example.com" and password "ValidPass123!"
        Then I should see an error message
        And the error message should contain "at least 3 characters"

    @ui @negative @validation
    Scenario: Registration fails with invalid email format
        When I register with username "john_doe" and email "invalid-email" and password "ValidPass123!"
        Then I should see an error message
        And the error message should contain "valid email"

    @ui @negative @validation
    Scenario: Registration fails with password too short
        When I register with username "john_doe" and email "john@example.com" and password "123"
        Then I should see an error message
        And the error message should contain "at least"

    @ui @negative @validation
    Scenario Outline: Registration validation with various invalid inputs
        When I register with username "<username>" and email "<email>" and password "<password>"
        Then I should see an error message
        And the error message should contain "<error_message>"

        Examples:
            | username   | email              | password       | error_message              |
            | ab         | valid@example.com  | ValidPass123!  | at least 3 characters      |
            | john_doe   | invalid            | ValidPass123!  | valid email                |
            | john_doe   | valid@example.com  | 123            | at least                   |
            |            | valid@example.com  | ValidPass123!  | required                   |
            | john_doe   |                    | ValidPass123!  | required                   |
            | john_doe   | valid@example.com  |                | required                   |

    @ui @boundary
    Scenario: Registration with minimum valid username length
        When I register with username "abc" and email "min@example.com" and password "MinPass123!"
        Then I should see a success message

    @ui @boundary
    Scenario: Registration with maximum valid username length
        When I register with username "a_very_long_username_that_is_still_valid_12345678" and email "max@example.com" and password "MaxPass123!"
        Then I should see a success message

    @ui @accessibility
    Scenario: Registration form has proper accessibility attributes
        Then the username input should have a label or placeholder
        And the email input should have a label or placeholder
        And the password input should have a label or placeholder
        And the register button should be keyboard accessible

    @ui @responsive
    Scenario: Registration form is visible on different screen sizes
        When I resize the browser to mobile size
        Then the registration form should still be visible
        When I resize the browser to tablet size
        Then the registration form should still be visible
        When I resize the browser to desktop size
        Then the registration form should still be visible
