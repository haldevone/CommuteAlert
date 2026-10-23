# Beslutslogg – CommuteAlert

Här skrivs varje viktigt tekniskt eller praktiskt val ner, med alternativen och varför jag valde som jag gjorde. Loggen används vid den muntliga redovisningen för att förklara och försvara valen.



## Ett MVC-projekt istället för separat frontend och backend
**Bakgrund:** Appen behöver ett gränssnitt för inloggning, bevakningar och historik, samt ett backend del.
**Alternativ:** 1) ASP.NET Core MVC i en lösning. 2) Separat frontend (till exempel React) mot ett API.
**Beslut:** MVC i en lösning, tiden är fyra veckor och testing samt säkerhet är fokus, orkar inte hålla på med CORS och två projekt


## Lagerindelning (Core, Infrastructure, Web, Tests)
**Bakgrund:** Reglerna för när ett larm ska skickas är kärnan och måste vara lätta att testa.
**Alternativ:** 1) Allt i ett projekt. 2) Uppdelning i lager.
**Beslut:** Allt i ett projekt, vill hålla nere komplexiteten men ändå behålla en bra struktur med tydliga mappar


## SQL Server istället för SQLite
**Bakgrund:** Appen lagrar användare, bevakningar, larmhistorik och krypterade webhook-adresser
**Alternativ:** 1) SQLite. 2) SQL Server
**Beslut:** SQL Server via EF Core, vill behålla lagrad data samt planerar för container i Azure


## Discord-webhook som notiskanal, bakom ett gränssnitt
**Bakgrund:** Larmet ska nå användaren när webbsidan är stängd.
**Alternativ:** 1) SignalR. 2) E-post. 3) Telegram-bot. 4) Discord-webhook.
**Beslut:** Discord-webhook, anropad via gränssnittet `INotifier`. SignalR når bara en öppen webläsare, behöver ha något som är uppkopplat ständigt.


## Egen webhook per användare, krypterad
**Bakgrund:** Varje användare ska ange sin egen Discord-adress efter inloggning.
**Alternativ:** 1) Lagra i klartext. 2) Lagra krypterad.
**Beslut:** Lagra krypterad med ASP.NET Core Data Protection, visa aldrig hela adressen, logga den aldrig, och godkänn bara adresser som börjar med `https://discord.com/api/webhooks/`.


## SL Transport och SL Deviations, utan API-nyckel
**Bakgrund:** Appen behöver avgångar och störningar.
**Alternativ:** 1) Trafiklab Realtime-API:er (nyckel). 2) SL Transport och SL Deviations. publikt
**Beslut:** SL Transport (avgångar, hållplatser, linjer) och SL Deviations (störningar). Håller det enkelt utan API nyckel


## Docker som tillägg, `dotnet run` som huvudväg
**Bakgrund:** Projektet ska gå att starta för den som granskar, och senare köras i Azure.
**Alternativ:** 1) Bara `dotnet run`. 2) Docker och `docker compose`.
**Beslut:** README visar båda. Databasen körs i container, och programmet kan köras både direkt och i container. Ett enda MVC-projekt startar med ett kommando, men databasen behöver en container, och samma container-paket kan användas i Azure.

## LarmFlödet
**Bakgrund:** Förklaring av larmfödet
**Beslut:** Användaren sparar en bevakning av valda parametrar, det sparas i db. Varje minut poolas från SL störningar och avgångsinformation. Det jämförs med sparade bevakningen och utverderas om den matchar så skickas det notis vidare till discord. Systemet förhindrar duplikat av larm.

