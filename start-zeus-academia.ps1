# Zeus Academia Student Portal - Startup Script
# This script starts both the backend API and frontend application

param(
    [switch]$WaitForExit,
    [switch]$SkipHealthCheck,
    [int]$HealthCheckTimeout = 30
)

# Ensure we're running from the correct directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = $scriptDir
Set-Location $projectRoot

Write-Host "🏛️ ZEUS ACADEMIA STUDENT PORTAL - STARTUP SCRIPT" -ForegroundColor Yellow
Write-Host "=================================================" -ForegroundColor Yellow
Write-Host "📂 Working Directory: $projectRoot" -ForegroundColor Gray
Write-Host ""

# Configuration
$apiPath = "C:\git\zeus.academia\src\Zeus.Academia.Api"
$frontendPath = "C:\git\zeus.academia\src\Zeus.Academia.StudentPortal"
$apiPort = 5000
$frontendPort = 5173
$apiUrl = "http://localhost:$apiPort"
$frontendUrl = "http://localhost:$frontendPort"

# Function to check if a port is in use
function Test-PortInUse {
    param([int]$Port)
    $connections = netstat -ano | Select-String ":$Port\s"
    return $connections.Count -gt 0
}

# Function to wait for service to be ready
function Wait-ForService {
    param(
        [string]$Url,
        [string]$ServiceName,
        [int]$TimeoutSeconds = 30
    )
    
    Write-Host "⏳ Waiting for $ServiceName to start..." -ForegroundColor Yellow
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    
    do {
        Start-Sleep -Seconds 2
        try {
            Invoke-RestMethod -Uri $Url -TimeoutSec 5 -ErrorAction Stop | Out-Null
            Write-Host "✅ $ServiceName is ready!" -ForegroundColor Green
            return $true
        }
        catch {
            Write-Host "." -NoNewline -ForegroundColor Gray
        }
    } while ($stopwatch.Elapsed.TotalSeconds -lt $TimeoutSeconds)
    
    Write-Host ""
    Write-Host "❌ $ServiceName failed to start within $TimeoutSeconds seconds" -ForegroundColor Red
    return $false
}

# Function to kill processes using a specific port
function Stop-ProcessOnPort {
    param([int]$Port)
    
    $connections = netstat -ano | Select-String ":$Port\s"
    if ($connections) {
        Write-Host "⚠️ Port $Port is in use. Stopping existing processes..." -ForegroundColor Yellow
        $pids = $connections | ForEach-Object { 
            ($_ -split '\s+')[-1] 
        } | Sort-Object -Unique
        
        foreach ($processId in $pids) {
            try {
                Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
                Write-Host "  Stopped process $processId" -ForegroundColor Gray
            }
            catch {
                Write-Host "  Could not stop process $processId" -ForegroundColor Yellow
            }
        }
        Start-Sleep -Seconds 2
    }
}

# Pre-flight checks
Write-Host "🔍 PRE-FLIGHT CHECKS" -ForegroundColor Cyan
Write-Host "=====================" -ForegroundColor Cyan

# Check if directories exist
if (-not (Test-Path $apiPath)) {
    Write-Host "❌ API directory not found: $apiPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $frontendPath)) {
    Write-Host "❌ Frontend directory not found: $frontendPath" -ForegroundColor Red
    exit 1
}

Write-Host "✅ API directory found: $apiPath" -ForegroundColor Green
Write-Host "✅ Frontend directory found: $frontendPath" -ForegroundColor Green

# Check required tools
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ .NET SDK not found. Please install .NET 9 SDK" -ForegroundColor Red
    exit 1
}

try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js: $nodeVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ Node.js not found. Please install Node.js 18+" -ForegroundColor Red
    exit 1
}

