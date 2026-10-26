# Testplan och testrapport – CommuteAlert


## 1. Syfte
Visa att systemet fungerar på en grundläggande nivå och hanterar fel och riskfall, och visa vad som testats, hur och med vilket resultat.

## 2. Testnivåer

Enhetstester - Regelmotor och logik i Core, utan databas och nätverk
Integrationstester - Flödet bevakning , larm sparat, mot testdatabas, hantering av fel och ogiltig data
Manuella tester -  Riktiga SL och Discord, gränssnittet,


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
9. Händelse utanför bevakningsföntret . Inget larm - OK
10. Avgångar från SL kan avläsas - läses korrekt - OK
11. Störningar från SL kan avläsas - läses korrekt - OK
12. Webhook-adress med rätt värd - Godkänns - OK
13. Webhook-adress med fel värd, 10 olika tester - Avvisas - OK
14. SLAPIService anropar rätt service och svar kan läsas - JSON objekt med rätt data - OK



## 4. Sammanfattning
- Antal testfall: [14]
- Godkända: [14] | Fel: [ ] | Ej körda: [0]
- Datum för senaste körning: [2026-10-15]




