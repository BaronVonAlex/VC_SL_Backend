# VC_SL API Documentation

## Overview

VC_SL is an ASP.NET Core 8.0 Web API designed to manage user profiles and gaming performance statistics. The system tracks user information and winrate metrics across three categories (Base Attack, Base Defense, and Fleet) organized by month and year, with a comprehensive leaderboard system.

## Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Database**: MySQL 8.0.36
- **ORM**: Entity Framework Core 9.0
- **Hosting**: Azure Web App
- **CI/CD**: GitHub Actions
- **API Documentation**: Swagger/OpenAPI

## Architecture

### Project Structure

```
VC_SL/
├── Controllers/          # API endpoints
│   ├── UsersController.cs
│   ├── WinrateController.cs
│   └── LeaderboardController.cs
├── Services/            # Business logic layer
│   ├── UserService.cs
│   ├── WinrateService.cs
│   ├── LeaderboardService.cs
│   └── UsernameHistoryHelper.cs
├── Data/               # Database context
│   └── ApplicationDbContext.cs
├── Models/
│   ├── Entities/       # Database entities
│   │   ├── User.cs
│   │   └── Winrate.cs
│   └── Dtos/          # Data transfer objects
│       ├── UserDto.cs
│       ├── WinrateDto.cs
│       ├── LeaderboardDto.cs
│       └── ...
└── Exceptions/         # Custom exception classes
    ├── NotFoundException.cs
    ├── ConflictException.cs
    └── ValidationException.cs
```

### Design Patterns

- **Repository Pattern**: Service layer abstracts data access
- **Dependency Injection**: All services registered via DI container
- **DTO Pattern**: Separation between entities and API contracts
- **Exception Handling**: Custom exceptions for different error scenarios

## Database Schema

### Users Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | int | PRIMARY KEY | User identifier (manually assigned) |
| `username_history` | varchar | NOT NULL | JSON array of historical usernames |
| `createdAt` | datetime | NOT NULL | Record creation timestamp |
| `updatedAt` | datetime | NOT NULL | Last modification timestamp |

### Winrates Table

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | int | PRIMARY KEY, AUTO_INCREMENT | Record identifier |
| `userId` | int | FOREIGN KEY, NOT NULL | References Users.Id |
| `month` | int | NOT NULL | Month (1-12) |
| `year` | int | NOT NULL | Year (2000-2100) |
| `baseAttackWinrate` | float | NULLABLE | Attack winrate % (0-100) |
| `baseDefenceWinrate` | float | NULLABLE | Defense winrate % (0-100) |
| `fleetWinrate` | float | NULLABLE | Fleet winrate % (0-100) |

**Indexes:**
- Unique composite index on `(userId, year, month)`

**Relationships:**
- `Winrates.userId` → `Users.Id` (Foreign Key)

## API Endpoints

### Users Controller (`/api/Users`)

#### Get User by ID

```http
GET /api/Users/GetUser/{id}
```

**Parameters:**
- `id` (path, integer, required): User ID

**Responses:**

`200 OK`
```json
{
  "id": 123,
  "usernameHistory": ["player1", "player2", "player3"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-03-20T14:45:00Z"
}
```

`404 Not Found`
```json
{
  "error": "User with ID 123 not found"
}
```

---

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

**Validation Rules:**
- `id`: Required, must be greater than 0
- `usernameHistory`: Optional string

**Responses:**

`201 Created`
```json
{
  "id": 123,
  "usernameHistory": ["initialUsername"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:30:00Z"
}
```

`400 Bad Request`
```json
{
  "Id": ["Id must be greater than 0"]
}
```

`409 Conflict`
```json
{
  "error": "User with ID 123 already exists"
}
```

**Notes:**
- If `usernameHistory` is not provided or empty, an empty array is stored
- The ID must be unique across all users
- Response includes a `Location` header pointing to the new resource

---

#### Update User

```http
PUT /api/Users/UpdateUser/{id}
```

**Parameters:**
- `id` (path, integer, required): User ID

**Request Body:**
```json
{
  "usernameHistory": "newUsername"
}
```

**Validation Rules:**
- `usernameHistory`: Required, 1-100 characters

**Responses:**

`200 OK`
```json
{
  "id": 123,
  "usernameHistory": ["player1", "player2", "newUsername"],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-03-20T14:45:00Z"
}
```

