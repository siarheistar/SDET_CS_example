# Feature: Order Management (Full Stack)
# This feature tests order creation and management from both UI and API perspectives
# Tags: @orders, @integration, @smoke, @regression

Feature: Order Management
    As a user
    I want to create and manage orders
    So that I can purchase products from the application

    # ==================== Frontend/UI Scenarios ====================

    @smoke @ui @positive
    Scenario: Create order via UI with valid product
        Given I am on the home page
        When I load the products list
        And I create an order for product ID 1 with quantity 2
        Then I should see a success message
        And the success message should contain "Order created"

    @ui @positive
    Scenario: Create multiple orders via UI
        Given I am on the home page
        When I create an order for product ID 1 with quantity 2
        Then I should see a success message
        When I create an order for product ID 2 with quantity 3
        Then I should see a success message

    @ui @negative @validation
    Scenario: Order creation fails with invalid product ID
        Given I am on the home page
        When I create an order for product ID -1 with quantity 2
        Then I should see an error message
        And the error message should contain "invalid product"

    @ui @negative @validation
    Scenario: Order creation fails with zero quantity
        Given I am on the home page
        When I create an order for product ID 1 with quantity 0
        Then I should see an error message
        And the error message should contain "quantity"

    @ui @boundary
    Scenario Outline: Order creation with boundary quantities
        Given I am on the home page
        When I create an order for product ID 1 with quantity <quantity>
        Then I should see <result>

        Examples:
            | quantity | result          |
            | 1        | success message |
            | 10       | success message |
            | 0        | error message   |
            | -1       | error message   |
            | 1000     | error message   |

    # ==================== Backend/API Scenarios ====================

    @smoke @api @positive
    Scenario: Create order via API with valid data
        Given the API is available
        And I have valid order data for product ID 1 and quantity 2
        When I send a POST request to "/api/orders"
        Then the response status code should be 201
        And the response should contain "order_id"
        And the response should contain "product_id"
        And the response should contain "quantity"

    @api @positive
    Scenario: Create order and verify all response fields
        Given I have the following order data:
            | Field      | Value |
            | ProductId  | 1     |
            | Quantity   | 5     |
            | CustomerName | John Doe |
        When I send a POST request to "/api/orders"
        Then the response status code should be 201
        And the response should match the order schema
        And the response field "product_id" should equal "1"
        And the response field "quantity" should equal "5"

    @api @negative @validation
    Scenario: API order creation fails with missing product ID
        Given I have order data without product_id
        When I send a POST request to "/api/orders"
        Then the response status code should be 400
        And the response should contain error message "product_id is required"

    @api @negative @validation
    Scenario: API order creation fails with invalid quantity
        Given I have order data with quantity "-1"
        When I send a POST request to "/api/orders"
        Then the response status code should be 400
        And the response should contain error message "invalid quantity"

    @api @performance
    Scenario: API order creation completes within acceptable time
        Given I have valid order data
        When I send a POST request to "/api/orders"
        Then the response status code should be 201
        And the response time should be less than 1500 milliseconds

    # ==================== Integration Scenarios ====================

    @integration @smoke
    Scenario: Create order via UI and verify via API
        Given I am on the home page
        When I create an order for product ID 1 with quantity 2
        Then I should see a success message
        When I retrieve the order via API
        Then the API response should contain the created order
        And the order product_id should be 1
        And the order quantity should be 2

    @integration @endtoend
    Scenario: Complete order workflow - UI to API to Database
        Given I am on the home page
        When I load the products list
        And I create an order for product ID 1 with quantity 3
        Then I should see a success message
        When I retrieve orders via API for the current user
        Then the API should return at least 1 order
        And the most recent order should have product_id 1
        And the most recent order should have quantity 3

    @integration @concurrent
    Scenario: Create orders concurrently via UI and API
        Given I have 2 browser sessions
        And I have API access
        When I create an order via UI in session 1
        And I create an order via API simultaneously
        Then both orders should be created successfully
        And the total order count should increase by 2

    # ==================== Product Availability ====================

    @business @validation
    Scenario: Cannot order more than available stock
        Given product ID 1 has 5 items in stock
        When I create an order for product ID 1 with quantity 10
        Then I should see an error message
        And the error message should contain "insufficient stock"

    @business @validation
    Scenario: Stock is reduced after order creation
        Given product ID 1 has 10 items in stock
        When I create an order for product ID 1 with quantity 3
        Then I should see a success message
        And product ID 1 should have 7 items in stock

    # ==================== Order Cancellation ====================

    @api @positive
    Scenario: Cancel order via API
        Given I have created an order with ID 123
        When I send a DELETE request to "/api/orders/123"
        Then the response status code should be 200
        And the response should contain "Order cancelled"

    @api @negative
    Scenario: Cannot cancel non-existent order
        When I send a DELETE request to "/api/orders/999999"
        Then the response status code should be 404
        And the response should contain error message "Order not found"
