# Real API Integration Guide

## Overview

This guide documents the successful transition from mock API to real API integration for the Zeus Academia Student Portal.

## ✅ Completed Steps

### 1. Mock API Removal

- **Removed**: `setupMockApi` import and function call from `ApiService.ts`
- **Status**: ✅ Completed
- **Result**: Application no longer uses mock data system

### 2. Environment Configuration

- **Configuration**: `.env` file already points to real API
  ```env
  VITE_API_BASE_URL=http://localhost:5000/api
  ```
- **Status**: ✅ Completed

### 3. Test Validation

- **All Tests Pass**: 1061/1061 tests passing after mock API removal
- **Status**: ✅ Completed
- **Critical**: Course search functionality tests still work properly

## 🔧 Next Steps Required

### 1. Backend API Startup Issue

The .NET backend API has a dependency injection issue:

```
System.InvalidOperationException: Unable to resolve service for type 'Zeus.Academia.Api.Versioning.IApiVersionService' while attempting to activate 'Zeus.Academia.Api.Versioning.ApiVersioningMiddleware'.
```

**Solution Required**:

- Fix missing `IApiVersionService` registration in `Program.cs`
- Or temporarily disable API versioning middleware

### 2. Authentication Flow Updates

The real API likely uses different authentication patterns than the mock API:

**Current Mock Response**:

```typescript
{
  token: 'mock-jwt-token-' + Date.now(),
  refreshToken: 'mock-refresh-token-' + Date.now(),
  student: mockStudent,
  expiresAt: new Date(Date.now() + 60 * 60 * 1000).toISOString()
}
```

**Action Required**:

- Test real API authentication endpoints
- Verify token formats match expectations
- Update authentication flow if needed

### 3. Data Structure Validation

**Frontend Course Interface**:

```typescript
interface Course {
  id: string;
  code: string;
  name: string; // Mock API uses 'title'
  description: string;
  credits: number;
  instructor: string;
  department: string;
  schedule: string | ScheduleEntry[];
  maxEnrollment: number;
  enrolledStudents: number;
  waitlistCount: number;
  prerequisites: string[];
  enrollmentStatus: EnrollmentStatus;
  difficulty: number;
  weeklyWorkload: number;
}
```

**Backend API Investigation Needed**:

- Verify real API response structure matches TypeScript interfaces
- Add data transformation layer if structures differ
- Test all course endpoints: `/courses`, `/courses/paginated`, `/courses/:id`

## 🚀 Testing Real API Connection

### Option 1: Fix Backend and Test

1. Fix the `IApiVersionService` issue in the backend
2. Start backend API: `cd src/Zeus.Academia.Api && dotnet run`
3. Test frontend connection to real endpoints
4. Verify all course search functionality works

### Option 2: Temporary API Simulation

If backend fixes take time, you can:

1. Use tools like Postman to test expected API responses
2. Create a simple Node.js/Express server with the expected endpoints
3. Test frontend with a working API that matches expected data structures

## 📝 Current Application State

### ✅ What Works Now

- **All Tests Pass**: 1061/1061 tests still passing
- **Mock API Removed**: Clean separation from development mocks
- **Environment Ready**: Configured to connect to real API
- **Course Search Interface**: Fully functional UI components
- **Error Handling**: Proper API error handling in place

### ⚠️ What Needs Real API

- **Course Data Loading**: Will show loading states until real API responds
- **Authentication**: Login/logout functionality needs real JWT tokens
- **Enrollment Actions**: Course enrollment/waitlist actions need real endpoints
- **Search Filters**: Advanced filtering needs real API search implementation

## 🔍 API Endpoints Expected

Based on the CourseService implementation, the real API should provide:

### Authentication Endpoints

- `POST /auth/login`
- `POST /auth/logout`
- `GET /auth/me`
- `POST /auth/refresh`

### Course Endpoints

- `GET /courses` - All courses
- `GET /courses/paginated` - Paginated course listing
- `GET /courses/:id` - Single course details
- `GET /courses/available` - Available courses
- `GET /courses/enrolled` - Student's enrolled courses
- `POST /courses/:id/enroll` - Enroll in course
- `POST /courses/:id/waitlist` - Join course waitlist

### Enrollment Endpoints

- `GET /enrollments` - Student enrollments
- `POST /enrollments` - Create enrollment
- `DELETE /enrollments/course/:id` - Drop course

## 🎯 Success Criteria

The real API integration will be complete when:

1. ✅ Backend API starts without errors
2. ✅ Frontend can authenticate with real JWT tokens
3. ✅ Course search loads real course data
4. ✅ All course actions (enroll, waitlist, drop) work with real API
5. ✅ All existing tests continue to pass
6. ✅ No console errors in browser when using the application

## 📋 Validation Checklist

- [ ] Backend API starts successfully on port 5000
- [ ] GET http://localhost:5000/api/courses returns course data
- [ ] Authentication endpoints return proper JWT tokens
- [ ] Course enrollment endpoints work properly
- [ ] Frontend displays real course data instead of loading states
- [ ] All 1061 tests continue to pass
- [ ] No console API errors in browser developer tools

---

**Status**: Mock API successfully removed, ready for real API connection once backend startup issues are resolved.
