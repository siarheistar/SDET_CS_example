#!/bin/bash

# SDET Framework Test Runner
# Simple script to run different types of tests

set -e

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${GREEN}========================================${NC}"
echo -e "${GREEN}   SDET Framework Test Runner${NC}"
echo -e "${GREEN}========================================${NC}"
echo ""

# Function to display usage
usage() {
    echo "Usage: $0 [test-type]"
    echo ""
    echo "Test Types:"
    echo "  unit      - Run Unit tests only (default, fast, no dependencies)"
    echo "  api       - Run API tests (requires API server)"
    echo "  ui        - Run UI tests (requires Playwright browser)"
    echo "  all       - Run ALL tests including BDD"
    echo "  smoke     - Run smoke tests only"
    echo ""
    echo "Examples:"
    echo "  $0          # Runs Unit tests"
    echo "  $0 unit     # Runs Unit tests"
    echo "  $0 api      # Runs API tests"
    echo "  $0 smoke    # Runs smoke tests"
    exit 1
}

# Parse argument
TEST_TYPE="${1:-unit}"

case "$TEST_TYPE" in
    unit)
        echo -e "${YELLOW}Running Unit Tests...${NC}"
        echo ""
        dotnet test --filter "TestCategory=Unit" --logger:"console;verbosity=normal"
        ;;
    api)
        echo -e "${YELLOW}Running API Tests...${NC}"
        echo -e "${YELLOW}Note: Requires API server running at http://localhost:5000${NC}"
        echo ""
        dotnet test --filter "TestCategory=API" --logger:"console;verbosity=normal"
        ;;
    ui)
        echo -e "${YELLOW}Running UI Tests...${NC}"
        echo -e "${YELLOW}Note: Requires Playwright browser installed${NC}"
        echo ""
        dotnet test --filter "TestCategory=UI" --logger:"console;verbosity=normal"
        ;;
    smoke)
        echo -e "${YELLOW}Running Smoke Tests...${NC}"
        echo ""
        dotnet test --filter "TestCategory=Smoke&TestCategory=Unit" --logger:"console;verbosity=normal"
        ;;
    all)
        echo -e "${YELLOW}Running ALL Tests (Unit + API + UI + BDD)...${NC}"
        echo -e "${RED}Warning: This requires API server and Playwright!${NC}"
        echo ""
        dotnet test --logger:"console;verbosity=detailed"
        ;;
    help|--help|-h)
        usage
        ;;
    *)
        echo -e "${RED}Error: Unknown test type '$TEST_TYPE'${NC}"
        echo ""
        usage
        ;;
esac

# Check exit code
if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}========================================${NC}"
    echo -e "${GREEN}   ✅ Tests Completed Successfully${NC}"
    echo -e "${GREEN}========================================${NC}"
else
    echo ""
    echo -e "${RED}========================================${NC}"
    echo -e "${RED}   ❌ Tests Failed${NC}"
    echo -e "${RED}========================================${NC}"
    exit 1
fi
