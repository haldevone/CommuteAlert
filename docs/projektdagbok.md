# Projektdagbok – CommunAlert



## Vecka 1 – Grund och undersökning

**Veckans mål** projektplan klar, repo och struktur, riktiga svar från SL sparade, SL-klient med modeller, första datamodellen.

- ## dag 1, vecka 1

    Bollade Ideer kring vilket projekt som jag skulle starta med tidsplanen som fokus. Gick vidare med "Pendlingslarm projektet". Sammanfattade funktion och innehåll. Gick igenom vad för dokumentation som krävdes, gjorde upp plan kring projektet, skapade Projektplan, Beslutslogg, ProjektDagbok, TestPlan och ReadME. Sammanfattade samtliga dokument med innehåll.

- ## dag 2, vecka 1
    Jobbade vidare med dokumentationen av ovannämnda. Gjorde färdigt ett grundprojekt som består av CommunAlert.Web och CommunAlert.Tests med boilerplate Identity.

- ## dag 3 vecka 1
    Laddade upp allt på github ny repo. Testade testData json fil från SL Departures & Deviations. Skapade data modeller för Sl efter json datan som jag fick från Departures/Deviations. 2 nya test filer som testar läsa datan från deviations & departures.json.

- ## dag 4 vecka 1
    Utvärderade och skapade datamodeller för applikationen, ALert, AlertType, Watch och WatchDays. Byggde Regler som är en service form utan interaktion med extern API som filtrerar relevanta avgångar, skippar inställda, håller sig till angivna gränsvärden hittar berörda aktiva trafikstörningar och retunerar i resultat, AlertRules & AlertSuggestions. Skapade enhetstester i AlertRulesTest

## Vecka 2 – Kärnan

**Veckans mål:** inloggning, bevakningar, regelmotor, dubblettskydd, Discord-notis, schemaläggning, enhetstester. Milstolpe: fungerande flöde från SL till Discord.



## Vecka 3 – Kvalitet och säkerhet

**Veckans mål:** felhantering, krypterad webhook, loggning, simuleringsläge, systematiska tester.



## Vecka 4 – Avslut

**Veckans mål:** README, Docker, GitHub Actions, testrapport, projektrapport, förberedelse inför redovisning.


