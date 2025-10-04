#!/usr/bin/env pwsh
# Production-Ready Endpoint Tests - Core Functionality

Write-Host "=== ZEUS ACADEMIA API - PRODUCTION READY ENDPOINTS ===" -ForegroundColor Green
Write-Host "Testing 10 core working endpoints for frontend integration..." -ForegroundColor Cyan

$apiUrl = "http://localhost:5000"
$coreTests = @()

function Test-CoreEndpoint {
    param(
        [string]$Method,
        [string]$Url,
        [string]$Description,
        [hashtable]$Body = $null
    )
    
    try {
        Write-Host "✅ Testing: $Description" -ForegroundColor Green
        
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
        Write-Host "   SUCCESS: $($response | ConvertTo-Json -Compress)" -ForegroundColor DarkGreen
        return @{ Test = $Description; Status = "PASS"; Response = $response }
    }
    catch {
        Write-Host "   FAILED: $($_.Exception.Message)" -ForegroundColor DarkRed
        return @{ Test = $Description; Status = "FAIL"; Error = $_.Exception.Message }
    }
}

Write-Host "`n🔐 AUTHENTICATION SYSTEM" -ForegroundColor Yellow
$coreTests += Test-CoreEndpoint "POST" "$apiUrl/api/auth/login" "User Login" @{ username = "student"; password = "test123" }
$coreTests += Test-CoreEndpoint "POST" "$apiUrl/api/auth/refresh" "Token Refresh" @{ refreshToken = "test-refresh-token" }

Write-Host "`n👨‍🎓 STUDENT PROFILE SYSTEM" -ForegroundColor Yellow
$coreTests += Test-CoreEndpoint "GET" "$apiUrl/api/student/profile" "Get Student Profile"
$coreTests += Test-CoreEndpoint "PUT" "$apiUrl/api/student/profile" "Update Profile" @{ firstName = "Updated"; lastName = "Student" }

Write-Host "`n📚 ENROLLMENT MANAGEMENT SYSTEM" -ForegroundColor Yellow
$coreTests += Test-CoreEndpoint "GET" "$apiUrl/api/student/enrollments" "View Enrollments"
$coreTests += Test-CoreEndpoint "POST" "$apiUrl/api/student/enroll/1" "Enroll in CS101"
$coreTests += Test-CoreEndpoint "DELETE" "$apiUrl/api/student/enroll/1" "Drop CS101"

Write-Host "`n📋 COURSE DISCOVERY SYSTEM" -ForegroundColor Yellow
$coreTests += Test-CoreEndpoint "GET" "$apiUrl/api/courses/paginated?page=1&size=10" "Browse Course Catalog"

Write-Host "`n🛠️ SYSTEM MONITORING" -ForegroundColor Yellow
$coreTests += Test-CoreEndpoint "GET" "$apiUrl/health" "API Health Status"
$coreTests += Test-CoreEndpoint "GET" "$apiUrl/api/test/performance?delay=50" "Performance Test"

# Results
Write-Host "`n=== PRODUCTION READINESS REPORT ===" -ForegroundColor Green
$passed = ($coreTests | Where-Object { $_.Status -eq "PASS" }).Count
$failed = ($coreTests | Where-Object { $_.Status -eq "FAIL" }).Count
$total = $coreTests.Count

Write-Host "Core Endpoints Tested: $total" -ForegroundColor White
Write-Host "Production Ready: $passed" -ForegroundColor Green
Write-Host "Issues Found: $failed" -ForegroundColor Red
Write-Host "Readiness Score: $([math]::Round(($passed/$total)*100, 1))%" -ForegroundColor Yellow

if ($passed -eq $total) {
    Write-Host "`n🚀 FRONTEND INTEGRATION READY!" -ForegroundColor Green
    Write-Host "All core systems operational - proceed with Vue.js integration" -ForegroundColor Green
}
elseif ($passed -ge 8) {
    Write-Host "`n✅ MOSTLY READY - Minor issues detected" -ForegroundColor Yellow
    Write-Host "Core functionality available for frontend integration" -ForegroundColor Yellow
}
else {
    Write-Host "`n⚠️  NEEDS ATTENTION - Multiple core systems failing" -ForegroundColor Red
}

Write-Host "`n=== INTEGRATION NEXT STEPS ===" -ForegroundColor Cyan
Write-Host "1. Update Vue.js ApiService to use working endpoints" -ForegroundColor White
Write-Host "2. Test user login flow in frontend" -ForegroundColor White
Write-Host "3. Test student profile management in frontend" -ForegroundColor White
Write-Host "4. Test course enrollment workflow in frontend" -ForegroundColor White
Write-Host "5. Verify CORS configuration with actual frontend calls" -ForegroundColor White