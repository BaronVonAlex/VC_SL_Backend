# VC_SL API Documentation

## Overview

VC_SL is an ASP.NET Core 8.0 Web API for managing user data and winrate statistics. The application tracks user information and their gaming performance metrics (base attack, base defense, and fleet winrates) organized by month and year.

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL 8.0.36
- **ORM**: Entity Framework Core 9.0
- **Hosting**: Azure Web App
- **CI/CD**: GitHub Actions

## Architecture

### Project Structure

```
VC_SL/
├── Controllers/          # API endpoints
├── Services/            # Business logic layer
├── Data/               # Database context
├── Models/
│   ├── Entities/       # Database entities
│   └── Dtos/          # Data transfer objects
└── Exceptions/         # Custom exception classes
```

## Database Schema

### Users Table
- `Id` (int, PK): User identifier
- `username_history` (string): JSON array of usernames
- `createdAt` (DateTime): Record creation timestamp
- `updatedAt` (DateTime): Last update timestamp

### Winrates Table
- `Id` (int, PK): Auto-generated identifier
- `userId` (int, FK): Reference to Users table
- `month` (int): Month (1-12)
- `year` (int): Year (2000-2100)
- `baseAttackWinrate` (float, nullable): Attack winrate percentage (0-100)
- `baseDefenceWinrate` (float, nullable): Defense winrate percentage (0-100)
- `fleetWinrate` (float, nullable): Fleet winrate percentage (0-100)
- **Unique Constraint**: (userId, year, month)

## API Endpoints

### Users Controller

#### Get User by ID
```http
GET /api/Users/GetUser/{id}
```

**Parameters:**
- `id` (path, required): User ID

**Response:**
- `200 OK`: Returns UserDto
- `404 Not Found`: User not found

**Example Response:**
```json
{
  "id": 123,
  "usernameHistory": ["player1", "player2"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-03-20T14:45:00Z"
}
```

#### Create User
```http
POST /api/Users/CreateUser
```

**Request Body:**
```json
{
  "id": 123,
  "usernameHistory": "initialUsername"
}
```

**Response:**
- `201 Created`: Returns created UserDto with Location header
- `400 Bad Request`: Invalid input
- `409 Conflict`: User with ID already exists

#### Update User
```http
PUT /api/Users/UpdateUser/{id}
```

**Parameters:**
- `id` (path, required): User ID

**Request Body:**
```json
{
  "usernameHistory": "newUsername"
}
```

**Response:**
- `200 OK`: Returns updated UserDto
- `400 Bad Request`: Invalid input
- `404 Not Found`: User not found

**Notes:**
- New usernames are appended to the history if not already present
- Supports single username or JSON array format

### Winrate Controller

#### Get Winrate for User
```http
GET /api/Winrate/GetWinrateForUser?userId={userId}&year={year}
```

**Parameters:**
- `userId` (query, required): User ID
- `year` (query, optional): Target year (defaults to current year)

**Response:**
- `200 OK`: Returns list of WinrateDto
- `404 Not Found`: User not found

**Example Response:**
```json
[
  {
    "userId": 123,
    "month": 1,
    "year": 2024,
    "baseAttackWinrate": 75.5,
    "baseDefenceWinrate": 82.3,
    "fleetWinrate": 68.9
  },
  {
    "userId": 123,
    "month": 2,
    "year": 2024,
    "baseAttackWinrate": 78.2,
    "baseDefenceWinrate": 85.1,
    "fleetWinrate": 71.4
  }
]
```

#### Update/Create Winrate
```http
POST /api/Winrate/UpdateWinrate
```

**Request Body:**
```json
{
  "userId": 123,
  "month": 3,
  "year": 2024,
  "baseAttackWinrate": 80.5,
  "baseDefenceWinrate": 87.2,
  "fleetWinrate": 73.8
}
```

**Response:**
- `200 OK`: Returns updated/created WinrateDto
- `201 Created`: New record created
- `400 Bad Request`: Validation errors
- `404 Not Found`: User not found

