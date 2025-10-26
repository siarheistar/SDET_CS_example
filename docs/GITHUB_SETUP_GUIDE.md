# GitHub Repository Setup Guide for Allure Reporting

## Quick Setup Checklist

- [ ] Configure GitHub Secrets
- [ ] Enable GitHub Actions
- [ ] (Optional) Enable GitHub Pages
- [ ] Push workflow files
- [ ] Run first test
- [ ] Verify report generation

---

## Step 1: Add GitHub Secrets

GitHub Secrets store sensitive configuration values like server URLs and credentials.

### How to Add Secrets

1. **Navigate to Repository Settings:**
   ```
   GitHub Repository → Settings (tab) → Secrets and variables → Actions
   ```

2. **Click "New repository secret"**

3. **Add Required Secrets:**

#### For Allure Server Solution:

| Secret Name | Example Value | Description |
|------------|---------------|-------------|
| `ALLURE_SERVER_URL` | `http://192.168.1.100:5050` | Your Allure server URL (IP or domain) |
| `ALLURE_SERVER_USER` | `admin` | (Optional) Username if authentication enabled |
| `ALLURE_SERVER_PASS` | `your-password` | (Optional) Password if authentication enabled |

#### For Slack Notifications (Optional):

| Secret Name | Example Value | Description |
|------------|---------------|-------------|
| `SLACK_WEBHOOK_URL` | `https://hooks.slack.com/services/...` | Slack incoming webhook URL |

### Screenshot Reference

```
Repository → Settings → Secrets and variables → Actions

┌─────────────────────────────────────────────────────────┐
│ Repository secrets                                      │
│                                                         │
│ [New repository secret]                                 │
│                                                         │
│ Name: ALLURE_SERVER_URL                                │
│ Value: http://192.168.1.100:5050                       │
│ [Add secret]                                            │
└─────────────────────────────────────────────────────────┘
```

---

## Step 2: Verify GitHub Actions is Enabled

1. Go to repository **Settings**
2. Navigate to **Actions** → **General**
3. Ensure "Allow all actions and reusable workflows" is selected
4. Scroll down to **Workflow permissions**
5. Select "Read and write permissions"
6. Check "Allow GitHub Actions to create and approve pull requests"
7. Click **Save**

### Why This Matters

These permissions allow the workflow to:
- ✅ Upload artifacts
- ✅ Comment on pull requests with report links
- ✅ Create job summaries
- ✅ Deploy to GitHub Pages (if using)

---

## Step 3: Enable GitHub Pages (If Using Pages Solution)

**Only required if using `.github/workflows/run-tests-github-pages.yml`**

1. Go to repository **Settings**
2. Navigate to **Pages** (in sidebar)
3. Under "Build and deployment":
   - **Source:** Select "GitHub Actions"
4. Click **Save**

Your reports will be available at:
```
https://[username].github.io/[repository-name]/
```

### For Private Repositories

GitHub Pages is only available for:
- ✅ Public repositories (free)
- ✅ GitHub Enterprise
- ❌ Private repos on free/pro plans

---

## Step 4: Push Workflow Files

### For Allure Server Solution

```bash
# Ensure workflow file exists
ls .github/workflows/run-tests-allure.yml

# Add and commit
git add .github/workflows/run-tests-allure.yml
git add allureConfig.json
git add docker-compose.yml
git commit -m "Add Allure reporting workflow"
git push origin main
```

### For GitHub Pages Solution

```bash
# Add workflow
git add .github/workflows/run-tests-github-pages.yml
git add allureConfig.json
git commit -m "Add GitHub Pages Allure workflow"
git push origin main
```

---

## Step 5: Run First Test

### Automatic Trigger

The workflow will run automatically on:
- ✅ Push to `main` or `develop` branches
- ✅ Pull requests to `main` or `develop`

### Manual Trigger

1. Go to **Actions** tab in GitHub
2. Select workflow: "Run Tests with Allure Report"
3. Click **Run workflow** dropdown
4. Select branch (usually `main`)
5. (Optional) Add test filter (e.g., `TestCategory=Unit`)
6. Click **Run workflow** button

### Monitor Progress