`400 Bad Request`
```json
{
  "UsernameHistory": ["Username is required"]
}
```

`404 Not Found`
```json
{
  "error": "User with ID 123 not found"
}
```

**Username History Behavior:**
- New usernames are appended only if not already present
- Supports multiple formats: single string, JSON array, or quoted string
- Automatically trims whitespace and removes duplicates
- Updates the `updatedAt` timestamp

---

### Winrate Controller (`/api/Winrate`)

#### Get Winrate for User

```http
GET /api/Winrate/GetWinrateForUser?userId={userId}&year={year}
```

**Parameters:**
- `userId` (query, integer, required): User ID
- `year` (query, integer, optional): Target year (defaults to current year)

**Responses:**

`200 OK`
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

`404 Not Found`
```json
{
  "error": "User with ID 123 not found"
}
```

**Notes:**
- Returns records ordered by month (ascending)
- If no year is specified, defaults to current year
- Returns empty array if no winrate data exists for the specified year

---

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

**Validation Rules:**
- `userId`: Required, must be greater than 0
- `month`: Required, 1-12
- `year`: Required, 2000-2100
- `baseAttackWinrate`: Optional, 0-100
- `baseDefenceWinrate`: Optional, 0-100
- `fleetWinrate`: Optional, 0-100

**Responses:**

`200 OK` (record updated)
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

`400 Bad Request`
```json
{
  "Month": ["Month must be between 1 and 12"],
  "BaseAttackWinrate": ["Winrate must be between 0 and 100"]
}
```

`404 Not Found`
```json
{
  "error": "User with ID 123 not found. Please create the user first."
}
```

**Upsert Behavior:**
- If a record exists for `(userId, year, month)`, it updates the existing record
- Otherwise, creates a new record
- All winrate fields are optional and can be set independently
- Validates user exists before creating/updating winrate

---

### Leaderboard Controller (`/api/Leaderboard`)

#### Get Leaderboard

```http
GET /api/Leaderboard?period={period}&category={category}&month={month}&year={year}&limit={limit}&minimumMonths={minimumMonths}
```

**Parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `period` | enum | Monthly | Period type: `Monthly`, `Yearly`, or `AllTime` |
| `category` | enum | Combined | Category: `Combined`, `BaseAttack`, `BaseDefence`, or `Fleet` |
| `month` | integer | null | Month (1-12), required for Monthly period |
| `year` | integer | null | Year, required for Monthly and Yearly periods |
| `limit` | integer | 100 | Number of top players (1-1000) |
| `minimumMonths` | integer | 1 | Minimum months played to qualify |

**Leaderboard Periods:**
- **Monthly**: Rankings for a specific month (requires `month` and `year`)
- **Yearly**: Rankings for an entire year (requires `year`)
- **AllTime**: Rankings across all recorded data

**Leaderboard Categories:**
- **Combined**: Average of all three winrate types
- **BaseAttack**: Base attack winrate only
- **BaseDefence**: Base defense winrate only
- **Fleet**: Fleet winrate only

**Responses:**

`200 OK`
```json
[
  {
    "rank": 1,
    "userId": 456,
    "username": "TopPlayer",
    "winrate": 92.45,
    "monthsPlayed": 12
  },
  {
    "rank": 2,
    "userId": 789,
    "username": "SecondPlace",
    "winrate": 89.32,
    "monthsPlayed": 11
  }
]
```

`400 Bad Request`
```json
{
  "errors": {
    "Month": ["Month is required for monthly leaderboard"],
    "Year": ["Year is required for monthly leaderboard"],
    "Limit": ["Limit must be between 1 and 1000"]
  }
}
```

**Calculation Logic:**
- **Combined winrate**: Average of base attack, base defense, and fleet winrates
- Rankings sorted by winrate in descending order
- Ties are ranked by appearance order
- Only considers months where user has at least one winrate entry
- `minimumMonths` filter applies only to Yearly and AllTime periods

**Examples:**

Monthly leaderboard for January 2024:
```
GET /api/Leaderboard?period=Monthly&category=Combined&month=1&year=2024&limit=50
```

Yearly fleet leaderboard for 2024, minimum 6 months:
```
GET /api/Leaderboard?period=Yearly&category=Fleet&year=2024&limit=100&minimumMonths=6
```

