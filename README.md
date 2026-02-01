# Todo App

Monorepo structure for backend (.NET) and frontend (Vue 3 + TypeScript).

## Structure

- backend/
  - src/
    - API/
    - Application/
    - Domain/
    - Infrastructure/
  - tests/
- frontend/
  - src/
    - api/
    - components/
    - composables/
    - models/
    - pages/
    - router/
    - stores/
    - utils/
- docs/
- tests/
  - backend/
  - frontend/

## Status

- Backend solution and projects are scaffolded in backend/.
- Frontend Vite (Vue 3 + TypeScript) is scaffolded in frontend/.

## Run

- Backend: dotnet run --project backend/src/API/TodoApp.Api.csproj
- Frontend: cd frontend && npm run dev

## Next steps

1. Add CQRS handlers, DTOs, and domain models.
2. Configure MongoDB and JWT in Infrastructure + API.
3. Implement auth and todos endpoints.
4. Wire frontend stores and pages to the API.
5. Add tests, linting, formatting, and CI.