1. Click on the running workflow
2. Click on the job (e.g., "Run Tests and Generate Allure Report")
3. Watch live logs
4. Expected duration: 3-5 minutes

---

## Step 6: Verify Report Generation

### For Allure Server Solution

#### Check Workflow Logs

Look for these success messages:

```
✅ Uploaded 23 files
📈 Generating Allure report...
🎉 Report available at: http://your-server:5050/...
```

#### Access Report

1. Find report URL in workflow summary:
   ```
   Actions → Select workflow run → Summary → 📊 Reports section
   ```

2. Or directly navigate to:
   ```
   http://your-server:5050/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html
   ```

3. Bookmark this URL for easy access

### For GitHub Pages Solution

#### Check Workflow Logs

Look for:

```
Deploying to GitHub Pages...
✅ Deployment successful
```

#### Access Report

1. Wait 2-3 minutes for Pages deployment
2. Navigate to:
   ```
   https://[username].github.io/[repository-name]/
   ```

---

## Step 7: Verify Report Contents

A successful report should show:

1. **Overview Page:**
   - ✅ Total test count
   - ✅ Pass/fail/skip statistics
   - ✅ Duration graphs
   - ✅ Trend charts (after 2+ runs)

2. **Suites Page:**
   - ✅ Test suites (e.g., "User Validation")
   - ✅ Individual test cases
   - ✅ Severity levels

3. **Graphs Page:**
   - ✅ Status breakdown
   - ✅ Severity distribution
   - ✅ Duration trends

4. **Timeline:**
   - ✅ Test execution sequence
   - ✅ Parallel execution view

5. **Behaviors/Features:**
   - ✅ Tests grouped by feature
   - ✅ Epic/Feature/Story hierarchy

---

## Troubleshooting

### Issue: Workflow Not Triggering

**Check:**
1. Workflow file is in `.github/workflows/` directory
2. File has `.yml` or `.yaml` extension
3. Syntax is valid (use YAML validator)
4. Branch name matches trigger condition

**Fix:**
```bash
# Verify file location
ls -la .github/workflows/

# Validate YAML syntax
yamllint .github/workflows/run-tests-allure.yml

# Check branch
git branch --show-current
```

### Issue: Secret Not Found

**Error in logs:**
```
Error: ALLURE_SERVER_URL is not set
```

**Fix:**
1. Verify secret name matches exactly (case-sensitive)
2. Check secret is in correct repository (not organization)
3. Re-add secret if needed
4. Re-run workflow

### Issue: Upload to Allure Server Fails

**Error in logs:**
```
curl: (7) Failed to connect to server
```

**Fix:**

1. **Check server is running:**
   ```bash
   # On VM
   docker ps | grep allure
   curl http://localhost:5050/allure-docker-service/version
   ```

2. **Check firewall:**
   ```bash
   # On VM
   sudo ufw status
   sudo ufw allow 5050/tcp
   ```

3. **Check URL format:**
   - ✅ Correct: `http://192.168.1.100:5050`
   - ❌ Wrong: `http://192.168.1.100:5050/` (trailing slash)
   - ❌ Wrong: `https://...` (if server uses HTTP)

4. **Test from local machine:**
   ```bash
   curl http://your-server:5050/allure-docker-service/version
   ```

### Issue: Tests Run But No Allure Results

**Check:**

1. **Allure.NUnit package installed:**
   ```xml
   <PackageReference Include="Allure.NUnit" Version="2.12.1" />
   ```

2. **Test attributes added:**
   ```csharp
   [AllureNUnit]
   [TestFixture]
   public class YourTests { }
   ```

3. **Config environment variable set:**
   ```yaml
   env:
     ALLURE_CONFIG: ${{ github.workspace }}/allureConfig.json
   ```

4. **Config file exists:**
   ```bash
   ls allureConfig.json
   ```

### Issue: GitHub Pages Shows 404

**Fix:**

1. Wait 2-3 minutes after workflow completes
2. Check Pages is enabled: Settings → Pages
3. Verify deployment: Actions → pages-build-deployment
4. Check Pages URL format:
   - ✅ `https://username.github.io/repo-name/`
   - ❌ `https://username.github.io/repo-name/index.html`

---

## Advanced Configuration

### Custom Test Filters

Run specific test categories:

