# 🚀 Guia de Deployment - RPG Quest Manager

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Pré-requisitos](#-pré-requisitos)
- [Deployment Local](#-deployment-local)
- [Deployment com Docker](#-deployment-com-docker)
- [Deployment em Produção](#-deployment-em-produção)
- [Configuração de Ambiente](#-configuração-de-ambiente)
- [Monitoramento](#-monitoramento)
- [Backup e Restore](#-backup-e-restore)
- [Troubleshooting](#-troubleshooting)

## 🎯 Visão Geral

Este guia cobre diferentes métodos de deployment do RPG Quest Manager, desde desenvolvimento local até produção.

### Arquitetura de Deployment
```
[Frontend] → [API] → [Database]
    ↓           ↓         ↓
  React      .NET 8    PostgreSQL
```

## ⚙️ Pré-requisitos

### Desenvolvimento Local
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 15+](https://www.postgresql.org/)
- [Git](https://git-scm.com/)

### Produção
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)
- [Nginx](https://nginx.org/) (opcional)
- [SSL Certificate](https://letsencrypt.org/) (recomendado)

## 🏠 Deployment Local

### 1. Clone e Configuração
```bash
# Clone o repositório
git clone <repository-url>
cd rpg-quest-manager

# Configure o banco de dados
# Edite src/RpgQuestManager.Api/appsettings.json
```

### 2. Backend
```bash
cd src/RpgQuestManager.Api

# Restaurar dependências
dotnet restore

# Aplicar migrações
dotnet ef database update

# Executar aplicação
dotnet run
```

### 3. Frontend
```bash
cd frontend

# Instalar dependências
npm install

# Executar em desenvolvimento
npm run dev

# Build para produção
npm run build
```

### 4. Verificar
- Backend: http://localhost:5000
- Frontend: http://localhost:3000
- API: http://localhost:5000/api

## 🐳 Deployment com Docker

### 1. Docker Compose (Recomendado)
```bash
# Subir todos os serviços
docker-compose up -d

# Verificar status
docker-compose ps

# Ver logs
docker-compose logs -f
```

### 2. Docker Individual
```bash
# Build da API
cd src/RpgQuestManager.Api
docker build -t rpg-api .

# Build do Frontend
cd frontend
docker build -t rpg-frontend .

# Executar containers
docker run -d -p 5000:5000 --name rpg-api rpg-api
docker run -d -p 3000:3000 --name rpg-frontend rpg-frontend
```

### 3. Docker Compose Personalizado
```yaml
# docker-compose.custom.yml
version: '3.8'
services:
  db:
    image: postgres:15
    environment:
      POSTGRES_DB: rpgquestmanager
      POSTGRES_USER: rpguser
      POSTGRES_PASSWORD: rpgpass123
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build: ./src/RpgQuestManager.Api
    ports:
      - "5000:5000"
    depends_on:
      - db
    environment:
      - ConnectionStrings__Default=Host=db;Port=5432;Database=rpgquestmanager;Username=rpguser;Password=rpgpass123
      - ASPNETCORE_ENVIRONMENT=Production

  web:
    build: ./frontend
    ports:
      - "3000:3000"
    depends_on:
      - api
    environment:
      - VITE_API_BASE_URL=http://localhost:5000

volumes:
  postgres_data:
```

```bash
# Executar com arquivo customizado
docker-compose -f docker-compose.custom.yml up -d
```

## 🌐 Deployment em Produção

### 1. Preparação
```bash
# Clone em servidor de produção
git clone <repository-url>
cd rpg-quest-manager

# Configurar variáveis de ambiente
cp .env.example .env.production
# Editar .env.production
```

### 2. Configuração de Produção
```bash
# Backend - appsettings.Production.json
{
  "ConnectionStrings": {
    "Default": "Host=prod-db;Port=5432;Database=rpgquestmanager;Username=prod_user;Password=secure_password"
  },
  "Jwt": {
    "Key": "production-secret-key-here",
    "Issuer": "RpgQuestManager",
    "Audience": "RpgQuestManager"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

```bash
# Frontend - .env.production
VITE_API_BASE_URL=https://api.rpgquestmanager.com
VITE_APP_NAME=RPG Quest Manager
VITE_APP_VERSION=6.0.0
```

### 3. Build de Produção
```bash
# Backend
cd src/RpgQuestManager.Api
dotnet publish -c Release -o ./publish

# Frontend
cd frontend
npm run build
```

### 4. Deploy com Docker
```bash
# Build e deploy
docker-compose -f docker-compose.prod.yml up -d

# Verificar
docker-compose -f docker-compose.prod.yml ps
```

### 5. Nginx (Opcional)
```nginx
# /etc/nginx/sites-available/rpg-quest-manager
server {
    listen 80;
    server_name rpgquestmanager.com;

    # Frontend
    location / {
        proxy_pass http://localhost:3000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    # API
    location /api {
        proxy_pass http://localhost:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## 🔧 Configuração de Ambiente

### Variáveis de Ambiente

#### Backend
```bash
# appsettings.json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=rpgquestmanager;Username=rpguser;Password=rpgpass123"
  },
  "Jwt": {
    "Key": "your-secret-key-here",
    "Issuer": "RpgQuestManager",
    "Audience": "RpgQuestManager"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

#### Frontend
```bash
# .env.local
VITE_API_BASE_URL=http://localhost:5000
VITE_APP_NAME=RPG Quest Manager
VITE_APP_VERSION=6.0.0
```

#### Docker
```bash
# .env
POSTGRES_DB=rpgquestmanager
POSTGRES_USER=rpguser
POSTGRES_PASSWORD=rpgpass123
API_PORT=5000
WEB_PORT=3000
```

### Configurações de Segurança

#### JWT
```bash
# Gerar chave segura
openssl rand -base64 32
```

#### CORS
```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://rpgquestmanager.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

#### HTTPS
```bash
# Certificado SSL
certbot --nginx -d rpgquestmanager.com
```

## 📊 Monitoramento

### Logs
```bash
# Docker logs
docker-compose logs -f api
docker-compose logs -f web
docker-compose logs -f db

# Logs de aplicação
tail -f /var/log/rpg-quest-manager/api.log
tail -f /var/log/rpg-quest-manager/web.log
```

### Métricas
```bash
# Status dos containers
docker stats

# Uso de recursos
htop
df -h
free -h
```

### Health Checks
```bash
# API
curl http://localhost:5000/health

# Frontend
curl http://localhost:3000

# Database
psql -h localhost -U rpguser -d rpgquestmanager -c "SELECT 1;"
```

## 💾 Backup e Restore

### Backup do Banco
```bash
# Backup completo
pg_dump -h localhost -U rpguser -d rpgquestmanager > backup_$(date +%Y%m%d_%H%M%S).sql

# Backup apenas dados
pg_dump -h localhost -U rpguser -d rpgquestmanager --data-only > data_backup_$(date +%Y%m%d_%H%M%S).sql

# Backup apenas estrutura
pg_dump -h localhost -U rpguser -d rpgquestmanager --schema-only > schema_backup_$(date +%Y%m%d_%H%M%S).sql
```

### Restore do Banco
```bash
# Restore completo
psql -h localhost -U rpguser -d rpgquestmanager < backup_20240103_120000.sql

# Restore apenas dados
psql -h localhost -U rpguser -d rpgquestmanager < data_backup_20240103_120000.sql
```

### Backup Automático
```bash
#!/bin/bash
# backup.sh
DATE=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups"
DB_NAME="rpgquestmanager"
DB_USER="rpguser"
DB_HOST="localhost"

# Criar diretório de backup
mkdir -p $BACKUP_DIR

# Backup do banco
pg_dump -h $DB_HOST -U $DB_USER -d $DB_NAME > $BACKUP_DIR/backup_$DATE.sql

# Manter apenas os últimos 7 backups
find $BACKUP_DIR -name "backup_*.sql" -mtime +7 -delete

echo "Backup completed: backup_$DATE.sql"
```

```bash
# Adicionar ao crontab
crontab -e
# 0 2 * * * /path/to/backup.sh
```

## 🔍 Troubleshooting

### Problemas Comuns

#### 1. Erro de Conexão com Banco
```bash
# Verificar se PostgreSQL está rodando
systemctl status postgresql

# Verificar conexão
psql -h localhost -U rpguser -d rpgquestmanager

# Verificar logs
tail -f /var/log/postgresql/postgresql-15-main.log
```

#### 2. Erro de Build
```bash
# Limpar cache do .NET
dotnet clean
dotnet restore

# Limpar cache do npm
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

#### 3. Erro de Porta em Uso
```bash
# Verificar portas em uso
netstat -tulpn | grep :5000
netstat -tulpn | grep :3000

# Matar processo
kill -9 <PID>
```

#### 4. Erro de Permissão
```bash
# Ajustar permissões
chmod +x scripts/*.sh
chown -R www-data:www-data /var/www/rpg-quest-manager
```

### Logs de Debug
```bash
# Backend
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --verbosity detailed

# Frontend
npm run dev -- --debug

# Docker
docker-compose logs -f --tail=100
```

### Comandos Úteis
```bash
# Reiniciar serviços
docker-compose restart
systemctl restart nginx
systemctl restart postgresql

# Verificar status
docker-compose ps
systemctl status nginx
systemctl status postgresql

# Limpar containers
docker-compose down
docker system prune -a

# Verificar espaço em disco
df -h
du -sh /var/lib/docker
```

## 🚀 Deploy Automatizado

### GitHub Actions
```yaml
# .github/workflows/deploy.yml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Deploy to server
      uses: appleboy/ssh-action@v0.1.5
      with:
        host: ${{ secrets.HOST }}
        username: ${{ secrets.USERNAME }}
        key: ${{ secrets.SSH_KEY }}
        script: |
          cd /var/www/rpg-quest-manager
          git pull origin main
          docker-compose -f docker-compose.prod.yml up -d --build
```

### Script de Deploy
```bash
#!/bin/bash
# deploy.sh
set -e

echo "Starting deployment..."

# Pull latest changes
git pull origin main

# Build and deploy
docker-compose -f docker-compose.prod.yml up -d --build

# Wait for services to be ready
sleep 30

# Run health checks
curl -f http://localhost:5000/health || exit 1
curl -f http://localhost:3000 || exit 1

echo "Deployment completed successfully!"
```

## 📋 Checklist de Deploy

### Pré-Deploy
- [ ] Testes passando
- [ ] Build local funcionando
- [ ] Variáveis de ambiente configuradas
- [ ] Backup do banco realizado
- [ ] Certificados SSL válidos

### Deploy
- [ ] Código atualizado
- [ ] Migrações aplicadas
- [ ] Serviços reiniciados
- [ ] Health checks passando
- [ ] Logs sem erros

### Pós-Deploy
- [ ] Funcionalidades testadas
- [ ] Performance verificada
- [ ] Monitoramento ativo
- [ ] Backup agendado
- [ ] Documentação atualizada

---

**Para suporte adicional, consulte o README.md ou abra uma issue no GitHub.**