All-time base attack leaderboard, top 10:
```
GET /api/Leaderboard?period=AllTime&category=BaseAttack&limit=10&minimumMonths=12
```

---

## Data Models

### DTOs (Data Transfer Objects)

#### CreateUserDto
```csharp
{
  "id": int,                    // Required, > 0
  "usernameHistory": string     // Optional
}
```

#### UpdateUserDto
```csharp
{
  "usernameHistory": string     // Required, 1-100 chars
}
```

#### UserDto
```csharp
{
  "id": int,
  "usernameHistory": string[],
  "createdAt": DateTime,
  "updatedAt": DateTime
}
```

#### UpdateWinrateDto
```csharp
{
  "userId": int,                    // Required, > 0
  "month": int,                     // Required, 1-12
  "year": int,                      // Required, 2000-2100
  "baseAttackWinrate": float?,      // Optional, 0-100
  "baseDefenceWinrate": float?,     // Optional, 0-100
  "fleetWinrate": float?            // Optional, 0-100
}
```

#### WinrateDto
```csharp
{
  "userId": int,
  "month": int,
  "year": int,
  "baseAttackWinrate": float?,
  "baseDefenceWinrate": float?,
  "fleetWinrate": float?
}
```

#### LeaderboardDto
```csharp
{
  "rank": int,
  "userId": int,
  "username": string,
  "winrate": float,
  "monthsPlayed": int
}
```

#### LeaderboardRequestDto
```csharp
{
  "period": LeaderboardPeriod,      // Monthly, Yearly, AllTime
  "category": LeaderboardCategory,  // Combined, BaseAttack, BaseDefence, Fleet
  "month": int?,
  "year": int?,
  "limit": int,                     // Default: 100
  "minimumMonths": int              // Default: 1
}
```

---

## Security

### API Key Authentication

All requests in **production** environments must include an API key header:

```http
X-API-Key: your-api-key-here
```

**Environment Behavior:**
- **Development**: No API key required, all requests allowed
- **Production**: Returns `403 Forbidden` if API key is invalid or missing

**Error Response (403):**
```
Forbidden: Invalid API Key
```

### CORS Configuration

The API allows cross-origin requests from:
```
https://purple-plant-051730d03.2.azurestaticapps.net
```

**Allowed Methods:** All HTTP methods  
**Allowed Headers:** All headers

To modify CORS settings, update the policy in `Program.cs`:
```csharp
policy.WithOrigins("your-origin-url")
    .AllowAnyMethod()
    .AllowAnyHeader();
```

---

## Configuration

### Connection String

Configure database connection in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "VcSlDbConnectionString": "server=your-server;port=3306;database=vcsl;user=root;password=yourpassword;SslMode=Preferred;"
  }
}
```

**For local development**, create `appsettings.Local.json` (gitignored):
```json
{
  "ConnectionStrings": {
    "VcSlDbConnectionString": "server=localhost;port=3306;database=vcsl_local;user=root;password=localpass;"
  }
}
```

### API Key Configuration

In production, set the API key via:

**appsettings.json:**
```json
{
  "ApiKey": "your-secure-api-key"
}
```

**Or environment variable:**
```bash
export ApiKey="your-secure-api-key"
```

### Environment Variables

Configuration precedence (highest to lowest):
1. Environment variables
2. `appsettings.Local.json`
3. `appsettings.json`

**Common variables:**
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`
- `ConnectionStrings__VcSlDbConnectionString`: Database connection
- `ApiKey`: Production API key

---

## Deployment

### GitHub Actions CI/CD

The project uses automated deployment to Azure Web App:

**Workflow:** `.github/workflows/main_vcsl.yml`

**Pipeline stages:**

1. **Build Job** (Windows runner)
    - Checkout code
    - Set up .NET 8.0
    - Build in Release configuration
    - Publish to output directory
    - Upload build artifact

2. **Deploy Job** (Windows runner)
    - Download build artifact
    - Login to Azure using OIDC
    - Deploy to Azure Web App "VCSL" (Production slot)

**Triggers:**
- Push to `main` branch
- Manual workflow dispatch

### Azure Configuration

**Required GitHub Secrets:**
- `AZUREAPPSERVICE_CLIENTID_*`: Azure service principal client ID
- `AZUREAPPSERVICE_TENANTID_*`: Azure tenant ID
- `AZUREAPPSERVICE_SUBSCRIPTIONID_*`: Azure subscription ID

