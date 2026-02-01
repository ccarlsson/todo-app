# Software Requirements Specification (SRS) – Todo Application

## 1. Introduktion

### 1.1 Syfte
Syftet med denna Software Requirements Specification (SRS) är att specificera funktionella och icke‑funktionella krav för en Todo‑applikation med:

- Backend byggd i **.NET 10** och **C# 14**
- Arkitektur enligt **SOLID**, **Clean Architecture** och **CQRS**
- Databas: **NoSQL**, specifikt **MongoDB**
- Frontend byggd i **Vue 3** och **TypeScript**
- Användarhantering med personliga todo‑flöden

Dokumentet ska kunna användas som underlag för utveckling, testning, dokumentation och undervisning.

### 1.2 Målgrupp
- Backend‑utvecklare
- Frontend‑utvecklare
- Testare
- Projektledare
- Studenter i t.ex. *Gränssnittsdesign* och *Digitalt skapande 2*

### 1.3 Omfattning
Systemet ska tillåta användare att:

- registrera sig och logga in
- hantera sina personliga todo‑listor (skapa, läsa, uppdatera, ta bort)
- se sina todos filtrerade och sorterade

Systemet ska exponera ett REST‑API och en webbaserad klient.

---

## 2. Systemöversikt

### 2.1 Övergripande arkitektur
Systemet består av två huvuddelar:

- **Backend API**
  - .NET 10, C# 14
  - Clean Architecture
  - SOLID
  - CQRS (Commands/Queries)
  - MongoDB som datalager
  - JWT‑baserad autentisering

- **Frontend**
  - Vue 3
  - TypeScript
  - State management med Pinia
  - Kommunikation med backend via HTTP (REST, JSON)

### 2.2 Hög nivå – komponenter

- **Client (Webbapp)**: UI, routing, state, anrop till API.
- **API**: Controllers, autentisering, validering, mapping.
- **Application‑lager**: Use cases, CQRS‑handlers, validering.
- **Domain‑lager**: Entiteter, value objects, domänlogik.
- **Infrastructure‑lager**: MongoDB‑åtkomst, repositories, externa tjänster.

---

## 3. Funktionella krav

### 3.1 Användarhantering

#### 3.1.1 Registrering
- Systemet ska tillåta nya användare att registrera sig med:
  - e‑postadress (obligatorisk)
  - lösenord (obligatoriskt)
- Systemet ska validera att e‑postadressen är unik.
- Lösenord ska lagras hashat i databasen.
- Vid lyckad registrering ska användaren kunna logga in med sina uppgifter.

#### 3.1.2 Inloggning
- Systemet ska tillåta användare att logga in med:
  - e‑postadress
  - lösenord
- Vid lyckad inloggning ska systemet returnera en **JWT‑token**.
- Vid misslyckad inloggning ska systemet returnera ett relevant felmeddelande.

#### 3.1.3 Autentisering och auktorisation
- Alla todo‑relaterade endpoints ska kräva en giltig JWT‑token.
- Systemet ska säkerställa att en användare endast kan komma åt sina egna todos.
- Försök att komma åt andra användares data ska resultera i **403 Forbidden** eller motsvarande.

---

### 3.2 Todo‑hantering

#### 3.2.1 Skapa Todo
- Användaren ska kunna skapa en ny todo med följande fält:
  - Titel (obligatoriskt)
  - Beskrivning (valfritt)
  - Förfallodatum (valfritt)
  - Prioritet (valfritt, t.ex. Low/Medium/High)
  - Status (default: `NotStarted`)
- Systemet ska koppla todo‑posten till den inloggade användarens ID.
- Vid lyckad skapning ska systemet returnera ID för den nya todo‑posten.

#### 3.2.2 Visa Todos
- Användaren ska kunna hämta en lista över sina todos.
- Systemet ska stödja filtrering på:
  - status
  - prioritet
  - förfallodatum (t.ex. kommande, förfallna)
- Systemet ska stödja sortering (t.ex. efter förfallodatum eller skapelsedatum).

#### 3.2.3 Visa en enskild Todo
- Användaren ska kunna hämta detaljer för en specifik todo via dess ID.
- Systemet ska säkerställa att todo‑posten tillhör den inloggade användaren.

