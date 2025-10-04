#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Tests frontend-backend integration for Zeus Academia API
.DESCRIPTION
    This script tests the integration between the Vue.js frontend services and the .NET backend API
    by simulating frontend API calls and validating responses.
#>

param(
    [string]$ApiBaseUrl = "http://localhost:5000"
)

Write-Host "🎯 Zeus Academia Frontend-Backend Integration Test" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Test results tracking
$testResults = @()
$totalTests = 0
$passedTests = 0

function Test-ApiEndpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Endpoint,
        [hashtable]$Body = @{},
        [hashtable]$Headers = @{},
        [int]$ExpectedStatusCode = 200,
        [scriptblock]$ValidationScript = $null
    )
    
    $totalTests++
    
    try {
        Write-Host "🧪 Testing: $Name" -ForegroundColor Yellow
        
        $params = @{
            Uri        = "$ApiBaseUrl$Endpoint"
            Method     = $Method
            TimeoutSec = 10
            Headers    = $Headers
        }
        
        if ($Body.Count -gt 0) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-RestMethod @params
        
        # Validate response
        $isValid = $true
        $errorMessage = ""
        
        if ($ValidationScript) {
            try {
                $validationResult = & $ValidationScript $response
                if (-not $validationResult) {
                    $isValid = $false
                    $errorMessage = "Validation script failed"
                }
            }
            catch {
                $isValid = $false
                $errorMessage = "Validation error: $($_.Exception.Message)"
            }
        }
        
        if ($isValid) {
            Write-Host "   ✅ PASS - $Name" -ForegroundColor Green
            $script:passedTests++
            $script:testResults += [PSCustomObject]@{
                Test         = $Name
                Status       = "PASS"
                Endpoint     = $Endpoint
                Method       = $Method
                ResponseTime = "< 1s"
                Notes        = "Success"
            }
        }
        else {
            Write-Host "   ❌ FAIL - $Name : $errorMessage" -ForegroundColor Red
            $script:testResults += [PSCustomObject]@{
                Test         = $Name
                Status       = "FAIL"
                Endpoint     = $Endpoint
                Method       = $Method
                ResponseTime = "< 1s"
                Notes        = $errorMessage
            }
        }
        
    }
    catch {
        $statusCode = $null
        if ($_.Exception.Response) {
            $statusCode = $_.Exception.Response.StatusCode.value__
        }
        
        if ($statusCode -eq $ExpectedStatusCode) {
            Write-Host "   ✅ PASS - $Name (Expected $ExpectedStatusCode)" -ForegroundColor Green
            $script:passedTests++
            $script:testResults += [PSCustomObject]@{
                Test         = $Name
                Status       = "PASS"
                Endpoint     = $Endpoint
                Method       = $Method
                ResponseTime = "< 1s"
                Notes        = "Expected status code $ExpectedStatusCode"
            }
        }
        else {
            Write-Host "   ❌ FAIL - $Name : $($_.Exception.Message)" -ForegroundColor Red
            $script:testResults += [PSCustomObject]@{
                Test         = $Name
                Status       = "FAIL"
                Endpoint     = $Endpoint
                Method       = $Method
                ResponseTime = "< 1s"
                Notes        = $_.Exception.Message
            }
        }
    }
}

# Check if API is running
Write-Host "🔍 Checking API availability..." -ForegroundColor Cyan
try {
    $healthResponse = Invoke-RestMethod -Uri "$ApiBaseUrl/health" -TimeoutSec 5
    Write-Host "✅ API is running: $($healthResponse.status)" -ForegroundColor Green
    Write-Host "   Service: $($healthResponse.service)" -ForegroundColor Gray
    Write-Host "   Version: $($healthResponse.version)" -ForegroundColor Gray
    Write-Host ""
}
catch {
    Write-Host "❌ API is not accessible at $ApiBaseUrl" -ForegroundColor Red
    Write-Host "   Please ensure the Zeus.Academia.Api is running" -ForegroundColor Yellow
    exit 1
}

Write-Host "🧪 Running Frontend Integration Tests..." -ForegroundColor Cyan
Write-Host ""

# Test 1: Authentication - Login
Test-ApiEndpoint -Name "Authentication - Login" -Method "POST" -Endpoint "/api/auth/login" `
    -Body @{ email = "john.smith@academia.edu"; password = "password123" } `
    -ValidationScript {
    param($response)
    return ($response.success -and $response.token -and $response.user)
}

# Store token for authenticated requests
$loginResponse = $null
try {
    $loginResponse = Invoke-RestMethod -Uri "$ApiBaseUrl/api/auth/login" -Method POST `
        -Body (@{ email = "john.smith@academia.edu"; password = "password123" } | ConvertTo-Json) `
        -ContentType "application/json" -TimeoutSec 5
    
    $authHeaders = @{
        "Authorization" = "Bearer $($loginResponse.token)"
    }
}
catch {
    $authHeaders = @{}
    Write-Host "⚠️ Could not obtain auth token for subsequent tests" -ForegroundColor Yellow
}

