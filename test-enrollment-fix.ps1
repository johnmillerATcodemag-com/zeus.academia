#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Quick test for the enrollment endpoint fix
.DESCRIPTION
    Tests the enrollment endpoint to ensure it properly handles string course IDs
#>

Write-Host "🔧 Enrollment Endpoint Fix Verification" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Test configuration
$apiUrl = "http://localhost:5000"
$testCourseId = "CS101"

try {
    Write-Host "Step 1: Testing API health..." -ForegroundColor Yellow
    $health = Invoke-RestMethod -Uri "$apiUrl/health" -TimeoutSec 3
    Write-Host "✅ API Status: $($health.status)" -ForegroundColor Green
    Write-Host ""
    
    Write-Host "Step 2: Getting authentication token..." -ForegroundColor Yellow
    $loginData = @{
        email    = "john.smith@academia.edu"
        password = "password123"
    } | ConvertTo-Json
    
    $loginResponse = Invoke-RestMethod -Uri "$apiUrl/api/auth/login" -Method POST -Body $loginData -ContentType "application/json" -TimeoutSec 5
    
    if ($loginResponse.success -and $loginResponse.token) {
        Write-Host "✅ Login successful - Token received" -ForegroundColor Green
        $token = $loginResponse.token
    }
    else {
        throw "Login failed - No token received"
    }
    Write-Host ""
    
    Write-Host "Step 3: Testing enrollment with course ID '$testCourseId'..." -ForegroundColor Yellow
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type"  = "application/json"
    }
    
    $enrollResponse = Invoke-RestMethod -Uri "$apiUrl/api/student/enroll/$testCourseId" -Method POST -Headers $headers -TimeoutSec 5
    
    Write-Host "✅ Enrollment request successful!" -ForegroundColor Green
    Write-Host "📄 Response details:" -ForegroundColor Cyan
    Write-Host "   Success: $($enrollResponse.Success)" -ForegroundColor White
    Write-Host "   Message: $($enrollResponse.Message)" -ForegroundColor White
    Write-Host "   Enrollment ID: $($enrollResponse.EnrollmentId)" -ForegroundColor White
    Write-Host "   Course ID: $($enrollResponse.CourseId)" -ForegroundColor White
    Write-Host "   Date: $($enrollResponse.EnrollmentDate)" -ForegroundColor White
    Write-Host ""
    
    Write-Host "Step 4: Testing course drop with course ID '$testCourseId'..." -ForegroundColor Yellow
    $dropResponse = Invoke-RestMethod -Uri "$apiUrl/api/student/enroll/$testCourseId" -Method DELETE -Headers $headers -TimeoutSec 5
    
    Write-Host "✅ Drop request successful!" -ForegroundColor Green
    Write-Host "📄 Response details:" -ForegroundColor Cyan
    Write-Host "   Success: $($dropResponse.Success)" -ForegroundColor White
    Write-Host "   Message: $($dropResponse.Message)" -ForegroundColor White
    Write-Host "   Course ID: $($dropResponse.CourseId)" -ForegroundColor White
    Write-Host "   Date: $($dropResponse.DropDate)" -ForegroundColor White
    Write-Host ""
    
    Write-Host "🎉 ALL TESTS PASSED!" -ForegroundColor Green
    Write-Host "✅ Enrollment endpoint now properly handles string course IDs" -ForegroundColor Green
    Write-Host "✅ Frontend integration should work correctly now" -ForegroundColor Green
    
}
catch {
    Write-Host "❌ Test failed: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        Write-Host "   Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        try {
            $errorBody = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorBody)
            $errorText = $reader.ReadToEnd()
            Write-Host "   Error Body: $errorText" -ForegroundColor Red
        }
        catch {
            Write-Host "   Could not read error details" -ForegroundColor Red
        }
    }
    
    Write-Host ""
    Write-Host "💡 Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "1. Ensure the Zeus Academia API is running on port 5000" -ForegroundColor Gray
    Write-Host "2. Check that the API endpoints are properly configured" -ForegroundColor Gray
    Write-Host "3. Verify CORS settings allow requests from frontend" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Test completed at $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray