# CommuteAlert

Webbapplikation som bevakar dina SL-resor och skickar ett larm till din Discord när en avgång är inställd eller försenad, eller när en störning påverkar din linje.


## Problemet

Man märker ofta för sent att en avgång är inställd eller försenad. Pendlingslarm låter dig ange din vanliga resa en gång och ger bara besked när något faktiskt påverkar den, under de tider du själv väljer.


## Funktioner och flöden

- Registrering och inloggning
- Admin hanterar användare
- Egen Discord-webhook med testmeddelande
- Bevakningar med trafikslag, hållplats, linje, dagar och tider
- Larm vid inställd avgång, försening över valt gränsvärde och störning på vald linje
- Skydd mot dubbla larm
- Historik över skickade larm

Som admin kommer du att få ett konto skapat från start. Som inbjuden användare måste admin skapa ett konto där användaren senare kan byta lösenordet. Hämta din webhook från din discord kanal och spara den under Settings, du kan prova att test skicka en notis till den. Skapa en bevakning, sök stationen som du vill bevaka och ange inom vilken tidsram du vill bevaka den. Ta bort eller pausa de bevakningar du inte vill ha.


## Teknik

ASP.NET Core MVC, Entity Framework Core, SQL Server i en docker container, Discord-webhook.
Data från SL Transport och SL Deviations (Trafiklab), utan API-nyckel.


## Starta

1. Installera Docker Desktop och starta det.

2. Skapa en .env-fil i projektets rotmapp med följande innehåll (Första gången endast):

  - MSSQL_SA_PASSWORD=EttLangtLosenord123!

3. Starta SQL Server-databasen från mappen där docker-compose.yml ligger (Vänta 30s innan du startar appen så SQL hinner starta):
  - docker compose up -d db

4. Konfigurera User Secrets (Första gången endast), (Lösen för Connectionstring måste va samma som i .env)
    
  - dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=CommuteAlert;User Id=sa;Password=EttLangtLosenord123!;TrustServerCertificate=True" --project src/CommuteAlert.Web/CommuteAlert.Web.csproj

  - dotnet user-secrets set "Admin:Email" "din@epost.se" --project src/CommuteAlert.Web/CommuteAlert.Web.csproj

  - dotnet user-secrets set "Admin:Password" "ett-langt-losenord-pa-minst-12-tecken" --project src/CommuteAlert.Web/CommuteAlert.Web.csproj

5. Starta projektet CommuteAlert.Web i Visual Studio.

  - dotnet run --project src/CommuteAlert.Web


## Köra testerna

dotnet test


## Projektstruktur

src/
CommuteAlert.Web/       - Modeller och regler, SL-klient, databas, Discord, MVC
tests/
  CommuteAlert.Tests/   - Tester
docs/                   - Projektdokumentation


## Dokumentation
- [Projektplan](docs/projektplan-rapport.md)
- [Projektdagbok](docs/projektdagbok.md)
- [Beslutslogg](docs/beslutslogg.md)
- [Testplan och testrapport](docs/testplan-och-testrapport.md)
- [Projektrapport](docs/projektrapport.md)
- [Säkerhet](docs/säkerhet.md)


## Användning av AI

AI har använts som stöd under projektet. Detaljer finns i projektdagboken och projektrapporten.

