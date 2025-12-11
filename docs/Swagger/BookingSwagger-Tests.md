## Hinweis zur Testdaten-Erstellung

- Im Projektordner `RoomBooking.Infrastructure/Seeding` befindet sich die Klasse `DevelopmentSeeder`.
- Diese legt beim Start im Entwicklungsmodus automatisch einen Beispiel-Raum und einen Test-Employee für alle API-Testfälle an.
- Die benötigte GUID für die Beispielanfragen muss aus der eigenen Datenbank entnommen werden


## Beispiel: Neue Buchung (Booking) anlegen

### Anfrage (POST /api/Booking)

```
{
"startDate": "2025-11-23T10:00:00Z",
"endDate": "2025-11-23T12:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `200 OK` (Buchung wurde erfolgreich angelegt)


## Fehlerfall: Buchung besteht bereits

### Anfrage (POST /api/Booking)

```
{
"startDate": "2025-11-23T10:00:00Z",
"endDate": "2025-11-23T12:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request` (Buchung besteht bereits)


## Fehlerfall: EndTime der Buchung vor StartTime

### Anfrage (POST /api/Booking)

```
{
"startDate": "2025-11-23T14:00:00Z",
"endDate": "2025-11-23T12:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request` (EndTime früher oder gleich StartTime)


## Fehlerfall: Buchungen überschneiden sich

### Anfrage (POST /api/Booking)

```
{
"startDate": "2025-11-23T09:45:00Z",
"endDate": "2025-11-23T10:45:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request` (Buchung besteht berreits)





## Fehlerfall: EndTime identisch mit StarTime der Buchung

### Anfrage (POST /api/Booking)

```
{
"startDate": "2025-11-23T14:00:00Z",
"endDate": "2025-11-23T14:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request` (EndTime früher oder gleich StartTime)


## Fehlerfall: StarTime in der Vergangenheit (Requestdatum: 22.11.25)

### Anfrage (POST /api/Booking)

```
{
"startDate": "2025-11-21T14:00:00Z",
"endDate": "2025-11-23T14:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request` (StartTime in der Vergangenheit)


## Beispiel: Buchung löschen

### Anfrage (DELETE /api/Booking/{id})

Pfad-Parameter: `id` = 3

**Antwort**
Status: `200 OK` (Buchung gelöscht)


## Fehlerfall: BookingId nich bekannt

Pfad-Parameter: `id` = 4

**Anwort**
Status: `404 Not Found` (BookingId nicht vorhanden)


## Fehlerfall: Ungültige BookingId

Pfad-Parameter: `id` = -1

**Antwort**
Status: `400 Bad Request` (Ungültige Id)


## Beispiel: Buchung ändern

### Anfrage (PUT /api/Booking/{id})

Pfad-Parameter: `id` = 10

Request: Ursprüngliche Buchung
```
{
"startDate": "2025-11-23T08:00:00Z",
"endDate": "2025-11-23T09:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

Request: Aktualisierte Buchung
```
{
"startDate": "2025-11-23T07:00:00Z",
"endDate": "2025-11-23T09:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `200 OK` (Aktualisierung erfolgreich)

## Fehlerfall: StartTime in der Vergangenheit

Request: Ursprüngliche Buchung
```
{
"startDate": "2025-11-23T07:00:00Z",
"endDate": "2025-11-23T09:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

Request: Aktualisierte Buchung
```
{
"id": 10,
"startDate": "2025-11-20T07:00:00",
"endDate": "2025-11-23T09:00:00",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request` (StartTime ist in der Vergangenheit)


## Fehlerfall: EndTime ist vor StartTime

Request: Ursprüngliche Buchung
```
{
"startDate": "2025-11-23T07:00:00Z",
"endDate": "2025-11-23T09:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

Request: Aktualisierte Buchung
```
{
"startDate": "2025-11-23T07:00:00Z",
"endDate": "2025-11-23T06:00:00Z",
"employeeId": "0129c4f2-33ca-4337-adc3-b4c21ac27f26",
"roomId": 1
}
```

**Antwort**
Status: `400 Bad Request ` (EndTime ist vor StartTime)