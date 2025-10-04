# Zeus Academia - Student Portal

A modern Vue.js 3 application with TypeScript for managing student academic activities at Zeus Academia.

## 🚀 Features

- **Vue.js 3 with TypeScript**: Modern reactive framework with full type safety
- **Vuex State Management**: Centralized state management with TypeScript support
- **Vite Build Pipeline**: Fast development and optimized production builds
- **Bootstrap 5 Integration**: Responsive design with Bootstrap Vue components
- **API Service Layer**: Comprehensive HTTP client with authentication handling
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Responsive Design**: Mobile-first approach with Bootstrap 5
- **Test-Driven Development**: Comprehensive test suite with Vitest

## 📋 Acceptance Criteria ✅

### ✅ AC1: Vue.js 3 with TypeScript Integration

- Vue 3 Composition API
- Full TypeScript support
- Component-based architecture
- Type-safe props and emits

### ✅ AC2: Vuex State Management with TypeScript

- Centralized state management
- Auth module with user authentication
- Courses module with enrollment management
- Type-safe mutations and actions

### ✅ AC3: Vite Build Pipeline

- Fast development server
- Optimized production builds
- Code splitting and tree shaking
- Asset optimization and minification

### ✅ AC4: Bootstrap 5 Integration

- Responsive design system
- Bootstrap Vue components
- Custom Zeus Academia theme
- Mobile-first approach

### ✅ AC5: API Service Layer with Authentication

- Axios-based HTTP client
- JWT token management
- Request/response interceptors
- Automatic token refresh
- Comprehensive error handling

## 🛠️ Technologies Used

- **Vue.js 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe JavaScript
- **Vuex 4** - State management
- **Vue Router 4** - Client-side routing
- **Vite** - Build tool and dev server
- **Bootstrap 5** - CSS framework
- **Bootstrap Vue Next** - Vue 3 Bootstrap components
- **Axios** - HTTP client
- **Vitest** - Testing framework
- **ESLint** - Code linting

## 📦 Installation

### Prerequisites

- Node.js 18+
- npm or yarn package manager

### Setup Steps

1. **Clone the repository** (if not already done)

   ```bash
   cd c:\git\zeus.academia\src\Zeus.Academia.StudentPortal
   ```

2. **Install dependencies**

   ```bash
   npm install
   ```

3. **Environment Configuration**
   Copy and configure environment files:

   ```bash
   cp .env.development .env.local
   ```

   Update `.env.local` with your API endpoints:

   ```
   VITE_API_BASE_URL=http://localhost:5000/api
   VITE_APP_TITLE=Zeus Academia - Student Portal
   ```

4. **Start development server**

   ```bash
   npm run dev
   ```

5. **Open in browser**
   Navigate to `http://localhost:5173`

## 🧪 Testing

### Run Tests

```bash
# Run all tests
npm run test

# Run tests with UI
npm run test:ui

# Run tests with coverage
npm run test:coverage
```

### Test Coverage

The project includes comprehensive tests covering:

- Component functionality
- Vuex store operations
- API service layer
- Authentication flows
- All acceptance criteria

## 🏗️ Build and Deploy

### Development Build

```bash
npm run dev
```

### Production Build

```bash
npm run build
```

### Preview Production Build

```bash
npm run preview
```

### Type Checking

```bash
npm run type-check
```

### Linting

```bash
npm run lint
```

## 📁 Project Structure

```
src/
├── components/          # Reusable Vue components
│   ├── NavBar.vue      # Navigation component
│   └── Footer.vue      # Footer component
├── views/              # Page components
│   ├── Home.vue        # Landing page
│   ├── Login.vue       # Authentication
│   ├── Dashboard.vue   # Student dashboard
│   ├── Courses.vue     # Course management
│   ├── Profile.vue     # User profile
│   └── NotFound.vue    # 404 page
├── store/              # Vuex store modules
│   ├── modules/
│   │   ├── auth.ts     # Authentication module
│   │   └── courses.ts  # Courses module
│   └── index.ts        # Main store configuration
├── services/           # API service layer
│   ├── ApiService.ts   # Base HTTP client
│   ├── AuthService.ts  # Authentication services
│   └── CourseService.ts # Course management services
├── types/              # TypeScript type definitions
│   └── index.ts        # Main type exports
├── router/             # Vue Router configuration
│   └── index.ts        # Route definitions
├── App.vue             # Root component
├── main.ts             # Application entry point
└── style.css          # Global styles

tests/                  # Test files
├── acceptance.test.ts  # A-C verification tests
├── components.test.ts  # Component tests
├── services.test.ts    # Service layer tests
└── setup.ts           # Test configuration
```

## 🔧 Configuration Files

- `package.json` - Dependencies and scripts
- `tsconfig.json` - TypeScript configuration
- `vite.config.ts` - Vite build configuration
- `vitest.config.ts` - Test configuration
- `.eslintrc.js` - ESLint configuration
- `.env.*` - Environment variables

## 🎯 Key Features

### Authentication System

- JWT token-based authentication
- Automatic token refresh
- Secure logout functionality
- Protected routes

### Course Management

- Browse available courses
- Enroll/drop courses
- View course schedules
- Track academic progress

### Student Dashboard

- Academic overview
- Quick stats
- Recent activity
- Course shortcuts

### Responsive Design

- Mobile-first approach
- Bootstrap 5 grid system
- Responsive navigation
- Touch-friendly interfaces

## 🔐 Security Features

- JWT token management
- Automatic token refresh
- Request/response interceptors
- CSRF protection ready
- Secure localStorage usage

## 🚦 Development Workflow

1. **Feature Development**

   - Create feature branch
   - Write tests first (TDD)
   - Implement functionality
   - Run tests and linting

2. **Code Quality**

   - TypeScript strict mode
   - ESLint configuration
   - Pre-commit hooks (recommended)
   - Code coverage reporting

3. **Testing Strategy**
   - Unit tests for components
   - Integration tests for services
   - E2E tests for critical flows
   - Acceptance criteria validation

## 📊 Performance Optimizations

- **Code Splitting**: Automatic route-based splitting
- **Tree Shaking**: Dead code elimination
- **Asset Optimization**: Image and font optimization
- **Bundle Analysis**: Built-in bundle analyzer
- **Lazy Loading**: Dynamic imports for routes

## 🔄 State Management

### Vuex Store Structure

- **Auth Module**: User authentication and profile
- **Courses Module**: Course data and enrollments
- **Loading States**: Global loading management
- **Error Handling**: Centralized error management

## 🎨 Theming and Styling

- **Bootstrap 5**: Modern CSS framework
- **Custom Zeus Theme**: Academia-specific styling
- **CSS Variables**: Consistent color scheme
- **Responsive Utilities**: Mobile-first classes

## 📱 Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## 🤝 Contributing

1. Follow TypeScript best practices
2. Write tests for new features
3. Follow the existing code style
4. Update documentation as needed

## 📞 Support

For technical support or questions about the Student Portal:

- Review the documentation
- Check the test files for usage examples
- Refer to the API service implementations

---

**Zeus Academia Student Portal** - Empowering students through technology 🎓
