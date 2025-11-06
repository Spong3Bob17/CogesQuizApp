# 🚀 Setup Completo - Coges Quiz App

Questa guida ti accompagnerà passo-passo nella configurazione completa dell'applicazione.

---

## 📋 Prerequisiti

Prima di iniziare, assicurati di avere installato:

- [ ] **.NET SDK 9.0** o superiore
- [ ] **MongoDB 4.4** o superiore
- [ ] **Git** (per clonare il repository)

---

## 1️⃣ Installazione MongoDB

### Windows

1. **Scarica MongoDB Community Server**:
    - Vai su: https://www.mongodb.com/try/download/community
    - Seleziona la versione per Windows
    - Scarica e installa

2. **Verifica l'installazione**:
   ```powershell
   mongod --version
   ```

3. **Avvia MongoDB come servizio**:
   ```powershell
   net start MongoDB
   ```

4. **Verifica che sia in esecuzione**:
   ```powershell
   mongo --eval "db.version()"
   ```

### macOS

```bash
# Installa tramite Homebrew
brew tap mongodb/brew
brew install mongodb-community

# Avvia MongoDB
brew services start mongodb-community

# Verifica
mongo --eval "db.version()"
```

### Linux (Ubuntu/Debian)

```bash
# Importa chiave pubblica
wget -qO - https://www.mongodb.org/static/pgp/server-4.4.asc | sudo apt-key add -

# Aggiungi repository
echo "deb [ arch=amd64,arm64 ] https://repo.mongodb.org/apt/ubuntu focal/mongodb-org/4.4 multiverse" | sudo tee /etc/apt/sources.list.d/mongodb-org-4.4.list

# Installa
sudo apt-get update
sudo apt-get install -y mongodb-org

# Avvia
sudo systemctl start mongod
sudo systemctl enable mongod

# Verifica
mongo --eval "db.version()"
```

---

## 2️⃣ Clone e Setup Progetto

### Clone Repository

```bash
# Clone
git clone https://github.com/your-username/CogesQuizApp.git
cd CogesQuizApp

# Verifica struttura
ls -la
```

Dovresti vedere:
```
CogesQuizApp/
CogesQuizApp.Tests/
Scripts/
README.md
CogesQuizApp.sln
```

### Ripristina Pacchetti

```bash
# Ripristina tutte le dipendenze
dotnet restore

# Verifica che non ci siano errori
dotnet build
```

Se vedi errori, verifica che:
- ✅ .NET SDK 9.0 sia installato
- ✅ Tutti i file `.csproj` siano presenti
- ✅ La connessione internet funzioni (per scaricare i pacchetti)

---

## 3️⃣ Configurazione Database

### Popola il Database

**Metodo 1: Usando MongoDB Shell** (Consigliato)

```bash
# Dalla root del progetto
mongo CogesQuizDB < Scripts/seed-database.js
```

Dovresti vedere:
```
🗑️  Database pulito
📚 Inserimento test...
✅ Test inseriti con successo
📊 Inserimento risultati di esempio...
✅ Risultati inseriti con successo
🔧 Creazione indici...
✅ Indici creati con successo
```

**Metodo 2: Usando MongoDB Compass** (Interfaccia Grafica)

1. Apri MongoDB Compass
2. Connetti a `mongodb://localhost:27017`
3. Crea database `CogesQuizDB`
4. Importa i dati manualmente dalle collezioni

### Verifica Dati

```bash
# Entra nella shell MongoDB
mongo CogesQuizDB

# Controlla i test
db.tests.countDocuments()
# Output: 5

# Vedi i titoli dei test
db.tests.find({}, { Title: 1 })

# Controlla i risultati
db.results.countDocuments()
# Output: 5

# Esci
exit
```

---

## 4️⃣ Prima Esecuzione

### Avvia l'Applicazione

```bash
cd CogesQuizApp
dotnet run
```

Dovresti vedere:
```
✅ Cartella wwwroot trovata in: /path/to/wwwroot
✅ Indici database creati con successo
🚀 Server avviato su http://localhost:8080/
👉 Endpoints API disponibili:
   GET  /tests
   GET  /results
   POST /results
   POST /user-answers
   GET  /user-answers/session/{id}
-----------------------------------------
```

### Testa l'Applicazione

1. **Apri il browser** e vai su: **http://localhost:8080**

2. **Verifica la Homepage**:
    - ✅ Vedi il titolo "Coges Quiz App"?
    - ✅ Il dropdown mostra i test disponibili?
    - ✅ Puoi inserire il tuo nome?

3. **Esegui un Test**:
    - Inserisci il tuo nome (es. "Test User")
    - Seleziona "Test di Matematica Base"
    - Clicca "Inizia il Quiz"
    - Rispondi alle domande
    - Controlla il punteggio finale

