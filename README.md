# 🍽️ Matamak - Restaurant Ordering & Management System

Matamak is a comprehensive, modern restaurant ordering and management system designed to digitalize daily food operations. It connects customers, cashiers, and system administrators through a seamless real-time workflow, linking a fully interactive Angular frontend with a robust C# .NET 10 Web API backend.

---

## 📖 Project Concept

The core idea behind Matamak is to build a unified platform that manages the entire lifecycle of restaurant orders. Customers can browse menus, apply discounts, make secure online payments, and track their order status in real time. Staff (cashiers) can manage and process dine-in, takeaway, or delivery orders, while admins manage inventory, analyze sales reports, and configure system settings. All of these features are fully implemented and interactive across both the frontend application and the backend API.

---

## 🎯 Project Objectives

- **Seamless Operations**: Automate dine-in, takeaway, and delivery order workflows.
- **Real-Time Coordination**: Provide immediate status updates for orders using WebSockets.
- **Secure Authorization**: Implement role-based access control (RBAC) separating Customers, Cashiers, and Admins.
- **Modern Interface**: Provide an intuitive, responsive, and aesthetically pleasing user experience.
- **Online Payment Integration**: Enable online payment workflows through payment gateway simulation (Paymob).

---

## 🛠️ Technologies Used

### Backend

- **Language & Runtime**: C# with .NET 10 SDK
- **Framework**: ASP.NET Core Web API (Clean Architecture structure)
- **Database & ORM**: SQL Server with Entity Framework Core 10 (Code-First)
- **Authentication**: ASP.NET Core Identity & JWT Bearer Token validation
- **Real-Time Communication**: ASP.NET Core SignalR
- **Mailing Service**: MailKit & MimeKit (Gmail SMTP integration)
- **Payment Gateway**: Paymob API Integration
- **API Documentation**: Swagger UI / OpenAPI

### Frontend

- **Framework**: Angular 22
- **Scripting Language**: TypeScript
- **State & Data Handling**: RxJS Observables
- **Styling**: Sass / SCSS, HTML5, Vanilla CSS

---

## ✨ Key Features

### 👤 Customer Experience

- **Online Ordering**: Place Takeaway or Delivery orders with custom notes and totals.
- **Menu Browsing**: Filter meals by Category or Country/Cuisine of origin, with real-time text search.
- **Order History & Tracking**: Check past transactions, real-time statuses, and unique sequential order tracking IDs.
- **Takeaway Cancellations**: Direct order cancellation ("إلغاء الطلب") option from the customer profile page.
- **Account Control**: Login, Sign-Up, and OTP verification (email-based) for account activation or password resets.

### 💼 Staff Experience (Cashier Dashboard)

- **Orders Workflow Management**: View and update live orders categorized by Dine-In, Takeaway, and Delivery.
  - _Delivery_: Advance status from _Pending_ ➡️ _With Driver_ ➡️ _Completed_, or cancel the order.
  - _Dine-In_: Live table tracking and status transitions (_Pending_ ➡️ _Cooking_ ➡️ _Served_ ➡️ _Completed_).
  - _Takeaway_: Track, update status, and cancel pickup orders.
- **Express Checkout**: Open customer menus and place rapid orders from the desk.
- **Non-Destructive Cancellations**: Cancelling a takeaway order updates its status to "Canceled" dynamically rather than deleting it from the system, preserving records.

### 👑 Administrator Panel (Admin Dashboard)

- **Menu Management**: Fully interactive interface to view, add, edit, and delete food items, categories, and countries/cuisines. Supports uploading actual image files directly which are saved on the server and displayed dynamically across the app.
- **User & Staff Account Control**: Retrieve list of all registered Admins, Cashiers, and Customers, create new Manager/Cashier accounts, or delete any account.
- **Sales Analytics & Reports**: Specialized reports dashboard with date-range filters calculating total revenue and successful transaction counts from paid/completed orders.
- **Coupon & Offers Manager**: Administer flat or percentage discount coupon codes.

---

## 📂 Folder Structure

```text
Matamak/
├── Core/                       # Application Contracts & Domain Layer
│   ├── DTO/                    # Data Transfer Objects
│   ├── IReprosatory/           # Repository Interfaces
│   ├── IServices/              # Service Interfaces
│   ├── Models/                 # Database Entity Models
│   └── ModelView/              # View Model representations
├── Infrastructure/             # Core Implementation Detail Layer
│   ├── Context/                # EF DataContext (SQL Server Configuration)
│   ├── Migrations/             # EF Code-First Migration Scripts
│   ├── Reprosatory/            # Repository Implementations
│   └── Services/               # Third-party integrations (Paymob, Email, SignalR)
├── Resturant/                  # Web API Project (Entry Point)
│   ├── Controllers/            # API Endpoints
│   ├── Properties/             # Launch settings & IIS configurations
│   ├── Program.cs              # Dependency Injection & Middleware Pipeline
│   └── appsettings.json        # Database Connection Strings & API Keys
└── Matamak.Frontend/           # Angular Client Application
    ├── src/
    │   ├── app/
    │   │   ├── core/           # Auth, Services, Guards, and Global Interceptors
    │   │   ├── features/       # Feature modules (Auth, Customer, Staff Dashboard)
    │   │   └── shared/         # Reusable layouts, UI Components, and Pipes
    │   └── environments/       # Environment configurations (Dev & Production)
    └── proxy.conf.json         # API reverse proxy configuration for development
```

---

## 🚀 Steps to Run the Project

### Prerequisites

- **Windows 10/11**
- **.NET 10 SDK**
- **Node.js** (LTS version)
- **SQL Server** (LocalDB or default instance)

### 1. Database Setup & Seeding

