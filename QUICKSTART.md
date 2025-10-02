# 🚀 Quick Start - RPG Quest Manager

Guia rápido para executar o projeto completo (Backend + Frontend) com **Docker Compose**.

## 📋 Pré-requisitos

- **Docker Desktop** instalado e rodando

Isso é TUDO que você precisa! 🎉

---

## 🎮 Executar o Projeto Completo (1 comando!)

### 🚀 Iniciar TUDO (API + Frontend + Banco + Redis + RabbitMQ)

```bash
# Clone o repositório
git clone <url-do-repo>
cd rpg-quest-manager

# Inicie todos os containers
docker-compose up -d --build
```

Aguarde ~1-2 minutos para todos os serviços iniciarem. Você pode verificar o status com:

```bash
docker-compose ps
```

### 🌐 Acessos

| Serviço | URL | Descrição |
|---------|-----|-----------|
| **Frontend** | http://localhost:3000 | ⭐ **COMECE AQUI!** Interface React |
| **Swagger UI** | http://localhost:5000/swagger | Documentação interativa da API |
| **API REST** | http://localhost:5000/api/v1 | Backend .NET 8 |
| **RabbitMQ** | http://localhost:15672 | Management UI (guest/guest) |

---

## 👤 Usuários de Teste (Seeder)

O banco já vem populado com dados de teste:

### 🛡️ Admin (acesso total):
- **Username:** `admin`
- **Password:** `admin123`
- **Permissões**: Criar/editar/deletar heróis, missões, itens, completar missões

### 🎮 Player (acesso limitado):
- **Username:** `player1`
- **Password:** `senha123`
- **Permissões**: Ver seu perfil, aceitar missões, visualizar catálogo
- **Herói vinculado**: Aragorn (Guerreiro, Nv. 15)

### 🎮 Player 2:
- **Username:** `gamer`
- **Password:** `senha456`
- **Permissões**: Ver seu perfil, aceitar missões, visualizar catálogo
- **Herói vinculado**: Gandalf (Mago, Nv. 20)

**Ou** registre um novo usuário pela tela de cadastro (será criado como Player sem herói).

---

## 🎯 Fluxo de Uso

### Para Players (Jogadores):

1. **Login** com `player1` ou `gamer`
2. **Tutorial Interativo** aparecerá automaticamente (primeira vez)
3. **Dashboard**: Veja estatísticas gerais do jogo
4. **Meu Perfil**: Veja seu herói, atributos, inventário e missões completadas
5. **Catálogo de Missões**: 
   - Aba "Catálogo": Veja todas as missões disponíveis e seus requisitos
   - Aba "Minhas Missões": Veja missões que você já aceitou
   - Aceite missões compatíveis com seu nível e classe
6. **Notificações** 🔔: Fique de olho no sino no topo para ver quando subir de nível e receber avisos de novas missões

### Para Admin (Administrador):

1. **Login** com `admin`
2. **Dashboard**: Veja estatísticas gerais
3. **Heróis**: Criar, editar, deletar heróis
4. **Missões (Admin Panel)**: Criar, editar, deletar, **completar missões** para jogadores
5. **Inimigos**: Gerenciar inimigos do jogo
6. **Itens**: Criar e gerenciar itens e equipamentos

---

## 📦 Containers Docker

Quando você executa `docker-compose up -d`, os seguintes containers são criados:

| Container | Porta | Descrição |
|-----------|-------|-----------|
| `rpg-postgres` | 5432 | PostgreSQL 15 (banco de dados) |
| `rpg-redis` | 6379 | Redis 7 (cache) |
| `rpg-rabbitmq` | 5672, 15672 | RabbitMQ 3 (mensageria) |
| `rpg-api` | 5000, 5001 | API .NET 8 (backend) |
| `rpg-frontend` | 3000 | React 18 + Nginx (frontend) |

Todos os containers estão na mesma rede Docker (`rpg-network`) e se comunicam entre si.  
O frontend utiliza **Nginx** como proxy reverso para comunicar com a API.

---

## 🆕 Sistema de Tutorial

Na **primeira vez que um novo jogador faz login**, um **tutorial interativo** aparece automaticamente!

O tutorial possui **7 passos** explicando:
- ✨ Como funciona o sistema de heróis
- 📚 Como aceitar missões no catálogo
- 🎯 Como completar missões
- 🎊 Sistema de level up e recompensas
- 🔔 Sistema de notificações
- 🚀 Dicas para começar

Você pode pular o tutorial a qualquer momento, mas é recomendado assistir na primeira vez!

