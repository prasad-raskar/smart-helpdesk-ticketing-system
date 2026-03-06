# 🎫 Smart Helpdesk Ticketing System

**A Professional-Grade Support Management Platform built with ASP.NET Core & Clean Architecture.**

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512bd4?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-brightgreen)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![JWT Auth](https://img.shields.io/badge/Auth-JWT-orange?logo=jsonwebtokens)](https://jwt.io/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue?logo=docker)](https://www.docker.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

---

## 🚀 Project Overview

The **Smart Helpdesk Ticketing System** is an enterprise-level backend solution designed to manage customer support lifecycles with high precision. In real-world support environments, manual ticket tracking leads to missed SLAs, fragmented communication, and poor visibility. This project solves those challenges by providing a centralized, secure, and scalable API orchestrating the entire support workflow—from initial inquiry to final resolution.

Built with **Clean Architecture**, this system prioritizes maintainability and testability, ensuring that business rules remain decoupled from technical implementations like databases or UI frameworks.

---

## ✨ Key Features

### 🛠️ Core Support Lifecycle
- **Advanced Ticket Management**: Full CRUD operations for support tickets with structured status and priority states.
- **Smart Assignment Engine**: Assign tickets to specific support agents with a transparent audit trail of ownership changes.
- **Threaded Communication**: Integrated comment system for seamless collaboration between users and support staff.

### 🔍 Production-Ready Querying
- **Pagination & Sorting**: Efficiently manage thousands of tickets with configurable page sizes and multi-column sorting.
- **Dynamic Filtering**: Server-side filtering by status, priority, and date ranges using optimized LINQ queries.

### 🛡️ Enterprise Security & Observability
- **JWT Authentication**: Industry-standard stateless authentication using Bearer tokens.
- **Role-Based Authorization (RBAC)**: Fine-grained access control (Admin, Agent, User) ensuring users only perform authorized actions.
- **Global Exception Middleware**: Standardized error responses across the entire API, preventing stack trace leakage in production.
- **Structured Logging**: Deep observability with Serilog, capturing request paths, user contexts, and database performance.

---

## 🏛️ System Architecture

The project implements **Clean Architecture (Onion Architecture)**, which organizes code into concentric layers based on their abstraction level.

### 🧱 The Layers
1. **Domain Layer**: The heart of the system. Contains POCO entities, enums, and core business rules. It has **zero dependencies** on any other layer or framework.
2. **Application Layer**: Contains business logic, service interfaces, and DTOs. It orchestrates the flow of data using the repository pattern.
3. **Infrastructure Layer**: Implements technical concerns such as Entity Framework Core (SQL Server), Repository implementations, and third-party integrations (e.g., JWT signing).
4. **API Layer**: The entry point. Handles HTTP requests, middleware configuration (Authorization, Logging, Exception Handling), and Dependency Injection registration.

---

## 🛠️ Tech Stack & Design Justification

| Technology | Purpose | Why? |
| :--- | :--- | :--- |
| **ASP.NET Core 8** | Core Framework | Top-tier performance, cross-platform, and native support for modern API features. |
| **SQL Server** | Database | ACID compliance and robust relational features required for secure ticket tracking. |
| **EF Core** | ORM | Industry-standard for .NET, enabling type-safe database operations and migrations. |
| **Serilog** | Logging | Structured logging allows for powerful log analysis and professional monitoring. |
| **BCrypt.NET** | Security | One of the most secure and widely-trusted algorithms for password hashing. |
| **AutoMapper** | Mapping | Specifically chosen to keep DTOs and Entities separate without boilerplate code. |
| **xUnit & Moq** | Testing | The gold standard for .NET unit testing and dependency mocking. |

---

## 📂 Folder Structure

```text
Src/
├── Helpdesk.Api/            # Entry point, Controllers, Middleware, DI Config
├── Helpdesk.Application/    # Business Logic, Services, Interfaces, DTOs
├── Helpdesk.Infrastructure/ # EF Core, Repositories, Migrations, Persistence
└── Helpdesk.Domain/         # Entities, Constants, Enums (Core Logic)
Tests/
└── Helpdesk.Tests/          # Unit tests for Application and Logic
```

---

## 🔑 Authentication Flow

This project uses **JWT (JSON Web Tokens)** for secure, stateless authentication.
1. **Register**: User creates an account (Password is hashed using **BCrypt**).
2. **Login**: System validates credentials and generates a signed JWT containing UserID and Role claims.
3. **Authorize**: Clients include the token in headers (`Authorization: Bearer <token>`).
4. **Validation**: The API validates the signature, issuer, and expiration on every request.

---

## ⚙️ Setup & Installation

### 📋 Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or Docker)

### 🚀 Local Development
1. **Clone the Project**
   ```bash
   git clone https://github.com/prasad-raskar/smart-helpdesk-ticketing-system.git
   cd smart-helpdesk-ticketing-system
   ```

2. **Database Setup**
   Update the connection string in `Src/Helpdesk.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=HelpdeskDb;Trusted_Connection=True;"
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update --project Src/Helpdesk.Infrastructure --startup-project Src/Helpdesk.Api
   ```

4. **Run the API**
   ```bash
   dotnet run --project Src/Helpdesk.Api
   ```

---

## 🐳 Docker Deployment

The system is fully containerized for consistent deployment across environments.

```bash
# Spin up the entire stack (API + SQL Server)
docker-compose up --build -d
```
The API will be available at `http://localhost:8080` with the database running in an isolated network.

---

## 📡 API Design & Samples

All endpoints utilize standard RESTful conventions and return formatted JSON.

### **POST /api/auth/register** (Public)
Creates a new user with a specific role.
```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "Password123!",
  "role": "User"
}
```

### **POST /api/ticket** (Protected - [Authorize])
Submit a new support ticket.
```json
{
  "title": "Slow System Performance",
  "description": "Internal dashboard is taking 10s+ to load.",
  "priority": 2, // High
  "createdByUserId": 1
}
```

### **GET /api/ticket** (Advanced Querying)
Retrieve a paginated, sorted list of tickets.
`GET /api/ticket?pageNumber=1&pageSize=10&status=Open&sortBy=createdAt&sortDirection=desc`

---

## 🔮 Future Roadmap
- [ ] **Real-time Notifications**: Integrate SignalR for live updates when tickets are assigned.
- [ ] **AI Classification**: Automatic priority assignment based on ticket description text analysis.
- [ ] **Email Integration**: Auto-generate tickets from incoming support emails.
- [ ] **Analytics Dashboard**: Weekly reports on average resolution time and agent performance.

---

## 📖 Learning Outcomes
Building this system provided deep insights into:
- Implementing **Clean Architecture** in a high-concurrency environment.
- Mastering **Asynchronous Programming** patterns in ASP.NET Core.
- Designing **Normalized Database Schemas** for audit-heavy applications.
- Managing **Dockerized Micro-services** and inter-container communication.

---

## 👨‍💻 Author
**Prasad Raskar**  
- GitHub: [@prasad-raskar](https://github.com/prasad-raskar)

## 📜 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
