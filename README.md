# 🎫 Smart Helpdesk Ticketing System

A production-ready ASP.NET Core Web API for managing support tickets, featuring Clean Architecture, JWT Authentication, and a robust SQL Server persistence layer.

---

## 🚀 Overview

The **Smart Helpdesk Ticketing System** is designed to streamline customer support operations. It allows customers to create tickets, admins to assign them to agents, and facilitates a complete communication trail through comments.

### Key Features:
- **Clean Architecture**: Decoupled layers for maximum testability.
- **JWT Authentication**: Secure user sessions with role-based claims.
- **Role-Based Authorization**: Granular access control for Admins, Agents, and Users.
- **Password Security**: BCrypt hashing for sensitive user data.
- **Automatic Auditing**: Every entity tracks `CreatedAt` and `UpdatedAt` automatically.
- **Assignment History**: Full audit trail of ticket reassignments.
- **Global Error Handling**: Consistent API response format for all exceptions.

---

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core 8.0 (Web API)
- **Language**: C#
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Security**: JWT Bearer Authentication
- **Logging**: Serilog
- **Documentation**: Swagger / OpenAPI

---

## 🏛️ Architecture Explanation

The project follows the **Clean Architecture (Onion)** pattern to ensure that business logic remains independent of external concerns.

### 📦 Project Structure
- `Helpdesk.Domain`: Core entities, enums, and domain logic. Zero dependencies.
- `Helpdesk.Application`: Service interfaces, DTOs, and business logic implementations.
- `Helpdesk.Infrastructure`: Data persistence (EF Core), repository implementations, and external service configurations.
- `Helpdesk.Api`: Controllers, middleware, and API configuration.

---

## 📊 Database Schema

The system uses a normalized SQL Server schema:
- **Users**: Core identity table (Admin, Agent, Customer roles).
- **Tickets**: Master record tracking status and priority.
- **TicketAssignments**: Audit table tracking assignment changes.
- **TicketComments**: Communication log for specific tickets.

> [!NOTE]
> Detailed SQL scripts can be found in `Database_Schema.sql`.

---

## 🔌 API Endpoints

### Authentication
- `POST /api/auth/register` - Create a new user.
- `POST /api/auth/login` - Authenticate and receive a JWT.

### Tickets (Protected)
- `GET /api/ticket` - List all tickets.
- `GET /api/ticket/{id}` - Get ticket details including comments.
- `POST /api/ticket` - Create a new ticket.
- `PATCH /api/ticket/{id}/status` - Update ticket status (Open -> Resolved).
- `POST /api/ticket/assign` - Assign a ticket to an agent.

### Comments (Protected)
- `POST /api/comment` - Add a reply to a ticket.
- `GET /api/comment/ticket/{id}` - View full conversation for a ticket.

---

## 🛠️ Setup Instructions

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or LocalDB)

### Steps to Run
1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd "Smart Helpdesk Ticketing System"
   ```

2. **Configure Connection String**:
   Update `Src/Helpdesk.Api/appsettings.json` with your SQL Server connection details.

3. **Install Dependencies & Add Migration**:
   ```bash
   dotnet restore
   dotnet ef migrations add InitialCreate --project Src/Helpdesk.Infrastructure --startup-project Src/Helpdesk.Api
   ```

4. **Update Database**:
   ```bash
   dotnet ef database update --project Src/Helpdesk.Infrastructure --startup-project Src/Helpdesk.Api
   ```

5. **Run the API**:
   ```bash
   dotnet run --project Src/Helpdesk.Api
   ```

6. **Test with Swagger**:
   Open `https://localhost:7001/swagger` in your browser.

---

## 🧪 Testing
A complete Postman collection is included in the root directory: `Helpdesk_Postman_Collection.json`. Import it into Postman to start testing the authenticated flows.

---

## 👨‍💻 Author
**Senior Backend Architect**  
*Building scalable, secure, and clean enterprise systems.*
