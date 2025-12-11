## Beispiel: Employee-Login

### Anfrage (POST /api/auth/login)
```
{
  "email": "user@example.com",
  "password": "TestPasswort1$"
}
```
**Antwort**
Status: `200 OK` (Logged In)

## Fehlerfall: Login mit falscher Email

### Anfrage (POST /api/auth/login)
```
{
  "email": "user11@example.com",
  "password": "TestPasswort1$"
}
```
**Antwort**
Status: `401 Unauthorized` (Ungültige Email oder Passwort)


## Fehlerfall: Login mit falschem Passwort

### Anfrage (POST /api/auth/login)
```
"email": "user@example.com",
"password": "TestPasswort1111$
```
**Antwort**
Status: `401 Unauthorized` (Ungültige Email oder Passwort)


## Beispiel: Employee-Logout

### Anfrage (POST /api/auth/logout)

Execute - No Parameters

**Antwort**
Status: `200 OK` (Logged Out)


