#!/usr/bin/env pwsh
# Zeus Academia API Smoke Test Script
# Tests all essential API endpoints to verify backend functionality

param(
    [string]$BaseUrl = "http://localhost:5000"
)

Write-Host "=== ZEUS ACADEMIA API SMOKE TEST ===" -ForegroundColor Cyan
Write-Host "Testing API at: $BaseUrl" -ForegroundColor Yellow
Write-Host ""

$TestResults = @()
$TotalTests = 0
$PassedTests = 0
$FailedTests = 0

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Url,
        [string]$Method = "GET",
        [hashtable]$Headers = @{},
        [string]$Body = $null,
        [int]$ExpectedStatus = 200
    )
    
    $global:TotalTests++
    
    try {
        $params = @{
            Uri        = $Url
            Method     = $Method
            Headers    = $Headers
            TimeoutSec = 10
        }
        
        if ($Body) {
            $params.Body = $Body
        }
        
        $startTime = Get-Date
        $response = Invoke-RestMethod @params
        $endTime = Get-Date
        $duration = [math]::Round(($endTime - $startTime).TotalMilliseconds, 2)
        
        Write-Host "✅ $Name" -ForegroundColor Green
        Write-Host "   Response time: ${duration}ms" -ForegroundColor Gray
        
        $global:PassedTests++
        $global:TestResults += [PSCustomObject]@{
            Test     = $Name
            Status   = "PASSED"
            Duration = "${duration}ms"
            Response = $response
        }
        
        return $response
    }
    catch {
        Write-Host "❌ $Name" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        
        $global:FailedTests++
        $global:TestResults += [PSCustomObject]@{
            Test     = $Name
            Status   = "FAILED"
            Duration = "N/A"
            Error    = $_.Exception.Message
        }
        
        return $null
    }
}

function Test-JsonStructure {
    param(
        [string]$Name,
        [object]$Response,
        [string[]]$RequiredFields
    )
    
    $global:TotalTests++
    
    try {
        foreach ($field in $RequiredFields) {
            if (-not ($Response.PSObject.Properties | Where-Object { $_.Name -eq $field })) {
                throw "Missing required field: $field"
            }
        }
        
        Write-Host "✅ $Name - JSON Structure" -ForegroundColor Green
        $global:PassedTests++
        $global:TestResults += [PSCustomObject]@{
            Test     = "$Name - JSON Structure"
            Status   = "PASSED"
            Duration = "N/A"
            Response = "Structure validated"
        }
    }
    catch {
        Write-Host "❌ $Name - JSON Structure" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        
        $global:FailedTests++
        $global:TestResults += [PSCustomObject]@{
            Test     = "$Name - JSON Structure"
            Status   = "FAILED"
            Duration = "N/A"
            Error    = $_.Exception.Message
        }
    }
}

# Test 1: API Root Endpoint
Write-Host "Testing API Connectivity..." -ForegroundColor Yellow
$rootResponse = Test-Endpoint -Name "Root Endpoint" -Url "$BaseUrl/"
if ($rootResponse) {
    Test-JsonStructure -Name "Root Endpoint" -Response $rootResponse -RequiredFields @("message", "status", "version")
}

# Test 2: Health Check
Write-Host "`nTesting Health Endpoint..." -ForegroundColor Yellow
$healthResponse = Test-Endpoint -Name "Health Check" -Url "$BaseUrl/health"
if ($healthResponse) {
    Test-JsonStructure -Name "Health Check" -Response $healthResponse -RequiredFields @("status", "service", "version")
}

# Test 3: Authentication Endpoints
Write-Host "`nTesting Authentication..." -ForegroundColor Yellow
$loginHeaders = @{"Content-Type" = "application/json" }
$loginBody = '{"username":"test","password":"test"}'
$authResponse = Test-Endpoint -Name "Auth Login" -Url "$BaseUrl/api/auth/login" -Method "POST" -Headers $loginHeaders -Body $loginBody
if ($authResponse) {
    Test-JsonStructure -Name "Auth Login" -Response $authResponse -RequiredFields @("success", "token", "user")
}

