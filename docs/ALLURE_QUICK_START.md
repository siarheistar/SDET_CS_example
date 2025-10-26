# Allure Reporting - Quick Start Guide

## 🎯 What You Get

Automatic test reporting with beautiful, interactive Allure reports that show:
- ✅ Test results with pass/fail/skip statistics
- 📊 Trend graphs showing test stability over time
- 🎨 Rich test details with screenshots, logs, and attachments
- 📈 Historical data to track quality improvements
- 🔗 Shareable URLs for stakeholders and managers

---

## 🚀 Quick Start (5 Minutes)

### For Allure Server Solution (Recommended)

**Prerequisites:**
- Linux VM with Docker installed
- GitHub repository with test project

**Setup Steps:**

1. **On Your Linux VM:**
   ```bash
   # Create directory
   mkdir ~/allure-server && cd ~/allure-server

   # Create Docker Compose file
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
       volumes:
         - ./projects:/app/projects
   EOF

   # Start service
   docker-compose up -d

   # Verify (should show version)
   curl http://localhost:5050/allure-docker-service/version
   ```

2. **In GitHub Repository:**
   ```bash
   # Add secret
   # Repository → Settings → Secrets → Actions → New repository secret
   # Name: ALLURE_SERVER_URL
   # Value: http://your-vm-ip:5050

   # Workflow file already exists at:
   # .github/workflows/run-tests-allure.yml
   ```

3. **Run Test:**
   ```bash
   # Go to GitHub → Actions tab
   # Click "Run workflow"
   # Wait 3-5 minutes
   # Report URL appears in summary
   ```

**That's it!** Your reports are now auto-generated on every push.

---

## 📁 Files Included

### Core Files

| File | Purpose | You Need To |
|------|---------|-------------|
| `.github/workflows/run-tests-allure.yml` | GitHub Actions workflow for Allure Server | Add secret: `ALLURE_SERVER_URL` |
| `.github/workflows/run-tests-github-pages.yml` | Alternative: GitHub Pages workflow | Enable GitHub Pages in settings |
| `allureConfig.json` | Allure results directory config | Already configured ✅ |
| `docker-compose.yml` | Local test execution | Already configured ✅ |

### Documentation

| File | Description |
|------|-------------|
| `docs/ALLURE_SOLUTIONS_COMPARISON.md` | **START HERE** - Detailed comparison of all solutions |
| `docs/ALLURE_SERVER_SETUP.md` | Complete Linux VM server setup guide |
| `docs/GITHUB_SETUP_GUIDE.md` | Step-by-step GitHub configuration |
| `docs/ALLURE_QUICK_START.md` | This file - quick reference |

### Scripts

| File | Purpose |
|------|---------|
| `scripts/upload-to-allure-server.sh` | Manually upload results to server |

---

## 🎨 Solution Options Summary

### Option 1: Allure Server on Linux VM ⭐ **RECOMMENDED**

**Best for:** You have a Linux VM + private repository

**Pros:**
- ✅ Real-time reports (available in seconds)
- ✅ Works with private repos
- ✅ Automatic history/trends
- ✅ No external dependencies
- ✅ Full control

**Setup Time:** 30 minutes (one-time)

**Files:** `.github/workflows/run-tests-allure.yml`

**Documentation:** [`docs/ALLURE_SERVER_SETUP.md`](ALLURE_SERVER_SETUP.md)

---

### Option 2: GitHub Pages

**Best for:** Public repositories only

**Pros:**
- ✅ Zero infrastructure needed
- ✅ Free hosting by GitHub
- ✅ Simple setup (5 minutes)

**Cons:**
- ❌ Only for public repos
- ❌ 2-3 minute deployment delay
- ❌ Manual history management

**Setup Time:** 5 minutes

**Files:** `.github/workflows/run-tests-github-pages.yml`

---

### Option 3: Artifacts Only

**Best for:** Developer-only review

**Pros:**
- ✅ Simplest setup (1 minute)
- ✅ Works for private repos

**Cons:**
- ❌ Must download & extract locally
- ❌ Not suitable for managers
- ❌ No web interface

**Setup Time:** 1 minute (add one step to workflow)

---

### Option 4: Allure TestOps (Commercial)

**Best for:** Enterprise with budget

**Pros:**
- ✅ Advanced analytics
- ✅ Professional support
- ✅ Test management features

**Cons:**
- ❌ Costs $39+/month

**Setup Time:** 10 minutes

---

## 📖 Which Documentation to Read?

### I want to understand all options first
👉 Read: [`docs/ALLURE_SOLUTIONS_COMPARISON.md`](ALLURE_SOLUTIONS_COMPARISON.md)

Comprehensive comparison with:
- Detailed pros/cons for each solution
- Cost analysis
- Decision matrix
- Implementation timelines

---

### I'm ready to set up Allure Server
👉 Read: [`docs/ALLURE_SERVER_SETUP.md`](ALLURE_SERVER_SETUP.md)

Complete guide including:
- Docker installation
- Server configuration
- Security setup
- SSL/HTTPS configuration
- Troubleshooting

---

### I need to configure GitHub
👉 Read: [`docs/GITHUB_SETUP_GUIDE.md`](GITHUB_SETUP_GUIDE.md)

