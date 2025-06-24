 RoomBookingC

Web-App zur Raum- und Arbeitsplatzbuchung – entwickelt mit ASP.NET Core, Entity Framework und Docker.

 Funktionen

- Räume tageweise im 30-Minuten-Takt buchen
- Konflikterkennung & Validierung
- Eigene Buchungen stornieren
- Übersichtliche Kalenderansicht (07:00–19:00 Uhr)
- Automatisches Ausgrauen vergangener Zeiten
- SQL Server & EF Code-First-Migrations

## ️ Tech Stack

- Frontend: HTML, CSS, JavaScript (Vanilla)
- Backend: ASP.NET Core 8, WebAPI, EF Core
- DB: SQL Server (Docker)
- Container: Docker, docker-compose

##  Setup (lokal)

```bash
git clone https://github.com/dein-nutzername/roombooking.git
cd roombooking
docker-compose up --build