#!/bin/bash

#############################################################
# Upload Allure Results to Remote Allure Server
#############################################################
# Usage:
#   ./upload-to-allure-server.sh [server-url] [project-id]
#
# Example:
#   ./upload-to-allure-server.sh http://192.168.1.100:5050 sdet-cs-framework
#
# Environment variables (optional):
#   ALLURE_SERVER_URL - Default server URL
#   ALLURE_PROJECT_ID - Default project ID
#   ALLURE_RESULTS_DIR - Results directory (default: ./allure-results)
#############################################################

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
ALLURE_SERVER="${1:-${ALLURE_SERVER_URL}}"
PROJECT_ID="${2:-${ALLURE_PROJECT_ID:-sdet-cs-framework}}"
RESULTS_DIR="${ALLURE_RESULTS_DIR:-./allure-results}"
EXECUTION_NAME="${EXECUTION_NAME:-Manual Upload}"
EXECUTION_FROM="${EXECUTION_FROM:-$(whoami)}"
EXECUTION_TYPE="${EXECUTION_TYPE:-manual}"

# Validation
if [ -z "$ALLURE_SERVER" ]; then
    echo -e "${RED}❌ Error: Allure server URL is required${NC}"
    echo "Usage: $0 <server-url> [project-id]"
    echo "Example: $0 http://192.168.1.100:5050 sdet-cs-framework"
    exit 1
fi

if [ ! -d "$RESULTS_DIR" ]; then
    echo -e "${RED}❌ Error: Results directory not found: $RESULTS_DIR${NC}"
    exit 1
fi

echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║       Allure Results Upload Script                    ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
echo ""
echo -e "${YELLOW}📊 Configuration:${NC}"
echo -e "  Server:      ${GREEN}$ALLURE_SERVER${NC}"
echo -e "  Project ID:  ${GREEN}$PROJECT_ID${NC}"
echo -e "  Results Dir: ${GREEN}$RESULTS_DIR${NC}"
echo -e "  Execution:   ${GREEN}$EXECUTION_NAME${NC}"
echo ""

# Function to check server availability
check_server() {
    echo -e "${YELLOW}🔍 Checking server availability...${NC}"

    if curl -s -o /dev/null -w "%{http_code}" "$ALLURE_SERVER/allure-docker-service/version" | grep -q "200"; then
        echo -e "${GREEN}✅ Server is reachable${NC}"

        # Get server version
        VERSION=$(curl -s "$ALLURE_SERVER/allure-docker-service/version" | grep -o '"version":"[^"]*"' | cut -d'"' -f4)
        echo -e "  Version: ${GREEN}$VERSION${NC}"
    else
        echo -e "${RED}❌ Error: Cannot reach Allure server${NC}"
        echo -e "  Please check:"
        echo -e "    - Server URL is correct"
        echo -e "    - Server is running"
        echo -e "    - Firewall allows connection"
        exit 1
    fi
    echo ""
}

# Function to create project
create_project() {
    echo -e "${YELLOW}📁 Creating project (if not exists)...${NC}"

    RESPONSE=$(curl -s -X POST "$ALLURE_SERVER/allure-docker-service/projects" \
        -H "Content-Type: application/json" \
        -d "{\"id\": \"$PROJECT_ID\"}" \
        -w "\n%{http_code}")

    HTTP_CODE=$(echo "$RESPONSE" | tail -n1)
    BODY=$(echo "$RESPONSE" | sed '$d')

    if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
        echo -e "${GREEN}✅ Project ready: $PROJECT_ID${NC}"
    elif echo "$BODY" | grep -q "already exists"; then
        echo -e "${GREEN}✅ Project already exists: $PROJECT_ID${NC}"
    else
        echo -e "${YELLOW}⚠️  Warning: Unexpected response (continuing anyway)${NC}"
    fi
    echo ""
}

# Function to clean previous results
clean_results() {
    echo -e "${YELLOW}🧹 Cleaning previous results...${NC}"

    curl -s -X GET "$ALLURE_SERVER/allure-docker-service/clean-results?project_id=$PROJECT_ID" > /dev/null

    echo -e "${GREEN}✅ Previous results cleaned${NC}"
    echo ""
}