try {
    $npmVersion = npm --version
    Write-Host "✅ npm: $npmVersion" -ForegroundColor Green
}
catch {
    Write-Host "❌ npm not found" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Clear ports if in use
Write-Host "🧹 PORT CLEANUP" -ForegroundColor Cyan
Write-Host "===============" -ForegroundColor Cyan

Stop-ProcessOnPort -Port $apiPort
Stop-ProcessOnPort -Port $frontendPort

Write-Host "✅ Ports cleared" -ForegroundColor Green
Write-Host ""

# Start Backend API
Write-Host "🚀 STARTING BACKEND API" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

try {
    Write-Host "📂 Navigating to API directory..." -ForegroundColor Gray
    Push-Location $apiPath
    
    Write-Host "🔨 Building API..." -ForegroundColor Yellow
    dotnet build --verbosity quiet | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ API build failed" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Write-Host "✅ API build successful" -ForegroundColor Green
    
    Write-Host "▶️ Starting API server..." -ForegroundColor Yellow
    
    # Start API in background
    $apiJob = Start-Job -ScriptBlock {
        param($path)
        Set-Location $path
        dotnet run --urls "http://localhost:5000"
    } -ArgumentList $apiPath
    
    Pop-Location
    
    Write-Host "🔄 API Job ID: $($apiJob.Id)" -ForegroundColor Gray
    
    if (-not $SkipHealthCheck) {
        $apiReady = Wait-ForService -Url "$apiUrl/health" -ServiceName "Backend API" -TimeoutSeconds $HealthCheckTimeout
        if (-not $apiReady) {
            Write-Host "❌ Failed to start Backend API" -ForegroundColor Red
            Stop-Job $apiJob -ErrorAction SilentlyContinue
            Remove-Job $apiJob -ErrorAction SilentlyContinue
            exit 1
        }
    }
}
catch {
    Write-Host "❌ Error starting Backend API: $($_.Exception.Message)" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host ""

# Start Frontend
Write-Host "🌐 STARTING FRONTEND" -ForegroundColor Cyan
Write-Host "====================" -ForegroundColor Cyan

try {
    Write-Host "📂 Navigating to frontend directory..." -ForegroundColor Gray
    Push-Location $frontendPath
    
    # Check if node_modules exists
    if (-not (Test-Path "node_modules")) {
        Write-Host "📦 Installing dependencies..." -ForegroundColor Yellow
        npm install --silent
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Frontend dependency installation failed" -ForegroundColor Red
            Pop-Location
            exit 1
        }
        Write-Host "✅ Dependencies installed" -ForegroundColor Green
    }
    else {
        Write-Host "✅ Dependencies already installed" -ForegroundColor Green
    }
    
    Write-Host "▶️ Starting frontend server..." -ForegroundColor Yellow
    
    # Start Frontend in background
    $frontendJob = Start-Job -ScriptBlock {
        param($path)
        Set-Location $path
        npm run dev
    } -ArgumentList $frontendPath
    
    Pop-Location
    
    Write-Host "🔄 Frontend Job ID: $($frontendJob.Id)" -ForegroundColor Gray
    
    if (-not $SkipHealthCheck) {
        # Wait a bit for Vite to start
        Start-Sleep -Seconds 5
        Write-Host "✅ Frontend starting (Vite typically takes 2-5 seconds)" -ForegroundColor Green
    }
}
catch {
    Write-Host "❌ Error starting Frontend: $($_.Exception.Message)" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host ""

# Report endpoints and status
Write-Host "📊 SERVICE STATUS & ENDPOINTS" -ForegroundColor Green
Write-Host "=============================" -ForegroundColor Green

# Test API endpoints
try {
    $health = Invoke-RestMethod -Uri "$apiUrl/health" -TimeoutSec 5
    Write-Host "✅ Backend API: RUNNING" -ForegroundColor Green
    Write-Host "   🌐 Health: $apiUrl/health" -ForegroundColor Cyan
    Write-Host "   📊 Status: $($health.status)" -ForegroundColor Cyan
    Write-Host "   🏷️ Service: $($health.service)" -ForegroundColor Cyan
    Write-Host "   📅 Version: $($health.version)" -ForegroundColor Cyan
    
    # Test key endpoints
    Write-Host "   🔗 Key Endpoints:" -ForegroundColor Cyan
    Write-Host "      • API Info: $apiUrl/" -ForegroundColor White
    Write-Host "      • Student Profile: $apiUrl/api/student/profile" -ForegroundColor White
    Write-Host "      • Course Catalog: $apiUrl/api/courses/paginated" -ForegroundColor White
    Write-Host "      • Enrollments: $apiUrl/api/student/enrollments" -ForegroundColor White
    Write-Host "      • Authentication: $apiUrl/api/auth/login" -ForegroundColor White
}
catch {
    Write-Host "⚠️ Backend API: STARTING (may take a few more seconds)" -ForegroundColor Yellow
    Write-Host "   🌐 Expected URL: $apiUrl" -ForegroundColor Cyan
}

Write-Host ""

# Check if frontend port is in use (indicates it's probably running)
if (Test-PortInUse -Port $frontendPort) {
    Write-Host "✅ Frontend: RUNNING" -ForegroundColor Green
    Write-Host "   🌐 Application: $frontendUrl/" -ForegroundColor Cyan
    Write-Host "   🔑 Demo Login:" -ForegroundColor Cyan
    Write-Host "      • Email: john.smith@academia.edu" -ForegroundColor White
    Write-Host "      • Password: password123" -ForegroundColor White
}
else {
    Write-Host "⚠️ Frontend: STARTING (Vite typically takes 2-10 seconds)" -ForegroundColor Yellow
    Write-Host "   🌐 Expected URL: $frontendUrl/" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "🏁 STARTUP COMPLETE" -ForegroundColor Green
Write-Host "===================" -ForegroundColor Green
Write-Host "🎯 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Open browser to: $frontendUrl" -ForegroundColor White
Write-Host "   2. Login with demo credentials above" -ForegroundColor White
Write-Host "   3. Explore the Zeus Academia Student Portal!" -ForegroundColor White
Write-Host ""
Write-Host "📋 Management Commands:" -ForegroundColor Yellow
Write-Host "   • View API health: Invoke-RestMethod $apiUrl/health" -ForegroundColor White
Write-Host "   • Stop services: Get-Job | Stop-Job; Get-Job | Remove-Job" -ForegroundColor White
Write-Host "   • Check ports: netstat -ano | findstr ':$apiPort :$frontendPort'" -ForegroundColor White
Write-Host ""

# Store job information for cleanup
$Global:ZeusJobs = @{
    API       = $apiJob
    Frontend  = $frontendJob
    StartTime = Get-Date
}

if ($WaitForExit) {
    Write-Host "⏸️ Press Ctrl+C to stop services..." -ForegroundColor Yellow
    try {
        # Wait for user interrupt
        while ($true) {
            Start-Sleep -Seconds 1
            
            # Check if jobs are still running
            if ($apiJob.State -ne 'Running' -and $frontendJob.State -ne 'Running') {
                Write-Host "⚠️ Both services have stopped" -ForegroundColor Yellow
                break
            }
        }
    }
    catch {
        Write-Host "🛑 Shutting down services..." -ForegroundColor Yellow
    }
    finally {
        # Cleanup jobs
        Write-Host "🧹 Cleaning up background jobs..." -ForegroundColor Gray
        Stop-Job $apiJob, $frontendJob -ErrorAction SilentlyContinue
        Remove-Job $apiJob, $frontendJob -ErrorAction SilentlyContinue
        Write-Host "✅ Cleanup complete" -ForegroundColor Green
    }
}
else {
    Write-Host "🔧 Services are running in background jobs" -ForegroundColor Cyan
    Write-Host "   Use 'Get-Job' to check status" -ForegroundColor Gray
    Write-Host "   Use 'Get-Job | Stop-Job; Get-Job | Remove-Job' to stop all services" -ForegroundColor Gray
}

Write-Host ""
Write-Host "🎉 Zeus Academia Student Portal is ready!" -ForegroundColor Green