# Multiplayer Card Game

A full-stack multiplayer card game application with a RESTful API backend and MVC frontend.

## Project Structure

```
backend/
├── CardGame.Api/          # RESTful API backend
├── CardGame.Mvc/          # MVC frontend application
└── README.md              # This file
```

## Features

- **6 Players**: Each game supports exactly 6 players
- **Card Dealing**: Deals 5 cards to each player from two standard 52-card decks (104 cards total)
- **Scoring System**: 
  - Hand score = sum of card values
  - Number cards = face value
  - J = 11, Q = 12, K = 13, A = 11
- **Tie Breaking**: Uses suit product when hand scores are tied
  - Suit values: ♦ = 1, ♥ = 2, ♠ = 3, ♣ = 4
  - Suit score = product of the 5 suit values
- **Winner Detection**: Automatically determines winner(s) based on hand scores and tie-breaker rules
- **Game History**: View past games with pagination
- **Redeal**: Re-deal cards for existing games

## API Endpoints

### POST /games
Creates a new game, deals cards, calculates scores, and determines winners.

**Response**: `201 Created` with game object including all players, cards, and scores.

### GET /games/{id}
Fetches a specific game with all details.

**Response**: `200 OK` with game object, or `404 Not Found` if game doesn't exist.

### GET /games?skip={skip}&take={take}
Lists games with pagination.

**Query Parameters**:
- `skip` (optional): Number of games to skip (default: 0)
- `take` (optional): Number of games to return (default: 10)

**Response**: `200 OK` with object containing `items` array and `total` count.

### POST /games/{id}/redeal
Re-deals cards for an existing game and re-calculates scores.

**Response**: `200 OK` with updated game object, or `404 Not Found` if game doesn't exist.

## Technology Stack

### Backend (CardGame.Api)
- **.NET 8.0**
- **ASP.NET Core Web API**
- **Entity Framework Core** with In-Memory Database
- **Swagger/OpenAPI** for API documentation

### Frontend (CardGame.Mvc)
- **.NET 8.0**
- **ASP.NET Core MVC**
- **Razor Views**
- **SVG** for poker table visualization

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Your preferred IDE (Visual Studio, VS Code, Rider, etc.)

### Running the API

1. Navigate to the API project:
   ```bash
   cd CardGame.Api
   ```

2. Run the API:
   ```bash
   dotnet run
   ```

3. The API will start on `http://localhost:5183`
4. Swagger UI will be available at `http://localhost:5183/swagger` (in Development mode)

### Running the MVC Frontend

1. **First, ensure the API is running** (see above)

2. Navigate to the MVC project:
   ```bash
   cd CardGame.Mvc
   ```

3. Update `appsettings.json` if your API is running on a different port:
   ```json
   {
     "ApiBaseUrl": "http://localhost:5183"
   }
   ```

4. Run the MVC application:
   ```bash
   dotnet run
   ```

5. Open your browser and navigate to the URL shown in the console (typically `http://localhost:5000` or `https://localhost:5001`)

## Testing

### Using Postman

A Postman collection is included: `CardGame.Api.postman_collection.json`

1. Import the collection into Postman
2. Update the `baseUrl` variable if your API runs on a different port
3. Test all endpoints

### Example API Calls

**Create a new game:**
```bash
curl -X POST http://localhost:5183/games
```

**Get a specific game:**
```bash
curl http://localhost:5183/games/1
```

**List games:**
```bash
curl "http://localhost:5183/games?skip=0&take=10"
```

**Redeal a game:**
```bash
curl -X POST http://localhost:5183/games/1/redeal
```

## Game Rules

### Card Values
- Number cards (2-10): Face value
- J (Jack): 11
- Q (Queen): 12
- K (King): 13
- A (Ace): 11 (not 1)

### Scoring
1. **Hand Score**: Sum of all 5 card values
2. **Winner**: Player(s) with the highest hand score
3. **Tie Breaking**: If multiple players have the same hand score:
   - Calculate suit product for tied players only
   - Suit product = product of all 5 suit values
   - Winner = player(s) with highest suit product
   - If still tied, multiple winners are possible

### Suit Values (for tie-breaking)
- ♦ (Diamonds): 1
- ♥ (Hearts): 2
- ♠ (Spades): 3
- ♣ (Clubs): 4

## Database Schema

The application uses an in-memory database (data resets on application restart).

### Tables

**games**
- `Id` (int, PK)
- `Name` (string)
- `CreatedAt` (DateTime)

**players**
- `Id` (int, PK)
- `Name` (string)
- `GameId` (int, FK → games.Id)

**cards**
- `Id` (int, PK)
- `Suit` (string)
- `Rank` (string)
- `Value` (int)
- `DeckId` (int)
- `IsJoker` (bool)
- `PlayerId` (int, FK → players.Id)

**scores**
- `Id` (int, PK)
- `PlayerId` (int, FK → players.Id)
- `HandSum` (int)
- `SuitProduct` (int)
- `IsWinner` (bool)

## Project Architecture

### Backend Architecture

```
CardGame.Api/
├── Controllers/          # API controllers
│   └── GamesController.cs
├── Services/            # Business logic
│   ├── GameService.cs
│   └── IGameServices.cs
├── Repositories/        # Data access layer
│   ├── GameRepository.cs
│   ├── IGameRepository.cs
│   ├── EfRepository.cs
│   └── IRepository.cs
├── Models/              # Domain models
│   ├── Game.cs
│   ├── Player.cs
│   ├── Card.cs
│   └── Score.cs
└── Data/                # Database context
    └── CardGameContext.cs
```

### Frontend Architecture

```
CardGame.Mvc/
├── Controllers/         # MVC controllers
│   ├── GameController.cs
│   └── HistoryController.cs
├── Services/           # API client service
│   └── GameApiService.cs
├── Models/             # View models
│   ├── Game.cs
│   ├── Player.cs
│   ├── Card.cs
│   ├── Score.cs
│   └── GameListResponse.cs
└── Views/              # Razor views
    ├── Game/
    │   └── Index.cshtml
    └── History/
        └── Index.cshtml
```

## Error Handling

- Global exception handler configured in `Program.cs`
- Errors return standardized ProblemDetails responses
- Frontend displays error messages via toast notifications
- API returns appropriate HTTP status codes (400, 404, 500)

## CORS Configuration

CORS is enabled for all origins, methods, and headers (configured in `Program.cs`). This allows the frontend to communicate with the API from any origin.

## Development Notes

- The in-memory database resets on application restart
- No migrations are needed (in-memory database)
- Swagger UI is only available in Development environment
- The API and MVC frontend can run on different ports

## Future Enhancements

Potential improvements:
- Persistent database (SQL Server, PostgreSQL, etc.)
- Real-time updates using SignalR
- User authentication and authorization
- Custom player names
- Game statistics and analytics
- Export game results

## License

This project is for educational/demonstration purposes.

## Contributing

Feel free to submit issues or pull requests for improvements.

