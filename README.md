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

## API Endpoints

| Method | Endpoint                  | Description |
|--------|---------------------------|-------------|
| POST   | `/api/auth/login`         | User login |
| POST   | `/api/employers/register` | Register an employer |
| GET    | `/api/payroll/generate`   | Generate payroll for employees |
| GET    | `/api/reports/tax`        | Retrieve tax reports |

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

