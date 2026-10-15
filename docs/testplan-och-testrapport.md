# Testplan och testrapport – CommuteAlert


## 1. Syfte
Visa att systemet fungerar på en grundläggande nivå och hanterar fel och riskfall, och visa vad som testats, hur och med vilket resultat.

## 2. Testnivåer

Enhetstester - Regelmotor och logik i Core, utan databas och nätverk
Integrationstester - Flödet bevakning , larm sparat, mot testdatabas, hantering av fel och ogiltig data
Manuella tester -  Riktiga SL och Discord, gränssnittet, simuleringsläge


## 3. Testfall

Resultat: **OK**, **Fel** eller **Ej körd**.

1. Avgång markerad som inställd - Larm skapas - OK
2. Försening större än gränsvärdet - Larm skapas - OK
3. Försening under gränsvärdet - Inget larm - OK
4. Avgång utanför bevakningens tidsfönster - Inget Larm - OK
5. Fel veckodag - Inget larm - OK
6. Störning på vald linje - Larm skapas - OK
7. Störning på annan linje - Inget larm - OK
8. Samma händelse i två omgångar i rad - Ett enda larm - OK
9. SL svarar inte (timeout) - Omförsök, ingen krasch, loggat - Ej körd
10. Tomt svar - Inga larm, loggat - Ej körd
11. Webhook-adress med fel värd - Avvisas - Ej körd
12. Läs webhook direkt i databasen - Inte i klartext - Ej körd
13. Ej inloggad användare öppnar inställningar - Skickas till inloggning - Ej körd
14. Skicka testmeddelande till riktig Discord - Meddelandet visas i kanalen - Ej körd
15. Användare A försöker nå användare Bs Bevakning - Nekas - Ej Körd



## 4. Sammanfattning
- Antal testfall: [15]
- Godkända: [8] | Fel: [ ] | Ej körda: [7]
- Datum för senaste körning: [2026-10-15]

## 5. Fel som hittades och åtgärder

| Fel | Hittades av testfall | Orsak | Åtgärd | Omtestat |