# Function to upload results
upload_results() {
    echo -e "${YELLOW}📤 Uploading result files...${NC}"

    cd "$RESULTS_DIR"

    FILE_COUNT=0
    SUCCESS_COUNT=0
    FAILED_COUNT=0

    # Count total files
    TOTAL_FILES=$(ls -1 *.json *.properties 2>/dev/null | wc -l)

    if [ "$TOTAL_FILES" -eq 0 ]; then
        echo -e "${RED}❌ No result files found in $RESULTS_DIR${NC}"
        exit 1
    fi

    echo -e "  Total files to upload: ${GREEN}$TOTAL_FILES${NC}"
    echo ""

    # Upload each file
    for file in *.json *.properties 2>/dev/null; do
        if [ -f "$file" ]; then
            FILE_COUNT=$((FILE_COUNT + 1))

            # Show progress
            printf "  [%3d/%3d] Uploading: %-50s " "$FILE_COUNT" "$TOTAL_FILES" "$file"

            # Upload file
            HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" \
                -X POST "$ALLURE_SERVER/allure-docker-service/send-results?project_id=$PROJECT_ID" \
                -H "Content-Type: multipart/form-data" \
                -F "files[]=@$file")

            if [ "$HTTP_CODE" = "200" ]; then
                echo -e "${GREEN}✅${NC}"
                SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
            else
                echo -e "${RED}❌ (HTTP $HTTP_CODE)${NC}"
                FAILED_COUNT=$((FAILED_COUNT + 1))
            fi
        fi
    done

    echo ""
    echo -e "${GREEN}✅ Upload complete:${NC}"
    echo -e "  Successful: ${GREEN}$SUCCESS_COUNT${NC}"
    if [ "$FAILED_COUNT" -gt 0 ]; then
        echo -e "  Failed:     ${RED}$FAILED_COUNT${NC}"
    fi
    echo ""

    # Go back to original directory
    cd - > /dev/null
}

# Function to generate report
generate_report() {
    echo -e "${YELLOW}📈 Generating Allure report...${NC}"

    RESPONSE=$(curl -s -X GET "$ALLURE_SERVER/allure-docker-service/generate-report?project_id=$PROJECT_ID&execution_name=$EXECUTION_NAME&execution_from=$EXECUTION_FROM&execution_type=$EXECUTION_TYPE")

    # Check if generation was successful
    if echo "$RESPONSE" | grep -q "successfully"; then
        echo -e "${GREEN}✅ Report generated successfully${NC}"
    else
        echo -e "${RED}❌ Report generation failed${NC}"
        echo -e "${RED}Response: $RESPONSE${NC}"
        exit 1
    fi
    echo ""
}

# Function to display report URLs
show_report_urls() {
    REPORT_URL="$ALLURE_SERVER/allure-docker-service/projects/$PROJECT_ID/reports/latest/index.html"
    API_URL="$ALLURE_SERVER/allure-docker-service/swagger"

    echo -e "${BLUE}╔════════════════════════════════════════════════════════╗${NC}"
    echo -e "${BLUE}║       🎉 Success! Report Ready                        ║${NC}"
    echo -e "${BLUE}╚════════════════════════════════════════════════════════╝${NC}"
    echo ""
    echo -e "${GREEN}🔗 Report URLs:${NC}"
    echo ""
    echo -e "  ${BLUE}Latest Report:${NC}"
    echo -e "  ${GREEN}$REPORT_URL${NC}"
    echo ""
    echo -e "  ${BLUE}API Documentation:${NC}"
    echo -e "  ${GREEN}$API_URL${NC}"
    echo ""
    echo -e "${YELLOW}💡 Tip: Bookmark the report URL for easy access!${NC}"
    echo ""
}

# Main execution
main() {
    check_server
    create_project
    clean_results
    upload_results
    generate_report
    show_report_urls

    # Open report in browser (optional, comment out if not needed)
    if command -v open &> /dev/null; then
        echo -e "${YELLOW}🌐 Opening report in browser...${NC}"
        open "$ALLURE_SERVER/allure-docker-service/projects/$PROJECT_ID/reports/latest/index.html"
    elif command -v xdg-open &> /dev/null; then
        echo -e "${YELLOW}🌐 Opening report in browser...${NC}"
        xdg-open "$ALLURE_SERVER/allure-docker-service/projects/$PROJECT_ID/reports/latest/index.html"
    fi
}

# Run main function
main
