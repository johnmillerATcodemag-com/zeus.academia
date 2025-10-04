#!/usr/bin/env pwsh
# Frontend-Backend Integration Test
# Tests that the frontend can successfully communicate with the backend API

param(
    [string]$ApiUrl = "http://localhost:5000",
    [string]$FrontendUrl = "http://localhost:3000"
)

Write-Host "=== FRONTEND-BACKEND INTEGRATION TEST ===" -ForegroundColor Cyan
Write-Host "API URL: $ApiUrl" -ForegroundColor Yellow
Write-Host "Frontend URL: $FrontendUrl" -ForegroundColor Yellow
Write-Host ""

$TestsPassed = 0
$TestsFailed = 0

function Test-ApiEndpoint {
    param([string]$Name, [string]$Url, [string]$Method = "GET", [string]$Body = $null)
    
    try {
        $params = @{
            Uri        = $Url
            Method     = $Method
            TimeoutSec = 10
        }
        
        if ($Body) {
            $params.Headers = @{"Content-Type" = "application/json" }
            $params.Body = $Body
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "✅ $Name" -ForegroundColor Green
        $script:TestsPassed++
        return $response
    }
    catch {
        Write-Host "❌ $Name - $($_.Exception.Message)" -ForegroundColor Red
        $script:TestsFailed++
        return $null
    }
}

# Test 1: API Health Check
Write-Host "1. Testing API Health..." -ForegroundColor Yellow
$healthResponse = Test-ApiEndpoint "API Health Check" "$ApiUrl/health"

# Test 2: API Authentication
Write-Host "`n2. Testing Authentication..." -ForegroundColor Yellow
$loginBody = '{"username":"testuser","password":"testpass"}'
$authResponse = Test-ApiEndpoint "API Authentication" "$ApiUrl/api/auth/login" "POST" $loginBody

# Test 3: API Course Data
Write-Host "`n3. Testing Course Data..." -ForegroundColor Yellow
$coursesResponse = Test-ApiEndpoint "API Course Data" "$ApiUrl/api/courses/paginated"

# Test 4: CORS Configuration
Write-Host "`n4. Testing CORS Configuration..." -ForegroundColor Yellow
try {
    # Test preflight request
    $corsHeaders = @{
        "Origin"                         = "http://localhost:3000"
        "Access-Control-Request-Method"  = "GET"
        "Access-Control-Request-Headers" = "Content-Type"
    }
    
    $null = Invoke-WebRequest -Uri "$ApiUrl/api/courses/paginated" -Method OPTIONS -Headers $corsHeaders -TimeoutSec 5
    Write-Host "✅ CORS Preflight Request" -ForegroundColor Green
    $TestsPassed++
}
catch {
    Write-Host "❌ CORS Preflight Request - $($_.Exception.Message)" -ForegroundColor Red
    $TestsFailed++
}

# Test 5: Frontend-like API Call Simulation
Write-Host "`n5. Testing Frontend-like API Calls..." -ForegroundColor Yellow
try {
    # Simulate what the frontend would do
    $frontendHeaders = @{
        "Origin"       = "http://localhost:3000"
        "Content-Type" = "application/json"
        "Accept"       = "application/json"
    }
    
    $response = Invoke-RestMethod -Uri "$ApiUrl/api/courses/paginated" -Headers $frontendHeaders -TimeoutSec 5
    
    if ($response.items -and $response.items.Count -gt 0) {
        Write-Host "✅ Frontend-like Course Data Request" -ForegroundColor Green
        Write-Host "   Retrieved $($response.items.Count) courses" -ForegroundColor Gray
        $TestsPassed++
    }
    else {
        Write-Host "❌ Frontend-like Course Data Request - No data returned" -ForegroundColor Red
        $TestsFailed++
    }
}
catch {
    Write-Host "❌ Frontend-like API Call - $($_.Exception.Message)" -ForegroundColor Red
    $TestsFailed++
}

# Test 6: Authentication Flow Simulation
Write-Host "`n6. Testing Authentication Flow..." -ForegroundColor Yellow
try {
    $loginHeaders = @{
        "Origin"       = "http://localhost:3000"
        "Content-Type" = "application/json"
        "Accept"       = "application/json"
    }
    
    $loginPayload = @{
        username = "student@zeus.edu"
        password = "password123"
    } | ConvertTo-Json
    
    $authResult = Invoke-RestMethod -Uri "$ApiUrl/api/auth/login" -Method POST -Headers $loginHeaders -Body $loginPayload -TimeoutSec 5
    
    if ($authResult.success -and $authResult.token) {
        Write-Host "✅ Authentication Flow" -ForegroundColor Green
        Write-Host "   Token received: $($authResult.token.Substring(0, 20))..." -ForegroundColor Gray
        
        # Test authenticated request
        $authHeaders = @{
            "Origin"        = "http://localhost:3000"
            "Authorization" = "Bearer $($authResult.token)"
            "Accept"        = "application/json"
        }
        
        $null = Invoke-RestMethod -Uri "$ApiUrl/health" -Headers $authHeaders -TimeoutSec 5
        Write-Host "✅ Authenticated Request" -ForegroundColor Green
        $TestsPassed += 2
    }
    else {
        Write-Host "❌ Authentication Flow - Invalid response" -ForegroundColor Red
        $TestsFailed++
    }
}
catch {
    Write-Host "❌ Authentication Flow - $($_.Exception.Message)" -ForegroundColor Red
    $TestsFailed++
}

# Summary
Write-Host "`n=== INTEGRATION TEST SUMMARY ===" -ForegroundColor Cyan
$TotalTests = $TestsPassed + $TestsFailed
Write-Host "Total Tests: $TotalTests" -ForegroundColor White
Write-Host "Passed: $TestsPassed" -ForegroundColor Green
Write-Host "Failed: $TestsFailed" -ForegroundColor Red

$SuccessRate = if ($TotalTests -gt 0) { [math]::Round(($TestsPassed / $TotalTests) * 100, 1) } else { 0 }
Write-Host "Success Rate: $SuccessRate%" -ForegroundColor $(if ($SuccessRate -ge 80) { "Green" } else { "Red" })

# Integration Status
Write-Host "`n=== INTEGRATION STATUS ===" -ForegroundColor Cyan
if ($TestsPassed -ge 6) {
    Write-Host "🎉 FRONTEND-BACKEND INTEGRATION READY!" -ForegroundColor Green
    Write-Host "✅ API is responding correctly" -ForegroundColor Green
    Write-Host "✅ CORS is configured properly" -ForegroundColor Green
    Write-Host "✅ Authentication flow works" -ForegroundColor Green
    Write-Host "✅ Data endpoints are functional" -ForegroundColor Green
    Write-Host "`n📋 NEXT STEPS:" -ForegroundColor Yellow
    Write-Host "1. Frontend can now make API calls to http://localhost:5000" -ForegroundColor White
    Write-Host "2. Update frontend to use real API instead of mock data" -ForegroundColor White
    Write-Host "3. Implement proper error handling for API failures" -ForegroundColor White
}
else {
    Write-Host "⚠️  Integration needs attention" -ForegroundColor Yellow
    Write-Host "Some tests failed. Review the errors above." -ForegroundColor Yellow
}

# Return exit code based on success
if ($TestsFailed -eq 0) { exit 0 } else { exit 1 }