**Azure Web App Settings:**
- App name: `VCSL`
- Slot: `Production`
- Runtime: .NET 8.0
- Platform: Windows

### Manual Deployment

For manual deployment:

```bash
# Build and publish
dotnet publish -c Release -o ./publish

# Deploy to Azure (using Azure CLI)
az webapp deploy --resource-group <rg-name> --name VCSL --src-path ./publish
```

---

## Error Handling

### HTTP Status Codes

| Code | Description | Usage |
|------|-------------|-------|
| 200 | OK | Successful GET, PUT requests |
| 201 | Created | Successful POST (resource created) |
| 400 | Bad Request | Validation errors, invalid input |
| 403 | Forbidden | Invalid or missing API key (production) |
| 404 | Not Found | Resource doesn't exist |
| 409 | Conflict | Resource already exists |
| 500 | Internal Server Error | Unhandled exceptions |

### Custom Exceptions

**NotFoundException**
```csharp
throw new NotFoundException("User with ID 123 not found");
```

**ConflictException**
```csharp
throw new ConflictException("User with ID 123 already exists");
```

**ValidationException (Abstract)**
```csharp
throw new LeaderboardValidationException(new Dictionary<string, List<string>> {
    { "Month", ["Month is required for monthly leaderboard"] }
});
```

### Error Response Formats

**Single error:**
```json
{
  "error": "User with ID 123 not found"
}
```

**Validation errors:**
```json
{
  "errors": {
    "UserId": ["UserId must be greater than 0"],
    "Month": ["Month must be between 1 and 12"]
  }
}
```

**Model state errors:**
```json
{
  "UsernameHistory": ["Username is required", "Username must be between 1 and 100 characters"]
}
```

---

## Development

### Prerequisites

- .NET 8.0 SDK or later
- MySQL 8.0 or later
- Visual Studio 2022, VS Code, or Rider
- Git

### Local Setup

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd VC_SL
   ```

2. **Configure database:**
    - Create MySQL database: `CREATE DATABASE vcsl_local;`
    - Create `appsettings.Local.json` with connection string

3. **Apply migrations (if applicable):**
   ```bash
   dotnet ef database update
   ```

4. **Run the application:**
   ```bash
   dotnet run --project VC_SL
   ```

5. **Access Swagger UI:**
    - HTTPS: `https://localhost:7034/swagger`
    - HTTP: `http://localhost:5184/swagger`

### Development Tools

**Package Manager Console:**
```bash
# Add migration
Add-Migration MigrationName

# Update database
Update-Database

# Remove last migration
Remove-Migration
```

**dotnet CLI:**
```bash
# Build
dotnet build

# Run tests (if available)
dotnet test

# Watch mode (auto-reload)
dotnet watch run
```

### Project Files

**Ignored files** (`.gitignore`):
- `bin/`, `obj/` - Build outputs
- `*.sqlite` - SQLite databases
- `appsettings.Development.json` - Development settings
- `appsettings.Local.json` - Local overrides
- `.vs/`, `.idea/` - IDE folders

---

## Special Features

### Username History Management

The system intelligently manages username history as a JSON array:

**Features:**
- Automatically prevents duplicate usernames
- Preserves chronological order of usernames
- Flexible input parsing supports multiple formats
- Thread-safe operations

**Supported input formats:**
```csharp
// Single string
"newUsername"

// JSON array string
"[\"name1\", \"name2\"]"

// Quoted string
"\"singleName\""
```

**Implementation:** `UsernameHistoryService.cs`

### Winrate Upsert Logic

Single endpoint for both create and update operations:

**Benefits:**
- Simplified API surface
- Idempotent operations
- Preserves partial data during updates
- Validates user existence before any operation

**Behavior:**
- Checks for existing record by `(userId, year, month)` composite key
- Updates only provided fields (null values are preserved)
- Creates new record if none exists
- Returns unified response format

### Leaderboard Calculation

Sophisticated ranking system with multiple aggregation modes:

**Features:**
- Flexible time period filtering (Monthly, Yearly, All-Time)
- Multiple category rankings (Combined, individual metrics)
- Configurable minimum participation threshold
- Efficient database aggregation

**Combined winrate formula:**
```
Combined = (BaseAttack + BaseDefence + Fleet) / 3
```

