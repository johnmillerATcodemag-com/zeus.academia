# Mock API Setup - Demo Mode 🎭

## Overview

The Zeus Academia Student Portal now includes a **Mock API system** for demonstration purposes. This allows you to showcase all the Task 2 features without needing a real backend server.

## How It Works

The mock API interceptepts HTTP requests and returns realistic mock data, simulating a real backend server with:
- Realistic response delays
- Proper error handling
- Data persistence during the session
- Complete CRUD operations

## Current Status

✅ **Mock API is ENABLED** - Perfect for showcasing features!

## Available Mock Endpoints

### Authentication
- `POST /auth/login` - Login with demo credentials
- `POST /auth/logout` - Logout user
- `GET /auth/me` - Get current user profile
- `PUT /auth/profile` - Update user profile ✅ **This fixes your "Network error"**
- `POST /auth/change-password` - Change password
- `POST /auth/refresh` - Refresh JWT token

### Emergency Contacts
- `GET /emergency-contacts` - List all emergency contacts
- `POST /emergency-contacts` - Add new emergency contact
- `PUT /emergency-contacts/:id` - Update emergency contact
- `DELETE /emergency-contacts/:id` - Delete emergency contact

### Documents
- `GET /documents` - List uploaded documents
- `POST /documents/upload` - Upload new document
- `POST /profile/photo` - Upload profile photo
- `DELETE /documents/:id` - Delete document

## Demo Credentials

**Login:** `student@zeus.edu`  
**Password:** `password123`

## Sample Data

The mock API comes pre-loaded with:
- **Student Profile**: John Doe, STU001, GPA 3.85
- **Emergency Contacts**: 2 pre-loaded contacts (Mother & Father)
- **Documents**: 2 sample documents (Transcript & Student ID)
- **Address**: Complete address information

## Testing the Fix

1. **Navigate to:** http://localhost:5177/
2. **Login** with the demo credentials
3. **Go to Profile page**
4. **Click "Edit Profile"**
5. **Make changes and save**
6. **✅ Should now work without "Network error"!**

## Mock API Features

### 🎯 Realistic Behavior
- Network delays (300ms - 2000ms depending on operation)
- File upload simulation with progress
- Form validation responses
- Error handling for edge cases

### 💾 Session Persistence
- Data changes persist during your session
- Add/edit/delete operations work as expected
- Profile updates are maintained

### 🔒 Authentication Simulation
- JWT token generation and validation
- Session management
- Auto-refresh token handling

## Configuration

The mock API is controlled by environment variables:

```env
# .env.development
VITE_MOCK_API=true  # Enables mock API
```

## Development vs Production

- **Development**: Mock API enabled by default
- **Production**: Mock API disabled, uses real backend
- **Demo Mode**: Can be enabled with `VITE_MOCK_API=true`

## Console Output

When the mock API is active, you'll see console messages like:
```
🎭 Mock API enabled for development/demo purposes
🎭 Mock response for: PUT /auth/profile
```

## Troubleshooting

If you're still getting network errors:

1. **Check the console** for mock API messages
2. **Refresh the browser** to reload the configuration
3. **Verify the .env.development** file has `VITE_MOCK_API=true`
4. **Restart the dev server** if needed

## Next Steps

Your **"Network error: Unable to connect to server"** issue should now be resolved! 

🎉 **Ready to showcase all Task 2 features without any network errors!**