# GitHub Actions Setup Guide

## ✅ Current Working Setup

Your repository uses **GitHub Pages** for Allure reports because GitHub Actions runs in the cloud and cannot reach your local VM.

### Available Workflows:

| Workflow | Status | Purpose |
|----------|--------|---------|
| `run-tests-github-pages.yml` | ✅ **ACTIVE** | Runs tests and publishes Allure report to GitHub Pages |
| `test-automation.yml` | ✅ ACTIVE | Full CI/CD pipeline with multiple test suites |
| `run-tests-allure.yml.disabled` | ❌ DISABLED | Requires publicly accessible Allure server |

---

## 🚀 Quick Start

### Step 1: Enable GitHub Pages

1. Go to: https://github.com/siarheistar/SDET_CS_example/settings/pages
2. **Source:** Select "GitHub Actions"
3. Click **Save**

### Step 2: Run Tests

1. Go to: https://github.com/siarheistar/SDET_CS_example/actions
2. Click **"Run Tests with GitHub Pages Report"**
3. Click **"Run workflow"** → Select `main` → **Run**

### Step 3: View Report

After workflow completes (~3-5 minutes):

**Report URL:**
```
https://siarheistar.github.io/SDET_CS_example/
```

---

## 📊 Why Not Use Local Allure Server?

**Problem:** GitHub Actions runs on GitHub's cloud servers (not your Mac).

```
GitHub Actions (Cloud)  ❌  Cannot reach  ❌  Your VM (10.211.55.12)
     │                                            │
     └── Public Internet ──────────────────────── Private Network
```

**Your local VM IP `10.211.55.12`** is only accessible:
- ✅ From your Mac
- ✅ From devices on your local network
- ❌ NOT from GitHub Actions (cloud)

---

## 🌐 Solutions Comparison

### Option 1: GitHub Pages (Current Setup) ⭐ RECOMMENDED

**How it works:**
```
GitHub Actions → Generate Allure Report → Deploy to GitHub Pages → Public URL
```

**Pros:**
- ✅ Zero configuration
- ✅ Free hosting
- ✅ Automatic HTTPS
- ✅ Always available
- ✅ Works immediately

**Cons:**
- ⚠️ Requires public repository (or GitHub Enterprise)
- ⚠️ 2-3 minute deployment delay

**Report URL:** `https://siarheistar.github.io/SDET_CS_example/`

---

### Option 2: Expose Allure Server to Internet

To use your local Allure server with GitHub Actions, you need to make it publicly accessible.

#### A) Using ngrok (Quick & Easy)

**On your Debian VM:**
```bash
# Install ngrok
curl -s https://ngrok-agent.s3.amazonaws.com/ngrok.asc | sudo tee /etc/apt/trusted.gpg.d/ngrok.asc
echo "deb https://ngrok-agent.s3.amazonaws.com buster main" | sudo tee /etc/apt/sources.list.d/ngrok.list
sudo apt update && sudo apt install ngrok

# Get auth token from: https://dashboard.ngrok.com/get-started/your-authtoken
ngrok config add-authtoken YOUR_AUTH_TOKEN

# Expose Allure server
ngrok http 5050
```

**Output:**
```
Forwarding   https://abc123xyz.ngrok.io -> http://localhost:5050
```

**Then:**
1. Copy the `https://abc123xyz.ngrok.io` URL
2. Add GitHub secret: `ALLURE_SERVER_URL` = `https://abc123xyz.ngrok.io`
3. Re-enable workflow:
   ```bash
   mv .github/workflows/run-tests-allure.yml.disabled .github/workflows/run-tests-allure.yml
   git add . && git commit -m "Enable Allure Server workflow with ngrok" && git push
   ```

**Pros:**
- ✅ Quick setup (5 minutes)
- ✅ Works with private repos
- ✅ HTTPS included

**Cons:**
- ⚠️ URL changes on ngrok restart (free tier)
- ⚠️ Must keep ngrok running
- ⚠️ Rate limits on free tier

---

#### B) Router Port Forwarding (Permanent)

