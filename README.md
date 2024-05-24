# Marmot Shop - Ecommerce Fullstack Project / Backend part

![.NET Core](https://img.shields.io/badge/.NET%20Core-v.8-purple)
![EF Core](https://img.shields.io/badge/EF%20Core-v.8-cyan)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-v.16-drakblue)
![PgAdmin 4](https://img.shields.io/badge/PgAdmin%204-v.8.2-lightblue)
![XUnit](https://img.shields.io/badge/XUnit-v.2.4.2-green)
![Moq](https://img.shields.io/badge/Moq-v.4.20-pink)

## Project Description

This repository contains the backend server for a fullstack ecommerce project built with ASP.NET Core 7, Entity Framework Core, and PostgreSQL. The frontend of the project, developed with React, Redux Toolkit, TypeScript, and TailwindCSS, is maintained in a [separate public repository](https://github.com/yuankeMiao/marmot-shop) and integrates with this backend to provide a seamless user experience and robust management system for administrators.

Check the deployed backend [here](https://catagonia.azurewebsites.net/index.html).
Check the deployed frontend [here](https://marmotshop.yuankedev.fun).

## Table of Contents

- [Features](#features)
  - [User Functionalities](#user-functionalities)
  - [Admin Functionalities](#admin-functionalities)
- [Architecture](#architecture)
- [Installation](#installation)
- [Usage](#usage)
- [Test](#test)
- [Contact Information](#contact-information)

## Features

### User Functionalities

- **User Management**: Users can register for an account and manage their profile.
- **Browse Products**: Users can view all available products, single products, search, and sort products.
- **Add to Cart**: Users can add products to a shopping cart and manage the cart.
- **Orders**: Users can place orders and view their order history.
- **Address**: Users can add, update and delete addresses.

### Admin Functionalities

- **User Management**: Admins can manage all users.
- **Product Management**: Admins can manage all products.
- **Order Management**: Admins can manage all orders.

## Architecture

The project follows a modular and scalable architecture based on the principles of CLEAN architecture. CLEAN architecture is designed to ensure the separation of concerns, making the system easier to maintain, test, and scale.

### CLEAN Architecture Overview

This project implements Each layer has distinct responsibilities and interacts with other layers through well-defined interfaces. This separation helps in maintaining a clear boundary between the different parts of the system.

1. Core Domain Layer (Ecommerce.Core)
- Centralizes core domain logic and entities.
- Includes common functionalities, repository abstractions, and value objects.
2. Application Service Layer (Ecommerce.Service)
- Implements business logic and orchestrates interactions between controllers and the core domain.
- Services handle DTO transformations and business operations related to each resource.
3. Controller Layer (Ecommerce.Controller)
- Contains controllers responsible for handling HTTP requests and responses.
- Controllers are organised by resource types (e.g., Auth, Category, Order, Product, Review, User).
4. Infrastructure Layer (Ecommerce.WebAPI)
- Manages infrastructure tasks and interaction with external systems.
- Contains database context, repositories, and middleware for error handling.
5. Testing Layer (Ecommerce.Test)
- Holds unit tests for core domain and application services.
- Ensures the reliability and correctness of the implemented functionalities.

### Project Structure

```
Ecommerce.Backend
├── Backend_Ecommerce.sln
├── Ecommerce.Controller
│   ├── src
│       ├── Controllers
├── Ecommerce.Core
│   ├── src
│       ├── Common
│       ├── Entity
│       ├── RepoAbstract
│       ├── ValueObject
├── Ecommerce.Service
│   ├── src
│       ├── DTO
│       ├── Service
│       ├── ServiceAbstract
│       ├── Shared
├── Ecommerce.Test
│   ├── src
│       ├── Core
│       ├── Service
├── Ecommerce.WebAPI
│   ├── src
│       ├── AuthorizationPolicy
│       ├── Database
│       ├── Middleware
│       ├── Repo
│       ├── Service
└── README.md
```

## Installation

To set up the project locally, follow these steps:

1. **Clone the repository:**

   ```bash
   git clone https://github.com/yuankeMiao/fs17_CSharp_FullStack.git
   cd fs17_CSharp_FullStack
   ```

2. **Set up the database:**

   Configure your database connection in the `appsettings.json` file located in the `Ecommerce.WebAPI` project. Please add this file if it is not there, and add your own information.

- appsettings.json Example

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Localhost": "Host=localhost;Username=admin;Password=yourpassword;Database=yourdatabase",
    "Remote": "Host=yourremotehost;Username=yourremoteuser;Password=yourremotepassword;Database=yourremotedatabase;SslMode=Require"
  },
  "Secrets": {
    "JwtKey": "yourjwtkey",
    "Issuer": "yourissuer"
  }
}
```


3. **Run the backend module:**
Try to build the application
```
dotnet build
```
- Run this command to create a new Migrations folder, which stores the snapshot of all the database context changes:
  _If there is already a folder Migrations in the Web API layer, delete it._

```
dotnet ef database drop
dotnet ef migrations add Create
```

- Apply all the changes to the database
```
dotnet ef database update
```

- Then run the application
```
dotnet watch
```

## Usage

To use the APIs provided by the Ecommerce Backend System, refer to the detailed API documentation available at the Swagger UI:

[Swagger API Documentation](https://marmote-shop-app.azurewebsites.net/index.html)

### Example API Endpoints

- **Authentication**
  - `POST /api/auth/login`: User login
  - `POST /api/auth/register`: User registration

- **Products**
  - `GET /api/products`: Retrieve all products
  - `POST /api/products`: Create a new product
  - `GET /api/products/{id}`: Retrieve a product by ID
  - `PUT /api/products/{id}`: Update a product by ID
  - `DELETE /api/products/{id}`: Delete a product by ID

- **Categories**
  - `GET /api/categories`: Retrieve all categories
  - `POST /api/categories`: Create a new category
  - `GET /api/categories/{id}`: Retrieve a category by ID
  - `PUT /api/categories/{id}`: Update a category by ID
  - `DELETE /api/categories/{id}`: Delete a category by ID

- **Orders**
  - `GET /api/orders`: Retrieve all orders
  - `POST /api/orders`: Create a new order
  - `GET /api/orders/{id}`: Retrieve an order by ID
  - `PUT /api/orders/{id}`: Update an order by ID
  - `DELETE /api/orders/{id}`: Delete an order by ID

- **Reviews**
  - `GET /api/reviews`: Retrieve all reviews
  - `POST /api/reviews`: Create a new review
  - `GET /api/reviews/{id}`: Retrieve a review by ID
  - `PUT /api/reviews/{id}`: Update a review by ID
  - `DELETE /api/reviews/{id}`: Delete a review by ID

- **Users**
  - `GET /api/users`: Retrieve all users
  - `GET /api/users/{id}`: Retrieve a user by ID
  - `PUT /api/users/{id}`: Update a user by ID
  - `DELETE /api/users/{id}`: Delete a user by ID

## Test
This project followed the principles of Test-Driven Development (TDD) to ensure the reliability, maintainability, and quality of our codebase. The unit test covered all method in Service layer.

Run tests:
- Navigate to root folder of backend module, then run the test
```
dotnet test
```


## Contact Information

For any questions, issues, or suggestions, please contact:

- **Project Maintainer**: [Yuanke Miao](mailto:yuankemiao.dev@gmail.com)
