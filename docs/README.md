# CommuteAlert

Webbapplikation som bevakar dina SL-resor och skickar ett larm till din Discord när en avgång är inställd eller försenad, eller när en störning påverkar din linje.


## Problemet
Man märker ofta för sent att en avgång är inställd eller försenad. Pendlingslarm låter dig ange din vanliga resa en gång och ger bara besked när något faktiskt påverkar den, under de tider du själv väljer.

## Funktioner
- Registrering och inloggning
- Egen Discord-webhook med testmeddelande
- Bevakningar med trafikslag, hållplats, linje, dagar och tider
- Larm vid inställd avgång, försening över valt gränsvärde och störning på vald linje
- Skydd mot dubbla larm
- Historik över skickade larm
- Simuleringsläge för demonstration


## Teknik
ASP.NET Core MVC, bakgrundstjänst, Entity Framework Core, SQL Server, Discord-webhook.
Data från SL Transport och SL Deviations (Trafiklab), utan API-nyckel.


## Starta

**1. Starta databasen**
docker compose up -d db


**2. Ställ in anslutningssträngen** (en gång)
dotnet user-secrets set "ConnectionStrings:Default" "[anslutningssträng]" --project src/CommuteAlert.Web


**3. Starta programmet**
dotnet run --project src/CommuteAlert.Web


Öppna sedan [adress] i webbläsaren, registrera ett konto och ange din Discord-webhook under inställningar.

**Alternativ: allt i Docker**
docker compose up





## Köra testerna
dotnet test


## Projektstruktur
src/
  Pendlingslarm.Web/            Modeller och regler, SL-klient, databas, Discord, MVC
tests/
  Pendlingslarm.Tests/
docs/                            Projektdokumentation


## Dokumentation
- [Projektplan](docs/projektplan.md)
- [Projektdagbok](docs/projektdagbok.md)
- [Beslutslogg](docs/beslutslogg.md)
- [Testplan och testrapport](docs/testplan-och-testrapport.md)
- [Projektrapport](docs/projektrapport.md)

## Säkerhet
Webhook-adresser lagras krypterade, visas aldrig i sin helhet och loggas inte.

## Användning av AI
AI har använts som stöd under projektet. Detaljer finns i projektdagboken och projektrapporten.

