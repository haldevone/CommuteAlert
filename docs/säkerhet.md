# Säkerhet

## Utgångspunkter

Applikationen körs lokalt: webbappen med `dotnet run` och databasen i Docker. Den är ändå byggd som om den skulle kunna nås av andra, eftersom säkerheten ska gå att granska och eftersom koden kan komma att köras på en server. Det som ska skyddas är användarkonton, användarnas Discord-webhook-adresser, anropsutrymmet mot SL:s API och hemligheter i repot. De största riskerna är att någon utan tillstånd får tillgång till appen, att lösenord och webhook-adresser läcker, och att servern används för att skicka anrop till adresser som användaren själv kan välja.

## Åtgärder som är genomförda

| Område | Hot | Åtgärd |
|---|---|---|
| Registrering | Obehöriga skapar konton och använder resurser | Självregistrering är stängd (sidan ger 404). Endast administratören skapar konton och ger ett tillfälligt lösenord som användaren byter

| Inloggning | Gissning av lösenord | Lösenord hashas av ASP.NET Core Identity, minst 12 tecken. Max 8 anrop per minut och IP-adress mot inloggningssidorna

| Åtkomst | Användare når andras data | `[Authorize]` på alla sidor utom startsidan. Bevakningar, larm och webhook knyts till användarens id, och ägaren kontrolleras vid ändring och borttagning

| Adminfunktioner | Vanlig användare skapar eller tar bort konton | Särskild regel ("Admin") där administratören identifieras via e-postadress i konfigurationen

| Formulär | Förfalskade förfrågningar (CSRF) | Antiforgery-token på alla formulär som ändrar data

| Webhook-adresser | Läckt databas avslöjar adresserna | Krypteras med ASP.NET Core Data Protection. Adressen visas aldrig efter att den sparats och skrivs aldrig i loggen

| Webhook-adresser | Servern anropar godtycklig adress (SSRF) | Endast https-adresser på discord.com med sökvägen /api/webhooks/, utan port, användarinformation eller frågesträng

| Meddelanden | Text från SL pingar alla i Discord (@everyone) | Pingar stängs av i varje anrop (`allowed_mentions`)

| Indata | SQL-injektion, skadligt skript i sidan (XSS) | Parameteriserade frågor via Entity Framework Core, kodning av all utdata i Razor, validering på serversidan, högst sex bevakningar per användare

| Extern data | Opålitlig text från SL | Behandlas som ren text, begränsas till 480 tecken och kodas vid visning

| Loggning | Hemligheter i loggar | Endast feltyper och id loggas. Loggning av HTTP-anrop är satt till varning, så att webhook-adressen inte skrivs ut

| Externa tjänster | Överbelastning, blockering av SL, krasch vid fel | En hämtning per unik hållplats och omgång, intervall 61 sekunder, timeout 10 sekunder, fel hanteras per hållplats. Osända larm försöks igen i 30 minuter

| Dubbla larm | Samma händelse skickas flera gånger | Unik nyckel per bevakning och händelse, både i koden och som unikt index i databasen

| Personuppgifter | Onödig lagring | Endast e-post, webhook, bevakningar och larm sparas. Tas ett konto bort försvinner all dess data

| Krypteringsnycklar | Sparade webhook-adresser går inte att läsa efter omstart | Nycklarna sparas i databasen, så att adresserna går att läsa även när appen startas om

| Hemligheter i repot | Lösenord eller webhook-adresser läcker via git | `.env` ignoreras av git och en mall (`.env.example`) finns i repot. Anslutningssträng och adminuppgifter ligger i användarhemligheter, inte i `appsettings.json`

| Transport | Okrypterad trafik | Appen körs med https och utvecklingscertifikat lokalt (HSTS används i produktionsläge)


## Hemligheter
- Databaslösenord: `.env` (ej i git). Mall i `.env.example`.
- Anslutningssträng, admin-e-post och admin-lösenord: användarhemligheter (.NET user secrets), lagras utanför projektmappen.
- Krypteringsnycklar: i databasen.
- Webhook-adresser: krypterade i databasen.

## Begränsningar och kända brister
- Det finns ingen e-postverifiering eller återställning av lösenord. Administratören tar bort och skapar om kontot vid behov.
- Krypteringsnycklarna ligger i samma databas som de krypterade webhook-adresserna. Den som läser hela databasen kan alltså dekryptera dem. Key Vault skulle lösa det.
- Appen ansluter till databasen som serveradministratör (`sa`) och inte som en användare med begränsade rättigheter.
- Det finns bara en administratör (via e-postadress i konfigurationen), ingen tvåfaktorsinloggning och ingen logg över administratörens handlingar.