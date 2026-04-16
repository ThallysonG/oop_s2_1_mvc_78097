# Community Library Desk System

## Overview

This project is an ASP.NET Core MVC application developed for the **Modern Programming Principles and Practice** module.

It simulates a small community library system where staff can manage books, members, and loans.

---

## Features

### Books

* CRUD operations
* Search by title or author
* Filter by category
* Filter by availability

### Members

* CRUD operations

### Loans

* Create loan (book + member)
* Prevent duplicate active loans
* Mark book as returned
* Overdue detection

### Admin

* Role management page (`/Admin/Roles`)
* Create and delete roles
* Protected with Admin authorization

---

## Technologies Used

* ASP.NET Core MVC (.NET 8)
* Entity Framework Core (SQLite)
* Identity
* Bogus (fake data)
* xUnit (tests)
* GitHub Actions (CI)

---

## Setup Instructions

1. Clone the repository
2. Run:

```
dotnet restore
dotnet ef database update
dotnet run
```

3. Open browser:

```
/Books
```

---

## Admin Access

Default admin user:

* Email: [admin@library.com](mailto:admin@library.com)
* Password: Admin123!

---

## Tests

Run tests with:

```
dotnet test
```

---

## CI

GitHub Actions automatically runs:

* build
* test

on push and pull requests.

---

## Author

Student Number: 78097
