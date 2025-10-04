# Backend API Smoke Test Runner Script
# This script helps run smoke tests to verify the backend API is working

param(
    [string]$TestSuite = "all",
    [switch]$Verbose = $false,
    [switch]$StartBackend = $false
)

Write-Host "🔍 Zeus Academia Backend API Smoke Test Runner" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Change to the frontend directory
$frontendPath = "c:\git\zeus.academia\src\Zeus.Academia.StudentPortal"
if (-not (Test-Path $frontendPath)) {
    Write-Host "❌ Frontend directory not found: $frontendPath" -ForegroundColor Red
    exit 1
}

Set-Location $frontendPath

# Check if backend should be started
if ($StartBackend) {
    Write-Host "🚀 Starting backend API..." -ForegroundColor Yellow
    $backendPath = "c:\git\zeus.academia\src\Zeus.Academia.Api"
    
    if (-not (Test-Path $backendPath)) {
        Write-Host "❌ Backend directory not found: $backendPath" -ForegroundColor Red
        exit 1
    }
    
    # Start backend in background
    Start-Process powershell -ArgumentList "-Command", "cd '$backendPath'; dotnet run" -WindowStyle Minimized
    Write-Host "⏳ Waiting 10 seconds for backend to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
}

# Check if backend is running
Write-Host "🔍 Checking if backend API is running..." -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -Method GET -TimeoutSec 5 -UseBasicParsing
    Write-Host "✅ Backend API is running (Status: $($response.StatusCode))" -ForegroundColor Green
}
catch {
    Write-Host "❌ Backend API is not running or not accessible" -ForegroundColor Red
    Write-Host "   Please start the backend API manually:" -ForegroundColor Yellow
    Write-Host "   cd c:\git\zeus.academia\src\Zeus.Academia.Api" -ForegroundColor Yellow
    Write-Host "   dotnet run" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "   Or run this script with -StartBackend flag" -ForegroundColor Yellow
    exit 1
}

# Determine test command based on parameters
$testCommand = "npm test -- backend-smoke.test.ts"

switch ($TestSuite.ToLower()) {
    "connectivity" { 
        $testCommand += " -t `"API Connectivity`""
        Write-Host "🧪 Running connectivity tests only..." -ForegroundColor Blue
    }
    "auth" { 
        $testCommand += " -t `"Authentication Endpoints`""
        Write-Host "🧪 Running authentication tests only..." -ForegroundColor Blue
    }
    "courses" { 
        $testCommand += " -t `"Course Endpoints`""
        Write-Host "🧪 Running course endpoint tests only..." -ForegroundColor Blue
    }
    "versioning" { 
        $testCommand += " -t `"API Versioning`""
        Write-Host "🧪 Running API versioning tests only..." -ForegroundColor Blue
    }
    "performance" { 
        $testCommand += " -t `"Performance Tests`""
        Write-Host "🧪 Running performance tests only..." -ForegroundColor Blue
    }
    "data" { 
        $testCommand += " -t `"Data Structure Tests`""
        Write-Host "🧪 Running data structure tests only..." -ForegroundColor Blue
    }
    default { 
        Write-Host "🧪 Running all smoke tests..." -ForegroundColor Blue
    }
}

if ($Verbose) {
    $testCommand += " --reporter=verbose"
    Write-Host "📝 Using verbose output..." -ForegroundColor Blue
}

Write-Host ""
Write-Host "Executing: $testCommand" -ForegroundColor Gray
Write-Host ""

# Run the tests
try {
    Invoke-Expression $testCommand
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "🎉 All smoke tests passed!" -ForegroundColor Green
        Write-Host "✅ Backend API is working correctly" -ForegroundColor Green
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Cyan
        Write-Host "1. Remove mock API from frontend (if still enabled)" -ForegroundColor White
        Write-Host "2. Test full frontend-backend integration" -ForegroundColor White
        Write-Host "3. Run complete test suite: npm test" -ForegroundColor White
    }
    else {
        Write-Host ""
        Write-Host "❌ Some smoke tests failed" -ForegroundColor Red
        Write-Host "Check the output above for specific issues" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Common issues:" -ForegroundColor Cyan
        Write-Host "- Backend API not running: dotnet run in API directory" -ForegroundColor White
        Write-Host "- Database connection issues: check connection string" -ForegroundColor White
        Write-Host "- Missing dependencies: dotnet restore in API directory" -ForegroundColor White
    }
}
catch {
    Write-Host "❌ Failed to run tests: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "📋 Test Summary Complete" -ForegroundColor Cyan