#### 3.2.4 Uppdatera Todo
- Användaren ska kunna uppdatera:
  - titel
  - beskrivning
  - förfallodatum
  - prioritet
  - status
- Systemet ska validera att todo‑posten tillhör den inloggade användaren.

#### 3.2.5 Ta bort Todo
- Användaren ska kunna ta bort en todo via dess ID.
- Systemet ska validera att todo‑posten tillhör den inloggade användaren.
- Vid lyckad borttagning ska systemet returnera en bekräftelse.

---

## 4. Icke‑funktionella krav

### 4.1 Prestanda
- API‑svarstid för standardoperationer (CRUD på todos) bör normalt vara < 200 ms under normal belastning.
- Systemet ska kunna hantera minst 100 samtidiga användare utan märkbar degradering.

### 4.2 Säkerhet
- Lösenord ska lagras hashade med en modern algoritm (t.ex. bcrypt eller liknande).
- JWT‑tokens ska signeras med en säker nyckel.
- JWT‑tokens ska ha begränsad livslängd (t.ex. 15–60 minuter).
- Systemet ska använda HTTPS i produktion.
- CORS ska konfigureras så att endast godkända klientdomäner kan anropa API:et.

### 4.3 Skalbarhet
- Backend ska kunna skalas horisontellt (flera instanser bakom en load balancer).
- MongoDB ska kunna köras i replika‑set/cluster vid behov.
- Frontend ska kunna distribueras som statiska filer via t.ex. CDN.

### 4.4 Tillgänglighet och driftsäkerhet
- Systemet ska sträva efter hög tillgänglighet (t.ex. 99 % eller bättre).
- Loggning ska finnas för fel, varningar och viktiga händelser.
- Det ska finnas stöd för grundläggande övervakning (health checks).

### 4.5 Kodkvalitet
- Backend‑kod ska följa **SOLID‑principerna**.
- Clean Architecture ska tillämpas för att separera domän, applikation, infrastruktur och API.
- CQRS ska användas för att separera läs‑ och skrivoperationer.
- Enhetstester ska finnas för centrala use cases (commands/queries).
- Frontend‑kod ska vara modulär, typad (TypeScript) och följa separation of concerns.

---

## 5. Backendarkitektur

### 5.1 Översikt – Clean Architecture + CQRS + MongoDB

Backend ska struktureras enligt Clean Architecture med följande lager:

- **Domain**
- **Application**
- **Infrastructure**
- **API**

CQRS ska användas i Application‑lagret:

- **Commands**: ändrar systemets tillstånd (CreateTodo, UpdateTodo, DeleteTodo, RegisterUser, LoginUser).
- **Queries**: hämtar data utan att ändra tillstånd (GetTodos, GetTodoById, GetUserProfile).

MongoDB används som datalager för både användare och todos.

### 5.2 Lagerstruktur (exempel)

src/
 ├─ Domain/
 │   ├─ Entities/
 │   │   ├─ User.cs
 │   │   └─ Todo.cs
 │   ├─ ValueObjects/
 │   │   ├─ Email.cs
 │   │   ├─ TodoStatus.cs
 │   │   └─ Priority.cs
 │   └─ Interfaces/
 │       └─ IAggregateRoot.cs (valfritt)
 ├─ Application/
 │   ├─ Commands/
 │   │   ├─ Todos/
 │   │   │   ├─ CreateTodoCommand.cs
 │   │   │   ├─ UpdateTodoCommand.cs
 │   │   │   └─ DeleteTodoCommand.cs
 │   │   └─ Users/
 │   │       ├─ RegisterUserCommand.cs
 │   │       └─ LoginUserCommand.cs
 │   ├─ Queries/
 │   │   ├─ Todos/
 │   │   │   ├─ GetTodosQuery.cs
 │   │   │   └─ GetTodoByIdQuery.cs
 │   │   └─ Users/
 │   │       └─ GetUserProfileQuery.cs (ev. framtida)
 │   ├─ DTOs/
 │   ├─ Behaviors/   # t.ex. validering, logging, pipeline behaviors
 │   └─ Interfaces/
 │       ├─ ITodoRepository.cs
 │       └─ IUserRepository.cs
 ├─ Infrastructure/
 │   ├─ Persistence/
 │   │   ├─ MongoDbContext.cs
 │   │   ├─ Repositories/
 │   │   │   ├─ TodoRepository.cs
 │   │   │   └─ UserRepository.cs
 │   │   └─ Configurations/
 │   ├─ Services/
 │   │   └─ JwtService.cs
 │   └─ Auth/
 └─ API/
     ├─ Controllers/
     │   ├─ AuthController.cs
     │   └─ TodosController.cs
     ├─ Models/      # Request/Response‑modeller
     └─ Filters/

