# Allure Report Server Setup on Linux VM

## Architecture Overview

```
GitHub Actions Pipeline → (HTTP POST) → Allure Docker Service (VM) → Web UI
```

## Step 1: Install Docker on Linux VM

```bash
# Update system
sudo apt-get update
sudo apt-get install -y ca-certificates curl gnupg lsb-release

# Add Docker's official GPG key
sudo mkdir -p /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# Set up Docker repository
echo \
  "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
  $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# Install Docker
sudo apt-get update
sudo apt-get install -y docker-ce docker-ce-cli containerd.io docker-compose-plugin

# Enable Docker to start on boot
sudo systemctl enable docker
sudo systemctl start docker

# Add current user to docker group (logout/login required)
sudo usermod -aG docker $USER
```

## Step 2: Create Allure Server Directory Structure

```bash
# Create directory for persistent storage
mkdir -p ~/allure-server/{results,reports}
cd ~/allure-server
```

## Step 3: Create Docker Compose Configuration

Create `docker-compose.yml`:

```yaml
version: '3.8'

services:
  allure:
    image: frankescobar/allure-docker-service:latest
    container_name: allure-server
    restart: unless-stopped
    ports:
      - "5050:5050"  # API port
      - "5252:5252"  # UI port (optional, for direct access)
    environment:
      # Check results every 3 seconds for new reports
      CHECK_RESULTS_EVERY_SECONDS: 3

      # Keep historical data
      KEEP_HISTORY: "TRUE"
      KEEP_HISTORY_LATEST: 25

      # Security token for API access
      SECURITY_USER: "admin"
      SECURITY_PASS: "your-secure-password-here"

      # Enable CORS for web access
      ENABLE_CORS: "TRUE"

      # Auto-generate report on result upload
      MAKE_VIEWER_ENDPOINTS_PUBLIC: "TRUE"

    volumes:
      # Persistent storage for projects
      - ./projects:/app/projects

      # Custom configuration (optional)
      - ./config:/app/config

    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5050/allure-docker-service/version"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

  # Optional: Nginx reverse proxy with SSL
  nginx:
    image: nginx:alpine
    container_name: allure-nginx
    restart: unless-stopped
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf:ro
      - ./ssl:/etc/nginx/ssl:ro
    depends_on:
      - allure
```

## Step 4: Start Allure Server

```bash
# Start the service
docker-compose up -d

# Check logs
docker-compose logs -f allure

# Verify service is running
curl http://localhost:5050/allure-docker-service/version
```

## Step 5: Configure Firewall (If Applicable)

```bash
# Allow HTTP/HTTPS traffic
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Allow Allure API port (if accessing directly)
sudo ufw allow 5050/tcp

# Enable firewall
sudo ufw enable
```

## Step 6: Access Allure UI

- **API Endpoint:** `http://your-vm-ip:5050`
- **Web UI:** `http://your-vm-ip:5050/allure-docker-service/latest-report` (after first report upload)
- **Project Reports:** `http://your-vm-ip:5050/allure-docker-service/projects/{project-id}/reports/latest/index.html`

## Step 7: Test Manual Upload

```bash
# Create a project
curl -X POST http://localhost:5050/allure-docker-service/projects \
  -H "Content-Type: application/json" \
  -d '{"id": "sdet-cs-framework"}'

# Upload test results (from your local machine)
cd /path/to/your/allure-results

# Clean previous results for project
curl -X GET "http://your-vm-ip:5050/allure-docker-service/clean-results?project_id=sdet-cs-framework"

# Upload each result file
for file in *.json; do
  curl -X POST "http://your-vm-ip:5050/allure-docker-service/send-results?project_id=sdet-cs-framework" \
    -H "Content-Type: multipart/form-data" \
    -F "files[]=@$file"
done

# Generate report
curl -X GET "http://your-vm-ip:5050/allure-docker-service/generate-report?project_id=sdet-cs-framework&execution_name=manual-test&execution_from=local&execution_type=manual"

# View report at:
# http://your-vm-ip:5050/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html
```

## Step 8: Setup System Service (Optional - for auto-restart)

Create `/etc/systemd/system/allure-server.service`:

```ini
[Unit]
Description=Allure Docker Service
Requires=docker.service
After=docker.service

[Service]
Type=oneshot
RemainAfterExit=yes
WorkingDirectory=/home/YOUR_USERNAME/allure-server
ExecStart=/usr/bin/docker-compose up -d
ExecStop=/usr/bin/docker-compose down
TimeoutStartSec=0

[Install]
WantedBy=multi-user.target
```

Enable the service:

```bash
sudo systemctl enable allure-server
sudo systemctl start allure-server
```

## Step 9: Setup SSL with Let's Encrypt (Production)

```bash
# Install certbot
sudo apt-get install -y certbot

# Get SSL certificate (requires domain name)
sudo certbot certonly --standalone -d allure.yourdomain.com

# Certificates will be in: /etc/letsencrypt/live/allure.yourdomain.com/
```

Create `nginx.conf` for HTTPS:

```nginx
events {
    worker_connections 1024;
}

http {
    upstream allure {
        server allure:5050;
    }

    server {
        listen 80;
        server_name allure.yourdomain.com;
        return 301 https://$server_name$request_uri;
    }

    server {
        listen 443 ssl http2;
        server_name allure.yourdomain.com;

        ssl_certificate /etc/nginx/ssl/fullchain.pem;
        ssl_certificate_key /etc/nginx/ssl/privkey.pem;

        client_max_body_size 100M;

        location / {
            proxy_pass http://allure;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
```

## Maintenance Commands

```bash
# View logs
docker-compose logs -f allure

# Restart service
docker-compose restart

# Update to latest version
docker-compose pull
docker-compose up -d

# Backup reports
tar -czf allure-backup-$(date +%Y%m%d).tar.gz projects/

# Clean old reports (keeps latest 25)
# Automatic via KEEP_HISTORY_LATEST setting
```

## Troubleshooting

### Issue: Cannot access from external network
```bash
# Check if service is running
docker ps | grep allure

# Check firewall
sudo ufw status

# Check if port is listening
sudo netstat -tlnp | grep 5050
```

### Issue: Upload fails with large files
```bash
# Increase client_max_body_size in nginx.conf
# Or increase Docker container limits
```

### Issue: Reports not generating
```bash
# Check Allure container logs
docker logs allure-server

# Verify results were uploaded
curl http://localhost:5050/allure-docker-service/projects/sdet-cs-framework
```

## Security Recommendations

1. **Enable Authentication:** Use `SECURITY_USER` and `SECURITY_PASS` environment variables
2. **Use HTTPS:** Configure SSL certificates via Let's Encrypt
3. **Firewall Rules:** Restrict access to specific IP ranges if possible
4. **Regular Updates:** Keep Docker images updated
5. **Backup Strategy:** Automated backups of `projects/` directory

## API Documentation

Full API documentation available at:
`http://your-vm-ip:5050/allure-docker-service/swagger`
