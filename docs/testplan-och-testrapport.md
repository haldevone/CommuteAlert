# Testplan och testrapport – CommuteAlert


## 1. Syfte
Visa att systemet fungerar på en grundläggande nivå och hanterar fel och riskfall, och visa vad som testats, hur och med vilket resultat.

## 2. Testnivåer

Enhetstester - Regelmotor och logik i Core, utan databas och nätverk
Integrationstester - Flödet bevakning , larm sparat, mot testdatabas, hantering av fel och ogiltig data
Manuella tester -  Riktiga SL och Discord, gränssnittet, simuleringsläge


## 3. Testfall

Resultat: **OK**, **Fel** eller **Ej körd**.

1. Avgång markerad som inställd - Larm skapas - Ej körd
2. Försening större än gränsvärdet - Larm skapas - Ej körd
3. Försening under gränsvärdet - Inget larm - Ej körd
4. Fel veckodag - Inget larm - Ej körd
5. Störning på vald linje - Larm skapas - Ej körd
6. Störning på annan linje - Inget larm - Ej körd
7. Samma händelse i två omgångar i rad - Ett enda larm - Ej körd
8. SL svarar inte (timeout) - Omförsök, ingen krasch, loggat - Ej körd
9. Tomt svar - Inga larm, loggat - Ej körd
10. Webhook-adress med fel värd - Avvisas - Ej körd
11. Läs webhook direkt i databasen - Inte i klartext - Ej körd
12. Ej inloggad användare öppnar inställningar - Skickas till inloggning - Ej körd
13. Skicka testmeddelande till riktig Discord - Meddelandet visas i kanalen - Ej körd



## 4. Sammanfattning
- Antal testfall: [13]
- Godkända: [ ] | Fel: [ ] | Ej körda: [ ]
- Datum för senaste körning: [ ]

## 5. Fel som hittades och åtgärder

| Fel | Hittades av testfall | Orsak | Åtgärd | Omtestat |



