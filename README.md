Readme · MDCopyPrime Dental

A full-stack dental clinic web application built with ASP.NET Core MVC, created as a portfolio project to demonstrate practical backend development skills — from database design through authentication, background processing, and automated testing.

About This Project

This project started from a static HTML/CSS template and was rebuilt into a working ASP.NET Core MVC application with a real MySQL-backed database, an authenticated admin panel, and automated tests. It was built as a learning and portfolio project to practice and demonstrate full-stack .NET development for job and freelance applications.

Features

Public Site


Home, Why Us, Services, Team, Pricing, and Dental Solutions pages
Blog system with full post listing and detail pages, backed by MySQL
Appointment booking form with server-side validation (required fields, email format, and a rule preventing appointments booked in the past)
Duplicate-booking prevention (same email + same date is blocked)
Automated HTML email confirmation sent on successful booking


Admin Panel


Secured with ASP.NET Core Identity (cookie-based authentication)
Dashboard with key metrics: total appointments, appointments today, appointments this month, total blog posts, and a 14-day appointment trend chart (Chart.js)
Full CRUD management for blog posts
Appointment management with status tracking (Pending, Confirmed, Completed, Cancelled)


Backend Architecture


Request/DTO pattern — form input models (e.g. AppointmentRequest) are kept separate from database entities, so validation rules and user-submittable fields are explicit and intentional
Background job queue — appointment confirmation emails are sent via an in-memory background task queue (IHostedService + System.Threading.Channels), so the booking request returns immediately instead of waiting on SMTP
EF Core + MySQL (via Pomelo.EntityFrameworkCore.MySql), with incremental migrations tracking schema changes over time


Tech Stack

LayerTechnologyFrameworkASP.NET Core MVC (.NET 10)DatabaseMySQL 8 (via Docker locally)ORMEntity Framework Core 9 + Pomelo MySQL providerAuthenticationASP.NET Core IdentityEmailMailKit (Gmail SMTP)Background ProcessingIHostedService + System.Threading.ChannelsFrontendRazor Views, Bootstrap, Chart.jsTestingxUnit, EF Core InMemory provider

Testing

The project includes both unit and integration tests:


Unit tests — validate AppointmentRequest business rules in isolation (required fields, email format, past-date rejection)
Integration tests — exercise AppointmentController against an EF Core InMemory database, covering successful bookings, invalid submissions, and duplicate-booking prevention logic


bashdotnet test

Project Structure

PrimeDental/
├── MyMvcApp/
│   ├── Controllers/
│   │   ├── Admin/          # Admin panel controllers (Dashboard, Posts, Appointments, Account)
│   │   ├── HomeController.cs
│   │   ├── BlogController.cs
│   │   └── AppointmentController.cs
│   ├── Models/
│   │   ├── Requests/       # Form-binding DTOs, separate from DB entities
│   │   ├── Settings/        # Strongly-typed configuration (e.g. EmailSettings)
│   │   └── Admin/           # Admin-specific view models
│   ├── Services/            # Email service, background task queue
│   ├── Data/                 # EF Core DbContext
│   ├── Views/
│   └── wwwroot/
└── MyMvcApp.Tests/
    ├── Controllers/
    └── Models/

Running Locally

Prerequisites: .NET 10 SDK, Docker (for MySQL), Gmail account with an App Password (for email)

bash# Start MySQL via Docker
docker run --name mysql -e MYSQL_ALLOW_EMPTY_PASSWORD=yes -p 3306:3306 -d mysql:8

# Set required secrets (never stored in source control)
cd MyMvcApp
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=primedental;User=root;Password=;"
dotnet user-secrets set "EmailSettings:SenderEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:AppPassword" "your-gmail-app-password"
dotnet user-secrets set "AdminUser:Email" "admin@example.com"
dotnet user-secrets set "AdminUser:Password" "ChooseAStrongPassword1"

# Apply database migrations
dotnet ef database update

# Run the app
dotnet watch run

The admin panel is available at /Admin/Login using the credentials set above.

Author

Built by Philip — full-stack developer focused on Laravel/PHP and ASP.NET Core, based in Ghana.


Portfolio: philipbuilds.page.gd
