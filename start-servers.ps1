# Start Zeus Academia API and Faculty Dashboard
Write-Host "🚀 Starting Zeus Academia API and Faculty Dashboard" -ForegroundColor Green

# Start API in background
Write-Host "Starting API server..." -ForegroundColor Yellow
$apiJob = Start-Job -ScriptBlock {
    Set-Location "C:\git\zeus.academia\src\Zeus.Academia.Api"
    dotnet run
}

# Wait a moment for API to start
Start-Sleep -Seconds 5

# Start Faculty Dashboard in background
Write-Host "Starting Faculty Dashboard..." -ForegroundColor Yellow
$dashboardJob = Start-Job -ScriptBlock {
    Set-Location "C:\git\zeus.academia\src\Zeus.Academia.FacultyDashboard"
    npm run dev
}

Write-Host "✅ Both servers starting in background" -ForegroundColor Green
Write-Host "API should be available at: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Faculty Dashboard should be available at: http://localhost:5173" -ForegroundColor Cyan
Write-Host "Test page available at: C:\git\zeus.academia\api-test.html" -ForegroundColor Cyan

Write-Host ""
Write-Host "To check server status:" -ForegroundColor White
Write-Host "Get-Job" -ForegroundColor Gray
Write-Host ""
Write-Host "To stop servers:" -ForegroundColor White
Write-Host "Get-Job | Stop-Job" -ForegroundColor Gray

# Show job status
Get-Job