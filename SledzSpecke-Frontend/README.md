# SledzSpecke Frontend

A medical specialization tracking application for Polish doctors, built with React (web) and designed for future React Native mobile deployment.

## Project Structure

```
SledzSpecke-Frontend/
├── packages/
│   ├── shared/          # Shared domain logic, types, and utilities
│   └── web/             # React web application
```

## Features Implemented

### ✅ Project Infrastructure
- Monorepo setup with npm workspaces
- TypeScript configuration
- Shared domain entities and value objects
- Clean Architecture principles

### ✅ Authentication Module
- Login page with email/password
- Registration page with SMK version selection
- JWT token management
- Protected routes

### ✅ Dashboard
- Module switching (Basic/Specialist)
- Progress cards for all specialization components:
  - Staże (Internships)
  - Dyżury (Medical Shifts)
  - Procedury (Procedures)
  - Kursy (Courses)
  - Samokształcenie (Self-education)
  - Publikacje (Publications)

### ✅ UI/UX
- Material-UI theme with turquoise primary color
- Responsive layout
- Polish language interface
- Navigation drawer with all sections

## Getting Started

1. Install dependencies:
```bash
npm install
cd packages/shared && npm run build
```

2. Start the development server:
```bash
cd packages/web
npm run dev
```

3. Open http://localhost:3000 in your browser

## Technology Stack

- **React 18** with TypeScript
- **Vite** for fast development
- **Material-UI** for components
- **React Router** for navigation
- **React Query** for server state
- **Zustand** for client state
- **React Hook Form** with Zod validation

## API Integration

The frontend is designed to work with the .NET backend API running on port 5000. Proxy configuration is set up in Vite to forward `/api` requests.

## Next Steps

- Complete medical shifts management
- Add procedures tracking
- Implement internships management
- Add courses and self-education modules
- Export functionality
- Deploy to VPS

## Mobile Development

The shared package architecture allows for easy migration to React Native for iOS and Android applications.