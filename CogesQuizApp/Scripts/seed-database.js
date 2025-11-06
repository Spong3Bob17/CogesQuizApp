// ============================================
// Script per popolare il database MongoDB
// con dati di esempio per Coges Quiz App
// ============================================

// Usa il database CogesQuizDB
db = db.getSiblingDB('CogesQuizDB');

// Elimina collezioni esistenti (opzionale - rimuovi in produzione!)
db.tests.drop();
db.results.drop();
db.user_answers.drop();

print("🗑️  Database pulito");

// ============================================
// TESTS - Inserisce test di esempio
// ============================================

print("📚 Inserimento test...");

// Test 1: Test di Matematica
db.tests.insertOne({
    Title: "Test di Matematica Base",
    Questions: [
        {
            Text: "Quanto fa 5 + 3?",
            Answers: [
                { Text: "6" },
                { Text: "7" },
                { Text: "8" },
                { Text: "9" }
            ],
            CorrectAnswerIndex: 2
        },
        {
            Text: "Qual è il risultato di 12 × 4?",
            Answers: [
                { Text: "44" },
                { Text: "46" },
                { Text: "48" },
                { Text: "50" }
            ],
            CorrectAnswerIndex: 2
        },
        {
            Text: "Quanto fa 100 ÷ 5?",
            Answers: [
                { Text: "15" },
                { Text: "20" },
                { Text: "25" },
                { Text: "30" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Qual è il quadrato di 7?",
            Answers: [
                { Text: "42" },
                { Text: "49" },
                { Text: "56" },
                { Text: "63" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Quanto fa 15 - 8?",
            Answers: [
                { Text: "5" },
                { Text: "6" },
                { Text: "7" },
                { Text: "8" }
            ],
            CorrectAnswerIndex: 2
        }
    ]
});

// Test 2: Test di Geografia
db.tests.insertOne({
    Title: "Test di Geografia Mondiale",
    Questions: [
        {
            Text: "Qual è la capitale dell'Italia?",
            Answers: [
                { Text: "Milano" },
                { Text: "Roma" },
                { Text: "Napoli" },
                { Text: "Firenze" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Qual è il fiume più lungo del mondo?",
            Answers: [
                { Text: "Nilo" },
                { Text: "Amazzonia" },
                { Text: "Yangtze" },
                { Text: "Mississippi" }
            ],
            CorrectAnswerIndex: 0
        },
        {
            Text: "In quale continente si trova l'Egitto?",
            Answers: [
                { Text: "Asia" },
                { Text: "Africa" },
                { Text: "Europa" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Qual è l'oceano più grande?",
            Answers: [
                { Text: "Atlantico" },
                { Text: "Indiano" },
                { Text: "Pacifico" },
                { Text: "Artico" },
                { Text: "Antartico" }
            ],
            CorrectAnswerIndex: 2
        }
    ]
});

// Test 3: Test di Storia
db.tests.insertOne({
    Title: "Test di Storia Moderna",
    Questions: [
        {
            Text: "In che anno è caduto il Muro di Berlino?",
            Answers: [
                { Text: "1987" },
                { Text: "1989" },
                { Text: "1991" },
                { Text: "1993" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Chi ha scoperto l'America?",
            Answers: [
                { Text: "Amerigo Vespucci" },
                { Text: "Cristoforo Colombo" },
                { Text: "Ferdinando Magellano" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "In che anno è iniziata la Prima Guerra Mondiale?",
            Answers: [
                { Text: "1912" },
                { Text: "1914" },
                { Text: "1916" },
                { Text: "1918" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Chi era il presidente degli USA durante la Guerra Fredda negli anni '80?",
            Answers: [
                { Text: "Jimmy Carter" },
                { Text: "Ronald Reagan" },
                { Text: "George H.W. Bush" },
                { Text: "Bill Clinton" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Quale impero romano durò più a lungo?",
            Answers: [
                { Text: "Impero Romano d'Occidente" },
                { Text: "Impero Romano d'Oriente (Bizantino)" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "In che anno l'Italia è diventata una repubblica?",
            Answers: [
                { Text: "1943" },
                { Text: "1945" },
                { Text: "1946" },
                { Text: "1948" }
            ],
            CorrectAnswerIndex: 2
        }
    ]
});

// Test 4: Test di Cultura Generale
db.tests.insertOne({
    Title: "Test di Cultura Generale",
    Questions: [
        {
            Text: "Chi ha dipinto la Gioconda?",
            Answers: [
                { Text: "Michelangelo" },
                { Text: "Leonardo da Vinci" },
                { Text: "Raffaello" },
                { Text: "Caravaggio" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Qual è il pianeta più vicino al Sole?",
            Answers: [
                { Text: "Venere" },
                { Text: "Mercurio" },
                { Text: "Marte" },
                { Text: "Terra" }
            ],
            CorrectAnswerIndex: 1
        },
        {
            Text: "Chi ha scritto 'I Promessi Sposi'?",
            Answers: [
                { Text: "Dante Alighieri" },
                { Text: "Giovanni Boccaccio" },
                { Text: "Alessandro Manzoni" },
                { Text: "Italo Calvino" }
            ],
            CorrectAnswerIndex: 2
        }
    ]
});

// Test 5: Test di Informatica
db.tests.insertOne({
    Title: "Test di Informatica Base",
    Questions: [
        {
            Text: "Cosa significa HTML?",
            Answers: [
                { Text: "HyperText Markup Language" },
                { Text: "High Tech Modern Language" },
                { Text: "Home Tool Markup Language" },
                { Text: "Hyperlinks and Text Markup Language" }
            ],
            CorrectAnswerIndex: 0
        },
        {
            Text: "Quale di questi è un linguaggio di programmazione?",
            Answers: [
                { Text: "HTML" },
                { Text: "CSS" },
                { Text: "JavaScript" },
                { Text: "HTTP" }
            ],
            CorrectAnswerIndex: 2
        },
        {
            Text: "Cosa significa CPU?",
            Answers: [
                { Text: "Central Processing Unit" },
                { Text: "Computer Personal Unit" },
                { Text: "Central Program Utility" }
            ],
            CorrectAnswerIndex: 0
        },
        {
            Text: "Qual è il sistema operativo open source più famoso?",
            Answers: [
                { Text: "Windows" },
                { Text: "macOS" },
                { Text: "Linux" },
                { Text: "Android" }
            ],
            CorrectAnswerIndex: 2
        },
        {
            Text: "Cosa significa SQL?",
            Answers: [
                { Text: "Structured Query Language" },
                { Text: "Simple Question Language" },
                { Text: "Standard Quality Language" },
                { Text: "Secure Query Line" }
            ],
            CorrectAnswerIndex: 0
        }
    ]
});

print("✅ Test inseriti con successo");

// ============================================
// RESULTS - Inserisce risultati di esempio
// ============================================

print("📊 Inserimento risultati di esempio...");

// Ottieni gli ID dei test appena creati
const mathTest = db.tests.findOne({ Title: "Test di Matematica Base" });
const geoTest = db.tests.findOne({ Title: "Test di Geografia Mondiale" });
const historyTest = db.tests.findOne({ Title: "Test di Storia Moderna" });

// Inserisci alcuni risultati di esempio
db.results.insertMany([
    {
        Username: "Mario Rossi",
        TestId: mathTest._id.toString(),
        TestTitle: mathTest.Title,
        Score: "5/5",
        CorrectAnswers: 5,
        TotalQuestions: 5,
        Date: new Date("2025-11-01T10:30:00Z"),
        SessionId: "session_example_1"
    },
    {
        Username: "Luigi Verdi",
        TestId: mathTest._id.toString(),
        TestTitle: mathTest.Title,
        Score: "4/5",
        CorrectAnswers: 4,
        TotalQuestions: 5,
        Date: new Date("2025-11-02T14:15:00Z"),
        SessionId: "session_example_2"
    },
    {
        Username: "Giulia Bianchi",
        TestId: geoTest._id.toString(),
        TestTitle: geoTest.Title,
        Score: "3/4",
        CorrectAnswers: 3,
        TotalQuestions: 4,
        Date: new Date("2025-11-03T09:45:00Z"),
        SessionId: "session_example_3"
    },
    {
        Username: "Marco Neri",
        TestId: historyTest._id.toString(),
        TestTitle: historyTest.Title,
        Score: "6/6",
        CorrectAnswers: 6,
        TotalQuestions: 6,
        Date: new Date("2025-11-04T16:20:00Z"),
        SessionId: "session_example_4"
    },
    {
        Username: "Anna Ferrari",
        TestId: geoTest._id.toString(),
        TestTitle: geoTest.Title,
        Score: "4/4",
        CorrectAnswers: 4,
        TotalQuestions: 4,
        Date: new Date("2025-11-05T11:00:00Z"),
        SessionId: "session_example_5"
    }
]);

print("✅ Risultati inseriti con successo");

// ============================================
// INDICI - Crea indici per performance
// ============================================

print("🔧 Creazione indici...");

// Indici per results
db.results.createIndex({ Date: -1, Username: 1 });
db.results.createIndex({ TestId: 1 });
db.results.createIndex({ SessionId: 1 });

// Indici per user_answers
db.user_answers.createIndex({ Username: 1, TestId: 1, AnsweredAt: 1 });
db.user_answers.createIndex({ SessionId: 1 });

print("✅ Indici creati con successo");

// ============================================
// RIEPILOGO
// ============================================

print("\n============================================");
print("✅ Database popolato con successo!");
print("============================================");
print(`📚 Test inseriti: ${db.tests.countDocuments()}`);
print(`📊 Risultati inseriti: ${db.results.countDocuments()}`);
print(`🔧 Indici creati su results e user_answers`);
print("============================================");
print("\n💡 Puoi ora avviare l'applicazione con: dotnet run");
print("🌐 L'app sarà disponibile su: http://localhost:8080\n");