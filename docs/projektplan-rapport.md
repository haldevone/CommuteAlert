# Projektplan - CommunAlert

Av: Enon H Hall




## 1. Bakgrund

Den som aktivt pendlar med SL och inte har tid att följa upp föresningar via deras platformer upptäcker oftast inställda avgångar försent.

Problemformulering: Hur kan en webbapplikation i .NET automatiskt bevaka en användares SL-resor och skicka ett larm till användarens Discord när en avgång är inställd eller försenad, eller när en störning påverkar vald linje – utan att skicka onödiga eller dubbla larm och utan att överskrida SL:s gräns för antal anrop?




## 2. Syfte och Mål

Ta fram en tjänst som kan integrera extern API, leverera nyttig information, hantera fel och skydda användarens uppgifter


Mål

* En användare kan skapa konto, logga in och ange sin egen Discord-webhook.
* Användaren kan skapa bevakningar genom att välja trafikslag, hållplats och linje, samt veckodagar och tider.
* Systemet hämtar avgångar och störningar från SL enligt schema och skickar larm vid inställd avgång, försening över ett valt gränsvärde, eller störning på vald linje.
* Samma händelse ger inte upprepade larm.
* Systemet klarar att SL:s API ligger nere eller ger ogiltig data utan att krascha.
* Projektet använder enhetstester, och viktiga flöden av integrationstester, med en redovisad testrapport.
* Hemligheter lagras och hanteras säkert.
* Projektet kan startas med hjälp av README.



## 3. Vald lösning

ASP.NET Core MVC.
SQL Server via EF Core. Lokalt i container.
Discord-webhook som notiskanal, bakom ett gränssnitt (INotifier) så att kanalen kan bytas.
Docker och docker-compose som tillägg, dotnet run som huvudväg i README.
GitHub Actions för bygge och tester.



## 4. Tidsplan

Projektet pågår i fyra veckor och delas upp ungefärligt enligt följande:

1. Projektplan klar. Repo, lösningsstruktur och dokument. Riktiga svar från SL. Datamodell.

2. Inloggning, bevakningar, regelmotor, dubblettskydd, Discord-notis, schemaläggning. Några enhetstester. Fungerande flöde från SL till Discord.

3. Felhantering, krypterad webhook med kontroll, loggning, simuleringsläge, enhetstester

4. README, Docker, GitHub Actions, slutlig testrapport, projektrapport, förberedelse inför redovisning. Länk till repo inlämnad i tid.



## 5. Testning

Detaljerna finns i testplan-och-testrapport.md.

* Enhetstester av regelmotorn (inställd, försenad, störning, tidsfönster, dubblettskydd).
* Integrationstester för flödet från bevakning till sparat larm, ogiltig data, tomt svar
* Manuella tester mot riktiga API:er och Discord

Varje testfall dokumenteras efter körning.



## 6. Säkerhet

Inloggning och åtkomst: varje användare kommer bara åt sina egna bevakningar.
Webhook-adresser: krypterade i databasen, kontrolleras mot Discords adress, visas inte i sin helhet.

Hemligheter utanför källkoden (användarhemligheter lokalt, miljövariabler vid körning).

Indata valideras. Skydd mot vanliga webbattacker via ramverkets inbyggda skydd.

Loggar innehåller inga hemligheter.

Gräns för hur ofta externa tjänster anropas.




## 7. Användning av AI

AI används som stöd, men jag ska alltid kunna förklara och försvara allt som lämnas in. Jag använder ingen kod som jag inte kan förklara rad för rad. All AI-genererad kod läses, körs och testas innan den används. Viktiga beslut tas av mig och motiveras i beslutsloggen.













