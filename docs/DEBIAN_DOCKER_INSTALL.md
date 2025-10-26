# Docker Installation on Debian 12 (Bookworm)

## Issue You Encountered

The error `Package 'docker-ce' has no installation candidate` means the Docker repository wasn't added to your sources list.

## Correct Installation Steps for Debian 12

### Step 1: Remove Old Docker Versions (If Any)

```bash
sudo apt-get remove -y docker docker-engine docker.io containerd runc
```

### Step 2: Update Package Index

```bash
sudo apt-get update
```

### Step 3: Install Prerequisites

```bash
sudo apt-get install -y \
    ca-certificates \
    curl \
    gnupg \
    lsb-release
```

### Step 4: Add Docker's Official GPG Key

```bash
# Create directory for keyrings
sudo install -m 0755 -d /etc/apt/keyrings

# Download and add Docker's GPG key
curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# Set proper permissions
sudo chmod a+r /etc/apt/keyrings/docker.gpg
```

### Step 5: Add Docker Repository to Sources

```bash
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
```

### Step 6: Update Package Index Again

```bash
sudo apt-get update
```

### Step 7: Install Docker Engine

```bash
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
```

### Step 8: Verify Installation

```bash
# Check Docker version
docker --version

# Check Docker Compose version
docker compose version

# Run hello-world test
sudo docker run hello-world
```

**Expected output:**
```
Hello from Docker!
This message shows that your installation appears to be working correctly.
```

### Step 9: Add Your User to Docker Group (Optional but Recommended)

This allows you to run Docker commands without `sudo`:

```bash
# Add current user to docker group
sudo usermod -aG docker $USER

# Apply group changes (or logout/login)
newgrp docker

# Test without sudo
docker run hello-world
```

### Step 10: Enable Docker to Start on Boot

```bash
sudo systemctl enable docker.service
sudo systemctl enable containerd.service
```

---

## Complete One-Line Installation Script

Copy and paste this entire block:

```bash
# Remove old versions
sudo apt-get remove -y docker docker-engine docker.io containerd runc

# Update and install prerequisites
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg lsb-release

# Add Docker GPG key
sudo install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
sudo chmod a+r /etc/apt/keyrings/docker.gpg

# Add Docker repository
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
  $(. /etc/os-release && echo "$VERSION_CODENAME") stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Update and install Docker
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

# Add user to docker group
sudo usermod -aG docker $USER

# Enable Docker on boot
sudo systemctl enable docker.service
sudo systemctl enable containerd.service

# Verify installation
echo ""
echo "=========================================="
echo "Docker Installation Complete!"
echo "=========================================="
docker --version
docker compose version
echo ""
echo "⚠️  IMPORTANT: Please logout and login again to use docker without sudo"
echo "Or run: newgrp docker"
echo ""
```

---

## Troubleshooting

### Issue: "Permission denied while trying to connect to Docker daemon"

**Solution:**
```bash
# Add user to docker group
sudo usermod -aG docker $USER

# Logout and login, OR:
newgrp docker

# Test
docker ps
```

### Issue: "Cannot connect to the Docker daemon"

**Solution:**
```bash
# Start Docker service
sudo systemctl start docker

# Check status
sudo systemctl status docker

# Enable on boot
sudo systemctl enable docker
```

### Issue: Repository not found errors

**Solution:**
```bash
# Check your Debian version
cat /etc/os-release

# If VERSION_CODENAME is empty, manually specify "bookworm"
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/debian \
  bookworm stable" | \
  sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

sudo apt-get update
```

---

## After Docker Installation: Setup Allure Server

Once Docker is installed, proceed with Allure setup:

```bash
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

# Start Allure service
docker compose up -d

# Wait a few seconds
sleep 5

# Check if running
docker ps

# Test API
curl http://localhost:5050/allure-docker-service/version

# Get your VM IP address
ip addr show | grep "inet " | grep -v "127.0.0.1"
```

**Expected output:**
```json
{
  "data": {
    "version": "2.x.x"
  }
}
```

---

## Verify Everything Works

```bash
# Check Docker is running
docker ps

# Check Allure container logs
docker logs allure-server

# Test Allure API
curl http://localhost:5050/allure-docker-service/version

# Check what port it's listening on
sudo netstat -tlnp | grep 5050

# Or using ss (modern alternative)
sudo ss -tlnp | grep 5050
```

