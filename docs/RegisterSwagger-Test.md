## Beispiel: Employee-Registrierung

### Anfrage (POST /api/User/register)
```
{
  "email": "user@example.com",
  "password": "TestPasswort1$"
}
```
**Antwort**
Status: `200 OK` (Employee registered)


## Fehlerfall: Registrierung mit falscher Email (kein @-Zeichen)

### Anfrage (PUT /api/User/register)
```
{
  "email": "userexample.com",
  "password": "TestPasswort1$"
}
```
**Antwort**
Status: `400 Bad Request`

Response body
```
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "The Email field is not a valid e-mail address."
    ]
  },
  "traceId": "00-9eb71c8c14881c69a3e09f1f7ab46ad8-65b39bb665d035ad-00"
}
```

## Fehlerfall: E-Mail zu lang

### Anfrage (PUT /api/User/register)
```
{
  "email": "sehrlangeeemailadresse.mehralsfuenfzigzeichen@example.com",
  "password": "TestPasswort1$"
}
```
**Antwort**
Status: `400 Bad Request`

Response body
```
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": [
      "The field Email must be a string or array type with a maximum length of '50'."
    ]
  },
  "traceId": "00-f1c9055c500f2a0c9d37d1e063fcfbd0-a7b53f61a6fc9f09-00"
}
```

## Fehlerfall: Passwort entspricht den Bestimmungen

### Anfrage (PUT /api/User/register)
```
{
  "email": "user1@example.com",
  "password": "TestPasswort"
}
```
**Antwort**
Status `400 Bad Request` 

Response body
```
{
  "errors": [
    "Passwords must have at least one non alphanumeric character.",
    "Passwords must have at least one digit ('0'-'9')."
  ]
}
```