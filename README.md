# TraitorGator

TraitorGator is an open source social deduction party game inspired by hidden-role room games. Players answer prompts, compare the answers, then vote for who they think got the traitor prompt.

## Gameplay

1. A host creates a lobby and shares the four-digit game code.
2. At least three players join.
3. One player is secretly assigned as the traitor.
4. Gators and the traitor receive similar but different prompts.
5. Everyone answers, reads the group result, and votes for a suspect.
6. If the traitor gets the most votes, the gators win. Otherwise the traitor survives until the final round.

## Tech Stack

- .NET 8
- ASP.NET Core API
- Blazor Server client
- EF Core with SQLite for local development

## Run Locally

Start the API:

```bash
dotnet run --project TraitorGator.API/TraitorGator.ApiHost.csproj --urls http://localhost:5256
```

Start the client in another terminal:

```bash
dotnet run --project TraitorGator.Client/TraitorGator.Client.csproj --urls http://localhost:5092
```

Open `http://localhost:5092`.

The API creates a local SQLite database from the EF model and seeds starter questions automatically. Delete `traitorgator.dev.db` to reset local data.

## Security Notes

- Do not commit `.env`, local SQLite databases, or generated build output.
- Player actions require a per-player secret created when joining/creating a lobby.
- Game state endpoints do not expose roles; roles are only returned with the matching player secret.
- The current Blazor flow carries that player secret in the route for simplicity. Use HTTPS locally/deployed, and replace route-carried session secrets before exposing games on untrusted networks.
- Configure CORS with `Cors:AllowedOrigins` before deploying a browser client separately from the API.
- Use HTTPS and real hosting secrets outside local development.

## License

MIT
