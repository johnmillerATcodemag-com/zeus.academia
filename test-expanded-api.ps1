#!/usr/bin/env pwsh
# Test script for expanded Zeus Academia API endpoints

Write-Host "=== EXPANDED API ENDPOINT TESTS ===" -ForegroundColor Green
Write-Host "Testing all expanded endpoints..." -ForegroundColor Cyan

$apiUrl = "http://localhost:5000"
$testResults = @()

function Test-Endpoint {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Description,
        [hashtable]$Body = $null
    )
    
    try {
        Write-Host "Testing: $Description" -ForegroundColor Yellow
        
        $params = @{
            Uri         = $Url
            Method      = $Method
            ContentType = "application/json"
            ErrorAction = "Stop"
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json)
        }
        
        $response = Invoke-RestMethod @params
        Write-Host "✅ SUCCESS: $Description" -ForegroundColor Green
        Write-Host "   Response: $($response | ConvertTo-Json -Compress)" -ForegroundColor DarkGreen
        return @{ Test = $Description; Status = "PASS"; Response = $response }
    }
    catch {
        Write-Host "❌ FAILED: $Description" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor DarkRed
        return @{ Test = $Description; Status = "FAIL"; Error = $_.Exception.Message }
    }
}

# Test all expanded endpoints
Write-Host "`n1. Testing Student Profile Endpoints..." -ForegroundColor Cyan
$testResults += Test-Endpoint "GET" "$apiUrl/api/student/profile" "Get Student Profile"
$testResults += Test-Endpoint "PUT" "$apiUrl/api/student/profile" "Update Student Profile" @{ firstName = "Jane"; lastName = "Doe" }

Write-Host "`n2. Testing Course Detail Endpoints..." -ForegroundColor Cyan
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses/1" "Get Course Details (ID=1)"
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses/2" "Get Course Details (ID=2)"
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses/999" "Get Non-existent Course (ID=999)"

Write-Host "`n3. Testing Course Search Endpoints..." -ForegroundColor Cyan
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses/search?q=computer" "Search Courses by Query"
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses/search?department=Mathematics" "Search Courses by Department"
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses/paginated?page=1&size=5" "Get Paginated Courses"

Write-Host "`n4. Testing Enrollment Endpoints..." -ForegroundColor Cyan
$testResults += Test-Endpoint "GET" "$apiUrl/api/student/enrollments" "Get Student Enrollments"
$testResults += Test-Endpoint "POST" "$apiUrl/api/student/enroll/3" "Enroll in Course (ID=3)"
$testResults += Test-Endpoint "DELETE" "$apiUrl/api/student/enroll/3" "Drop Course (ID=3)"

Write-Host "`n5. Testing Authentication & Utility Endpoints..." -ForegroundColor Cyan
$testResults += Test-Endpoint "POST" "$apiUrl/api/auth/refresh" "Refresh Auth Token" @{ refreshToken = "mock-token" }
$testResults += Test-Endpoint "GET" "$apiUrl/api/version" "Get API Version"
$testResults += Test-Endpoint "GET" "$apiUrl/api/test/404" "Test 404 Response"
$testResults += Test-Endpoint "GET" "$apiUrl/api/test/performance?delay=100" "Test Performance Endpoint"

Write-Host "`n6. Testing Original Basic Endpoints..." -ForegroundColor Cyan
$testResults += Test-Endpoint "GET" "$apiUrl/health" "Health Check"
$testResults += Test-Endpoint "GET" "$apiUrl/api/courses" "Get All Courses"
$testResults += Test-Endpoint "POST" "$apiUrl/api/auth/login" "Authentication Login" @{ username = "test"; password = "test" }

# Summary
Write-Host "`n=== TEST SUMMARY ===" -ForegroundColor Green
$passed = ($testResults | Where-Object { $_.Status -eq "PASS" }).Count
$failed = ($testResults | Where-Object { $_.Status -eq "FAIL" }).Count
$total = $testResults.Count

Write-Host "Total Tests: $total" -ForegroundColor White
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor Red
Write-Host "Success Rate: $([math]::Round(($passed/$total)*100, 1))%" -ForegroundColor Yellow

if ($failed -eq 0) {
    Write-Host "`n🎉 ALL EXPANDED ENDPOINTS ARE WORKING!" -ForegroundColor Green
    Write-Host "The API expansion was successful!" -ForegroundColor Green
}
else {
    Write-Host "`n⚠️  Some endpoints need attention" -ForegroundColor Yellow
    Write-Host "Review the failed tests above." -ForegroundColor Yellow
}

Write-Host "`n=== EXPANDED API STATUS ===" -ForegroundColor Green
if ($passed -ge 15) {
    Write-Host "✅ API expansion successful - ready for frontend integration" -ForegroundColor Green
}
elseif ($passed -ge 10) {
    Write-Host "⚠️  API partially expanded - some features ready" -ForegroundColor Yellow
}
else {
    Write-Host "❌ API expansion needs work" -ForegroundColor Red
}