$registerResponse = Test-Endpoint -Name "Auth Register" -Url "$BaseUrl/api/auth/register" -Method "POST" -Headers $loginHeaders -Body $loginBody
if ($registerResponse) {
    Test-JsonStructure -Name "Auth Register" -Response $registerResponse -RequiredFields @("success", "message")
}

# Test 4: Course Endpoints
Write-Host "`nTesting Course Data..." -ForegroundColor Yellow
$coursesResponse = Test-Endpoint -Name "Courses Paginated" -Url "$BaseUrl/api/courses/paginated"
if ($coursesResponse) {
    Test-JsonStructure -Name "Courses Paginated" -Response $coursesResponse -RequiredFields @("items", "pageNumber", "pageSize", "totalCount")
    
    # Validate course item structure
    if ($coursesResponse.items -and $coursesResponse.items.Count -gt 0) {
        Test-JsonStructure -Name "Course Item" -Response $coursesResponse.items[0] -RequiredFields @("id", "code", "title", "credits")
    }
}

# Test 5: API Versioning
Write-Host "`nTesting API Versioning..." -ForegroundColor Yellow
$versionHeaders = @{"X-API-Version" = "1.0" }
Test-Endpoint -Name "API Versioning" -Url "$BaseUrl/api/version" -Headers $versionHeaders

# Test 6: Error Handling
Write-Host "`nTesting Error Handling..." -ForegroundColor Yellow
try {
    $notFoundResponse = Invoke-RestMethod -Uri "$BaseUrl/nonexistent" -Method GET -ErrorAction SilentlyContinue
}
catch {
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "✅ 404 Error Handling" -ForegroundColor Green
        $global:PassedTests++
        $global:TotalTests++
        $global:TestResults += [PSCustomObject]@{
            Test     = "404 Error Handling"
            Status   = "PASSED"
            Duration = "N/A"
            Response = "Correct 404 response"
        }
    }
    else {
        Write-Host "❌ 404 Error Handling" -ForegroundColor Red
        $global:FailedTests++
        $global:TotalTests++
        $global:TestResults += [PSCustomObject]@{
            Test     = "404 Error Handling"
            Status   = "FAILED"
            Duration = "N/A"
            Error    = "Expected 404, got $($_.Exception.Response.StatusCode)"
        }
    }
}

# Test 7: Performance Check
Write-Host "`nTesting Performance..." -ForegroundColor Yellow
$performanceResults = @()
for ($i = 1; $i -le 5; $i++) {
    $startTime = Get-Date
    try {
        Invoke-RestMethod -Uri "$BaseUrl/health" -Method GET -TimeoutSec 5 | Out-Null
        $endTime = Get-Date
        $duration = ($endTime - $startTime).TotalMilliseconds
        $performanceResults += $duration
    }
    catch {
        $performanceResults += 9999  # High value for failed requests
    }
}

$avgResponseTime = [math]::Round(($performanceResults | Measure-Object -Average).Average, 2)
if ($avgResponseTime -lt 1000) {
    Write-Host "✅ Performance Test - Avg: ${avgResponseTime}ms" -ForegroundColor Green
    $global:PassedTests++
}
else {
    Write-Host "❌ Performance Test - Avg: ${avgResponseTime}ms (too slow)" -ForegroundColor Red
    $global:FailedTests++
}
$global:TotalTests++

# Test Summary
Write-Host "`n=== TEST SUMMARY ===" -ForegroundColor Cyan
Write-Host "Total Tests: $TotalTests" -ForegroundColor White
Write-Host "Passed: $PassedTests" -ForegroundColor Green
Write-Host "Failed: $FailedTests" -ForegroundColor Red

$successRate = [math]::Round(($PassedTests / $TotalTests) * 100, 1)
Write-Host "Success Rate: $successRate%" -ForegroundColor $(if ($successRate -ge 80) { "Green" } else { "Red" })

# Detailed Results
Write-Host "`n=== DETAILED RESULTS ===" -ForegroundColor Cyan
$TestResults | Format-Table -AutoSize

# Final Status
if ($FailedTests -eq 0) {
    Write-Host "`n🎉 ALL TESTS PASSED! Backend API is working correctly." -ForegroundColor Green
    exit 0
}
else {
    Write-Host "`n⚠️  Some tests failed. Backend API needs attention." -ForegroundColor Yellow
    exit 1
}