**Steps:**
1. Find your public IP: https://whatismyipaddress.com
2. Login to your router (usually `192.168.1.1` or `10.0.0.1`)
3. Setup port forwarding:
   - **External Port:** 5050
   - **Internal IP:** 10.211.55.12
   - **Internal Port:** 5050
   - **Protocol:** TCP
4. Add GitHub secret: `ALLURE_SERVER_URL` = `http://YOUR_PUBLIC_IP:5050`
5. Re-enable workflow (same as ngrok option above)

**Pros:**
- ✅ Permanent URL
- ✅ No third-party service
- ✅ Works with private repos

**Cons:**
- ⚠️ Exposes your VM to internet (security risk!)
- ⚠️ Requires static IP or dynamic DNS
- ⚠️ Router configuration needed

**Security Recommendations:**
- Enable authentication on Allure server
- Use firewall rules to limit access
- Consider using VPN instead

---

### Option 3: GitHub Artifacts (Simplest)

**How it works:**
Tests run → Artifacts uploaded → Download manually

**Already configured!** Just:
1. Run any workflow
2. Scroll to **Artifacts** section
3. Download `allure-report.zip`
4. Extract and open `index.html`

**Pros:**
- ✅ Already working
- ✅ No setup needed
- ✅ Works with private repos

**Cons:**
- ⚠️ Manual download required
- ⚠️ Not a shareable URL
- ⚠️ 90-day retention

---

## 🎯 Recommendations

| Your Situation | Best Option |
|----------------|-------------|
| **Demo to manager (public repo OK)** | ✅ **GitHub Pages** |
| **Private repo, need URL** | ngrok or port forwarding |
| **Internal team only** | Artifacts |
| **Enterprise setup** | Self-hosted GitHub Actions runner on your network |

---

## 🔧 Current Configuration

### Local VM Allure Server:
- **URL:** http://10.211.55.12:5050
- **Access:** Local network only
- **Status:** Running ✅
- **Use:** Manual uploads from Mac

### GitHub Actions:
- **Active Workflow:** GitHub Pages
- **Report URL:** https://siarheistar.github.io/SDET_CS_example/
- **Updates:** Automatic on push to `main`

---

## 📝 Manual Local Testing

You can still use your local Allure server for development:

```bash
# Run tests locally
docker-compose up --build unit-api-tests

# Upload to local Allure server
./scripts/upload-to-allure-server.sh http://10.211.55.12:5050 sdet-cs-framework

# View report
open http://10.211.55.12:5050/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html
```

---

## 🎓 Understanding the Architecture

### Local Development:
```
Your Mac → Docker Tests → Allure Results → Local VM Server → View in Browser
   ✅         ✅              ✅               ✅                 ✅
```

### GitHub Actions (before fix):
```
GitHub Cloud → Tests → Allure Results → Local VM (10.211.55.12)
     ✅          ✅         ✅                    ❌ Cannot reach!
```

### GitHub Actions (current working):
```
GitHub Cloud → Tests → Allure Results → GitHub Pages → Public URL
     ✅          ✅         ✅               ✅            ✅
```

---

## 🚀 Next Steps

1. ✅ **Enable GitHub Pages** (see Step 1 above)
2. ✅ **Run workflow** (see Step 2 above)
3. ✅ **View report** at `https://siarheistar.github.io/SDET_CS_example/`
4. 📧 **Share URL** with your manager
5. ⭐ **Bookmark** the report URL

---

## ❓ FAQ

**Q: Can I use both local server AND GitHub Pages?**
A: Yes! Local server for development, GitHub Pages for CI/CD.

**Q: Why is my repo public now?**
A: GitHub Pages free tier requires public repos. For private repos, use GitHub Enterprise or ngrok.

**Q: How do I update the report?**
A: Just push code to `main` branch - report auto-updates in 3-5 minutes.

**Q: Can I customize the report?**
A: Yes! Edit tests with more `[AllureDescription]`, `[AllureSeverity]`, etc.

---

For more details, see:
- [Allure Solutions Comparison](docs/ALLURE_SOLUTIONS_COMPARISON.md)
- [GitHub Setup Guide](docs/GITHUB_SETUP_GUIDE.md)
- [Quick Start](docs/ALLURE_QUICK_START.md)
