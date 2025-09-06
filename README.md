# JudoTV – Mobile & WebAPI Project
## n Projectbeschrijving
JudoTV is een cross-platform project met een *.NET MAUI mobiele app* en een *ASP.NET Core WebAPI backend*.
Het doel is om een alles-in-één oplossing te bieden voor judoka’s, trainers en clubs: agenda, events, De applicatie combineert *persoonlijk trainingsbeheer* met *wereldwijde wedstrijdinformatie*, zodat ---
## Belangrijkste functies
- n *Agenda* – plan je trainingen, noteer technieken en voeg foto’s of opmerkingen toe.
- n *Events* – zie aankomende toernooien en evenementen wereldwijd.
- n *Athletes* – ontdek judoka’s per gewichtscategorie en bekijk hun details.
- n *Randori analyse* – voeg per tegenstander persoonlijke notities toe.
- n *Login* – via JWT en Google OAuth2.
---
##  Authenticatie & Login
De app gebruikt een combinatie van *JWT-authenticatie* en *Google Login*.
- *JWT: gebruiker logt in met email/wachtwoord ® backend geeft JWT-token ® opgeslagen in secure - **Google OAuth2*: OAuth2-flow via de backend (/auth/google). Gebruiker logt in met Google ® backend ### Belangrijke routes
- POST /auth/login ® JWT login
- GET /auth/google ® Google OAuth login
- GET /auth/me ® profiel van ingelogde gebruiker
---
##  Online Strategie
De online strategie is opgebouwd rond *mobile-first*:
- De *.NET MAUI app* is de primaire toegang tot de data (agenda, events, judoka’s).
- De *ASP.NET Core WebAPI* is generiek en kan later ook gebruikt worden door een webportaal of andere - Alle communicatie verloopt via *REST API’s* en is beveiligd met JWT.
- De backend is klaar voor *cloud hosting* (Azure, AWS, …).
---
## Structuur van het project
###  Mobile App
*Pages/*
- HomePage.xaml – startpagina met navigatie naar Events, Athletes en Agenda
- EventsPage.xaml – overzicht van aankomende events
- AthletesPage.xaml – lijst met judoka’s per categorie
- AthleteDetailPage.xaml – detailpagina per judoka met opmerkingen
- AgendaPage.xaml – persoonlijke agenda met trainingen
- AddTrainingPage.xaml – training toevoegen of bewerken
*ViewModels/*
- HomePageViewModel.cs
- EventsViewModel.cs
- AthletesViewModel.cs
- AthleteDetailViewModel.cs
- AgendaViewModel.cs
- AddTrainingViewModel.cs
*Models/*
- EventModel.cs
- JudokaModel.cs
- TrainingEntryModel.cs
- TechniqueScoreModel.cs
- OpponentNoteModel.cs
*Services/*
- EventService.cs
- JudokasService.cs
- TrainingService.cs
*Interfaces/*
- IEventService.cs
- IJudokaService.cs
- ITrainingService.cs
---
###  WebAPI
*Controllers/*
- EventsController.cs – aankomende events (/api/events/upcoming)
- JudokasController.cs – judoka’s per categorie (/api/judokas/by-category/{category})
- AuthController.cs – login via JWT en Google
*Entities/*
- Judoka.cs
- Event.cs
- TrainingEntry.cs
*Data/*
- ApplicationDbContext.cs – configuratie van database en DbSets
---
##  Databaseconfiguratie
De backend gebruikt *Entity Framework Core* met SQL Server of SQLite.
### Tabellen
- *Judokas*: Id, FullName, Country, Category
- *Events*: Id, Title, Location, Date
- *TrainingEntries*: Id, Date, Type, Comment, TechniqueScores, OpponentNotes
- *Users*: (optioneel) voor authenticatie en rollenbeheer
### Migraties uitvoeren
bash
dotnet ef migrations add InitialCreate
dotnet ef database update

---
##  Belangrijke API-routes
### Events
- GET /api/events/upcoming ® lijst van aankomende events
### Judokas
- GET /api/judokas/by-category/{category} ® lijst van judoka’s per categorie
### Trainingen
- GET /api/trainings/{userId} ® alle trainingen van gebruiker
- POST /api/trainings ® nieuwe training toevoegen
### Authenticatie
- POST /auth/login ® login met JWT
- GET /auth/google ® login via Google OAuth2
---
##  Setup & Installatie
1. *Clone repository*
bash
git clone https://github.com/username/judotv.git
cd judotv

2. *Backend (WebAPI) installeren*
bash
cd Mde.Project.WebApi
dotnet restore
dotnet run

3. *Mobile App (MAUI) installeren*
bash
cd Mde.Project.Mobile
dotnet restore
dotnet build
dotnet maui run

---
##  Gebruikte technologieën
- .NET 7
- MAUI – mobiele app (Android, iOS, Windows)
- ASP.NET Core WebAPI – backend
- Entity Framework Core – ORM
- SQLite / SQL Server – database
- JWT / OAuth2 – authenticatie
- CommunityToolkit.MVVM – MVVM-architectuur
- Dependency Injection – services en viewmodels
---
## Secrets & API Keys
Gebruik appsettings.json voor configuratie van JWT en Google OAuth.
*Voorbeeld appsettings.json*
json
{
"Jwt": {
"Key": "JOUW_SUPER_GEHEIME_KEY_HIER",
"Issuer": "https://localhost:7238",
"Audience": "https://localhost:7238"
},
"Authentication": {
"Google": {
"ClientId": "GOOGLE_CLIENT_ID_HIER",
"ClientSecret": "GOOGLE_CLIENT_SECRET_HIER"
}
},
"ConnectionStrings": {
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JudoTVDb;Trusted_Connection=True;"
}
}

---
## Wireframes (conceptueel)
- *HomePage*: navigatiebalk + events-overzicht
- *EventsPage*: lijst met alle aankomende wedstrijden
- *AthletesPage*: filter per gewichtscategorie + lijst van judoka’s
- *AthleteDetailPage*: detailinfo judoka + opmerkingen
- *AgendaPage*: trainingen-overzicht + selectie van items
- *AddTrainingPage*: toevoegen van training, technieken en opmerkingen
---
## Toekomstige uitbreidingen
- Upload van foto’s/video’s naar cloud (nu enkel lokaal in app).
- Real-time chat/discussie per judoka of event.
- Push-notificaties voor wedstrijden en geplande trainingen.
- Analytics van technieken (grafieken per judoka).
- Koppeling met sportfederatie API’s.
---
##  Bronnen
- [Google OAuth2](https://developers.google.com/identity/protocols/oauth2)
- [CommunityToolkit.MVVM](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [ASP.NET Core WebAPI](https://learn.microsoft.com/en-us/aspnet/core/web-api)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/