Step-by-step guide for:
- Adding GitHub secrets
- Enabling workflows
- Running first test
- Troubleshooting common issues

---

### I just want to run tests locally
👉 Use: [`scripts/upload-to-allure-server.sh`](../scripts/upload-to-allure-server.sh)

```bash
# Set server URL
export ALLURE_SERVER_URL="http://your-vm-ip:5050"

# Run tests
docker-compose up --build unit-api-tests

# Upload results
./scripts/upload-to-allure-server.sh
```

---

## 🔗 Important URLs

### After Setup, You'll Have:

**For Allure Server Solution:**
```
Report (always latest):
http://your-server:5050/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html

API Documentation:
http://your-server:5050/allure-docker-service/swagger

All Projects:
http://your-server:5050/allure-docker-service/projects
```

**For GitHub Pages Solution:**
```
Report:
https://[username].github.io/[repository-name]/
```

---

## 🎯 Quick Decision Guide

**Answer these questions:**

1. **Do you have a Linux VM available?**
   - ✅ Yes → Use Allure Server
   - ❌ No → Continue to question 2

2. **Is your repository public?**
   - ✅ Yes → Use GitHub Pages
   - ❌ No → Use Artifacts (basic) or buy TestOps

3. **Do you have budget for testing tools?**
   - ✅ Yes ($39+/month) → Consider TestOps
   - ❌ No → Get a Linux VM → Use Allure Server

---

## ⚡ Troubleshooting Quick Reference

### Tests Run But No Report Generated

**Check:**
1. Allure.NUnit package installed? (Check SDET.Tests.csproj)
2. Test class has `[AllureNUnit]` attribute?
3. `allureConfig.json` file exists?
4. Environment variable `ALLURE_CONFIG` set in workflow?

**Fix:**
```csharp
// Add to test file
using Allure.NUnit;
using Allure.NUnit.Attributes;

[AllureNUnit]  // ← Add this
[TestFixture]
public class YourTests { }
```

---

### Can't Connect to Allure Server

**Check:**
1. Server running? `docker ps | grep allure`
2. Firewall open? `sudo ufw allow 5050/tcp`
3. URL correct in GitHub secret?
4. Can reach from local? `curl http://server:5050/allure-docker-service/version`

---

### GitHub Pages Shows 404

**Fix:**
1. Wait 2-3 minutes after workflow completes
2. Check Pages enabled: Settings → Pages → Source: GitHub Actions
3. Verify workflow succeeded: Actions tab

---

## 📞 Get Help

### Documentation Files

1. **Solution Comparison** - [`docs/ALLURE_SOLUTIONS_COMPARISON.md`](ALLURE_SOLUTIONS_COMPARISON.md)
2. **Server Setup** - [`docs/ALLURE_SERVER_SETUP.md`](ALLURE_SERVER_SETUP.md)
3. **GitHub Setup** - [`docs/GITHUB_SETUP_GUIDE.md`](GITHUB_SETUP_GUIDE.md)

### External Resources

- Allure Framework: https://docs.qameta.io/allure/
- Allure Docker Service: https://github.com/fescobar/allure-docker-service
- GitHub Actions: https://docs.github.com/en/actions

---

## 🎓 Next Steps After Setup

1. **✅ Verify First Report**
   - Run workflow
   - Check report loads
   - Verify tests appear

2. **📧 Share with Team**
   - Send report URL to stakeholders
   - Add URL to team wiki/documentation
   - Bookmark for easy access

3. **🔔 Setup Notifications** (Optional)
   - Add Slack webhook to workflow
   - Get notified on test failures
   - See: Workflow file comments

4. **🔒 Secure Server** (Production)
   - Enable authentication on Allure server
   - Setup SSL/HTTPS with Let's Encrypt
   - Configure firewall rules
   - See: `docs/ALLURE_SERVER_SETUP.md` → Step 9

5. **📈 Monitor Trends**
   - Review reports weekly
   - Track flaky tests
   - Identify areas needing improvement

---

## 🎉 Success Checklist

After following this guide, you should have:

- [ ] Allure server running (or GitHub Pages enabled)
- [ ] GitHub Actions workflow configured
- [ ] GitHub secrets added
- [ ] First test run completed successfully
- [ ] Report accessible via URL
- [ ] Tests showing in report with correct data
- [ ] Team members can access report
- [ ] Bookmarked report URL

**All done?** You're ready to demo to your manager! 🚀

---

## 📝 Example Demo Script

**For Manager Presentation:**

> "I've set up automated test reporting for our project. Here's what you can see:
>
> 1. **[Open report URL]** - This is our latest test results
> 2. **Overview tab** - Shows 10 tests passed out of 15 total
> 3. **Suites tab** - Our tests organized by feature
> 4. **Graphs tab** - Visual breakdown of results
> 5. **Timeline** - Shows which tests run in parallel
>
> The best part: This updates automatically on every code change.
> You can bookmark this URL and always see the latest results.
>
> Historical trends show up after a few runs, so you can track quality over time."

---

**Questions?** See detailed documentation in `docs/` folder or check troubleshooting sections.

