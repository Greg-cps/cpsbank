# CPSBank

CPSBank is a simple ASP.NET Core application for managing clients, developed with Razor Pages and SQL Server as the backend database. This project demonstrates CRUD (Create, Read, Update, Delete) operations on a client database.

## Table of Contents
- [Features](#features)
- [Technologies](#technologies)
- [Setup](#setup)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features
- List all clients with pagination.
- Search for clients by name or email.
- Create new client records.
- Edit existing client records.
- Delete client records.
- Error logging using `ILogger`.

## Technologies
- **ASP.NET Core Razor Pages**: The web application framework used for server-side rendering.
- **SQL Server**: The database used to store client data.
- **Entity Framework Core**: Used for database access and data manipulation.
- **Bootstrap**: Used for styling and responsive design.
- **Logger**: Built-in logging via `ILogger` for error handling.

## Setup

### Prerequisites
- .NET 6.0 SDK or higher
- SQL Server
- A GitHub account for cloning the repository

### Steps to run locally
1. **Clone the repository**:
    ```bash
    git clone https://github.com/Greg-cps/cpsbank.git
    ```
   
2. **Navigate into the project folder**:
    ```bash
    cd cpsbank
    ```

3. **Set up the database**:
   - Ensure that you have SQL Server installed and running.
   - Update the connection string in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=cpsbank;Trusted_Connection=True;"
     }
     ```
   - Run database migrations or create the required tables.

4. **Run the application**:
    ```bash
    dotnet run
    ```

5. **Access the application**:
   Open your browser and navigate to `http://localhost:5000/Clients`.

## Usage
- **List Clients**: Displays paginated list of clients.
- **Add Client**: Click on 'Add New Client' to add a client record.
- **Edit Client**: Click on 'Edit' next to a client record to update the details.
- **Delete Client**: Click on 'Delete' to remove a client from the database.

## Contributing
Contributions are welcome! If you'd like to help with improving the app or fixing bugs, please:
1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-name`).
3. Commit your changes (`git commit -m 'Add new feature'`).
4. Push to your branch (`git push origin feature-name`).
5. Open a pull request.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.
