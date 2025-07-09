# RoomBookingC

**Web-App zur Raum- und Arbeitsplatzbuchung**  
Entwickelt mit ASP.NET Core, Entity Framework Core und Docker.

---

## Screenshots

![RoomBooking](Raumbuchung_Übersicht.png)
![Roombooking](MiniKalendar_Detail.PNG)
![Roombooking](Buchungsfenster_Detail.png)

## Funktionen

- Räume tageweise im **30-Minuten-Takt** buchen
- **Konflikterkennung** & Validierung bei Buchungen
- **Eigene Buchungen stornieren**
- Übersichtliche **Kalenderansicht** (07:00–19:00 Uhr)
- **Automatisches Ausgrauen** vergangener Zeitfenster
- Persistente Speicherung via **SQL Server & EF Core Code-First**

---

##  Tech Stack

- **Frontend:** HTML, CSS, JavaScript (Vanilla)
- **Backend:** ASP.NET Core 8 (Web API), Entity Framework Core
- **Datenbank:** SQL Server (Docker-Container)
- **Containerisierung:** Docker, Docker Compose

---

##  Setup (lokal)

```bash
git clone https://github.com/michaelbannach/roombooking.git
cd roombooking
docker-compose up --build