```bash
# In GitHub Actions UI, when running workflow manually
Test filter: TestCategory=Unit
Test filter: TestCategory=API
Test filter: TestCategory=Smoke
Test filter: Priority=High
```

Or modify workflow to add more options:

```yaml
workflow_dispatch:
  inputs:
    test_filter:
      description: 'Test filter'
      required: false
      default: 'TestCategory=Unit|TestCategory=API'
      type: choice
      options:
        - 'TestCategory=Unit|TestCategory=API'
        - 'TestCategory=Unit'
        - 'TestCategory=API'
        - 'TestCategory=Smoke'
        - 'Priority=High'
```

### Scheduled Runs

Add to workflow triggers:

```yaml
on:
  push:
    branches: [ main, develop ]
  schedule:
    # Run daily at 3 AM UTC
    - cron: '0 3 * * *'
```

### Multiple Environments

```yaml
jobs:
  test:
    strategy:
      matrix:
        environment: [staging, production]
    env:
      ALLURE_PROJECT_ID: 'sdet-${{ matrix.environment }}'
      API_URL: ${{ secrets[format('API_URL_{0}', matrix.environment)] }}
```

---

## Security Best Practices

### 1. Never Commit Secrets

```bash
# Add to .gitignore
echo "*.env" >> .gitignore
echo ".env.local" >> .gitignore
echo "*secret*" >> .gitignore
```

### 2. Restrict Secret Access

- Use environment secrets for production
- Use repository secrets for development
- Don't share secrets across repositories unnecessarily

### 3. Rotate Credentials

- Change Allure server password every 90 days
- Regenerate API tokens periodically
- Update Slack webhooks if compromised

### 4. Enable Branch Protection

```
Settings → Branches → Add rule

Branch name pattern: main

✅ Require a pull request before merging
✅ Require status checks to pass
✅ Require branches to be up to date
```

---

## Monitoring and Maintenance

### Weekly Checks

- [ ] Verify reports are generating successfully
- [ ] Check Allure server disk space
- [ ] Review failed test trends

### Monthly Maintenance

- [ ] Update Docker images: `docker-compose pull && docker-compose up -d`
- [ ] Backup Allure server data
- [ ] Review and archive old reports

### Commands

```bash
# Check server health
curl http://your-server:5050/allure-docker-service/version

# Check disk usage
docker exec allure-server df -h

# View recent logs
docker logs allure-server --tail 100 --follow

# Backup reports
ssh user@server 'cd ~/allure-server && tar -czf /tmp/allure-backup.tar.gz projects/'
scp user@server:/tmp/allure-backup.tar.gz ./backups/
```

---

## Getting Help

### Documentation

- **This Guide:** Full setup and troubleshooting
- **Allure Server Setup:** [`docs/ALLURE_SERVER_SETUP.md`](ALLURE_SERVER_SETUP.md)
- **Solution Comparison:** [`docs/ALLURE_SOLUTIONS_COMPARISON.md`](ALLURE_SOLUTIONS_COMPARISON.md)

### Support Resources

- **GitHub Actions:** https://docs.github.com/en/actions
- **Allure Framework:** https://docs.qameta.io/allure/
- **Allure Docker:** https://github.com/fescobar/allure-docker-service

### Internal Support

1. Check workflow logs in Actions tab
2. Review this documentation
3. Test locally using `scripts/upload-to-allure-server.sh`
4. Contact DevOps team for server issues

---

## Next Steps

After successful setup:

1. ✅ **Bookmark Report URL** - Share with team
2. ✅ **Create Documentation** - Add report URL to README
3. ✅ **Train Team** - Show how to access reports
4. ✅ **Setup Notifications** - Add Slack/Teams webhooks
5. ✅ **Monitor Usage** - Track report access and test trends

### Example README Addition

```markdown
## 📊 Test Reports

Latest test results are available at:
- **Allure Report:** http://your-server:5050/allure-docker-service/projects/sdet-cs-framework/reports/latest/index.html
- **GitHub Actions:** See [Actions tab](../../actions) for detailed logs

Reports are automatically generated on every push to `main` and `develop` branches.
```

---

**Setup Complete! 🎉**

Your CI/CD pipeline is now configured to automatically generate and serve beautiful Allure reports.