**Ranking algorithm:**
1. Calculate average winrates per user for selected period
2. Filter by minimum months played (except Monthly)
3. Sort by selected category winrate (descending)
4. Apply limit to top N players
5. Assign sequential ranks

---

## Testing

### Manual Testing with Swagger

1. Navigate to Swagger UI: `https://localhost:7034/swagger`
2. Click "Authorize" (if API key required)
3. Expand endpoint sections
4. Click "Try it out"
5. Fill in parameters and request body
6. Click "Execute"

### Example Test Scenarios

**Create user and add winrates:**
```bash
# 1. Create user
POST /api/Users/CreateUser
{ "id": 999, "usernameHistory": "TestPlayer" }

# 2. Add January winrate
POST /api/Winrate/UpdateWinrate
{ "userId": 999, "month": 1, "year": 2024, 
  "baseAttackWinrate": 75.0, "baseDefenceWinrate": 80.0, "fleetWinrate": 70.0 }

# 3. Add February winrate
POST /api/Winrate/UpdateWinrate
{ "userId": 999, "month": 2, "year": 2024,
  "baseAttackWinrate": 78.0, "baseDefenceWinrate": 82.0, "fleetWinrate": 73.0 }

# 4. Get yearly winrates
GET /api/Winrate/GetWinrateForUser?userId=999&year=2024

# 5. View yearly leaderboard
GET /api/Leaderboard?period=Yearly&year=2024&limit=10
```

---

## Best Practices

### API Usage

1. **Always validate user exists** before creating winrate records
2. **Use upsert endpoint** (`UpdateWinrate`) to avoid duplicate checks
3. **Cache leaderboard results** for frequently accessed periods
4. **Implement pagination** for large result sets (future enhancement)
5. **Store API key securely** in environment variables or key vault

### Performance Optimization

1. **Database indexes** are already configured on frequently queried columns
2. **Leaderboard queries** use efficient GROUP BY aggregations
3. **Username history** stored as JSON to avoid extra table joins
4. **Consider caching** for static leaderboards (monthly periods that have ended)

### Data Integrity

1. **Unique constraint** on `(userId, year, month)` prevents duplicates
2. **Foreign key** ensures winrates reference valid users
3. **Validation** enforces data constraints at API level
4. **Timestamps** track record lifecycle for auditing

---

## API Versioning

**Current Version:** No explicit versioning (v1 implicit)

**Future Versioning Strategy:**
- URL-based: `/api/v2/Users`
- Header-based: `Api-Version: 2.0`
- Query parameter: `/api/Users?api-version=2.0`

---

## Monitoring and Logging

### Application Insights (Azure)

When configured, the API logs:
- Request/response details
- Exception traces
- Performance metrics
- Custom events

### Log Levels

Configured in `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Levels:**
- `Trace`: Detailed diagnostic information
- `Debug`: Debugging information
- `Information`: General informational messages
- `Warning`: Warnings about potential issues
- `Error`: Error events
- `Critical`: Critical failures

---

## Support and Troubleshooting

### Common Issues

**"403 Forbidden" in production:**
- Verify API key is set in environment variables
- Check `X-API-Key` header is included in request

**Connection refused / Database errors:**
- Verify MySQL server is running
- Check connection string in configuration
- Ensure database exists and migrations are applied

**CORS errors from frontend:**
- Verify origin is included in CORS policy
- Check browser console for specific CORS error
- Ensure credentials are included if required

**Swagger not loading:**
- Only available in Development environment
- Check `ASPNETCORE_ENVIRONMENT` variable

### Getting Help

- **Application Logs**: Check Azure App Service logs or local console
- **Azure Support**: Use Azure Portal support center
- **Documentation**: Refer to this document for API details
- **Database Issues**: Verify connection and schema with MySQL Workbench

---

## Roadmap

### Potential Future Enhancements

- [ ] Pagination support for large datasets
- [ ] Filtering and search on leaderboards
- [ ] User roles and permissions (Admin, Player, Viewer)
- [ ] Historical leaderboard snapshots
- [ ] Statistics dashboard endpoints
- [ ] Webhook notifications for rank changes
- [ ] Batch winrate upload endpoint
- [ ] Export functionality (CSV, Excel)
- [ ] GraphQL API support
- [ ] Real-time leaderboard updates via SignalR



**Last Updated:** October 19, 2025  
**API Version:** 1.0  
**Document Version:** 1.0