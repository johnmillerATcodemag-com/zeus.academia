#!/usr/bin/env pwsh

# Zeus Academia Admin Interface Verification Script
Write-Host "🔍 ZEUS ACADEMIA ADMIN INTERFACE VERIFICATION" -ForegroundColor Yellow
Write-Host "=============================================" -ForegroundColor Yellow
Write-Host ""

# Check if services are running
Write-Host "📋 Checking Service Status..." -ForegroundColor Cyan

# Check API Service
try {
    $apiResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/health" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ API Service: RUNNING on port 5000" -ForegroundColor Green
    Write-Host "   Status: $($apiResponse.status)" -ForegroundColor Gray
}
catch {
    Write-Host "❌ API Service: NOT RESPONDING on port 5000" -ForegroundColor Red
}

# Check Admin Interface
try {
    $adminResponse = Invoke-WebRequest -Uri "http://localhost:5175" -TimeoutSec 5 -ErrorAction Stop
    if ($adminResponse.StatusCode -eq 200) {
        Write-Host "✅ Admin Interface: RUNNING on port 5175" -ForegroundColor Green
    }
}
catch {
    Write-Host "❌ Admin Interface: NOT RESPONDING on port 5175" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎯 Access Instructions:" -ForegroundColor Yellow
Write-Host "1. Navigate to: http://localhost:5175" -ForegroundColor White
Write-Host "2. Login with any credentials (mock authentication)" -ForegroundColor White
Write-Host "3. Click 'Bulk Management' tab to see comprehensive interface" -ForegroundColor White
Write-Host ""
Write-Host "🔧 Interface Features Available:" -ForegroundColor Cyan
Write-Host "• Statistics Dashboard (4 cards)" -ForegroundColor White
Write-Host "• File Upload with Drag-and-Drop" -ForegroundColor White
Write-Host "• User Preview Table" -ForegroundColor White
Write-Host "• Bulk Operations Panel" -ForegroundColor White
Write-Host "• Operation History" -ForegroundColor White
Write-Host "• Progress Tracking" -ForegroundColor White
Write-Host ""

# Open browser automatically
Write-Host "🌐 Opening admin interface in browser..." -ForegroundColor Cyan
Start-Process "http://localhost:5175"