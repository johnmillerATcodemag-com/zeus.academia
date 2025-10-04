# Backend API Smoke Test Runner

This script helps you run comprehensive smoke tests to verify that the backend API is working correctly.

## Prerequisites

1. **Start the Backend API**:

   ```powershell
   cd c:\git\zeus.academia\src\Zeus.Academia.Api
   dotnet run
   ```

2. **Verify API is running**:
   - The API should be accessible at http://localhost:5000
   - Check that you don't see startup errors in the terminal

## Running the Smoke Tests

### Option 1: Run All Smoke Tests

```powershell
cd c:\git\zeus.academia\src\Zeus.Academia.StudentPortal
npm test -- backend-smoke.test.ts
```

### Option 2: Run Specific Test Suites

```powershell
# Test basic connectivity only
npm test -- backend-smoke.test.ts -t "API Connectivity"

# Test authentication endpoints
npm test -- backend-smoke.test.ts -t "Authentication Endpoints"

# Test course endpoints
npm test -- backend-smoke.test.ts -t "Course Endpoints"

# Test API versioning
npm test -- backend-smoke.test.ts -t "API Versioning"

# Test performance
npm test -- backend-smoke.test.ts -t "Performance Tests"
```

### Option 3: Run with Verbose Output

```powershell
npm test -- backend-smoke.test.ts --reporter=verbose
```

## Expected Results

### ✅ Success Indicators:

- API server responds to health checks
- Authentication endpoints exist (may return 401/400 for invalid credentials)
- Course endpoints exist (may return 401/403 if authentication required)
- API versioning middleware works without server errors
- Error handling returns appropriate status codes
- Response times are reasonable (<5 seconds)

### ❌ Failure Indicators:

- `ECONNREFUSED` errors → Backend API is not running
- `404` errors for core endpoints → API routing issues
- `500` errors → Server-side configuration problems
- Timeout errors → Performance issues or server overload

## Troubleshooting

### Backend Not Running

```
❌ Backend API is not running. Please start the API server on http://localhost:5000
```

**Solution**: Start the backend API with `dotnet run`

### Dependency Injection Errors

```
System.InvalidOperationException: Unable to resolve service for type 'Zeus.Academia.Api.Versioning.IApiVersionService'
```

**Solution**: The IApiVersionService registration was added to fix this issue

### Database Connection Issues

```
Server returned 500 - possible database connection issue
```

**Solution**: Check database connection string and ensure database is accessible

### CORS Issues

```
Access to XMLHttpRequest blocked by CORS policy
```

**Solution**: Verify CORS configuration in the API startup

## Understanding Test Results

The smoke tests are designed to:

1. **Verify Connectivity**: Ensure the API server is running and reachable
2. **Check Endpoints**: Confirm critical endpoints exist and respond
3. **Validate Structure**: Verify response formats match frontend expectations
4. **Test Error Handling**: Ensure graceful error responses
5. **Measure Performance**: Check response times are acceptable

### Test Categories:

- **Smoke Tests**: Basic connectivity and endpoint availability
- **Data Structure Tests**: Verify API responses match frontend requirements
- **Performance Tests**: Ensure reasonable response times

## Integration with Development Workflow

1. **Before Frontend Development**: Run smoke tests to ensure backend is ready
2. **After Backend Changes**: Verify existing functionality still works
3. **Before Deployment**: Ensure all systems are functioning correctly
4. **Troubleshooting**: Identify specific API issues quickly

## Next Steps

Once smoke tests pass:

1. Remove mock API from frontend (if still enabled)
2. Test full frontend-backend integration
3. Run full test suite to ensure compatibility
4. Perform user acceptance testing

## Example Output

```
✅ API server is running (status: 200)
✅ API responded with status: 200
✅ Auth/login endpoint exists (status: 401)
✅ Auth/register endpoint exists (status: 400)
✅ Courses endpoint exists (status: 401)
✅ Courses/paginated endpoint exists (status: 401)
✅ Course search handled (status: 401)
✅ API versioning handled correctly (status: 401)
✅ Missing API version handled gracefully (status: 401)
✅ 404 handling works correctly
✅ Malformed request handled gracefully (status: 400)
✅ Error responses are properly formatted
⚠️ Courses endpoint requires authentication (expected)
✅ API response time: 245ms
✅ Handled 5 concurrent requests in 1247ms
```
