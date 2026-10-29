# Beslutslogg – CommuteAlert

Här skrivs varje viktigt tekniskt eller praktiskt val ner, med alternativen och varför jag valde som jag gjorde. 

## Så ska det funka
1. Sök station: Du skriver ett namn (till exempel Södertälje) och får en lista med träffar från SL:s hållplatslista. Hållplatslista hämtas en gång och sparas i minnet i 24 timmar,för att inte belasta SL.
2. Skapa bevakning: När du valt station hämtar sidan de avgångar som går just nu och bygger menyn utifrån dem: allt på stationen, ett helt trafikslag, eller en viss linje i en viss riktning (“METRO 13 to Norsborg”). Då slipper vi ladda SL:s hela linjelista.
3. Dagar, tider och försening väljer du med kryssrutor och tidsfält.
4. Lista och ta bort: Du ser bara dina egna bevakningar, och borttagningen kontrollerar att bevakningen är din.

## Ett MVC-projekt istället för separat frontend och backend
Appen behöver ett gränssnitt för inloggning, bevakningar och historik, samt ett backend del.
1) ASP.NET Core MVC i en lösning. 2) Separat frontend (till exempel React) mot ett API.
MVC i en lösning, tiden är fyra veckor och testing samt säkerhet är fokus, orkar inte hålla på med CORS och två projekt


## Lagerindelning (Core, Infrastructure, Web, Tests)
Reglerna för när ett larm ska skickas är kärnan och måste vara lätta att testa.
1) Allt i ett projekt. 2) Uppdelning i lager.
Allt i ett projekt, vill hålla nere komplexiteten men ändå behålla en bra struktur med tydliga mappar


## SQL Server istället för SQLite, docker?
Appen lagrar användare, bevakningar, larmhistorik och krypterade webhook-adresser
1) SQLite. 2) SQL Server 3) Båda
Båda. SQL Server via EF Core, vill behålla lagrad data samt planerar för container i Azure därför kör jag i Docker. Sqlite kör jag endast tester.


## Discord-webhook som notiskanal, bakom ett gränssnitt
Larmet ska nå användaren när webbsidan är stängd.
1) SignalR. 2) E-post. 3) Telegram-bot. 4) Discord-webhook.
Discord-webhook, anropad via gränssnittet `INotifier`. SignalR når bara en öppen webläsare, behöver ha något som är uppkopplat ständigt.


## Egen webhook per användare, krypterad
Varje användare ska ange sin egen Discord-adress efter inloggning.
1) Lagra i klartext. 2) Lagra krypterad.
Lagra krypterad med ASP.NET Core Data Protection, visa aldrig hela adressen, logga den aldrig, och godkänn bara adresser som börjar med `https://discord.com/api/webhooks/`.


## SL Transport och SL Deviations, utan API-nyckel
Appen behöver avgångar och störningar.
1) Trafiklab Realtime-API:er (nyckel). 2) SL Transport och SL Deviations. publikt
SL Transport (avgångar, hållplatser, linjer) och SL Deviations (störningar) Publika. Håller det enkelt utan API nyckel


## Docker som tillägg, `dotnet run` som huvudväg
Projektet ska gå att starta för den som granskar, och senare köras i Azure.
1) Bara `dotnet run`. 2) Docker och `docker compose`. 
README visar båda. Databasen körs i container, och programmet kan köras både direkt och i container. Ett enda MVC-projekt startar med ett kommando, men databasen behöver en container, och samma container-paket kan användas i Azure.

## Behörigheter och användare
Admin ska skapas automatiskt och äger sedan hela appen, detta då tanken var att ha appen live och endast bjuda in "test demo användare", de får en inbjudan via admin där admin CRUDAR deras konto.

## Larm Flödet
Förklaring av larmfödet
Användaren sparar en bevakning av valda parametrar, det sparas i db. Varje minut poolas från SL störningar och avgångsinformation. Det jämförs med sparade bevakningen och utverderas om den matchar så skickas det notis vidare till discord. Systemet förhindrar duplikat av larm.

## Appen i Docker, i Azure
Planerade att ha allt i en container sedan ha det i azure, men pga kostnader som tillkommer då appen städigt poolar förfrågningar (1/min) så har jag valt att inte ladda upp den i Azure.

## Vidare utveckling
Är nöjd med funktionaliteten men man kan lägga till att radera historiken, ha allt i en docker container så att man startar med ett kommando allt mm.

