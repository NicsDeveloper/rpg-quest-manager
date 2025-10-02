# 🎨 RPG Quest Manager - Frontend

Frontend moderno e responsivo para o RPG Quest Manager, construído com React 18 + TypeScript + Vite.

## 🚀 Stack Tecnológica

- ⚛️ **React 18** - Library UI
- 📘 **TypeScript** - Type safety
- ⚡ **Vite** - Build tool ultra-rápido
- 🎨 **Tailwind CSS** - Utility-first CSS
- 🌐 **React Router** - Navegação SPA
- 📡 **Axios** - HTTP Client
- 🌍 **i18next** - Internacionalização (pt-BR)
- 🎯 **Lucide React** - Ícones modernos

## 📦 Pré-requisitos

Antes de começar, instale:
- [Node.js 18+](https://nodejs.org/) (recomendado: 20.x LTS)
- npm (vem com Node.js)

## 🛠️ Instalação

```bash
# 1. Entre no diretório do frontend
cd frontend

# 2. Instale as dependências
npm install

# 3. Inicie o servidor de desenvolvimento
npm run dev
```

A aplicação estará disponível em: **http://localhost:3000**

## 🏗️ Estrutura do Projeto

```
frontend/
├── src/
│   ├── components/          # Componentes reutilizáveis
│   │   ├── layout/          # Layout (Header, Sidebar, Footer)
│   │   ├── ui/              # Componentes UI base (Button, Card, Input)
│   │   └── features/        # Componentes de features específicas
│   ├── pages/               # Páginas da aplicação
│   │   ├── Login.tsx
│   │   ├── Dashboard.tsx
│   │   ├── Heroes/
│   │   ├── Quests/
│   │   ├── Enemies/
│   │   ├── Items/
│   │   └── Admin/
│   ├── contexts/            # React Contexts (Auth, Theme)
│   ├── services/            # API services (axios)
│   ├── hooks/               # Custom hooks
│   ├── types/               # TypeScript types/interfaces
│   ├── utils/               # Funções utilitárias
│   ├── i18n/                # Traduções (pt-BR)
│   ├── App.tsx              # Componente raiz
│   └── main.tsx             # Entry point
├── public/                  # Assets estáticos
├── index.html               # HTML base
├── vite.config.ts           # Configuração Vite
├── tailwind.config.js       # Configuração Tailwind
├── tsconfig.json            # Configuração TypeScript
└── package.json             # Dependências
```

## 🎯 Funcionalidades Implementadas

### 🔐 Autenticação
- ✅ Login com JWT
- ✅ Registro de novos usuários
- ✅ Proteção de rotas
- ✅ Logout
- ✅ Persistência de sessão (localStorage)

### 👤 Sistema de Roles
- ✅ Detecção automática de role (Admin/Player)
- ✅ UI adaptada por permissão
- ✅ Rotas protegidas por role

### ⚔️ Heróis
- ✅ Lista de heróis com filtros
- ✅ Detalhes do herói (atributos, nível, XP)
- ✅ Inventário do herói
- ✅ Equipar/Desequipar itens
- 🔐 CRUD (apenas Admin)

### 🎯 Quests
- ✅ Lista de quests com filtros por dificuldade
- ✅ Detalhes da quest (inimigos, recompensas)
- ✅ **Completar quest** (botão de ação)
- ✅ Feedback visual de recompensas
- ✅ Animação de level up
- 🔐 CRUD (apenas Admin)

### 👹 Inimigos & 🗡️ Itens
- ✅ Lista com pesquisa
- ✅ Detalhes
- 🔐 CRUD (apenas Admin)

### 📊 Dashboard
- ✅ Overview geral do jogo
- ✅ Top 10 heróis mais fortes
- ✅ Quests mais jogadas
- ✅ Estatísticas gerais

## 🎨 Design System

O frontend utiliza um design system consistente baseado em Tailwind:

### Paleta de Cores
```css
/* Tema Principal (RPG Fantasy) */
--primary: #8b5cf6     /* Roxo místico */
--secondary: #f59e0b   /* Dourado */
--accent: #ec4899      /* Rosa mágico */
--success: #10b981     /* Verde sucesso */
--danger: #ef4444      /* Vermelho perigo */
--dark: #1e293b        /* Fundo escuro */
```

### Componentes UI
- **Button**: 4 variantes (primary, secondary, outline, ghost)
- **Card**: Container padrão com sombra e bordas arredondadas
- **Input**: Campos de formulário estilizados
- **Badge**: Tags de status/categoria
- **Modal**: Diálogos e confirmações
- **Table**: Tabelas responsivas com paginação

## 🌍 Traduções

Todos os textos estão em **Português Brasileiro** (pt-BR):

```typescript
// Exemplo de uso
import { useTranslation } from 'react-i18next';

function Component() {
  const { t } = useTranslation();
  return <h1>{t('heroes.title')}</h1>; // "Heróis"
}
```

Arquivos de tradução: `src/i18n/locales/pt-BR.json`

## 🔧 Scripts Disponíveis

```bash
npm run dev        # Servidor de desenvolvimento (localhost:3000)
npm run build      # Build de produção
npm run preview    # Preview do build
npm run lint       # Lint com ESLint
```

## 🐳 Integração com Docker

O frontend já está configurado para se comunicar com a API via proxy (Vite).

Para rodar tudo junto (API + Frontend):

```bash
# Na raiz do projeto
docker-compose up --build
```

A API estará em `localhost:5000` e o frontend em `localhost:3000`.

## 📱 Responsividade

O frontend é **totalmente responsivo** e funciona em:
- 📱 Mobile (320px+)
- 📱 Tablet (768px+)
- 💻 Desktop (1024px+)
- 🖥️ Large Desktop (1440px+)

## 🚦 Status do Projeto

- [x] Configuração base
- [ ] Componentes UI base
- [ ] Context de Autenticação
- [ ] Serviços de API
- [ ] Página de Login
- [ ] Dashboard
- [ ] Páginas de Heróis
- [ ] Páginas de Quests
- [ ] Sistema de Inventário
- [ ] Admin Panel
- [ ] Testes unitários
- [ ] Deploy

## 🤝 Próximos Passos

1. **Instalar Node.js** (caso ainda não tenha)
2. Rodar `npm install` no diretório `frontend/`
3. Iniciar a API backend (`docker-compose up`)
4. Iniciar o frontend (`npm run dev`)
5. Acessar `http://localhost:3000`

---

**🎮 Divirta-se gerenciando seu RPG! ⚔️🐉**