4. **Verifica la Classifica**:
    - Vai su http://localhost:8080/results.html
    - ✅ Vedi la tabella con i risultati?
    - ✅ Il tuo risultato appare?

### Testa gli API Endpoints

**Con cURL:**

```bash
# GET tutti i test
curl http://localhost:8080/tests

# GET tutti i risultati
curl http://localhost:8080/results
```

**Con Browser:**
- http://localhost:8080/tests (dovresti vedere JSON)
- http://localhost:8080/results (dovresti vedere JSON)

---

## 5️⃣ Esegui i Test

### Test Completo

```bash
cd CogesQuizApp.Tests
dotnet test
```

Output atteso:
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    35, Skipped:     0, Total:    35
Time: 2.5s
```

### Solo Unit Tests (no MongoDB)

```bash
dotnet test --filter TestCategory!=Integration
```

### Solo Integration Tests (richiede MongoDB)

```bash
dotnet test --filter TestCategory=Integration
```

---

## 6️⃣ Troubleshooting

### Problema: "MongoDB connection failed"

**Causa**: MongoDB non è in esecuzione

**Soluzione**:
```bash
# Windows
net start MongoDB

# macOS
brew services start mongodb-community

# Linux
sudo systemctl start mongod
```

### Problema: "Port 8080 already in use"

**Causa**: Un'altra applicazione usa la porta 8080

**Soluzione 1**: Ferma l'altra applicazione

**Soluzione 2**: Cambia porta in `Program.cs`:
```csharp
listener.Prefixes.Add("http://localhost:9000/"); // Usa porta 9000
```

### Problema: "wwwroot folder not found"

**Causa**: Percorso wwwroot non corretto

**Soluzione**: Verifica che la cartella `CogesQuizApp/wwwroot` esista con:
- index.html
- quiz.html
- results.html
- style.css

### Problema: Test falliscono con "ObjectId not valid"

**Causa**: MongoDB IDs non validi nei test

**Soluzione**: Gli ID devono essere ObjectId validi (24 caratteri hex). Usa `ObjectId.GenerateNewId()` nei test.

### Problema: "Unsupported expression" nei test con Moq

**Causa**: Stai provando a mockare metodi non-virtual

**Soluzione**: Usa l'interfaccia `IDatabaseService` invece di `DatabaseService`:
```csharp
private Mock<IDatabaseService> _mockDbService;
```

---

## 7️⃣ Setup IDE

### Visual Studio 2022

1. Apri `CogesQuizApp.sln`
2. Set `CogesQuizApp` come Startup Project
3. Premi F5 per avviare in debug

### JetBrains Rider

1. Apri `CogesQuizApp.sln`
2. Build → Rebuild Solution
3. Run → Run 'CogesQuizApp'

### Visual Studio Code

1. Installa estensione "C# Dev Kit"
2. Apri la cartella del progetto
3. Premi F5, seleziona ".NET"

---

## 8️⃣ Configurazioni Aggiuntive (Opzionale)

### Cambia Connection String

In `Program.cs`:
```csharp
// Per MongoDB Atlas (cloud)
string connectionString = "mongodb+srv://username:password@cluster.mongodb.net";

// Per MongoDB con autenticazione
string connectionString = "mongodb://admin:password@localhost:27017";
```

### Cambia Nome Database

```csharp
string databaseName = "MioDatabase"; // Invece di CogesQuizDB
```

### Abilita Logging

Aggiungi in `Program.cs`:
```csharp
Console.WriteLine($"[INFO] Richiesta ricevuta: {method} {path}");
```

---

## 9️⃣ Checklist Finale

Prima di considerare il setup completo, verifica:

- [ ] MongoDB è in esecuzione
- [ ] Database è popolato con dati di esempio
- [ ] `dotnet build` compila senza errori
- [ ] `dotnet test` passa tutti i test
- [ ] `dotnet run` avvia il server
- [ ] http://localhost:8080 mostra la homepage
- [ ] Puoi completare un test end-to-end
- [ ] I risultati vengono salvati nel database
- [ ] La classifica mostra i risultati

---

## 🎉 Setup Completato!

Se tutti i punti della checklist sono ✅, il setup è completo!

### Prossimi Passi:

1. **Esplora il Codice**: Familiarizza con la struttura del progetto
2. **Prova le Feature**: Testa tutte le funzionalità
3. **Leggi la Documentazione**: Consulta README.md per dettagli
4. **Personalizza**: Modifica stili, aggiungi test, ecc.

---

## 📞 Supporto

Se incontri problemi non coperti in questa guida:

1. Controlla la sezione [Troubleshooting](#6️⃣-troubleshooting)
2. Verifica i log di MongoDB: `C:\data\db\mongodb.log` (Windows)
3. Controlla i log dell'applicazione nella console

---

**Buon lavoro! 🚀**