### 5.3 Domain‑lager

#### 5.3.1 Entiteter
- **User**
  - Id
  - Email (value object)
  - PasswordHash
- **Todo**
  - Id
  - UserId
  - Title
  - Description
  - DueDate
  - Priority (value object eller enum)
  - Status (value object eller enum)
  - CreatedAt
  - UpdatedAt

#### 5.3.2 Value Objects
- **Email**
  - Validerar korrekt e‑postformat.
- **TodoStatus**
  - T.ex. `NotStarted`, `InProgress`, `Completed`.
- **Priority**
  - T.ex. `Low`, `Medium`, `High`.

### 5.4 Application‑lager

#### 5.4.1 Commands
Commands representerar intentioner att ändra systemets tillstånd.

Exempel:

public record CreateTodoCommand(
    string UserId,
    string Title,
    string? Description,
    DateTime? DueDate,
    string? Priority
) : IRequest<string>;

Varje command har en **CommandHandler** som:

- validerar input
- hämtar/uppdaterar domänobjekt via repositories
- sparar ändringar i databasen
- returnerar resultat (t.ex. ID)

#### 5.4.2 Queries
Queries hämtar data utan att ändra tillstånd.

Exempel:

public record GetTodosQuery(string UserId) : IRequest<List<TodoDto>>;

Varje query har en **QueryHandler** som:

- hämtar data från MongoDB via repository
- mappar till DTOs
- returnerar resultatet

#### 5.4.3 DTOs
DTOs används för att:

- separera interna domänmodeller från externa API‑modeller
- undvika att domänobjekt exponeras direkt

Exempel: `TodoDto`, `UserDto`.

### 5.5 Infrastructure‑lager

#### 5.5.1 MongoDB‑modellering

**User‑collection (exempel):**

{
  "_id": "ObjectId",
  "email": "string",
  "passwordHash": "string"
}

**Todo‑collection (exempel):**

{
  "_id": "ObjectId",
  "userId": "ObjectId",
  "title": "string",
  "description": "string",
  "dueDate": "2025-01-01T00:00:00Z",
  "priority": "High",
  "status": "NotStarted",
  "createdAt": "2025-01-01T00:00:00Z",
  "updatedAt": "2025-01-01T00:00:00Z"
}

#### 5.5.2 Repository‑mönster

Exempel på repository‑interface:

public interface ITodoRepository
{
    Task<string> CreateAsync(Todo todo);
    Task<List<Todo>> GetByUserAsync(string userId);
    Task<Todo?> GetByIdAsync(string id, string userId);
    Task UpdateAsync(Todo todo);
    Task DeleteAsync(string id, string userId);
}

Implementationen använder MongoDB‑drivern men döljer detaljerna för Application‑lagret.

### 5.6 API‑lager

- **Controllers** tar emot HTTP‑anrop, validerar request‑modeller och skickar vidare till MediatR (commands/queries).
- **Models** (request/response) används för att definiera API‑kontrakt.
- **Middleware** hanterar autentisering (JWT), felhantering och logging.

---

## 6. Frontendarkitektur – Vue + TypeScript

### 6.1 Teknisk stack

- **Vue 3**
- **TypeScript**
- **Pinia** för state management
- **Vue Router** för routing
- HTTP‑klient: Axios eller Fetch

### 6.2 Rekommenderad mappstruktur

src/
 ├─ api/              # HTTP‑klienter, API‑services
 ├─ components/       # Återanvändbara UI‑komponenter
 ├─ composables/      # Vue composables (t.ex. useTodo, useAuth)
 ├─ models/           # TypeScript‑interfaces (Todo, User, AuthResponse, etc.)
 ├─ pages/            # Sidor (LoginPage, RegisterPage, TodoListPage, etc.)
 ├─ router/           # Vue Router‑konfiguration
 ├─ stores/           # Pinia stores (authStore, todoStore)
 ├─ utils/            # Hjälpfunktioner
 └─ App.vue