**Expected output:**
```
tcp   0   0   0.0.0.0:5050   0.0.0.0:*   LISTEN   12345/docker-proxy
```

---

## Configure Firewall (If UFW is Active)

```bash
# Check if UFW is active
sudo ufw status

# If active, allow port 5050
sudo ufw allow 5050/tcp

# Reload firewall
sudo ufw reload
```

---

## Get Your VM IP Address for GitHub Actions

```bash
# Get IP address
hostname -I | awk '{print $1}'

# Or more detailed
ip addr show | grep "inet " | grep -v "127.0.0.1" | awk '{print $2}' | cut -d/ -f1
```

**Save this IP for GitHub Actions secret:**
```
ALLURE_SERVER_URL = http://YOUR_VM_IP:5050
```

For example:
```
ALLURE_SERVER_URL = http://192.168.1.100:5050
```

---

## Test from Your Local Machine

After getting the IP, test from your Mac:

```bash
# Replace with your VM IP
VM_IP="192.168.1.100"

# Test connection
curl http://$VM_IP:5050/allure-docker-service/version

# If successful, you'll see the version JSON
# If it fails, check:
# 1. Firewall on VM
# 2. Docker container is running
# 3. Network connectivity
```

---

## Quick Health Check Script

Create this script on your VM to quickly check everything:

```bash
cat > ~/check-allure.sh << 'EOF'
#!/bin/bash

echo "=========================================="
echo "Allure Server Health Check"
echo "=========================================="
echo ""

echo "1. Docker Status:"
if systemctl is-active --quiet docker; then
    echo "   ✅ Docker is running"
else
    echo "   ❌ Docker is not running"
    echo "   Fix: sudo systemctl start docker"
fi
echo ""

echo "2. Allure Container:"
if docker ps | grep -q allure-server; then
    echo "   ✅ Allure container is running"
else
    echo "   ❌ Allure container is not running"
    echo "   Fix: cd ~/allure-server && docker compose up -d"
fi
echo ""

echo "3. API Response:"
if curl -s http://localhost:5050/allure-docker-service/version > /dev/null; then
    echo "   ✅ API is responding"
    VERSION=$(curl -s http://localhost:5050/allure-docker-service/version | grep -o '"version":"[^"]*"' | cut -d'"' -f4)
    echo "   Version: $VERSION"
else
    echo "   ❌ API is not responding"
fi
echo ""

echo "4. VM IP Address:"
IP=$(hostname -I | awk '{print $1}')
echo "   $IP"
echo ""

echo "5. Port Listening:"
if sudo ss -tlnp | grep -q :5050; then
    echo "   ✅ Port 5050 is listening"
else
    echo "   ❌ Port 5050 is not listening"
fi
echo ""

echo "=========================================="
echo "Access URLs:"
echo "=========================================="
echo "From VM:      http://localhost:5050"
echo "From Network: http://$IP:5050"
echo "API Docs:     http://$IP:5050/allure-docker-service/swagger"
echo ""
EOF

chmod +x ~/check-allure.sh
~/check-allure.sh
```

---

## Next Steps

After Docker and Allure are installed:

1. ✅ Run the health check script: `~/check-allure.sh`
2. ✅ Note your VM IP address
3. ✅ Test from your Mac: `curl http://VM_IP:5050/allure-docker-service/version`
4. ✅ Add GitHub secret: `ALLURE_SERVER_URL = http://VM_IP:5050`
5. ✅ Push workflow and run first test

---

## Common Issues After Installation

### Allure container keeps restarting

```bash
# Check logs
docker logs allure-server

# Common fix: Permission issues with volumes
sudo chown -R $USER:$USER ~/allure-server/projects
```

### Can't access from network

```bash
# Check if Docker is binding to all interfaces
docker port allure-server

# Expected output:
# 5050/tcp -> 0.0.0.0:5050

# If shows 127.0.0.1:5050, update docker-compose.yml:
# ports:
#   - "0.0.0.0:5050:5050"
```

### Firewall blocking

```bash
# Temporarily disable firewall to test
sudo ufw disable

# Test connection from Mac
curl http://VM_IP:5050/allure-docker-service/version

# If works, re-enable and add rule
sudo ufw enable
sudo ufw allow 5050/tcp
```

---

**Installation Complete!** 🎉

You're now ready to use the Allure server with GitHub Actions.
