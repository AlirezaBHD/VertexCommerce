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

- [x] **Modular Monolith** with 5 bounded contexts
- [x] **Polyglot Persistence** (PostgreSQL + MongoDB)
- [x] **Hybrid API** (REST + GraphQL)
- [x] **Product Catalog** with categories, SKU, pricing
- [x] **Shopping Cart** with MongoDB persistence
- [x] **Order Management** with state machine
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
