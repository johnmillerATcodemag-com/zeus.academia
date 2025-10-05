# Zeus Academia - Stop Script
# This script stops the backend API, Student Portal, and Faculty Dashboard

# Ensure we're running from the correct directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = $scriptDir
Set-Location $projectRoot

Write-Host "🛑 ZEUS ACADEMIA - STOP SCRIPT" -ForegroundColor Red
Write-Host "==============================" -ForegroundColor Red
Write-Host "📂 Working Directory: $projectRoot" -ForegroundColor Gray
Write-Host ""

# Configuration
$apiPort = 5000
$frontendPort = 5173
$facultyDashboardPort = 5174

# Function to kill processes using a specific port
function Stop-ProcessOnPort {
    param(
        [int]$Port,
        [string]$ServiceName
    )
    
    Write-Host "🔍 Checking port $Port for $ServiceName..." -ForegroundColor Yellow
    $connections = netstat -ano | Select-String ":$Port\s"
    
    if ($connections) {
        Write-Host "⚠️ Found processes on port $Port. Stopping..." -ForegroundColor Yellow
        $pids = $connections | ForEach-Object { 
            ($_ -split '\s+')[-1] 
        } | Sort-Object -Unique
        
        $processCount = 0
        foreach ($processId in $pids) {
            try {
                $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
                if ($process) {
                    Write-Host "  📋 Process: $($process.ProcessName) (PID: $processId)" -ForegroundColor Gray
                    Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
                    Write-Host "  ✅ Stopped process $processId" -ForegroundColor Green
                    $processCount++
                }
            }
            catch {
                Write-Host "  ⚠️ Could not stop process $processId" -ForegroundColor Yellow
            }
        }
        
        if ($processCount -gt 0) {
            Write-Host "✅ Stopped $processCount process(es) for $ServiceName" -ForegroundColor Green
        }
        else {
            Write-Host "ℹ️ No processes to stop for $ServiceName" -ForegroundColor Gray
        }
    }
    else {
        Write-Host "✅ No processes found on port $Port ($ServiceName)" -ForegroundColor Green
    }
    
    Write-Host ""
}

# Stop PowerShell background jobs
Write-Host "🔄 STOPPING BACKGROUND JOBS" -ForegroundColor Cyan
Write-Host "============================" -ForegroundColor Cyan

$jobs = Get-Job -ErrorAction SilentlyContinue
if ($jobs) {
    Write-Host "📋 Found $($jobs.Count) background job(s)" -ForegroundColor Yellow
    
    foreach ($job in $jobs) {
        Write-Host "  🔄 Job $($job.Id): $($job.Name) - $($job.State)" -ForegroundColor Gray
        
        if ($job.State -eq 'Running') {
            Stop-Job $job -ErrorAction SilentlyContinue
            Write-Host "  ⏹️ Stopped job $($job.Id)" -ForegroundColor Yellow
        }
        
        Remove-Job $job -ErrorAction SilentlyContinue
        Write-Host "  🗑️ Removed job $($job.Id)" -ForegroundColor Gray
    }
    
    Write-Host "✅ All background jobs stopped and removed" -ForegroundColor Green
}
else {
    Write-Host "ℹ️ No background jobs found" -ForegroundColor Gray
}

Write-Host ""

# Stop processes on specific ports
Write-Host "🔌 STOPPING PORT-BASED PROCESSES" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan

Stop-ProcessOnPort -Port $apiPort -ServiceName "Backend API"
Stop-ProcessOnPort -Port $frontendPort -ServiceName "Student Portal"
Stop-ProcessOnPort -Port $facultyDashboardPort -ServiceName "Faculty Dashboard"

# Verify cleanup
Write-Host "✅ CLEANUP VERIFICATION" -ForegroundColor Green
Write-Host "=======================" -ForegroundColor Green

$apiCheck = netstat -ano | Select-String ":$apiPort\s"
$studentPortalCheck = netstat -ano | Select-String ":$frontendPort\s"
$facultyDashboardCheck = netstat -ano | Select-String ":$facultyDashboardPort\s"

if (-not $apiCheck) {
    Write-Host "✅ Port $apiPort (Backend API): Clean" -ForegroundColor Green
}
else {
    Write-Host "⚠️ Port $apiPort (Backend API): Still in use" -ForegroundColor Yellow
}

if (-not $studentPortalCheck) {
    Write-Host "✅ Port $frontendPort (Student Portal): Clean" -ForegroundColor Green
}
else {
    Write-Host "⚠️ Port $frontendPort (Student Portal): Still in use" -ForegroundColor Yellow
}

if (-not $facultyDashboardCheck) {
    Write-Host "✅ Port $facultyDashboardPort (Faculty Dashboard): Clean" -ForegroundColor Green
}
else {
    Write-Host "⚠️ Port $facultyDashboardPort (Faculty Dashboard): Still in use" -ForegroundColor Yellow
}

# Check for any remaining jobs
$remainingJobs = Get-Job -ErrorAction SilentlyContinue
if (-not $remainingJobs) {
    Write-Host "✅ Background jobs: Clean" -ForegroundColor Green
}
else {
    Write-Host "⚠️ Background jobs: $($remainingJobs.Count) remaining" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🏁 SHUTDOWN COMPLETE" -ForegroundColor Green
Write-Host "====================" -ForegroundColor Green
Write-Host "✅ Zeus Academia services have been stopped" -ForegroundColor Green
Write-Host ""
Write-Host "🔄 To restart services, run:" -ForegroundColor Yellow
Write-Host "   .\start-zeus-academia.ps1" -ForegroundColor White
Write-Host ""