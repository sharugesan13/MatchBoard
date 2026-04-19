# MatchBoard – Blind Matching System

## 📌 Project Description
MatchBoard is a web-based application designed to manage and automate the matching process between students and supervisors in an academic environment. The system uses a blind matching approach where initial identity details are hidden to ensure unbiased selection based on project proposals and supervisor expertise.

---

## 🚀 Features
- User authentication system (Student, Supervisor, Admin)
- Student project submission system
- Supervisor expertise management
- Blind matching algorithm between students and supervisors
- Admin dashboard for system monitoring and control
- Match reveal system after assignment completion
- Role-based access control

---

## 🛠️ Tech Stack
- ASP.NET Core MVC (.NET 9.0)
- Entity Framework Core
- SQL Server
- HTML5, CSS3, JavaScript
- Bootstrap
- jQuery
- Unit Testing Framework (.NET Test Project)

---

## 📁 Project Structure
MatchBoard/
│── MatchBoard.Web/ → Main web application
│ │── Controllers/ → MVC Controllers
│ │── Models/ → Domain & ViewModels
│ │── Views/ → Razor Views
│ │── Services/ → Business Logic (MatchingService)
│ │── Data/ → Database Context
│ │── Migrations/ → EF Core Migrations
│ │── wwwroot/ → Static files (CSS, JS, Libraries)
│
│── MatchBoard.Tests/ → Unit Testing Project


---

##  How to Run the Project

### 1. Clone Repository
```bash
git clone https://github.com/sharugesan13/MatchBoard.git
cd MatchBoard

### 2. Restore Dependencies
dotnet restore MatchBoard.sln

### 3. Build Project
dotnet build MatchBoard.sln

### 4. Run Database Migrations
dotnet ef database update

### 5. Run Application
dotnet run --project MatchBoard.Web

### 6. Running Tests
dotnet test MatchBoard.sln

Notes for Evaluators
Ensure SQL Server is installed and running
Update connection string in appsettings.json if required
Application follows MVC architecture with layered separation of concerns

## Author
Developed by Group BE – Academic Project Submission
