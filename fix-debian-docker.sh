#!/bin/bash

################################################################################
# Fixed Allure Server Setup Script for Debian 12
# This version removes conflicting Ubuntu Docker repos first
################################################################################

set -e

echo "=========================================="
echo "Allure Server Setup for Debian 12"
echo "=========================================="
echo ""

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Step 0: Remove old/conflicting Docker repositories
echo -e "${YELLOW}Step 0/10: Removing old/conflicting Docker repositories...${NC}"
sudo rm -f /etc/apt/sources.list.d/docker.list 2>/dev/null || true
sudo rm -f /etc/apt/sources.list.d/archive_uri-* 2>/dev/null || true
sudo rm -f /etc/apt/keyrings/docker.gpg 2>/dev/null || true
echo -e "${GREEN}✅ Done${NC}\n"

# Step 1: Remove old Docker versions
echo -e "${YELLOW}Step 1/10: Removing old Docker versions...${NC}"
sudo apt-get remove -y docker docker-engine docker.io containerd runc 2>/dev/null || true
echo -e "${GREEN}✅ Done${NC}\n"

# Step 2: Update package index
echo -e "${YELLOW}Step 2/10: Updating package index...${NC}"
sudo apt-get update
echo -e "${GREEN}✅ Done${NC}\n"

# Step 3: Install prerequisites
echo -e "${YELLOW}Step 3/10: Installing prerequisites...${NC}"
sudo apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release
echo -e "${GREEN}✅ Done${NC}\n"

# Step 4: Add Docker's GPG key for DEBIAN (not Ubuntu)
echo -e "${YELLOW}Step 4/10: Adding Docker's GPG key for Debian...${NC}"
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg
echo -e "${GREEN}✅ Done${NC}\n"

# Step 5: Add Docker repository for DEBIAN (explicitly)
echo -e "${YELLOW}Step 5/10: Adding Docker repository for Debian...${NC}"
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
  bookworm stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
echo -e "${GREEN}✅ Done${NC}\n"

# Step 6: Update package index again
echo -e "${YELLOW}Step 6/10: Updating package index with Docker repo...${NC}"
sudo apt-get update
echo -e "${GREEN}✅ Done${NC}\n"

# Step 7: Install Docker
echo -e "${YELLOW}Step 7/10: Installing Docker Engine...${NC}"
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
echo -e "${GREEN}✅ Done${NC}\n"

# Step 8: Configure user permissions
echo -e "${YELLOW}Step 8/10: Adding user to docker group...${NC}"
sudo usermod -aG docker $USER
echo -e "${GREEN}✅ Done${NC}\n"

# Step 9: Enable Docker on boot
echo -e "${YELLOW}Step 9/10: Enabling Docker on boot...${NC}"
sudo systemctl enable docker.service
sudo systemctl enable containerd.service
sudo systemctl start docker.service
echo -e "${GREEN}✅ Done${NC}\n"

# Step 10: Setup Allure Server
echo -e "${YELLOW}Step 10/10: Setting up Allure Server...${NC}"

# Create directory
mkdir -p ~/allure-server
cd ~/allure-server

# Create docker-compose.yml
cat > docker-compose.yml << 'EOF'
version: '3.8'

services:
  allure:
    image: frankescobar/allure-docker-service:latest
    container_name: allure-server
    restart: unless-stopped
    ports:
      - "5050:5050"
    environment:
      CHECK_RESULTS_EVERY_SECONDS: 3
      KEEP_HISTORY: "TRUE"
      KEEP_HISTORY_LATEST: 25
      SECURITY_USER: "admin"
      SECURITY_PASS: "change-this-password"
      ENABLE_CORS: "TRUE"
      MAKE_VIEWER_ENDPOINTS_PUBLIC: "TRUE"
    volumes:
      - ./projects:/app/projects
EOF

# Start Allure service (using sudo for first time)
sudo docker compose up -d

echo -e "${GREEN}✅ Done${NC}\n"

# Wait for service to start
echo -e "${YELLOW}Waiting for Allure service to start...${NC}"
sleep 10

# Verify installation
echo ""
echo "=========================================="
echo "Installation Summary"
echo "=========================================="
echo ""

# Docker version
echo -e "${YELLOW}Docker Version:${NC}"
docker --version
docker compose version
echo ""

