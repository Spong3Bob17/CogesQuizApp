# 🎯 Coges Quiz App

Sistema di quiz interattivo sviluppato in C# .NET con MongoDB, HTML, CSS e JavaScript.

## 📋 Indice

- [Descrizione](#descrizione)
- [Requisiti](#requisiti)
- [Installazione](#installazione)
- [Configurazione](#configurazione)
- [Esecuzione](#esecuzione)
- [Testing](#testing)
- [Struttura del Progetto](#struttura-del-progetto)
- [Tecnologie Utilizzate](#tecnologie-utilizzate)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Deployment](#deployment)

---

## 📖 Descrizione

**Coges Quiz App** è un'applicazione web completa per la gestione e l'esecuzione di quiz interattivi. Gli utenti possono:

- ✅ Inserire il proprio nome e selezionare un test
- ✅ Rispondere a domande con numero variabile di opzioni (2-n)
- ✅ Visualizzare una progress bar dinamica durante il test
- ✅ Vedere il risultato finale con punteggio
- ✅ Consultare la classifica dei risultati

### Caratteristiche Principali

- **Frontend moderno** con design responsive
- **Backend robusto** con architettura object-oriented
- **Tracking completo** di ogni risposta nel database
- **Progress bar dinamica** durante l'esecuzione del test
- **Validazione input** con messaggi di errore chiari
- **Indici MongoDB** ottimizzati per performance
- **Unit testing completo** con NUnit, Moq e FluentAssertions

---

## 🔧 Requisiti

### Software Necessario

- **.NET SDK 9.0** o superiore
- **MongoDB 4.4** o superiore
- **Browser moderno** (Chrome, Firefox, Edge, Safari)

### Opzionale

- **JetBrains Rider** o **Visual Studio 2022** (per sviluppo)
- **MongoDB Compass** (per gestione database visuale)

---

## 📦 Installazione

### 1. Clona il Repository

```bash
git clone https://github.com/your-username/CogesQuizApp.git
cd CogesQuizApp
```

### 2. Installa MongoDB

#### Windows:
```bash
# Scarica da: https://www.mongodb.com/try/download/community
# Installa e avvia il servizio
net start MongoDB
```

#### macOS:
```bash
brew tap mongodb/brew
brew install mongodb-community
brew services start mongodb-community
```

#### Linux (Ubuntu):
```bash
sudo apt-get install -y mongodb
sudo systemctl start mongodb
sudo systemctl enable mongodb
```

### 3. Verifica MongoDB

```bash
# Verifica che MongoDB sia in esecuzione
mongod --version
mongo --eval "db.version()"
```

### 4. Ripristina i Pacchetti NuGet

```bash
dotnet restore
```

### 5. Compila il Progetto

```bash
dotnet build
```

---

## ⚙️ Configurazione

### Configurazione Database

Il progetto è configurato per connettersi a MongoDB in locale:

- **Connection String**: `mongodb://localhost:27017`
- **Database Name**: `CogesQuizDB`

Per modificare la configurazione, apri `CogesQuizApp/Program.cs`:

```csharp
string connectionString = "mongodb://localhost:27017";
string databaseName = "CogesQuizDB";
```

### Popolare il Database

Esegui lo script per inserire dati di esempio:

```bash
# Dalla root del progetto
mongo CogesQuizDB < Scripts/seed-database.js
```

Oppure usa MongoDB Compass per importare i dati manualmente.

---

## 🚀 Esecuzione

### Avvia l'Applicazione

```bash
cd CogesQuizApp
dotnet run
```

L'applicazione sarà disponibile su: **http://localhost:8080**

### Accedi all'Applicazione

1. Apri il browser e vai su **http://localhost:8080**
2. Inserisci il tuo nome
3. Seleziona un test dal menu a tendina
4. Clicca su "Inizia il Quiz"
5. Rispondi alle domande
6. Visualizza il tuo risultato e la classifica

---

## 🧪 Testing

### Esegui Tutti i Test

```bash
dotnet test
```

### Esegui Solo Unit Tests (no MongoDB richiesto)

```bash
dotnet test --filter TestCategory!=Integration
```

### Esegui Solo Integration Tests (MongoDB richiesto)

```bash
dotnet test --filter TestCategory=Integration
```

### Test Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

Per maggiori dettagli sui test, consulta [CogesQuizApp.Tests/README_TESTS.md](CogesQuizApp.Tests/README_TESTS.md)

---

## 📁 Struttura del Progetto

```
CogesQuizApp/
├── CogesQuizApp/                   # Progetto principale
│   ├── Controllers/                # Controller per gestire le richieste HTTP
│   │   ├── TestController.cs
│   │   ├── ResultController.cs
│   │   └── UserAnswerController.cs
│   ├── Models/                     # Modelli di dominio
│   │   ├── Test.cs
│   │   ├── Result.cs
│   │   └── UserAnswer.cs
│   ├── Services/                   # Servizi business logic
│   │   ├── IDatabaseService.cs
│   │   └── DatabaseService.cs
│   ├── wwwroot/                    # File statici (HTML, CSS, JS)
│   │   ├── index.html
│   │   ├── quiz.html
│   │   ├── results.html
│   │   └── style.css
│   ├── Program.cs                  # Entry point dell'applicazione
│   └── CogesQuizApp.csproj
├── CogesQuizApp.Tests/             # Progetto di test
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Helpers/
│   └── CogesQuizApp.Tests.csproj
├── Scripts/                        # Script per database
│   └── seed-database.js
├── README.md                       # Questo file
└── CogesQuizApp.sln               # Solution file
```

---

## 🛠️ Tecnologie Utilizzate

### Backend
- **C# .NET 9.0** - Framework principale
- **MongoDB.Driver 3.5.0** - Driver per MongoDB
- **HttpListener** - Server HTTP integrato

### Frontend
- **HTML5** - Struttura pagine
- **CSS3** - Styling con gradients e animazioni
- **JavaScript (Vanilla)** - Logica client-side
- **Fetch API** - Comunicazione con backend

### Testing
- **NUnit 4.4.0** - Framework di testing
- **Moq 4.20.70** - Mocking library
- **FluentAssertions 6.12.0** - Assertions leggibili

### Database
- **MongoDB 4.4+** - Database NoSQL
- **Indici** ottimizzati per query veloci

---

## 🌐 API Endpoints

### Tests

| Metodo | Endpoint | Descrizione |
|--------|----------|-------------|
| GET | `/tests` | Recupera tutti i test disponibili |

**Esempio Response:**
```json
[
  {
    "Id": "507f1f77bcf86cd799439011",
    "Title": "Test di Matematica",
    "Questions": [...]
  }
]
```

### Results

| Metodo | Endpoint | Descrizione |
|--------|----------|-------------|
| GET | `/results` | Recupera tutti i risultati ordinati per data |
| POST | `/results` | Salva un nuovo risultato |

**POST Request Body:**
```json
{
  "Username": "Mario",
  "TestId": "507f1f77bcf86cd799439011",
  "TestTitle": "Test di Matematica",
  "Score": "8/10",
  "CorrectAnswers": 8,
  "TotalQuestions": 10,
  "SessionId": "session_123456"
}
```

### User Answers

| Metodo | Endpoint | Descrizione |
|--------|----------|-------------|
| POST | `/user-answers` | Salva una risposta singola |
| GET | `/user-answers/session/{id}` | Recupera risposte per sessione |

**POST Request Body:**
```json
{
  "Username": "Mario",
  "TestId": "507f1f77bcf86cd799439011",
  "QuestionIndex": 0,
  "QuestionText": "Quanto fa 2+2?",
  "SelectedAnswerIndex": 1,
  "SelectedAnswerText": "4",
  "CorrectAnswerIndex": 1,
  "IsCorrect": true,
  "SessionId": "session_123456"
}
```

---

## 💾 Database Schema

### Collection: `tests`

```javascript
{
  "_id": ObjectId,
  "Title": "String",
  "Questions": [
    {
      "Text": "String",
      "Answers": [
        { "Text": "String" }
      ],
      "CorrectAnswerIndex": Number
    }
  ]
}
```

**Indici:**
- `_id` (default)

### Collection: `results`

```javascript
{
  "_id": ObjectId,
  "Username": "String",
  "TestId": "String",
  "TestTitle": "String",
  "Score": "String (format: '8/10')",
  "CorrectAnswers": Number,
  "TotalQuestions": Number,
  "Date": ISODate,
  "SessionId": "String"
}
```

**Indici:**
- `_id` (default)
- `{ Date: -1, Username: 1 }` (composto)

### Collection: `user_answers`

```javascript
{
  "_id": ObjectId,
  "Username": "String",
  "TestId": "String",
  "TestTitle": "String",
  "QuestionIndex": Number,
  "QuestionText": "String",
  "SelectedAnswerIndex": Number,
  "SelectedAnswerText": "String",
  "CorrectAnswerIndex": Number,
  "IsCorrect": Boolean,
  "AnsweredAt": ISODate,
  "SessionId": "String"
}
```

**Indici:**
- `_id` (default)
- `{ Username: 1, TestId: 1, AnsweredAt: 1 }` (composto)
- `{ SessionId: 1 }` (singolo)

---

## 🚢 Deployment

### Preparazione

1. **Build in Release Mode:**
```bash
dotnet publish -c Release -o ./publish
```

2. **Configura MongoDB per produzione** (se necessario):
```csharp
// In Program.cs, usa variabili d'ambiente
string connectionString = Environment.GetEnvironmentVariable("MONGODB_URI") 
                         ?? "mongodb://localhost:27017";
```

3. **Configura il server web:**

#### IIS (Windows)
- Installa ASP.NET Core Hosting Bundle
- Crea un nuovo sito in IIS
- Punta alla cartella `publish`

#### Linux (Nginx + systemd)
```bash
# Crea servizio systemd
sudo nano /etc/systemd/system/cogesquiz.service

[Unit]
Description=Coges Quiz App

[Service]
WorkingDirectory=/var/www/cogesquiz
ExecStart=/usr/bin/dotnet /var/www/cogesquiz/CogesQuizApp.dll
Restart=always

[Install]
WantedBy=multi-user.target
```

#### Docker (opzionale)
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CogesQuizApp.dll"]
```

---

## 📝 Note Importanti

### Sicurezza

- ⚠️ **Non esporre MongoDB senza autenticazione** in produzione
- ✅ Implementa autenticazione MongoDB
- ✅ Usa HTTPS in produzione
- ✅ Valida e sanitizza tutti gli input utente
- ✅ Implementa rate limiting per le API

### Performance

- Gli **indici MongoDB** sono creati automaticamente all'avvio
- Il database è ottimizzato per crescita nel tempo
- Considera l'uso di caching per query frequenti

### Limitazioni

- Non è presente un sistema di autenticazione utenti
- I test sono pubblicamente accessibili
- Non c'è limite al numero di tentativi per test

---

## 🤝 Contribuire

Per contribuire al progetto:

1. Fai un fork del repository
2. Crea un branch per la tua feature (`git checkout -b feature/AmazingFeature`)
3. Commit le modifiche (`git commit -m 'Add some AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Apri una Pull Request

---

## 📄 Licenza

Questo progetto è stato sviluppato come test tecnico per Coges.

---

## 👨‍💻 Autore

**[Il Tuo Nome]**

- GitHub: [@Spong3Bob17](https://github.com/Spong3Bob17)
- Email: leonardomaran17@gmail.com

---

## 🙏 Ringraziamenti

- Coges per l'opportunità
- MongoDB per l'eccellente database NoSQL
- NUnit team per il framework di testing

---

**Ultimo aggiornamento**: Novembre 2025  
**Versione**: 1.0.0
