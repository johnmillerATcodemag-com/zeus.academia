# Zeus Academia Student Portal - Quick Start Scripts

This directory contains PowerShell scripts to easily start and stop the Zeus Academia Student Portal application.

## Scripts Overview

### 🚀 `start-zeus-academia.ps1`

**Primary startup script that launches both backend API and frontend application**

**Features:**

- ✅ **Pre-flight checks**: Verifies .NET SDK, Node.js, and directory structure
- 🧹 **Port cleanup**: Automatically clears ports 5000 and 5173 if in use
- 🔨 **Automatic building**: Builds the .NET API and installs npm dependencies
- 🌐 **Parallel startup**: Starts both services simultaneously in background jobs
- 📊 **Health monitoring**: Verifies services are running and reports endpoints
- 🎯 **Service discovery**: Reports all key API endpoints and demo credentials

**Usage:**

```powershell
# Basic startup (recommended) - works from any directory
.\start-zeus-academia.ps1

# Or use full path from anywhere
C:\git\zeus.academia\start-zeus-academia.ps1

# Skip health checks for faster startup
.\start-zeus-academia.ps1 -SkipHealthCheck

# Start services and wait (blocks terminal until Ctrl+C)
.\start-zeus-academia.ps1 -WaitForExit

# Custom health check timeout
.\start-zeus-academia.ps1 -HealthCheckTimeout 60
```

### 🛑 `stop-zeus-academia.ps1`

**Cleanup script that stops all Zeus Academia services**

**Features:**

- 🔄 **Background job cleanup**: Stops and removes PowerShell background jobs
- 🔌 **Port-based cleanup**: Finds and stops processes using ports 5000 and 5173
- ✅ **Verification**: Confirms all services have been stopped
- 📋 **Process reporting**: Shows which processes were stopped

**Usage:**

```powershell
# Stop all services - works from any directory
.\stop-zeus-academia.ps1

# Or use full path from anywhere
C:\git\zeus.academia\stop-zeus-academia.ps1
```

## Service Endpoints

After running `start-zeus-academia.ps1`, the following endpoints will be available:

### 🖥️ **Frontend Application**

- **URL**: http://localhost:5173/
- **Demo Login**:
  - Email: `john.smith@academia.edu`
  - Password: `password123`

### 🔌 **Backend API**

- **Health Check**: http://localhost:5000/health
- **API Root**: http://localhost:5000/
- **Key Endpoints**:
  - Student Profile: `/api/student/profile`
  - Course Catalog: `/api/courses/paginated`
  - Enrollments: `/api/student/enrollments`
  - Authentication: `/api/auth/login`

## Quick Start Guide

1. **Start Services:**

   ```powershell
   .\start-zeus-academia.ps1
   ```

2. **Open Browser:**

   - Navigate to http://localhost:5173/

3. **Login:**

   - Email: john.smith@academia.edu
   - Password: password123

4. **Explore Features:**

   - View student profile and GPA
   - Browse course catalog
   - Manage course enrollments
   - Update profile information

5. **Stop Services:**
   ```powershell
   .\stop-zeus-academia.ps1
   ```

## Management Commands

While services are running, you can use these PowerShell commands:

```powershell
# Check service health
Invoke-RestMethod http://localhost:5000/health

# View background jobs
Get-Job

# Stop all background jobs
Get-Job | Stop-Job; Get-Job | Remove-Job

# Check what's using the ports
netstat -ano | findstr ":5000 :5173"

# Test API endpoints
Invoke-RestMethod http://localhost:5000/api/student/profile
```

## Troubleshooting

### Common Issues:

**Port already in use:**

- The startup script automatically cleans ports, but you can also run the stop script first

**Services won't start:**

- Ensure you have .NET 9 SDK and Node.js 18+ installed
- Check that you're in the correct directory (`C:\git\zeus.academia`)
- Verify directories exist and contain the expected files

**Frontend shows connection errors:**

- Wait 30-60 seconds for the API to fully start
- Check API health: `Invoke-RestMethod http://localhost:5000/health`
- Restart services if needed

### Manual Alternatives:

If the scripts don't work, you can start services manually:

**Backend API:**

```powershell
cd "C:\git\zeus.academia\src\Zeus.Academia.Api"
dotnet run
```

**Frontend:**

```powershell
cd "C:\git\zeus.academia\src\Zeus.Academia.StudentPortal"
npm run dev
```

## Script Details

### Prerequisites Checked:

- ✅ .NET 9 SDK installed
- ✅ Node.js 18+ installed
- ✅ npm available
- ✅ Project directories exist

### Ports Used:

- **5000**: Backend API (.NET)
- **5173**: Frontend (Vite development server)

### Background Jobs:

The startup script creates PowerShell background jobs for both services. Use `Get-Job` to monitor them and the stop script to clean them up properly.

---

💡 **Tip**: Keep both scripts in the root directory (`C:\git\zeus.academia\`) for easy access and consistent paths.
