```
██╗   ██╗███████╗██████╗ ████████╗███████╗██╗  ██╗
██║   ██║██╔════╝██╔══██╗╚══██╔══╝██╔════╝╚██╗██╔╝
██║   ██║█████╗  ██████╔╝   ██║   █████╗   ╚███╔╝ 
╚██╗ ██╔╝██╔══╝  ██╔══██╗   ██║   ██╔══╝   ██╔██╗ 
 ╚████╔╝ ███████╗██║  ██║   ██║   ███████╗██╔╝ ██╗
  ╚═══╝  ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝
```

# 🛒 Vertex Commerce

> **A Modern E-Commerce Backend Platform Built with .NET 10, Modular Monolith Architecture & Polyglot Persistence**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?style=for-the-badge&logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![GraphQL](https://img.shields.io/badge/GraphQL-E10098?style=for-the-badge&logo=graphql&logoColor=white)](https://graphql.org/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)

---

> ⚠️ **Work in Progress**: This project is actively under development. Core features are implemented and functional, with additional enhancements coming soon.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Design Patterns](#-design-patterns)
- [Features](#-features)
- [Notifications](#notifications)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Roadmap](#-roadmap)

---

## 🎯 Overview

**Vertex Commerce** is a portfolio project demonstrating modern software architecture and best practices in building scalable e-commerce backends. It showcases:

- **Modular Monolith Architecture** with clear bounded contexts
- **Polyglot Persistence** using PostgreSQL for transactions and MongoDB for read models
- **Hybrid API Strategy** combining REST for commands and GraphQL for queries
- **Domain-Driven Design (DDD)** with rich domain models
- **CQRS Pattern** for optimized read/write operations

---

## 🏗 Architecture

### High-Level Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                          CLIENTS                                │
│               (Web App, Mobile App, Admin Panel)                │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                        API GATEWAY                              │
│                    VertexCommerce.Api                           │
│            ┌─────────────┬─────────────┐                        │
│            │  REST API   │   GraphQL   │                        │
│            │ (Commands)  │  (Queries)  │                        │
│            └─────────────┴─────────────┘                        │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                    MODULAR MONOLITH                             │
├───────────┬───────────┬───────────┬───────────┬─────────────────┤
│  Catalog  │  Orders   │  Basket   │ Identity  │   Customers     │
│  Module   │  Module   │  Module   │  Module   │    Module       │
├───────────┴───────────┴───────────┴───────────┴─────────────────┤
│                       SHARED KERNEL                             │
│            (Common abstractions, Domain primitives)             │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                       DATA LAYER                                │
│       ┌────────────────┐         ┌────────────────┐             │
│       │  PostgreSQL    │         │    MongoDB     │             │
│       │  (Write/OLTP)  │         │  (Read/Cache)  │             │
│       │                │         │                │             │
│       │ • Products     │         │ • Basket       │             │
│       │ • Categories   │         │ • ProductRead  │             │
│       │ • Orders       │         │   Models       │             │
│       │ • Users        │         │                │             │
│       └────────────────┘         └────────────────┘             │
└─────────────────────────────────────────────────────────────────┘
```

### Module Communication

```
┌───────────┐     ┌───────────┐     ┌───────────┐
│  Catalog  │────▶│  Basket   │────▶│  Orders   │
│  Module   │     │  Module   │     │  Module   │
└───────────┘     └───────────┘     └───────────┘
      │                                   │
      │ IProductService                   │
      └───────────────────────────────────┘
                                          │
┌───────────┐     ┌───────────┐           │
│ Identity  │────▶│ Customers │◀──────────┘
│  Module   │     │  Module   │
└───────────┘     └───────────┘

Communication: Via shared interfaces (not direct DB access)
```

---

## 🛠 Technology Stack

| Category | Technologies |
|----------|-------------|
| **Runtime** | .NET 10, C# 14 |
| **Databases** | PostgreSQL 16, MongoDB 7 |
| **ORM** | Entity Framework Core 10, MongoDB.Driver |
| **API** | Minimal APIs (REST), HotChocolate (GraphQL) |
| **Authentication** | JWT Bearer, BCrypt |
| **Patterns** | MediatR (CQRS), FluentValidation |
| **Documentation** | Swagger/OpenAPI |
| **Infrastructure** | Docker, Docker Compose |
| **Logging** | Serilog |

---

## 📁 Project Structure

```
VertexCommerce/
│
├── src/
│   ├── VertexCommerce.Api/                    # API Host
│   │   ├── Endpoints/                         # REST Endpoints
│   │   ├── GraphQL/                           # GraphQL Queries & Types
│   │   └── Extensions/                        # Helper Extensions
│   │
│   ├── VertexCommerce.Modules.Catalog/        # Catalog Module
│   │   ├── Domain/                            # Entities, Value Objects
│   │   ├── Features/                          # CQRS Commands & Queries
│   │   ├── ReadModels/                        # MongoDB Read Models
│   │   └── Persistence/                       # EF Core DbContext
│   │
│   ├── VertexCommerce.Modules.Orders/         # Orders Module
│   ├── VertexCommerce.Modules.Basket/         # Basket Module (MongoDB)
│   ├── VertexCommerce.Modules.Identity/       # Identity Module
│   ├── VertexCommerce.Modules.Customers/      # Customers Module
│   ├── VertexCommerce.Modules.Notifications/  # Notification inbox, SSE and Web Push
│   │
│   └── VertexCommerce.Shared/                 # Shared Kernel
│       ├── CQRS/                              # ICommand, IQuery, Result
│       ├── Domain/                            # Entity, AggregateRoot
│       └── Services/                          # Cross-module contracts
│
├── tests/
│   ├── VertexCommerce.UnitTests/
│   ├── VertexCommerce.IntegrationTests/
│   └── VertexCommerce.ArchitectureTests/
│
├── docker/
│   └── docker-compose.yml
│
└── VertexCommerce.slnx
```

---

## 🎨 Design Patterns

### Implemented Patterns

| Pattern | Usage |
|---------|-------|
| **CQRS** | Separate read/write models with REST for commands, GraphQL for queries |
| **Repository** | Data access abstraction for both PostgreSQL and MongoDB |
| **Unit of Work** | Transaction management across repositories |
| **Result Pattern** | Railway-oriented programming for error handling |
| **Mediator** | MediatR for loose coupling between endpoints and handlers |
| **Factory** | Entity creation with validation in static factory methods |
| **State Machine** | Order status transitions with business rule enforcement |
| **Specification** | Dynamic query building for filtering |

### CQRS Implementation

```
┌─────────────────────────┐     ┌─────────────────────────┐
│      COMMANDS           │     │       QUERIES           │
│    (Write Side)         │     │     (Read Side)         │
├─────────────────────────┤     ├─────────────────────────┤
│                         │     │                         │
│  CreateProductCommand   │     │  GraphQL Queries        │
│  UpdateOrderCommand     │     │  GetProducts            │
│  AddToBasketCommand     │     │  SearchProducts         │
│                         │     │                         │
│  ┌─────────────────┐    │     │  ┌─────────────────┐    │
│  │   PostgreSQL    │    │     │  │    MongoDB      │    │
│  │   (Normalized)  │    │     │  │ (Denormalized)  │    │
│  └─────────────────┘    │     │  └─────────────────┘    │
│                         │     │                         │
│  REST API               │     │  GraphQL API            │
└─────────────────────────┘     └─────────────────────────┘
```

---

## ✨ Features

### ✅ Implemented

- [x] **Modular Monolith** with independently registered modules
- [x] **Polyglot Persistence** (PostgreSQL + MongoDB)
- [x] **Hybrid API** (REST + GraphQL)
- [x] **Product Catalog** with categories, SKU, pricing
- [x] **Shopping Cart** with MongoDB persistence
- [x] **Order Management** with state machine
- [x] **Order Notifications** with a persisted inbox, SSE and Web Push
- [x] **JWT Authentication** with refresh tokens
- [x] **Customer Profiles** with multiple addresses
- [x] **Automatic Sync** between PostgreSQL and MongoDB

### 🔜 Roadmap

- [ ] Payment gateway integration
- [ ] Inventory management with reservations
- [ ] Event sourcing for order history
- [ ] Elasticsearch for advanced search
- [ ] Redis caching layer
- [ ] Kubernetes deployment configs

---

## Notifications

After Orders saves a change, MediatR publishes the corresponding order event. Notifications creates inbox entries for the customer and, for new orders and submitted payment receipts, for administrators. The module records the event ID and saves all inbox entries together in one PostgreSQL transaction. Reprocessing that event ID skips the batch and live delivery. A separate event can create a fresh batch even when the order and status match.

The same transaction creates one Web Push job per active subscription when Push is enabled. After commit, the module broadcasts SSE and a background worker processes the stored Push jobs independently. Clients can fetch committed entries under `/api/notifications` after reconnecting. Failed writes can be retried by delivering the same event ID again. Recovery before an event reaches Notifications depends on the publisher. Existing inbox entries are preserved by the additive migration, and event deduplication applies to IDs recorded after the migration.

The module uses the PostgreSQL `notifications` schema. It reads `ConnectionStrings__NotificationsDb` and falls back to `ConnectionStrings__IdentityDb`. During startup it reconciles the legacy initial migration ID before applying migrations.

### Web Push subscriptions

Each endpoint belongs to one account. The owner can re-register it to refresh its keys. A different account receives HTTP 409. Unsubscribe is scoped to the authenticated user and always returns HTTP 204 for both existing and absent subscriptions.

Registration and delivery accept HTTPS endpoints from Google FCM, Mozilla Autopush, Apple Web Push and Windows Notification Service. Outbound connections use public IP addresses on port 443. Redirects and proxies are disabled for this client. Unsupported legacy endpoints stay in the database so their owners can still unsubscribe. The delivery logs identify subscriptions by their internal IDs.

Deploy the backend before the bundled service workers in the storefront and dashboard. Browser notifications include the recipient ID so each worker can check the browser's active subscription owner before displaying a notification.

### Web Push delivery

In production, keep a stable VAPID key pair and subject via `WebPush__Subject`, `WebPush__PublicKey` and `WebPush__PrivateKey`. Set `WebPush__Enabled=true` to require valid credentials at startup. With Push disabled, the key and registration endpoints return HTTP 503 and existing subscriptions can still be removed. Without an explicit setting, production enables Push only when the configured credentials pass validation and logs a warning for an invalid configured pair. In Development the module can use temporary keys when credentials are missing or invalid in automatic mode. When rotating a production key pair, have users disable and re-enable browser Push so the browser registers a subscription for the new key.

The worker leases a job for one minute, makes the provider request outside the database transaction, and finishes only while it owns the job's lease. Another worker can reclaim abandoned work. A 2xx response records provider acceptance. HTTP 404 and 410 retire the matching subscription if it has stayed unchanged. Network timeouts, HTTP 408, 429 and 5xx are retried up to five attempts in total. The delays are 15 seconds, 1 minute, 5 minutes and 30 minutes, extended when the provider sends a later `Retry-After`. Each job expires 24 hours after creation. A restart between provider acceptance and recording the result may cause another provider request. Retries use the same Web Push `Topic` for coalescing pending messages.

### Retry a failed delivery

Use the delivery ID from the structured log. After fixing the provider or VAPID issue, a database operator can requeue one unexpired job whose outcome is `Rejected` or `AttemptsExhausted`. The statement leaves successful jobs and active leases alone. To inspect queue health, count pending rows by age and terminal rows by outcome. Completed rows remain available for diagnosis.

```sql
-- In psql, set delivery_id to the job UUID before running the statement.
UPDATE notifications."PushDeliveries"
SET "Attempts" = 0,
    "Outcome" = NULL,
    "CompletedAt" = NULL,
    "NextAttemptAt" = CURRENT_TIMESTAMP
WHERE "Id" = :'delivery_id'::uuid
  AND "CompletedAt" IS NOT NULL
  AND "LeaseId" IS NULL
  AND "Outcome" IN ('Rejected', 'AttemptsExhausted')
  AND "ExpiresAt" > CURRENT_TIMESTAMP
RETURNING "Id";
```

Deploy the additive migration before processing Web Push jobs. If rolling back the application, leave the database migration applied to retain queued jobs and full-length historical payloads.

### Notification contract tests

```bash
dotnet test tests/VertexCommerce.Modules.Notifications.Tests
```

The contract tests exercise HTTP and SSE through a local Kestrel host with the Identity module's JWT authentication. Web Push transport tests use controlled provider responses.

Set `VERTEX_NOTIFICATIONS_TEST_DATABASE` to a connection string for a disposable PostgreSQL server to include PostgreSQL tests for concurrent ownership, event processing, atomic rollback, migration compatibility, worker leases and delivery retries. The test role needs permission to create a database. The test creates and drops its own database.

---

## 🚀 Getting Started

### Prerequisites

- Docker & Docker Compose

### Quick Start

```bash
# Clone the repository
git clone https://github.com/yourusername/VertexCommerce.git
cd VertexCommerce

# Start infrastructure
docker-compose -f docker/docker-compose.yml up -d

# Run the application
dotnet run --project src/VertexCommerce.Api

# Access the APIs
# REST API: https://localhost:5001/swagger
# GraphQL: https://localhost:5001/graphql
```

### Configuration

```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=vertex;Username=postgres;Password=postgres",
    "MongoDB": "mongodb://localhost:27017"
  },
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "VertexCommerce",
    "Audience": "VertexCommerce"
  }
}
```

---

## 📚 API Documentation

### REST Endpoints (Commands)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register new user |
| `POST` | `/api/auth/login` | Login |
| `POST` | `/api/products` | Create product (Admin) |
| `PUT` | `/api/products/{id}` | Update product (Admin) |
| `POST` | `/api/basket/items` | Add item to basket |
| `POST` | `/api/checkout` | Create order from basket |

---

## 🔒 Security

- **JWT Bearer Authentication** with access/refresh token rotation
- **BCrypt Password Hashing** (cost factor 12)
- **Role-based Authorization** (Admin, User)
- **Server-side Price Validation** (prevents manipulation)
- **Input Validation** with FluentValidation
- **Parameterized Queries** (SQL injection prevention)

---

## 📊 Why This Project?

This project demonstrates:

| Aspect | What It Shows |
|--------|--------------|
| **Architecture** | Understanding of modular design & bounded contexts |
| **Database Design** | Ability to choose right database for each use case |
| **API Design** | Modern hybrid approach (REST + GraphQL) |
| **Code Quality** | Clean code, SOLID principles, proper patterns |
| **Security** | Production-ready authentication & authorization |

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<p align="center">
  <sub>Built with ❤️ using .NET 10</sub>
</p>

```
═══════════════════════════════════════════════════════════════════════════
```
