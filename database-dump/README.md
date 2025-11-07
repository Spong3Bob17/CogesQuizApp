# 📦 MongoDB Database Dump - CogesQuizDB

Questo dump contiene il database MongoDB completo in formato BSON (Binary JSON).

## 📊 Contenuto

### Collezione: `tests` (5 documenti)
- Test di Matematica Base (5 domande)
- Test di Geografia Mondiale (4 domande)
- Test di Storia Moderna (6 domande)
- Test di Cultura Generale (3 domande)
- Test di Informatica Base (5 domande)

### Collezione: `results` (6 documenti)
- Risultati di esempio per la classifica
- Include diversi utenti e punteggi

### Collezione: `user_answers` (0 documenti)
- Si popola durante l'uso dell'applicazione
- Traccia ogni risposta data dagli utenti

---

## 🔧 Ripristino Database

### Ripristino Completo
```bash
# Dalla root del progetto
mongorestore --db CogesQuizDB database-dump/CogesQuizDB
```

### Ripristino con Sovrascrittura

Se il database esiste già e vuoi sovrascriverlo:
```bash
mongorestore --db CogesQuizDB --drop database-dump/CogesQuizDB
```

L'opzione `--drop` elimina le collezioni esistenti prima del ripristino.

---

## ✅ Verifica Ripristino
```bash
# Entra in MongoDB shell
mongosh CogesQuizDB

# Conta i documenti
db.tests.countDocuments()        # Output: 5
db.results.countDocuments()      # Output: 6
db.user_answers.countDocuments() # Output: 0

# Visualizza i titoli dei test
db.tests.find({}, { Title: 1, _id: 0 })

# Esci
exit
```

---

## 📋 Struttura File
```
database-dump/
└── CogesQuizDB/
    ├── tests.bson                  # Dati test (formato binario BSON)
    ├── tests.metadata.json         # Metadata e indici
    ├── results.bson                # Dati risultati
    ├── results.metadata.json       # Metadata e indici
    ├── user_answers.bson           # Risposte utente
    ├── user_answers.metadata.json  # Metadata e indici
    └── README.md                   # Questo file
```

---

## 🆚 Dump vs Script: Quando Usare Cosa?

### Usa il **Dump BSON** quando:
- ✅ Vuoi replicare esattamente il database
- ✅ Hai bisogno di un backup completo
- ✅ Preferisci il formato binario (più veloce)
- ✅ Vuoi includere gli indici automaticamente
```bash
mongorestore --db CogesQuizDB database-dump/CogesQuizDB
```

### Usa lo **Script JS** quando:
- ✅ Vuoi un setup pulito e ripetibile
- ✅ Preferisci vedere i dati in chiaro
- ✅ Vuoi modificare facilmente i dati
- ✅ Preferisci un approccio "infrastructure as code"
```bash
mongosh CogesQuizDB < Scripts/seed-database.js
```

**Entrambi i metodi sono validi e professionali!**

---

## 🔄 Aggiornare il Dump

Se modifichi i dati e vuoi aggiornare il dump:
```bash
# Elimina il vecchio dump
rm -rf database-dump/CogesQuizDB

# Crea nuovo dump
mongodump --db CogesQuizDB --out database-dump
```

---

## ⚠️ Note Tecniche

- **Formato**: BSON (Binary JSON) - più efficiente di JSON
- **Indici**: Gli indici vengono ripristinati automaticamente dai file `.metadata.json`
- **Compatibilità**: Compatibile con MongoDB 4.4+
- **Dimensione**: I file BSON sono compressi e ottimizzati

---

## 📞 Troubleshooting

### Errore: "Failed: error connecting to db server"
MongoDB non è in esecuzione:
```bash
# Windows
net start MongoDB
```

### Errore: "duplicate key error"
Il database esiste già:
```bash
# Usa l'opzione --drop
mongorestore --db CogesQuizDB --drop database-dump/CogesQuizDB
```

---

**Creato**: Novembre 2025  
**Tool**: mongodump 100.x  
**MongoDB Version**: 4.4+