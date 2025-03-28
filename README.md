# EPSPlus

EPSPlus is a robust enterprise payroll system designed to streamline payroll processing, automate tax calculations, and ensure compliance with financial regulations. The system provides an intuitive interface for employers and employees, offering secure payroll management and reporting capabilities.

## Features

- **Employee Management**: Add, update, and manage employee details.
- **Payroll Processing**: Automate salary calculations, deductions, and bonuses.
- **Tax Compliance**: Integrated tax calculation to ensure legal compliance.
- **Payment Processing**: Supports multiple payment methods, including direct bank deposits.
- **User Roles & Permissions**: Role-based access control for administrators, HR personnel, and employees.
- **Reports & Analytics**: Generate payroll summaries, tax reports, and financial insights.
- **Secure Authentication**: Implements JWT-based authentication for secure access.

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

API Endpoints

Authentication & Authorization

Register Member: POST /api/auth/register/member

Register Employer: POST /api/auth/register/employer

Register Admin: POST /api/auth/register/admin

Login: POST /api/auth/login

Reset Password: POST /api/auth/reset-password

Member Management

Register Member: POST /api/members

Update Member: PUT /api/members/{id}

Retrieve Member: GET /api/members/{id}

Soft Delete Member: DELETE /api/members/{id}

Contribution Processing

Add Monthly Contribution: POST /api/contributions/monthly

Add Voluntary Contribution: POST /api/contributions/voluntary

Get Contributions: GET /api/contributions/member/{id}

Generate Contribution Statement: GET /api/contributions/statement/{id}

Employer Management

Register Employer: POST /api/employers

Update Employer: PUT /api/employers/{id}

Retrieve Employer: GET /api/employers/{id}

Transactions & Background Jobs (Hangfire)

Validate Contributions (Background Job)

Update Benefit Eligibility (Background Job)

Process Failed Transactions (Background Job)

Send Notifications (Background Job)

Business Rules & Validation

Member Validation: Age must be between 18-70, email & phone must be unique.

Contribution Validation: Amount must be greater than 0, valid contribution dates required.

Employer Validation: Must be active and have a valid registration number.

Benefit Eligibility: Minimum contribution period required before payout eligibility.

Error Handling & Logging

Uses structured logging with Serilog.

Implements retry mechanisms for failed transactions.

Provides clear error messages and status codes.

Security Considerations

Uses JWT-based authentication & authorization.

Implements input validation to prevent SQL Injection & XSS attacks.

Stores secure passwords using hashing.
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

