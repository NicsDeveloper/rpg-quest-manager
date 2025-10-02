# 🚀 Quick Start - RPG Quest Manager

Guia rápido para executar o projeto completo (Backend + Frontend).

## 📋 Pré-requisitos

- **Docker Desktop** (para backend, PostgreSQL, Redis, RabbitMQ)
- **Node.js 22+** (para frontend)
- **.NET 8 SDK** (opcional, apenas para desenvolvimento do backend)

---

## 🎮 Executar o Projeto Completo

### 1️⃣ Iniciar o Backend (API + Banco + Redis + RabbitMQ)

```bash
docker-compose up -d
```

Aguarde alguns segundos para os containers iniciarem. Você pode verificar com:

```bash
docker-compose ps
```

A API estará disponível em: **http://localhost:5000**

Swagger UI: **http://localhost:5000/swagger**

---

### 2️⃣ Iniciar o Frontend

```bash
cd frontend
npm install    # Apenas na primeira vez
npm run dev
```

O frontend estará disponível em: **http://localhost:3000**

---

## 👤 Usuários de Teste (Seeder)

O banco já vem populado com dados de teste:

### Admin (acesso total):
- **Username:** `admin`
- **Password:** `admin123`

### Player (acesso limitado):
- **Username:** `player1`
- **Password:** `senha123`

**Ou** registre um novo usuário pela tela de cadastro (será criado como Player).

---

## 🧭 Navegação do Frontend

Após fazer login, você terá acesso a:

1. **Dashboard** - Estatísticas gerais, heróis mais fortes, missões populares
2. **Heróis** - Visualizar, criar (admin), editar (admin), deletar (admin)
3. **Missões** - Visualizar, completar, criar (admin), editar (admin), deletar (admin)
4. **Itens** - Visualizar, criar (admin), deletar (admin)
5. **Inimigos** - Visualizar, criar (admin), editar (admin), deletar (admin) - **Apenas Admin**

---

## 🔐 Diferenças entre Roles

### Player (Usuário comum)
✅ Visualizar heróis, missões, itens, inimigos
✅ Completar missões com seus heróis
❌ Não pode criar/editar/deletar nada

### Admin (Administrador)
✅ Tudo que o Player pode fazer
✅ Criar, editar e deletar heróis
✅ Criar, editar e deletar missões
✅ Criar e deletar itens
✅ Criar, editar e deletar inimigos
✅ Acesso à página de Inimigos

---

## 🎯 Fluxo de Uso Típico

1. **Login** com `admin` / `admin123`
2. Vá para **Dashboard** e veja as estatísticas
3. Acesse **Heróis** e veja os heróis existentes (Thorin, Elara, Grimgar, etc.)
4. Acesse **Missões** e clique em **Completar Missão**
5. Selecione um herói (ex: Thorin)
6. O herói ganha:
   - 🪙 Ouro
   - ⭐ Experiência (pode subir de nível!)
   - 📦 Item da recompensa (adicionado ao inventário automaticamente)
7. Acesse **Itens** e veja todos os itens disponíveis
8. Crie novos itens, missões ou heróis como Admin

---

## 🛠️ Comandos Úteis

### Backend (Docker)
```bash
docker-compose up -d          # Inicia todos os serviços
docker-compose down           # Para e remove os containers
docker-compose logs api       # Ver logs da API
docker-compose restart api    # Reinicia a API
```

### Frontend
```bash
npm run dev      # Servidor de desenvolvimento (http://localhost:3000)
npm run build    # Build de produção
npm run preview  # Preview do build de produção
```

### Banco de Dados
```bash
docker exec -it rpg-postgres psql -U postgres -d rpgquestdb
```

---

## 📊 Portas Utilizadas

| Serviço     | Porta  | URL                        |
|-------------|--------|----------------------------|
| Frontend    | 3000   | http://localhost:3000      |
| API         | 5000   | http://localhost:5000      |
| Swagger     | 5000   | http://localhost:5000/swagger |
| PostgreSQL  | 5432   | localhost:5432             |
| Redis       | 6379   | localhost:6379             |
| RabbitMQ    | 5672   | localhost:5672             |
| RabbitMQ UI | 15672  | http://localhost:15672     |

---

## 🐛 Troubleshooting

### Erro: "Cannot connect to Docker"
- Certifique-se de que o Docker Desktop está rodando

### Erro: "Port already in use"
- Verifique se não há outros serviços usando as portas 3000, 5000, 5432, 6379
- Ou altere as portas no `docker-compose.yml` e `vite.config.ts`

### Frontend não conecta com a API
- Verifique se o backend está rodando: `docker-compose ps`
- O proxy do Vite está configurado para `/api` → `http://localhost:5000`

### Erro 401 (Unauthorized) no frontend
- Faça logout e login novamente
- O token JWT expira após 7 dias

---

## 📚 Documentação Completa

- **Backend:** Ver `README.md` na raiz do projeto
- **Frontend:** Ver `frontend/README.md`
- **API Swagger:** http://localhost:5000/swagger
- **Postman Collection:** Ver `postman/` na raiz

---

## 🎨 Tecnologias Utilizadas

### Backend
- **.NET 8** + **C#**
- **PostgreSQL** (banco de dados)
- **Entity Framework Core** (ORM)
- **Redis** (cache)
- **RabbitMQ + MassTransit** (mensageria)
- **JWT** (autenticação)
- **Serilog** (logs)
- **Swagger** (documentação)
- **FluentValidation** (validações)
- **AutoMapper** (mapeamento)
- **xUnit** (testes)

### Frontend
- **React 18** + **TypeScript**
- **Vite** (build tool)
- **Tailwind CSS** (estilização)
- **React Router** (rotas)
- **Axios** (HTTP client)
- **i18next** (internacionalização pt-BR/EN)

---

## ⚡ Performance

- Frontend: Bundle otimizado de **~295KB** (92KB gzipped)
- Backend: Cache Redis para heróis mais fortes e missões mais jogadas
- Eventos assíncronos com RabbitMQ para conclusão de missões

---

## 🎉 Próximos Passos

1. Explore todas as funcionalidades do CRUD
2. Complete missões e veja os heróis evoluindo
3. Crie seus próprios heróis, itens e missões personalizados
4. Teste diferentes combinações de atributos e recompensas
5. Use o Swagger para testar a API diretamente

**Divirta-se! 🎮✨**

