# Eatonville Dance Studio Management Portal

![Status: In Development](https://img.shields.io/badge/Status-In%20Development-blue)
![Framework: .NET 10 MVC](https://img.shields.io/badge/Framework-.NET%2010%20MVC-purple)
![Database: SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)

## Overview
The Eatonville Dance Studio Management Portal is a full-stack Web application designed to eliminate the logistical chaos of running a modern dance studio. It replaces scattered spreadsheets and paper forms with a centralized platform for class registration, secure tuition billing, competition tracking, and direct communication.

Built using .NET 10 ASP.NET Core MVC, the platform serves three distinct user roles: **Parents/Dancers**, **Instructors**, and **Studio Directors**, providing each with a tailored dashboard and role-based authorization to manage their specific responsibilities.

## Key Features

### Parent & Dancer Experience
*   **Household Management:** Group multiple dancers under a single parent account with automated multi-class tuition discounting.
*   **Self-Service Dashboard:** Register for classes, track upcoming events, and view competition itineraries.
*   **Secure Billing:** View itemized ledgers for tuition, costume fees, and fundraiser credits, and pay securely via integrated payment gateways.
*   **Digital Waivers:** Review and digitally sign liability and medical releases.

### Instructor Tools
*   **Class Management:** View approved class rosters, flag medical notes, and submit daily digital attendance.
*   **Secure Communication:** Post announcements, choreography notes, and private rehearsal videos directly to class-specific message boards.
*   **Payroll Tracking:** Log and track teaching hours and private lessons for administrative review.

### Director (Admin) Control
*   **Global Oversight:** God-mode CRUD access for all users, classes, events, and financials.
*   **Competition Logistics:** Track costume sizing and ordering, build performance itineraries, and establish backstage call times.
*   **Automated Comms:** Broadcast urgent SMS/Email alerts (e.g., weather closures) and automated payment reminders.
*   **Financial Reporting:** Track studio-wide revenue, apply custom invoice charges, and manage fundraiser credits.

## Tech Stack
*   **Framework:** .NET 10 (ASP.NET Core MVC)
*   **Language:** C# 14, Razor (HTML5 / CSS3 / JavaScript)
*   **Data Access:** Entity Framework Core 10 (Code First / DbContext)
*   **Database:** Microsoft SQL Server
*   **Authentication & Auth:** ASP.NET Core Identity (Role-Based Access Control)
*   **Third-Party Integrations:** Stripe.net SDK (Payments), Twilio / SendGrid SDKs (SMS & Email Alerts)

## Project Structure & Roadmap
The development lifecycle is broken down into 10 core Epics, managed via GitHub Issues:
1.  **Public Website & Authentication** (Identity, RBAC, Login)
2.  **Parent/Dancer Registration & Profiles** (Verification workflows, Student CRUD)
3.  **Parent/Dancer Dashboard & Financials** (Ledgers, Stripe Checkout)
4.  **Instructor Portal & Class Management** (Rosters, Attendance)
5.  **Director Master Control** (Global management, Invoicing, Analytics)
6.  **Family & Multi-Dancer Logic** (Household grouping, Automated discounts)
7.  **Recital & Competition Logistics** (Costume tracking, Call times, Video Vault)
8.  **Digital Waivers & Legal Compliance** (E-signatures, Medical flags)
9.  **Fundraiser Credit Ledger** (Account crediting system)
10. **Automated Communications** (Twilio/SendGrid background services)

## Getting Started

### Prerequisites
*   [.NET 10 SDK](https://dotnet.microsoft.com/)
*   [Visual Studio 2026](https://visualstudio.microsoft.com/) (with *ASP.NET and web development* workload installed)
*   Microsoft SQL Server (LocalDB, Express, or SQL Server 2022/2026)
*   Entity Framework Core CLI Tools installed globally:
    ```bash
    dotnet tool install --global dotnet-ef
    ```

### Installation & Setup

1. Clone the repository:
   ```bash
   git clone [https://github.com/yourusername/eatonville-dance-portal.git](https://github.com/yourusername/eatonville-dance-portal.git)

## Author

**Zac Kimball**
*   **Role:** Lead Developer & Software Architect
*   **Stack:** .NET 10 / C# 14 / SQL Server
*   **Organization:** Zeus Digital Designs
*   **GitHub:** [@ZacharyKimball](https://github.com/zkimball85) 

---

## License
Copyright © 2026 Zachary S. Kimball / Zeus Digital Designs. All Rights Reserved.

**Proprietary and Confidential.** 
This repository and its contents are closed-source. Unauthorized copying, modification, distribution, or use of this software, via any medium, is strictly prohibited.
