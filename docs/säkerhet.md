# Säkerhet

## Utgångspunkter
Applikationen ska ligga på internet. Det som ska skyddas är användarkonton, användarnas Discord-webhook-adresser, anropsutrymmet mot SL:s API och kostnaden i Azure. De största riskerna är obehöriga användare, att hemligheter läcker och att servern missbrukas för att anropa andra adresser.

## Åtgärder

| Område | Hot | Åtgärd |
|---|---|---|---|

| Registrering | Obehöriga skapar konton och använder resurser | Självregistrering är stängd (sidan ger 404). Endast administratören skapar konton och ger ett tillfälligt lösenord som användaren byter

| Inloggning | Gissning av lösenord | Lösenord hashade av ASP.NET Core Identity. Max 8 anrop per minut och IP-adress mot inloggningssidorna

| Åtkomst | Användare når andras data | `[Authorize]` på alla sidor utom startsidan. Bevakningar, larm och webhook knyts till användarens id, och ägaren kontrolleras vid ändring och borttagning

| Adminfunktioner | Vanlig användare skapar eller tar bort konton | Särskild regel ("Admin") där administratören identifieras via e-post i konfigurationen

| Formulär | Förfalskade förfrågningar (CSRF) | Antiforgery-token på alla formulär som ändrar data

| Webhook-adresser | Läckt databas avslöjar adresserna | Krypterade med ASP.NET Core Data Protection. Adressen visas aldrig efter att den sparats och skrivs aldrig i loggen

| Webhook-adresser | Servern anropar godtycklig adress (SSRF) | Endast https-adresser på discord.com med sökvägen /api/webhooks/, utan port, användarinformation eller frågesträng

| Meddelanden | Text från SL pingar alla i Discord (@everyone) | Pingar stängs av i varje anrop (`allowed_mentions`)

| Indata | SQL-injektion, skadlig skript i sidan (XSS) | Parameteriserade frågor via Entity Framework Core, kodning av all utdata i Razor, validering på serversidan, högst sex bevakningar per användare

| Extern data | Opålitlig text från SL | Behandlas som ren text, begränsas till 480 tecken och kodas vid visning

| Loggning | Hemligheter i loggar | Endast feltyper och id loggas. Loggning av HTTP-anrop är satt till varning, så att webhook-adressen inte skrivs ut

| Externa tjänster | Överbelastning, blockering av SL, krasch vid fel | En hämtning per unik hållplats och omgång, intervall 61 sekunder, timeout 10 sekunder, fel hanteras per hållplats. Osända larm försöks igen i 30 minuter

| Dubbla larm | Samma händelse skickas flera gånger | Unik nyckel per bevakning och händelse, både i koden och som unikt index i databasen

| Personuppgifter | Onödig lagring | Endast e-post, webhook, bevakningar och larm sparas. Tas ett konto bort försvinner all dess data

| Krypteringsnycklar | Nycklar försvinner vid omstart i container | Nycklarna sparas i databasen

| Transport | Okrypterad trafik, fel klientadress bakom proxy | HTTPS och HSTS, `ASPNETCORE_FORWARDEDHEADERS_ENABLED` i Azure

| Databas i Azure | Öppen databas | Brandvägg som släpper in bara appen, egen databasanvändare med minsta nödvändiga rättigheter


## Hemligheter
- Anslutningssträng, admin-e-post och admin-lösenord: användarhemligheter lokalt, appinställningar i Azure.
- Krypteringsnycklar: i databasen.
- Webhook-adresser: krypterade i databasen.

## Val som inte gjorts
- JWT används inte. Cookie-inloggningen i Identity (HttpOnly, SameSite) passar en vanlig serverrenderad webbapp bättre. JWT hör hemma i API:er för andra klienter.
- Azure Key Vault används inte i första versionen. Hemligheterna ligger i appinställningar, och Key Vault är en möjlig förbättring.

## Begränsningar och kända brister
- Kontolåsning efter felaktiga inloggningar är inte aktiv, eftersom det färdiga inloggningsbiblioteket inte räknar misslyckade försök. Det ersätts av begränsningen av antal anrop per minut.
- Det finns ingen e-postverifiering eller återställning av lösenord. Administratören tar bort och skapar om kontot vid behov.
- Krypteringsnycklarna ligger i samma databas som de krypterade webhook-adresserna. Den som läser hela databasen kan alltså dekryptera dem. Key Vault skulle lösa det.
- Inga säkerhetsrubriker eller innehållspolicy (CSP), och ikonerna hämtas från en CDN utan integritetskontroll.
- Det finns bara en administratör (via e-postadress i konfigurationen), ingen tvåfaktorsinloggning och ingen logg över administratörens handlingar.
- Discords begränsning av antal anrop (felkod 429) hanteras genom nytt försök i nästa omgång, men appen väntar inte den tid Discord anger.

## Kontroller
- Öppna registreringssidan direkt: ska ge 404.
- Logga in som vanlig användare och försök öppna adminsidan: ska nekas.
- Logga in som användare A och försök ta bort användare B:s bevakning: ska ge "hittades inte".
- Skicka många inloggningsförsök snabbt: ska ge felkod 429.
- `git log --all -S"Password="` och sökning efter `discord.com/api/webhooks` i historiken: inga riktiga hemligheter.