# Test 2: Authentication - Register
Test-ApiEndpoint -Name "Authentication - Register" -Method "POST" -Endpoint "/api/auth/register" `
    -Body @{ 
    email     = "new.student@academia.edu"
    password  = "newpassword123"
    firstName = "New"
    lastName  = "Student"
    studentId = "STU999"
} `
    -ValidationScript {
    param($response)
    return ($response.success -and $response.token)
}

# Test 3: Student Profile - Get
Test-ApiEndpoint -Name "Student Profile - Get" -Method "GET" -Endpoint "/api/student/profile" `
    -Headers $authHeaders `
    -ValidationScript {
    param($response)
    return ($response.firstName -and $response.lastName -and $response.gpa -ne $null)
}

# Test 4: Student Profile - Update
Test-ApiEndpoint -Name "Student Profile - Update" -Method "PUT" -Endpoint "/api/student/profile" `
    -Headers $authHeaders `
    -Body @{
    firstName = "Updated"
    lastName  = "Student"
    phone     = "555-123-4567"
} `
    -ValidationScript {
    param($response)
    return ($response.firstName -eq "Updated" -and $response.phone -eq "555-123-4567")
}

# Test 5: Student Enrollments - Get
Test-ApiEndpoint -Name "Student Enrollments - Get" -Method "GET" -Endpoint "/api/student/enrollments" `
    -Headers $authHeaders `
    -ValidationScript {
    param($response)
    return ($response.enrollments -is [Array] -and $response.totalCredits -ge 0)
}

# Test 6: Courses - Get Paginated
Test-ApiEndpoint -Name "Courses - Get Paginated" -Method "GET" -Endpoint "/api/courses/paginated" `
    -ValidationScript {
    param($response)
    return ($response.courses -is [Array] -and $response.courses.Count -gt 0)
}

# Test 7: Course Enrollment - Enroll
Test-ApiEndpoint -Name "Course Enrollment - Enroll" -Method "POST" -Endpoint "/api/student/enroll/CS301" `
    -Headers $authHeaders `
    -ValidationScript {
    param($response)
    return ($response.success -and $response.message -like "*enrolled*")
}

# Test 8: Course Enrollment - Drop
Test-ApiEndpoint -Name "Course Enrollment - Drop" -Method "DELETE" -Endpoint "/api/student/enroll/CS301" `
    -Headers $authHeaders `
    -ValidationScript {
    param($response)
    return ($response.success)
}

# Test 9: System Health Check
Test-ApiEndpoint -Name "System Health Check" -Method "GET" -Endpoint "/health" `
    -ValidationScript {
    param($response)
    return ($response.status -eq "Healthy")
}

# Test 10: CORS Headers (Options Request)
Test-ApiEndpoint -Name "CORS Headers Check" -Method "OPTIONS" -Endpoint "/api/auth/login" `
    -ExpectedStatusCode = 200

Write-Host ""
Write-Host "📊 Test Results Summary" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
$testResults | Format-Table -AutoSize

Write-Host ""
Write-Host "📈 Overall Results:" -ForegroundColor Cyan
Write-Host "   Total Tests: $totalTests" -ForegroundColor White
Write-Host "   Passed: $passedTests" -ForegroundColor Green
Write-Host "   Failed: $($totalTests - $passedTests)" -ForegroundColor Red
Write-Host "   Success Rate: $([math]::Round(($passedTests / $totalTests) * 100, 1))%" -ForegroundColor $(if ($passedTests -eq $totalTests) { 'Green' } else { 'Yellow' })

Write-Host ""
if ($passedTests -eq $totalTests) {
    Write-Host "🎉 All integration tests passed! Frontend-Backend integration is working correctly." -ForegroundColor Green
    Write-Host "✅ Your Vue.js frontend can successfully communicate with the .NET API" -ForegroundColor Green
    Write-Host "✅ Authentication, profiles, courses, and enrollment all functional" -ForegroundColor Green
}
else {
    Write-Host "⚠️  Some integration tests failed. Check the results above." -ForegroundColor Yellow
    Write-Host "💡 Common issues:" -ForegroundColor Cyan
    Write-Host "   - CORS configuration needs adjustment" -ForegroundColor Gray
    Write-Host "   - API endpoint URLs don't match frontend expectations" -ForegroundColor Gray
    Write-Host "   - Response format differs from frontend interface expectations" -ForegroundColor Gray
}

Write-Host ""
Write-Host "🚀 Next Steps for Frontend Integration:" -ForegroundColor Cyan
Write-Host "1. Update frontend environment variables if needed" -ForegroundColor White
Write-Host "2. Test the actual Vue.js application with the backend" -ForegroundColor White
Write-Host "3. Verify user interface displays data correctly" -ForegroundColor White
Write-Host "4. Test full user workflows (login → dashboard → enrollment)" -ForegroundColor White

Write-Host ""
Write-Host "Frontend Integration Test Complete! 🎓" -ForegroundColor Magenta