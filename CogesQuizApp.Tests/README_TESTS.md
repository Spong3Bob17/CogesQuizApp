# 🧪 CogesQuizApp - Test Suite

## Panoramica

Questo progetto contiene i test unitari e di integrazione per l'applicazione CogesQuizApp. I test sono scritti utilizzando **NUnit**, **Moq** e **FluentAssertions**.

## 📂 Struttura dei Test

```
CogesQuizApp.Tests/
├── Models/
│   └── ModelTests.cs          # Test per i modelli di dominio
├── Services/
│   └── DatabaseServiceTests.cs # Test per DatabaseService
├── Controllers/
│   └── ControllerTests.cs     # Test per i controller
├── Helpers/
│   └── TestHelpers.cs         # Utilità per i test
└── README_TESTS.md            # Questa documentazione
```

## 🔧 Prerequisiti

### Software Richiesto

- **.NET 9.0 SDK** o superiore
- **MongoDB** in esecuzione su `localhost:27017` (per test di integrazione)

### Pacchetti NuGet

I seguenti pacchetti sono già configurati nel `.csproj`:

- `NUnit` - Framework di testing
- `NUnit3TestAdapter` - Adapter per Visual Studio / Rider
- `Moq` - Libreria per mocking
- `FluentAssertions` - Assertions fluide e leggibili
- `coverlet.collector` - Code coverage

## 🚀 Esecuzione dei Test

### Da Command Line

```bash
# Esegui tutti i test
dotnet test

# Esegui test con output dettagliato
dotnet test --verbosity detailed

# Esegui test con code coverage
dotnet test /p:CollectCoverage=true

# Esegui solo test di una categoria specifica
dotnet test --filter TestCategory=Integration
```

### Da Visual Studio / Rider

1. Apri il **Test Explorer** (View → Test Explorer)
2. Clicca su "Run All" per eseguire tutti i test
3. Oppure clicca col destro su un test specifico → "Run"

### Da VS Code

1. Installa l'estensione **.NET Core Test Explorer**
2. I test appariranno nella sidebar "Testing"
3. Clicca sull'icona play per eseguire

## 📊 Categorie di Test

### Unit Tests (Isolati)

Test che non richiedono database o risorse esterne:

- **ModelTests**: Validazione dei modelli
- **ControllerTests** (con mock): Logica dei controller

```bash
# Esegui solo unit tests
dotnet test --filter TestCategory!=Integration
```

### Integration Tests

Test che richiedono MongoDB attivo:

- **DatabaseServiceTests**: Operazioni CRUD reali

```bash
# Esegui solo integration tests
dotnet test --filter TestCategory=Integration
```

⚠️ **Nota**: I test di integrazione utilizzano il database `CogesQuizDB_Test` che viene pulito dopo ogni test.

## 📝 Cosa Viene Testato

### 1. Modelli (`ModelTests.cs`)

✅ Creazione corretta dei modelli  
✅ Validazione delle proprietà  
✅ Numero variabile di risposte (2-n)  
✅ Formato dello score  
✅ Date di default  
✅ Collegamento SessionId

### 2. DatabaseService (`DatabaseServiceTests.cs`)

✅ CRUD operations per Test  
✅ CRUD operations per Result  
✅ CRUD operations per UserAnswer  
✅ Ordinamento per data  
✅ Filtri per username e testId  
✅ Recupero per sessione  
✅ Statistiche (count, average)

### 3. Controller (`ControllerTests.cs`)

✅ Istanziazione controller  
✅ Chiamate ai metodi del DatabaseService  
✅ Validazione input  
✅ Gestione errori  
✅ Logica di business (calcolo percentuali, punteggi)

### 4. Helper (`TestHelpers.cs`)

✅ Factory methods per oggetti di test  
✅ Generazione SessionId  
✅ Validazione formati  
✅ Calcolo percentuali

## 🎯 Coverage Obiettivo

L'obiettivo è mantenere un code coverage **>80%** per:

- Modelli: >90%
- Services: >85%
- Controllers: >75%

## 🐛 Debug dei Test Falliti

### Test di Integrazione Falliscono

**Problema**: `MongoConnectionException` o timeout

**Soluzione**:
```bash
# Verifica che MongoDB sia in esecuzione
mongod --version

# Avvia MongoDB se non è attivo
# Windows: net start MongoDB
# Linux/Mac: sudo systemctl start mongod
```

### Test con Date Falliscono

**Problema**: Differenze temporali nei test

**Soluzione**: I test usano `BeCloseTo()` con tolleranza di 5 secondi:
```csharp
result.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
```

### Mock non Funziona

**Problema**: Setup del mock non corretto

**Soluzione**: Verifica che il setup usi `It.IsAny<T>()` o valori specifici:
```csharp
_mockDbService.Setup(x => x.SaveResult(It.IsAny<r>()));
```

## 📈 Eseguire Code Coverage

### Con Coverlet (Consigliato)

```bash
# Genera report di coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Con output HTML (richiede ReportGenerator)
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

### Con Visual Studio

1. Vai a Test → Analyze Code Coverage for All Tests
2. Il report apparirà in Code Coverage Results

## 🔄 CI/CD Integration

Esempio di configurazione per GitHub Actions:

```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    
    services:
      mongodb:
        image: mongo:latest
        ports:
          - 27017:27017
    
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 9.0.x
      - name: Run tests
        run: dotnet test --verbosity normal
```

## ✍️ Scrivere Nuovi Test

### Template per Unit Test

```csharp
[Test]
public void MetodoTestato_Condizione_RisultatoAtteso()
{
    // Arrange - Prepara i dati
    var input = CreateSampleData();
    
    // Act - Esegui l'azione
    var result = SomeMethod(input);
    
    // Assert - Verifica il risultato
    result.Should().Be(expectedValue);
}
```

### Template per Integration Test

```csharp
[Test]
[Category("Integration")]
public void DatabaseOperation_Scenario_ExpectedOutcome()
{
    // Arrange
    var data = CreateTestData();
    _dbService.SaveData(data);
    
    // Act
    var result = _dbService.GetData();
    
    // Assert
    result.Should().NotBeNull();
    
    // Cleanup
    _dbService.DeleteData(data.Id);
}
```

## 📚 Risorse Utili

- [NUnit Documentation](https://docs.nunit.org/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions](https://fluentassertions.com/)
- [MongoDB .NET Driver](https://mongodb.github.io/mongo-csharp-driver/)

## 🤝 Best Practices

1. **Naming**: Usa nomi descrittivi `MetodoTestato_Scenario_RisultatoAtteso`
2. **AAA Pattern**: Arrange, Act, Assert
3. **Isolamento**: Ogni test deve essere indipendente
4. **Cleanup**: Pulisci sempre i dati di test
5. **Fast**: I test devono essere veloci (<100ms unit, <1s integration)
6. **Deterministic**: Stessi input → stesso output sempre

## 📞 Supporto

Per problemi con i test, controlla:

1. Questo README
2. I commenti nel codice dei test
3. La documentazione NUnit/Moq
4. GitHub Issues del progetto

---

**Ultimo aggiornamento**: Novembre 2025  
**Versione Test Suite**: 1.0.0