# Projectdoel – JudoTV Mobile App

## 1. Doel van de applicatie
De JudoTV-app is bedoeld om judoka’s, trainers en fans een centrale plaats te bieden voor het raadplegen van wedstrijdinformatie en het beheren van hun persoonlijke trainingsagenda’s en het registreren van technische prestaties.  
De app richt zich op twee hoofddoelgroepen:
- *Judoka’s* die hun trainingsschema willen beheren en hun technische vooruitgang willen bijhouden via grafische interactie 
- *Fans en volgers* die op de hoogte willen blijven van wedstrijden, atleten en rankings.

Met deze app kunnen gebruikers:
- Toegang krijgen tot actuele wedstrijdinformatie (events, atleten per gewichtsklasse).
- Eigen trainingen plannen en details registreren (type training, technieken en scores). hierin kunnen ze ook tekst invoeren via spraakberichten.
- Foto’s en video’s toevoegen aan trainingsmomenten voor analyse en feedback.
- Inloggen om persoonlijke gegevens te beschermen en enkel eigen data te zien.

---

## 2. Online strategie
De online strategie is gericht op *community building & personal engagement*:
- *Community building*: De app centraliseert alle wedstrijd- en atleetinformatie, waardoor gebruikers op een laagdrempelige manier toegang hebben tot actuele data.
- *Personal engagement*: Via de agenda- en trainingsregistratie kunnen gebruikers hun eigen progressie opvolgen. Door het uploaden van foto’s en video’s wordt de beleving persoonlijker en kan feedback van trainers makkelijker gedeeld worden.
- *Multiplatform bereik*: Door gebruik te maken van .NET MAUI is de app beschikbaar op Android en (indien gewenst) desktop, met één codebase.

---

## 3. Mobile features

### 3.1 Authenticatie en autorisatie
- *Waarom*: Nodig om persoonlijke trainingsdata te beveiligen en te zorgen dat elke gebruiker enkel zijn eigen gegevens kan inzien.
- *Uitwerking*: Loginpagina met JWT-authenticatie, koppeling naar backend voor validatie, tokenopslag in SecureStorage.

### 3.2 Foto/video toevoegen aan trainingen
- *Waarom*: Visuele feedback is belangrijk in judo om techniek te analyseren en te verbeteren en het herbekijken van wedstrijden tegen een bepaalde tegenstander
- *Uitwerking*: Integratie met MediaPicker (platformintegratie) voor het maken of uploaden van foto’s/video’s bij een trainingsmoment.

### 3.3 Agendabeheer & techniekregistratie
- *Waarom*: Judoka’s moeten trainingsmomenten plannen en bijhouden wat ze hebben gedaan voor meer vooruitgang te boeken.
- *Uitwerking*:
  - Agenda-overzicht waarin trainingen per dag zichtbaar zijn.
  - Toevoegen van trainingstype: randori, krachttraining of techniektraining.
  - Bij randori-trainingen kan de gebruiker technieken selecteren (uchi mata, seoi nage, uki goshi) en scores bijhouden.

---

## 4. Wireframes (tekstuele beschrijving)
> Grafische wireframes worden in een later fases nog  bijgewerkt en upgeload.

### 4.1 Loginpagina
- Velden: e-mail, wachtwoord
- Knop: "Inloggen"
- Link: "Account aanmaken" (optioneel voor uitbreiding)

### 4.2 Homepagina
- Bovenste balk met navigatieknoppen:  Events, Athletes, Agenda.
- Sectie "Aankomende wedstrijden" in lijstweergave.

### 4.3 Athletespagina
- Zelfde bovenbalk als Homepagina.
- Lijst met gewichtsklassen.
- Per gewichtsklasse een lijst van 5 atleten met naam, land, ranking.

### 4.4 Agendapagina
- Maandoverzicht (kalenderweergave).
- Bij het aanklikken van een dag: lijst van trainingen.
- Knop "Training toevoegen" → dialoog met keuze type training en optionele foto/video.
- Bij randori-training → extra velden voor technieken + score per techniek.

---

## 5. Verwachte resultaten
- Een volledig functionerende cross-platform app die authenticeert via JWT.
- Agenda- en trainingsregistratie gekoppeld aan de ingelogde gebruiker.
- Upload van foto’s/video’s bij trainingsmomenten.
- Dynamische wedstrijd- en atleetinformatie vanuit de backend.