**Notes:**
- This endpoint performs an "upsert" operation
- If a record exists for the user/year/month combination, it updates
- Otherwise, it creates a new record
- All winrate values are optional and nullable

## Data Transfer Objects (DTOs)

### CreateUserDto
- `Id` (required): Must be greater than 0
- `UsernameHistory` (optional): Initial username

### UpdateUserDto
- `UsernameHistory` (required): 1-100 characters

### UpdateWinrateDto
- `UserId` (required): Must be greater than 0
- `Month` (required): 1-12
- `Year` (required): 2000-2100
- `BaseAttackWinrate` (optional): 0-100
- `BaseDefenceWinrate` (optional): 0-100
- `FleetWinrate` (optional): 0-100

### UserDto
- `Id`: User identifier
- `UsernameHistory`: List of usernames
- `CreatedAt`: Creation timestamp
- `UpdatedAt`: Last update timestamp

### WinrateDto
- `UserId`: User identifier
- `Month`: 1-12
- `Year`: Year value
- `BaseAttackWinrate`: Attack winrate (nullable)
- `BaseDefenceWinrate`: Defense winrate (nullable)
- `FleetWinrate`: Fleet winrate (nullable)

## Security

### API Key Authentication
In production environments, all requests must include an API key header:

```http
X-API-Key: your-api-key-here
```

- Development environment: No API key required
- Production environment: Returns `403 Forbidden` if API key is invalid or missing

### CORS Policy
The API allows cross-origin requests from:
- `https://purple-plant-051730d03.2.azurestaticapps.net`

## Configuration

### Connection String
Configure the database connection in `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "VcSlDbConnectionString": "server=localhost;port=3306;database=vcsl;user=root;password=yourpassword;SslMode=Preferred;"
  }
}
```

### API Key
Set the API key in configuration:
```json
{
  "ApiKey": "your-secure-api-key"
}
```

## Deployment

### GitHub Actions CI/CD
The project uses automated deployment to Azure Web App via GitHub Actions:

1. **Build**: Compiles .NET 8.0 application
2. **Publish**: Creates deployment package
3. **Deploy**: Deploys to Azure Web App "VCSL" (Production slot)

### Azure Configuration
Required GitHub Secrets:
- `AZUREAPPSERVICE_CLIENTID_*`: Azure service principal client ID
- `AZUREAPPSERVICE_TENANTID_*`: Azure tenant ID
- `AZUREAPPSERVICE_SUBSCRIPTIONID_*`: Azure subscription ID

## Error Handling

### Custom Exceptions
- **NotFoundException**: Resource not found (404)
- **ConflictException**: Resource already exists (409)
- **ValidationException**: Input validation failed (400)

### Error Response Format
```json
{
  "error": "Error message here"
}
```

For validation errors:
```json
{
  "errors": {
    "FieldName": ["Error message 1", "Error message 2"]
  }
}
```

## Development

### Prerequisites
- .NET 8.0 SDK
- MySQL 8.0+
- Visual Studio 2022 or VS Code

### Local Setup
1. Clone the repository
2. Create `appsettings.Local.json` with your connection string
3. Run migrations (if applicable)
4. Start the application: `dotnet run`
5. Access Swagger UI: `https://localhost:7034/swagger`

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to "Development" for local development
- Connection strings can be overridden via environment variables

## Special Features

### Username History Management
The system maintains a history of usernames as a JSON array:
- Automatically prevents duplicates
- Supports adding single usernames or arrays
- Flexible input parsing (strings, JSON arrays, quoted strings)

### Winrate Upsert Logic
- Single endpoint for both create and update operations
- Preserves existing data when updating specific metrics
- Validates user existence before creating winrate records

## API Versions
Current version uses standard ASP.NET Core conventions without explicit versioning.

## Support
For issues or questions, refer to the application logs or Azure Application Insights (if configured).