---

## 🎮 Funcionalidades Principais

### 🦸 Sistema de Heróis
- Cada usuário pode ter um herói vinculado
- Atributos: Força, Inteligência, Destreza
- Sistema de nível e experiência (XP)
- Inventário de itens e equipamentos

### 🎯 Sistema de Missões
- **Catálogo Público**: Todos os jogadores podem ver as missões disponíveis
- **Requisitos**: Missões têm requisitos de nível e classe (Guerreiro, Mago, Arqueiro, ou "Any")
- **Aceitar Missão**: Jogadores podem aceitar missões que atendem aos requisitos
- **Completar Missão**: Apenas admins podem completar missões (simulando progresso do jogo)
- **Recompensas**: Ouro, XP e itens são adicionados automaticamente ao inventário

### 📈 Sistema de Level Up
- Quando um herói completa missões e ganha XP suficiente, ele sobe de nível
- **Recompensas automáticas**: +2 em todos os atributos, ouro extra
- **Notificação**: Jogador recebe uma notificação mostrando o novo nível e novas missões disponíveis
- **Acesso a novas missões**: Missões de nível mais alto ficam disponíveis

### 🔔 Sistema de Notificações
- Notificações aparecem em tempo real no sino 🔔 no topo da tela
- Avisos de level up com detalhes das recompensas
- Lista de novas missões disponíveis após subir de nível
- Marcar como lida individual ou todas de uma vez

### 🎁 Sistema de Inventário e Itens
- Itens são adicionados automaticamente ao completar missões
- Equipamentos têm bônus de atributos (Força, Inteligência, Destreza)
- Quantidade de itens é gerenciada automaticamente (stacking)
- Visualize todo o inventário no "Meu Perfil"

### 🔐 Sistema de Autenticação e Permissões
- **JWT Authentication**: Login seguro com tokens
- **Roles**: Admin e Player com permissões diferentes
- **Admin**: Acesso total ao sistema (CRUD de heróis, missões, itens, completar missões)
- **Player**: Acesso ao seu perfil, catálogo de missões, aceitar missões

---

## 🛠️ Comandos Úteis

### Parar todos os containers:
```bash
docker-compose down
```

### Ver logs em tempo real:
```bash
# Todos os serviços
docker-compose logs -f

# Apenas a API
docker-compose logs -f api

# Apenas o frontend
docker-compose logs -f frontend
```

### Rebuild completo (após mudanças no código):
```bash
docker-compose down
docker-compose up -d --build
```

### Limpar volumes (ATENÇÃO: apaga dados do banco):
```bash
docker-compose down -v
docker-compose up -d --build
```

---

## 🧪 Testar a API (Swagger)

1. Acesse http://localhost:5000/swagger
2. Clique em **Authorize** (🔒)
3. Faça login com `admin` / `admin123` via endpoint `/api/v1/auth/login`
4. Copie o token retornado
5. Cole no campo "Value" do Authorize: `Bearer {seu-token}`
6. Teste os endpoints diretamente no Swagger!

---

## 🔥 Desenvolvimento Local (sem Docker)

Se você quiser rodar o backend ou frontend **fora do Docker** para desenvolvimento:

### Backend (API):
```bash
cd src/RpgQuestManager.Api
dotnet run
```

### Frontend:
```bash
cd frontend
npm install
npm run dev
```

Neste caso, configure as connection strings em `appsettings.Development.json` para apontar para `localhost` ao invés de nomes de containers.

---

## 📚 Mais Documentação

Para documentação completa, veja o [README.md](README.md) principal.

---

## 🆘 Troubleshooting

### Frontend não carrega:
- Verifique se o container `rpg-frontend` está rodando: `docker-compose ps`
- Veja os logs: `docker-compose logs -f frontend`
- Acesse diretamente a API: http://localhost:5000/swagger

### API não responde:
- Verifique se o container `rpg-api` está healthy: `docker-compose ps`
- Veja os logs: `docker-compose logs -f api`
- Certifique-se que o PostgreSQL está rodando: `docker-compose ps postgres`

### Erro ao fazer login:
- Certifique-se que usou os usuários corretos do seeder
- Verifique se o banco foi populado: `docker-compose logs api | grep "Seed"`

### Docker não tem espaço:
```bash
docker system prune -a --volumes
```

---

## 🎉 Pronto!

Agora você tem um **RPG Quest Manager completo** rodando localmente! 🚀

Explore, teste, e divirta-se gerenciando sua aventura épica! ⚔️🛡️✨
