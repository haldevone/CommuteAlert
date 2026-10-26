# Projektdagbok – CommunAlert



## Vecka 1 – 

**Veckans mål** projektplan klar, repo och struktur, riktiga svar från SL sparade, SL-klient med modeller, första datamodellen.

- ## dag 1, vecka 1

    Bollade Ideer kring vilket projekt som jag skulle starta med tidsplanen som fokus. Gick vidare med "Pendlingslarm projektet". Sammanfattade funktion och innehåll. Gick igenom vad för dokumentation som krävdes, gjorde upp plan kring projektet, skapade Projektplan, Beslutslogg, ProjektDagbok, TestPlan och ReadME. Sammanfattade samtliga dokument med innehåll.

- ## dag 2, vecka 1
    Jobbade vidare med dokumentationen av ovannämnda. Gjorde färdigt ett grundprojekt som består av CommunAlert.Web och CommunAlert.Tests med boilerplate Identity.

- ## dag 3 vecka 1
    Laddade upp allt på github ny repo. Testade testData json fil från SL Departures & Deviations. Skapade data modeller för Sl efter json datan som jag fick från Departures/Deviations. 2 nya test filer som testar läsa datan från deviations & departures.json.

- ## dag 4 vecka 1
    Utvärderade och skapade datamodeller för applikationen, ALert, AlertType, Watch och WatchDays. Byggde Regler som är en service form utan interaktion med extern API som filtrerar relevanta avgångar, skippar inställda, håller sig till angivna gränsvärden hittar berörda aktiva trafikstörningar och retunerar i resultat, AlertRules & AlertSuggestions. 

- ## dag 5 vecka 1
    Byggde AlertService som hämtar data från SL jämför med skapade bevakningar och retunerar berörda varningar. Skapade enhetstester i AlertRulesTest.

- ## dag 6 vecka 1
    Skapade WebhookValidator för att skydda manipulering mot URLen, och webhookProtecor för att kryptera den. Discordnotifier används för att skicka meddelande innehålet som JSON till discord (webhookUrlen). AlertSender med funktionen att hämta oskickade alerts från db och skicka om de till discord.
    

## Vecka 2 – 

**Veckans mål:** inloggning, bevakningar, regelmotor, dubblettskydd, Discord-notis, schemaläggning, enhetstester. 

- ## dag 1 vecka 2
    Skapade AlertScheduler som automatiskt kör kontrollen (poolar varje minut) av SL och skickar alerts med jämna mellanrum.

- ## dag 2 vecka 2
    Skapade tester som kontrollerar discord webhook adressen.

- ## dag 3 vecka 2
    Slutförde alla tester, SLAPIService, AlertService, Rules, SlModels.

- ## dag 4 vecka 2
    Skapade SlsiteService som hämtar och cachar data från SL om hållplatser, samt sök funktioner. WatchesCOntroller, sökfunktion filtrering av bevakningar/hållplatser valbara alternativ, getCreate och Post Create bevakning samt Delete.

- ## dag 5 vecka 2
    Ny Vy Index, Create och Search, Index listar skapade bevakningar som man kan ta bort med knapp för Sök, Sök listar sökta stationer som blir klickbara och väljer man de så kommer man till Create där man får skapa sin bevakning.

- ## dag 6 vecka 2
    Hämtning av hållplatserna hämtas separat vid 60s, fel vid enstaka hållplats loggas. AlertsController, listar de historiska utskick som skett

- ## dag 7 vecka 2
    Implementerade svenska och funktioner som översätter olika delan, har ändrat tillbaka till engelska. Jobbade med CSS och stylat om hela appen. La till en Home sektion som listar senaste 3 larmer samt antal sparade bevakningar.


## Vecka 3 – 

**Veckans mål:** felhantering, krypterad webhook, loggning, registrering av användare & säkerhet, azure deployment

- ## dag 1 vecka 3
    Byggde om att admin skapas vid början av applikationen. Admin ger tillgång till andra användare via privat användaruppgits invitation.

## Vecka 4 – 

**Veckans mål:** README, Docker, GitHub Actions, testrapport, projektrapport, förberedelse inför redovisning.