### 6.3 Koppling till CQRS‑tänket

Även om frontend inte strikt implementerar CQRS kan man:

- behandla **“write”‑operationer** (skapa/uppdatera/radera) som commands i services/stores
- behandla **“read”‑operationer** (hämta listor/detaljer) som queries

Exempel:

- `todoService.createTodo(payload)` → motsvarar `CreateTodoCommand`
- `todoService.getTodos()` → motsvarar `GetTodosQuery`

### 6.4 State management (Pinia)

Exempel på `todoStore`:

- state: lista av todos, laddningsstatus, fel
- actions:
  - `fetchTodos()`
  - `createTodo()`
  - `updateTodo()`
  - `deleteTodo()`

### 6.5 Sidor och flöden

- **LoginPage**
  - Formulär för e‑post och lösenord
  - Anropar authStore → login → sparar JWT
- **RegisterPage**
  - Formulär för registrering
- **TodoListPage**
  - Visar lista med todos
  - Möjlighet att filtrera/sortera
  - Länkar till skapa/uppdatera
- **TodoEditPage**
  - Formulär för att skapa/uppdatera todo

---

## 7. API‑specifikation (översikt)

### 7.1 Autentisering

#### POST /auth/register
- **Beskrivning:** Registrera ny användare.
- **Request body (exempel):**
  {
    "email": "user@example.com",
    "password": "Secret123!"
  }
- **Svar vid success:**
  - 201 Created eller 200 OK
  - Enkel bekräftelse eller användar‑DTO (utan lösenord).

#### POST /auth/login
- **Beskrivning:** Logga in och få JWT‑token.
- **Request body (exempel):**
  {
    "email": "user@example.com",
    "password": "Secret123!"
  }
- **Svar vid success (exempel):**
  {
    "token": "jwt-token-here",
    "expiresIn": 3600
  }

### 7.2 Todos

Alla endpoints nedan kräver giltig JWT‑token i `Authorization: Bearer <token>`.

#### GET /todos
- **Beskrivning:** Hämta alla todos för inloggad användare.
- **Query‑parametrar (valfria):**
  - `status`
  - `priority`
  - `sortBy` (t.ex. `dueDate`, `createdAt`)
- **Svar (exempel):**
  [
    {
      "id": "todo-id-1",
      "title": "Köpa mjölk",
      "description": "Glöm inte laktosfri",
      "dueDate": "2025-01-01T00:00:00Z",
      "priority": "High",
      "status": "NotStarted",
      "createdAt": "2024-12-31T10:00:00Z",
      "updatedAt": "2024-12-31T10:00:00Z"
    }
  ]

#### GET /todos/{id}
- **Beskrivning:** Hämta en specifik todo.
- **Svar:** Todo‑objekt om det tillhör användaren, annars 404/403.

#### POST /todos
- **Beskrivning:** Skapa ny todo.
- **Request body (exempel):**
  {
    "title": "Köpa mjölk",
    "description": "Glöm inte laktosfri",
    "dueDate": "2025-01-01T00:00:00Z",
    "priority": "High"
  }
- **Svar vid success (exempel):**
  {
    "id": "nytt-todo-id"
  }

#### PUT /todos/{id}
- **Beskrivning:** Uppdatera befintlig todo.
- **Request body (exempel):**
  {
    "title": "Köpa mjölk och bröd",
    "description": "Glöm inte laktosfri",
    "dueDate": "2025-01-02T00:00:00Z",
    "priority": "Medium",
    "status": "InProgress"
  }

#### DELETE /todos/{id}
- **Beskrivning:** Ta bort en todo.
- **Svar:** 204 No Content eller 200 OK med bekräftelse.

---

## 8. Framtida utbyggnad (icke‑bindande)

- Delade listor mellan användare
- Påminnelser/notiser
- Taggar och kategorier
- Offline‑stöd i frontend
- Rollbaserad åtkomst (admin, etc.)