Ensure SQL Server is running locally. To apply database migrations and seed default data:

1. Open a terminal at the root of the project.
2. Run the database update command:
   ```bash
   dotnet ef database update --project Infrastructure --startup-project Resturant
   ```
3. On startup, the backend automatically seeds:
   - **Administrative Users**:
     - **Admin**: `admin@gmail.com` / `123456789`
     - **Cashier**: `cashier@gmail.com` / `147258369`
   - **Menu Database**: Seeding three default categories (_وجبات, حلويات, مشروبات_) and inserting 3 default dishes under each category (complete with descriptions, pricing, and images).

### 2. Run the Backend API

You can run the backend from Visual Studio Community or the CLI.

- **Via Visual Studio (Recommended)**:
  - Open `Matamak.sln` in Visual Studio 2022.
  - Set **Resturant** as the startup project.
  - Run using **IIS Express** (hosts the API on `https://localhost:44357` which matches the frontend proxy settings).
- **Via CLI**:
  - Run the command:
    ```bash
    dotnet run --project Resturant
    ```
  - Note: Update the target port in `Matamak.Frontend/proxy.conf.json` to match the active CLI port (`5270` or `7092`).

### 3. Run the Frontend (Angular)

1. Open a terminal in `Matamak.Frontend/`.
2. Install npm dependencies (if not done already):
   ```bash
   npm install
   ```
3. Start the Angular local dev server:
   ```bash
   npm start
   ```
4. Open your browser and go to `http://localhost:4200`.

---

## 🖼️ Project Screenshots

_Here are some visual insights into the Matamak system:_

### 👤 Customer Experience

#### Customer Menu Page

![Customer Menu Page](Matamak/screenshots/customer_menu.png)

### 💼 Staff & Cashier Interface

#### Cashier Dashboard

![Cashier Dashboard](Matamak/screenshots/cashier_dashboard.png)

#### Cashier Order Placement Screen

![Cashier Order Placement Screen](Matamak/screenshots/cashier_order.png)

### 👑 Administrator Panel

#### Admin Dashboard Overview

![Admin Dashboard Overview](Matamak/screenshots/admin_dashboard.png)

#### Menu Management

![Menu Management](Matamak/screenshots/menu_management.png)

---

## ⚠️ Challenges Faced & Resolutions

- **API Crash on Empty Tables**: Resolving backend unhandled exceptions when requesting menu items from an empty database. Fixed by replacing exception throwing with empty list returns in the repository and service layers.
- **Admin dashboard mockup shell**: Connecting the frontend shell dashboard to active endpoints. Resolved by writing CRUD logic for items/categories/countries in `CatalogService`, user account list queries in `AuthService`, and order state transitions, inventory updates, and reports fetching in `OrderService`.
- **EF Core Pending Model Changes**: Managing Entity Framework migration discrepancies when new fields were added without migrations. Resolved by adding a unified `UpdatePendingModelChanges` migration.
- **CORS Restrictions**: Resolving cross-origin requests between the local Angular application (`localhost:4200`) and the ASP.NET Core API server. Resolved by configuring permissive CORS middleware in `Program.cs`.
- **Swagger Endpoint Collisions**: Encountered ASP.NET Core HTTP 500 routing/Swashbuckle schema errors due to duplicate HTTP verb attributes on single controller actions. Resolved by segregating actions and applying unique route mappings.
- **Dynamic Menu Image Uploading & Rendering**: Fixed an issue where the Angular frontend ignored dynamically uploaded item images and defaulted to random Unsplash placeholders. Created a direct multipart-form image upload endpoint on the backend saving to `wwwroot/uploads`, and adjusted the frontend templates/components to dynamically parse and display this direct path (`imageUrl`).
- **Non-Destructive Takeaway Cancellations**: Initially, takeaway order cancellations deleted order records from the UI and database. Refactored the workflow to execute a soft state change (`Canceled`), and adjusted authorization rules on the endpoint so that standard `Customer` users can only request a cancellation, while `Admin` and `Cashier` roles can update orders to any state.
- **Zero Sales/Profit Analytics Calculations**: Resolving empty analytics results due to the database payment records table being unpopulated. Refactored the dashboard sales reports calculation logic to query total revenue and total counts directly from all successful orders (`Paid`, `Completed`, `Delivered`).
- **Resetting Order Identifiers**: Fixed order tracking sequences resetting to `#1` on fresh sessions or daily rolls by replacing temporary UI sequence counters with database-generated auto-incremented primary keys (`id`).

---

## 🔮 Future Improvements

- **Advanced Reports Visualizer**: Integrate Chart.js or D3.js in the frontend to visualize sales statistics.
- **Dockerization**: Create Docker files for backend, frontend, and SQL Server to allow single-command deployment.
- **CI/CD Pipeline**: Build GitHub Actions workflows for automated code testing and linting.
- **Refactoring Database Schemas**: Clean up minor database schema naming conventions (e.g., correcting spelling of `Delivary`, `Oredr`, and `Reprosatory`).

---

## 👥 Team Members

- **Mostafa Mahmoud Amin** - _Project Lead & Full Stack Developer_
- **Zyad Ayman Abdel-Salam** - _Full Stack Developer_
- **Ahmed Mohamed Abdullah** - _System Analyst_
- **Ibrahim Medhat Abbas** - _Presentation & Decomentation Specialist_
- **Belal Muhammad Abdo** - _Frontend Developer_

---

## 🎥 Explanatory Video

Click the link below to watch a video walk-through and explanation of the project features and code structure:

🔗 **[Watch Project Explanatory Video](https://drive.google.com/file/d/1vzcUd504_-6dpp0o9l5K4lWh8LzshTWL/view?usp=drive_link)**