# Allure container status
echo -e "${YELLOW}Allure Container Status:${NC}"
sudo docker ps | grep allure || echo -e "${RED}Container not running!${NC}"
echo ""

# Test API
echo -e "${YELLOW}Allure API Test:${NC}"
sleep 3
if curl -s http://localhost:5050/allure-docker-service/version > /dev/null; then
    echo -e "${GREEN}✅ API is responding${NC}"
    VERSION=$(curl -s http://localhost:5050/allure-docker-service/version | grep -o '"version":"[^"]*"' | cut -d'"' -f4)
    echo -e "Version: ${GREEN}$VERSION${NC}"
else
    echo -e "${RED}❌ API is not responding${NC}"
    echo "Checking logs:"
    sudo docker logs allure-server --tail 20
fi
echo ""

# Get IP address
VM_IP=$(hostname -I | awk '{print $1}')
echo -e "${YELLOW}VM IP Address:${NC} ${GREEN}$VM_IP${NC}"
echo ""

# Create health check script
cat > ~/check-allure.sh << 'HEALTHCHECK'
#!/bin/bash

GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m'

echo "=========================================="
echo "Allure Server Health Check"
echo "=========================================="
echo ""

echo "1. Docker Status:"
if systemctl is-active --quiet docker; then
    echo -e "   ${GREEN}✅ Docker is running${NC}"
else
    echo -e "   ${RED}❌ Docker is not running${NC}"
    echo "   Fix: sudo systemctl start docker"
fi
echo ""

echo "2. Allure Container:"
if docker ps | grep -q allure-server; then
    echo -e "   ${GREEN}✅ Allure container is running${NC}"
else
    echo -e "   ${RED}❌ Allure container is not running${NC}"
    echo "   Fix: cd ~/allure-server && docker compose up -d"
fi
echo ""

echo "3. API Response:"
if curl -s http://localhost:5050/allure-docker-service/version > /dev/null; then
    echo -e "   ${GREEN}✅ API is responding${NC}"
    VERSION=$(curl -s http://localhost:5050/allure-docker-service/version | grep -o '"version":"[^"]*"' | cut -d'"' -f4)
    echo "   Version: $VERSION"
else
    echo -e "   ${RED}❌ API is not responding${NC}"
fi
echo ""

echo "4. VM IP Address:"
IP=$(hostname -I | awk '{print $1}')
echo "   $IP"
echo ""

echo "5. Port Listening:"
if sudo ss -tlnp | grep -q :5050; then
    echo -e "   ${GREEN}✅ Port 5050 is listening${NC}"
else
    echo -e "   ${RED}❌ Port 5050 is not listening${NC}"
fi
echo ""

echo "=========================================="
echo "Access URLs:"
echo "=========================================="
echo "From VM:      http://localhost:5050"
echo "From Network: http://$IP:5050"
echo "API Docs:     http://$IP:5050/allure-docker-service/swagger"
echo ""

echo "=========================================="
echo "GitHub Actions Secret:"
echo "=========================================="
echo "ALLURE_SERVER_URL = http://$IP:5050"
echo ""
HEALTHCHECK

chmod +x ~/check-allure.sh

echo "=========================================="
echo "🎉 Installation Complete!"
echo "=========================================="
echo ""
echo -e "${GREEN}✅ Docker installed successfully${NC}"
echo -e "${GREEN}✅ Allure server is running${NC}"
echo ""
echo "Access URLs:"
echo "  • From VM:      http://localhost:5050"
echo "  • From Network: http://$VM_IP:5050"
echo "  • API Docs:     http://$VM_IP:5050/allure-docker-service/swagger"
echo ""
echo "GitHub Actions Secret (add this in GitHub):"
echo "  Name:  ALLURE_SERVER_URL"
echo "  Value: http://$VM_IP:5050"
echo ""
echo "Health Check:"
echo "  Run: ~/check-allure.sh"
echo ""
echo -e "${YELLOW}⚠️  IMPORTANT: Please logout and login again to use docker without sudo${NC}"
echo -e "   Or run: ${GREEN}newgrp docker${NC}"
echo ""
echo "Test from your Mac:"
echo "  curl http://$VM_IP:5050/allure-docker-service/version"
echo ""
