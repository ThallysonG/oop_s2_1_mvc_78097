# 📚 Community Library Desk

A simple ASP.NET Core MVC application for managing a community library.  
This system allows users to manage books, members, and loans, with authentication and role-based access control.

---

## 🚀 Features

- 📖 Book Management (CRUD)
- 👤 Member Management (CRUD)
- 🔄 Loan Management
- 🔐 Authentication with ASP.NET Identity
- 🛡️ Role-based Authorization (Admin/User)
- 📊 Dashboard with system statistics
- 🔎 Search and filtering
- ⚠️ Overdue loan detection

---

## 🧱 Technologies Used

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQLite Database
- ASP.NET Identity
- Bootstrap 5

---

## 🖥️ System Overview

The system is divided into:

- **Books** → Manage book catalog
- **Members** → Manage library users
- **Loans** → Track borrowed books
- **Admin Roles** → Manage user roles (Admin only)

---

## 🔐 Authentication & Roles

The system uses ASP.NET Identity.

### Roles:
- **Admin**
  - Full access
  - Can manage roles
- **User**
  - Can manage books, members, and loans

---

## 🔑 Default Login Credentials

```text
Admin User:
Email: admin@library.com
Password: Admin123!