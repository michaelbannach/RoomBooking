## Beispiel: Neuen Raum (Room) anlegen

### Anfrage (POST /api/Room)

```
{
  "name": "Raum 2",
  "capacity": 25
}
```

**Antwort**
Status: `200 OK`


### Anfrage (GET /api/Room)

Response body
```
[
  {
    "id": 1,
    "name": "Raum 1",
    "capacity": 12
  },
  {
    "id": 2,
    "name": "Raum 2",
    "capacity": 25
  }
]
```

Antwort
Status: `200 OK`


## Fehlerfall: Raumname bereits vergeben

### Anfrage (POST /api/Room)

```
{
  "name": "Raum 2",
  "capacity": 20
}
```

**Antwort**
Status: `400 Bad Request` (Raumname ist bereits vergeben)


## Fehlerfall: Capacity kleiner gleich null

### Anfrage (POST /api/Room)

```
{
  "name": "Raum 3",
  "capacity": -20
}
```

Antwort
Status: `400 Bad Request` (Capacity darf nicht kleiner gleich null sein)


## Fehlerfall: Uvollständiger Post

### Anfrge (POST /api/Room)

```
{                           
  "name": "Raum 4",
  "capacity": 
}
```
und
```
{
  
"capacity": 20
  
}
```

**Antwort**
Status `400 Bad Request`


## Beispiel: Raum aktualisieren

### Anfrage (PUT /api/Room)

Request: Ursprünglich
```
  {
    "id": 6,
    "name": "Raum 3",
    "capacity": 30
  }
```
Request: Aktualisiert
```
{
  "id": 6,
  "name": "Raum 3",
  "capacity": 25
}
```
**Antwort**
Status `200 OK`


## Fehlerfall: Neuer Name bereits vergeben

### Anfrage (PUT /api/Room)

Request: Ursprünglich
```
{
  "id": 6,
  "name": "Raum 3",
  "capacity": 25
}
```

Request: Aktualisiert
```
{
  "id": 6,
  "name": "Raum 2",
  "capacity": 25
}
```

**Antwort**
Status `400 Bad Request` (Raumname ist bereits vergeben)


## Fehlerfall: Neue Kapazität kleiner-gleich null

### Anfrage (PUT /api/Room)
Request: Ursprünglich
```
{
  "id": 6,
  "name": "Raum 3",
  "capacity": 25
}
```

Request: Aktualisiert
```
{
  "id": 6,
  "name": "Raum 3",
  "capacity": -25
}
```

**Antwort**
Status `400 Bad Request` (Capacity darf nicht kleiner gleich null sein)


