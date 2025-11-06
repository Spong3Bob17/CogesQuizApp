# 🚢 Guida al Deployment - Coges Quiz App

Questa guida descrive come deployare l'applicazione in ambienti di produzione.

---

## 📋 Indice

- [Preparazione](#preparazione)
- [Deploy su Windows (IIS)](#deploy-su-windows-iis)
- [Deploy su Linux (Nginx)](#deploy-su-linux-nginx)
- [Deploy con Docker](#deploy-con-docker)
- [Configurazione MongoDB Produzione](#configurazione-mongodb-produzione)
- [Sicurezza](#sicurezza)
- [Monitoring](#monitoring)

---

## 🔧 Preparazione

### 1. Build in Release Mode

```bash
cd CogesQuizApp
dotnet publish -c Release -o ./publish
```

Questo crea una cartella `publish` con tutti i file necessari.

### 2. Configura Variabili d'Ambiente

Modifica `Program.cs` per usare variabili d'ambiente:

```csharp
// Leggi da variabili d'ambiente
string connectionString = Environment.GetEnvironmentVariable("MONGODB_URI") 
                         ?? "mongodb://localhost:27017";
string databaseName = Environment.GetEnvironmentVariable("DB_NAME") 
                     ?? "CogesQuizDB";
string port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// Usa la porta configurata
listener.Prefixes.Add($"http://localhost:{port}/");
```

### 3. Rebuild

```bash
dotnet publish -c Release -o ./publish
```

---

## 🪟 Deploy su Windows (IIS)

### Prerequisiti

- Windows Server 2016 o superiore
- IIS installato
- .NET Hosting Bundle installato

### Step 1: Installa .NET Hosting Bundle

1. Scarica da: https://dotnet.microsoft.com/download/dotnet/9.0
2. Cerca "Hosting Bundle"
3. Installa e riavvia IIS:
   ```powershell
   iisreset
   ```

### Step 2: Crea Applicazione IIS

1. Apri **IIS Manager**
2. Click destro su **Sites** → **Add Website**
3. Configura:
    - **Site name**: CogesQuizApp
    - **Physical path**: `C:\inetpub\CogesQuizApp` (copia qui la cartella `publish`)
    - **Port**: 80 (o altra porta)
4. Imposta **Application Pool**:
    - **.NET CLR version**: No Managed Code
    - **Managed pipeline mode**: Integrated

### Step 3: Configura Permessi

```powershell
# Dai permessi IIS_IUSRS alla cartella
icacls "C:\inetpub\CogesQuizApp" /grant "IIS_IUSRS:(OI)(CI)F" /T
```

### Step 4: Configura web.config

Crea `web.config` nella cartella publish:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" 
                arguments=".\CogesQuizApp.dll" 
                stdoutLogEnabled="true" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess" />
  </system.webServer>
</configuration>
```

### Step 5: Imposta Variabili d'Ambiente

In IIS Manager:
1. Seleziona il sito
2. **Configuration Editor**
3. Section: `system.webServer/aspNetCore`
4. Espandi `environmentVariables`
5. Aggiungi:
    - `MONGODB_URI`: `mongodb://your-mongo-server:27017`
    - `DB_NAME`: `CogesQuizDB`

### Step 6: Avvia

```powershell
# Avvia il sito
Start-Website -Name "CogesQuizApp"

# Verifica
Invoke-WebRequest http://localhost
```

---

## 🐧 Deploy su Linux (Nginx)

### Prerequisiti

- Ubuntu 20.04+ / Debian 11+
- Nginx
- MongoDB

### Step 1: Installa .NET Runtime

```bash
# Ubuntu 22.04
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-9.0
```

### Step 2: Copia File

```bash
# Crea directory
sudo mkdir -p /var/www/cogesquiz

# Copia file publish
sudo cp -r ./publish/* /var/www/cogesquiz/

# Imposta permessi
sudo chown -R www-data:www-data /var/www/cogesquiz
```

### Step 3: Crea Servizio Systemd

```bash
sudo nano /etc/systemd/system/cogesquiz.service
```

Contenuto:

```ini
[Unit]
Description=Coges Quiz App
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/cogesquiz
ExecStart=/usr/bin/dotnet /var/www/cogesquiz/CogesQuizApp.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=cogesquiz
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=MONGODB_URI=mongodb://localhost:27017
Environment=DB_NAME=CogesQuizDB
Environment=PORT=5000

[Install]
WantedBy=multi-user.target
```

### Step 4: Avvia Servizio

```bash
# Reload daemon
sudo systemctl daemon-reload

# Abilita avvio automatico
sudo systemctl enable cogesquiz

# Avvia servizio
sudo systemctl start cogesquiz

# Verifica stato
sudo systemctl status cogesquiz
```

### Step 5: Configura Nginx

```bash
sudo nano /etc/nginx/sites-available/cogesquiz
```

Contenuto:

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Abilita sito:

```bash
sudo ln -s /etc/nginx/sites-available/cogesquiz /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

### Step 6: Configura HTTPS (Opzionale ma Consigliato)

```bash
# Installa Certbot
sudo apt-get install -y certbot python3-certbot-nginx

# Ottieni certificato SSL
sudo certbot --nginx -d your-domain.com

# Auto-renewal
sudo certbot renew --dry-run
```

---

## 🐳 Deploy con Docker

### Dockerfile

Crea `Dockerfile` nella root del progetto:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

# Copia csproj e ripristina dipendenze
COPY CogesQuizApp/*.csproj ./CogesQuizApp/
COPY CogesQuizApp.Tests/*.csproj ./CogesQuizApp.Tests/
RUN dotnet restore ./CogesQuizApp/CogesQuizApp.csproj

# Copia tutto e builda
COPY CogesQuizApp/. ./CogesQuizApp/
WORKDIR /source/CogesQuizApp
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .

# Esponi porta
EXPOSE 8080

# Variabili d'ambiente di default
ENV MONGODB_URI=mongodb://mongo:27017
ENV DB_NAME=CogesQuizDB
ENV PORT=8080

# Entry point
ENTRYPOINT ["dotnet", "CogesQuizApp.dll"]
```

### docker-compose.yml

```yaml
version: '3.8'

services:
  mongo:
    image: mongo:latest
    container_name: cogesquiz-mongo
    restart: always
    ports:
      - "27017:27017"
    volumes:
      - mongo-data:/data/db
      - ./Scripts/seed-database.js:/docker-entrypoint-initdb.d/seed.js:ro
    environment:
      MONGO_INITDB_DATABASE: CogesQuizDB

  app:
    build: .
    container_name: cogesquiz-app
    restart: always
    ports:
      - "8080:8080"
    environment:
      MONGODB_URI: mongodb://mongo:27017
      DB_NAME: CogesQuizDB
      PORT: 8080
    depends_on:
      - mongo

volumes:
  mongo-data:
```

### Build e Run

```bash
# Build immagine
docker-compose build

# Avvia container
docker-compose up -d

# Verifica logs
docker-compose logs -f app

# Ferma
docker-compose down
```

### Deploy su Cloud

#### Docker Hub

```bash
# Login
docker login

# Tag
docker tag cogesquiz-app your-username/cogesquiz:latest

# Push
docker push your-username/cogesquiz:latest
```

#### Azure Container Instances

```bash
az container create \
  --resource-group myResourceGroup \
  --name cogesquiz \
  --image your-username/cogesquiz:latest \
  --dns-name-label cogesquiz \
  --ports 8080 \
  --environment-variables \
    MONGODB_URI="mongodb://..." \
    DB_NAME="CogesQuizDB"
```

---

## 🔒 Configurazione MongoDB Produzione

### Abilita Autenticazione

```bash
# Entra in MongoDB
mongosh

# Crea admin user
use admin
db.createUser({
  user: "admin",
  pwd: "strong_password_here",
  roles: ["root"]
})

# Crea app user
use CogesQuizDB
db.createUser({
  user: "cogesquiz_user",
  pwd: "app_password_here",
  roles: [
    { role: "readWrite", db: "CogesQuizDB" }
  ]
})
```

### Abilita Auth in mongod.conf

```yaml
# /etc/mongod.conf
security:
  authorization: enabled
```

Riavvia MongoDB:
```bash
sudo systemctl restart mongod
```

### Aggiorna Connection String

```
mongodb://cogesquiz_user:app_password_here@localhost:27017/CogesQuizDB?authSource=CogesQuizDB
```

---

## 🔐 Sicurezza

### Checklist Sicurezza

- [ ] **MongoDB con autenticazione**
- [ ] **HTTPS abilitato** (Let's Encrypt)
- [ ] **Firewall configurato** (solo porte 80, 443, 22)
- [ ] **Input validation** su tutti gli endpoint
- [ ] **Rate limiting** per le API
- [ ] **Backup automatici** del database
- [ ] **Logs centralizzati**
- [ ] **Monitoring attivo**

### Implementa Rate Limiting

Aggiungi in `Program.cs`:

```csharp
// Simple rate limiting (per IP)
var requestCounts = new Dictionary<string, (int count, DateTime resetTime)>();

// In handleRequest:
var clientIp = context.Request.RemoteEndPoint.Address.ToString();
if (!CheckRateLimit(clientIp, requestCounts))
{
    context.Response.StatusCode = 429; // Too Many Requests
    return;
}
```

### Abilita CORS (se necessario)

```csharp
context.Response.Headers.Add("Access-Control-Allow-Origin", "https://your-domain.com");
context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST");
```

---

## 📊 Monitoring

### Log Files

Linux:
```bash
# Systemd logs
sudo journalctl -u cogesquiz -f

# Application logs
tail -f /var/www/cogesquiz/logs/app.log
```

Windows:
- Event Viewer → Applications
- IIS logs: `C:\inetpub\logs\LogFiles`

### Health Check Endpoint

Aggiungi in `Program.cs`:

```csharp
if (path == "/health")
{
    var health = new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        mongodb = IsMongoDBHealthy()
    };
    SendJsonResponse(response, health);
}
```

### Monitoring Tools

- **Prometheus + Grafana** per metriche
- **ELK Stack** per log aggregati
- **UptimeRobot** per uptime monitoring

---

## 🔄 Aggiornamenti

### Deploy Nuova Versione

```bash
# 1. Build nuova versione
dotnet publish -c Release -o ./publish_new

# 2. Backup vecchia versione
sudo mv /var/www/cogesquiz /var/www/cogesquiz_backup

# 3. Deploy nuova versione
sudo mv ./publish_new /var/www/cogesquiz

# 4. Restart servizio
sudo systemctl restart cogesquiz

# 5. Verifica
curl http://localhost/health
```

### Rollback

```bash
# Ripristina backup
sudo rm -rf /var/www/cogesquiz
sudo mv /var/www/cogesquiz_backup /var/www/cogesquiz
sudo systemctl restart cogesquiz
```

---

## 📞 Support

Per problemi di deployment:

1. Controlla i log
2. Verifica connettività MongoDB
3. Testa gli endpoint API manualmente
4. Controlla configurazione firewall

---

**Buon Deploy! 🚀**