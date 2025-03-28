# Pension Contribution Management System (EPS+)

## Overview

EPS+ is a robust Pension Contribution Management System designed to streamline customer onboarding, pension contributions, and background processing. Built using .NET Core, Entity Framework, SQL Server, and Hangfire, the system follows Clean Architecture and Domain-Driven Design (DDD) principles to ensure scalability, maintainability, and security.

## Features

- **Customer Onboarding**: Streamlined registration for members, employers, and administrators.
- **Pension Contribution Management**: Supports monthly and voluntary contributions with automated processing.
- **Transaction Handling**: Ensures secure and efficient contribution transactions with status tracking.
- **Employer Management**: Employers can onboard employees and oversee their contributions.
- **Benefit Eligibility Assessment**: Automatically determines member eligibility for pension benefits.
- **Background Processing**: Uses Hangfire for validating contributions, processing failed transactions, and updating benefit eligibility.
- **User Roles & Permissions**: Role-based access control for administrators, employers, and members.
- **Reports & Statements**: Generates contribution summaries, benefit eligibility reports, and transaction histories.
- **Secure Authentication**: Implements JWT-based authentication with role-based authorization.
- **Logging & Error Handling**: Structured logging using Serilog and robust error-handling mechanisms.

## System Architecture

The system is structured into multiple layers:

- **Presentation Layer:** RESTful API using ASP.NET Core
- **Application Layer:** Business logic and workflow handling
- **Domain Layer:** Core business models and domain logic
- **Infrastructure Layer:** Handles database access, background jobs, and logging

## Entity-Relationship Diagram (ERD)

### Key Entities & Relationships:

- `ApplicationUser` (1 - N) `Member`, `Employer`, `Admin`
- `Member` (1 - N) `Contributions`
- `Employer` (1 - N) `Members`
- `Contribution` (N - 1) `Member`
- `Transaction` (N - 1) `Contribution`
- `Benefit Eligibility` (1 - 1) `Member`

## Database Schema

### Main Entities & Attributes:

- **ApplicationUser** (`UserID`, `FullName`, `PhoneNumber`, `CreatedAt`, `UserType`, `IsActive`)
- **Member** (`MemberID`, `Name`, `DateOfBirth`, `Email`, `Phone`, `Age`, `EmployerID`)
- **Employer** (`EmployerID`, `CompanyName`, `RegistrationNumber`)
- **Contribution** (`ContributionID`, `MemberID`, `ContributionType`, `Amount`, `ContributionDate`)
- **BenefitEligibility** (`EligibilityID`, `MemberID`, `EligibleDate`, `Status`)

## Installation

### Prerequisites
- .NET 8.0 or later
- SQL Server
- Entity Framework Core

### Setup Instructions
1. Clone the repository:
   ```bash
   git clone https://github.com/murphite/EPSPlus.git
   ```
2. Navigate to the project folder:
   ```bash
   cd EPSPlus
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Update the database:
   ```bash
   dotnet ef database update
   ```
5. Run the application:
   ```bash
   dotnet run
   ```

## API Endpoints

### Authentication & Authorization

- **Register Member:** `POST /api/auth/register/member`
- **Register Employer:** `POST /api/auth/register/employer`
- **Register Admin:** `POST /api/auth/register/admin`
- **Login:** `POST /api/auth/login`
- **Reset Password:** `POST /api/auth/reset-password`

### Member Management

- **Register Member:** `POST /api/members`
- **Update Member:** `PUT /api/members/{id}`
- **Retrieve Member:** `GET /api/members/{id}`
- **Soft Delete Member:** `DELETE /api/members/{id}`

### Contribution Processing

- **Add Monthly Contribution:** `POST /api/contributions/monthly`
- **Add Voluntary Contribution:** `POST /api/contributions/voluntary`
- **Get Contributions:** `GET /api/contributions/member/{id}`
- **Generate Contribution Statement:** `GET /api/contributions/statement/{id}`

### Employer Management

- **Register Employer:** `POST /api/employers`
- **Update Employer:** `PUT /api/employers/{id}`
- **Retrieve Employer:** `GET /api/employers/{id}`

### Transactions & Background Jobs (Hangfire)

- **Validate Contributions** *(Background Job)*
- **Update Benefit Eligibility** *(Background Job)*
- **Process Failed Transactions** *(Background Job)*
- **Send Notifications** *(Background Job)*

## Business Rules & Validation

- **Member Validation:** Age must be between 18-70, email & phone must be unique.
- **Contribution Validation:** Amount must be greater than 0, valid contribution dates required.
- **Employer Validation:** Must be active and have a valid registration number.
- **Benefit Eligibility:** Minimum contribution period required before payout eligibility.

## Error Handling & Logging

- Uses **structured logging** with Serilog.
- Implements **retry mechanisms** for failed transactions.
- Provides **clear error messages** and status codes.

## Security Considerations

- Uses **JWT-based authentication & authorization**.
- Implements **input validation** to prevent SQL Injection & XSS attacks.
- Stores **secure passwords** using hashing.


## Technologies Used

- **Backend**: .NET 8, C#, Entity Framework Core
- **Database**: SQL Server
- **Security**: JWT Authentication
- **Testing**: xUnit, Moq

## Contributing

Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch (`feature/your-feature`).
3. Commit your changes with descriptive messages.
4. Push the changes and create a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any questions or support, please contact [ogbeidemurphy@gmail.com](mailto:ogbeidemurphy@gmail.com).

## Conclusion

EPS+ is designed for **scalability, maintainability, and security**, ensuring compliance with pension contribution business rules. With robust **error handling, logging, and background processing**, the system provides reliability and efficiency in managing pension contributions.



