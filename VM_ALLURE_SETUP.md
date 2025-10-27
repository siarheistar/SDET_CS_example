# Allure Server on VM - Setup Guide

This guide shows how to expose your VM's Allure server so GitHub Actions can upload test results.

## Problem
GitHub Actions runs in the cloud and cannot reach your VM's private IP (10.211.55.12:5050).

## Solution
Use **ngrok** to create a public URL that forwards to your VM's Allure server.

---

## Step 1: Install ngrok on Debian VM

SSH to your VM (`10.211.55.12`) and run:

```bash
# Download and install ngrok
curl -s https://ngrok-agent.s3.amazonaws.com/ngrok.asc | \
  sudo tee /etc/apt/trusted.gpg.d/ngrok.asc >/dev/null

echo "deb https://ngrok-agent.s3.amazonaws.com buster main" | \
  sudo tee /etc/apt/sources.list.d/ngrok.list

sudo apt update
sudo apt install ngrok
```

**Alternative (manual download for ARM64):**
```bash
cd ~
wget https://bin.equinox.io/c/bNyj1mQVY4c/ngrok-v3-stable-linux-arm64.tgz
tar -xvzf ngrok-v3-stable-linux-arm64.tgz
sudo mv ngrok /usr/local/bin/
```

---

## Step 2: Get ngrok Auth Token

1. Go to: https://dashboard.ngrok.com/signup
2. Sign up (free account)
3. Copy your authtoken from: https://dashboard.ngrok.com/get-started/your-authtoken
4. Configure ngrok:

```bash
ngrok config add-authtoken YOUR_TOKEN_HERE
```

---

## Step 3: Start Allure Server (if not running)

```bash
cd ~/allure-server
docker-compose up -d allure
```

Verify it's running:
```bash
curl http://localhost:5050/allure-docker-service/version
```

Should return: `{"data":{"version":"2.27.0"}}`

---

## Step 4: Expose Allure Server with ngrok

```bash
ngrok http 5050
```

You'll see output like:
```
Session Status                online
Account                       your@email.com
Version                       3.x.x
Region                        United States (us)
Forwarding                    https://abc123def456.ngrok.io -> http://localhost:5050
```

**COPY THE HTTPS URL!** (e.g., `https://abc123def456.ngrok.io`)

**Keep this terminal open** - closing it will stop the tunnel.

---

## Step 5: Test ngrok URL

From your Mac, test the public URL:

```bash
curl https://YOUR-NGROK-URL.ngrok.io/allure-docker-service/version
```

Should return version info. If it works, ngrok is set up correctly!

---

## Step 6: Add ngrok URL to GitHub Secrets

1. Go to: https://github.com/siarheistar/SDET_CS_example/settings/secrets/actions
2. Click "New repository secret"
3. Name: `ALLURE_SERVER_URL`
4. Value: `https://YOUR-NGROK-URL.ngrok.io` (YOUR actual ngrok URL)
5. Click "Add secret"

---

## Step 7: Enable the Workflow

On your Mac:

```bash
cd /Users/sergei/Projects/SDET_CS_example
mv .github/workflows/run-tests-allure.yml.disabled .github/workflows/run-tests-allure.yml
git add .github/workflows/run-tests-allure.yml
git commit -m "Enable Allure server workflow"
git push origin main
```

---

## Step 8: Watch it Work!

GitHub Actions will now:
1. Run tests
2. Generate TRX files
3. Convert to Allure format
4. Upload to your VM via ngrok
5. Allure server generates the report

**View Report:**
- https://YOUR-NGROK-URL.ngrok.io/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html

---

## Keep ngrok Running Permanently

### Option A: Run ngrok in background with systemd

Create service file:
```bash
sudo nano /etc/systemd/system/ngrok.service
```

Content:
```ini
[Unit]
Description=ngrok tunnel
After=network.target

[Service]
Type=simple
User=parallels
WorkingDirectory=/home/parallels
ExecStart=/usr/local/bin/ngrok http 5050
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

Enable and start:
```bash
sudo systemctl daemon-reload
sudo systemctl enable ngrok
sudo systemctl start ngrok
```

Check status:
```bash
sudo systemctl status ngrok
```

Get the URL:
```bash
curl http://localhost:4040/api/tunnels | jq '.tunnels[0].public_url'
```

### Option B: Run in screen/tmux

```bash
# Install screen
sudo apt install screen

# Start screen session
screen -S ngrok

# Run ngrok
ngrok http 5050

# Detach: Press Ctrl+A, then D
# Reattach later: screen -r ngrok
```

---

## Troubleshooting

### ngrok URL changes on restart
- Free ngrok URLs change every time you restart
- Update GitHub secret with new URL each time
- Or upgrade to ngrok paid plan for static URLs

### Can't reach Allure server
```bash
# Check if Allure is running
docker ps | grep allure

# Check ngrok status
curl http://localhost:4040/api/tunnels

# Test locally first
curl http://localhost:5050/allure-docker-service/version
```

### GitHub Actions fails to upload
- Check ngrok is running: `curl http://localhost:4040/api/tunnels`
- Check GitHub secret is set correctly
- Check ngrok URL in secret includes `https://`

---

## Summary

Once set up:
1. ✅ VM runs Allure server (port 5050)
2. ✅ ngrok exposes it publicly
3. ✅ GitHub Actions uploads results
4. ✅ Allure generates beautiful reports
5. ✅ You access via ngrok URL

**Report URL Format:**
`https://YOUR-NGROK-URL.ngrok.io/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html`
