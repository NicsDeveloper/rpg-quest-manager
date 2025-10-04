# 🛡️ Guia de Diretrizes para Agentes IA - RPG Quest Manager

## ⚠️ REGRAS CRÍTICAS - LEIA PRIMEIRO

### 🚫 NUNCA FAÇA
1. **NÃO modifique arquivos de migração existentes** - Isso pode quebrar o banco de dados
2. **NÃO delete arquivos sem confirmação explícita** - Sempre pergunte antes
3. **NÃO altere a estrutura do banco sem criar nova migração** - Use `dotnet ef migrations add`
4. **NÃO modifique arquivos de configuração Docker** sem entender o impacto
5. **NÃO altere dependências** sem testar completamente
6. **NÃO duplique código** - Sempre reutilize componentes existentes
7. **NÃO quebre a compatibilidade** com APIs existentes

### ✅ SEMPRE FAÇA
1. **SEMPRE teste mudanças** com `npm run build` e `dotnet build`
2. **SEMPRE verifique lints** com `read_lints` antes de finalizar
3. **SEMPRE atualize documentação** quando fizer mudanças
4. **SEMPRE use TypeScript** para tipagem forte
5. **SEMPRE siga padrões existentes** de nomenclatura e estrutura
6. **SEMPRE crie componentes reutilizáveis** quando possível
7. **SEMPRE mantenha consistência** com o design system existente

## 📋 CHECKLIST OBRIGATÓRIO

Antes de qualquer mudança:
- [ ] Li o README.md completo?
- [ ] Entendi a arquitetura do projeto?
- [ ] Verifiquei se já existe funcionalidade similar?
- [ ] Testei localmente?
- [ ] Atualizei a documentação?
- [ ] Verifiquei compatibilidade com APIs existentes?

## 🏗️ ARQUITETURA DO PROJETO

### Backend (.NET 8)
```
src/RpgQuestManager.Api/
├── Controllers/          # Endpoints da API
├── Models/              # Entidades do banco
├── Services/            # Lógica de negócio
├── Data/               # Contexto do banco e seeders
├── Migrations/         # Migrações do Entity Framework
└── Program.cs          # Configuração da aplicação
```

### Frontend (React + TypeScript)
```
frontend/src/
├── components/         # Componentes reutilizáveis
│   ├── ui/            # Componentes base (Button, Card, etc.)
│   ├── animations/    # Componentes de animação
│   └── ...           # Componentes específicos
├── pages/             # Páginas da aplicação
├── contexts/          # Contextos React (Auth, Character)
├── services/          # Serviços para comunicação com API
└── App.tsx           # Componente principal
```

## 🔧 PADRÕES DE CÓDIGO

### Backend (C#)
- Use **async/await** para operações assíncronas
- Siga o padrão **Repository/Service**
- Valide inputs com **Data Annotations**
- Use **Entity Framework** para acesso a dados
- Implemente **Dependency Injection**

### Frontend (TypeScript/React)
- Use **hooks** para gerenciamento de estado
- Implemente **TypeScript** com tipagem forte
- Use **Tailwind CSS** para estilização
- Siga padrões **React** modernos
- Implemente **error boundaries** quando necessário

## 🎨 DESIGN SYSTEM

### Componentes Base
- `Button` - Botões com variantes (primary, secondary, danger)
- `Card` - Containers com título opcional
- `Modal` - Modais com tamanhos (sm, md, lg, xl)
- `ProgressBar` - Barras de progresso
- `FadeIn`, `SlideIn` - Animações

### Cores e Estilos
- Use classes **Tailwind CSS** existentes
- Mantenha consistência com paleta atual
- Use **Lucide React** para ícones
- Implemente **responsividade** mobile-first

## 🗄️ BANCO DE DADOS

### Entidades Principais
- `User` - Usuários do sistema
- `Character` - Personagens dos jogadores
- `Monster` - Monstros para combate
- `Quest` - Missões do jogo
- `Item` - Itens do inventário
- `Achievement` - Conquistas
- `Party` - Grupos multiplayer
- `SpecialAbility` - Habilidades especiais

### Migrações
- **NUNCA** modifique migrações existentes
- Use `dotnet ef migrations add NomeDaMigracao`
- Teste migrações antes de aplicar
- Documente mudanças no banco

## 🔌 APIs E ENDPOINTS

### Estrutura de Endpoints
```
/api/auth/*          # Autenticação
/api/characters/*    # Personagens
/api/combat/*        # Sistema de combate
/api/quests/*        # Missões
/api/inventory/*     # Inventário
/api/shop/*          # Loja
/api/achievements/*  # Conquistas
/api/parties/*       # Grupos
/api/abilities/*     # Habilidades especiais
```

### Padrões de Response
- Use **DTOs** para responses
- Implemente **error handling** consistente
- Retorne **status codes** apropriados
- Documente endpoints com **XML comments**

## 🧪 TESTES E QUALIDADE

### Verificações Obrigatórias
1. **Build** - `npm run build` e `dotnet build`
2. **Lints** - `read_lints` para frontend
3. **TypeScript** - Verificar tipos
4. **Funcionalidade** - Testar manualmente
5. **Performance** - Verificar impactos

### Ferramentas de Desenvolvimento
- **Docker Compose** para ambiente local
- **Entity Framework** para banco de dados
- **Vite** para build do frontend
- **Tailwind CSS** para estilização

## 📚 DOCUMENTAÇÃO

### Arquivos de Documentação
- `README.md` - Documentação principal
- `AGENT_GUIDELINES.md` - Este arquivo
- Comentários no código
- Documentação de APIs

### Atualizações Obrigatórias
- **SEMPRE** atualize README.md quando:
  - Adicionar novas funcionalidades
  - Modificar APIs
  - Alterar estrutura do projeto
  - Adicionar dependências
  - Modificar configurações

## 🚀 DEPLOYMENT

### Ambiente Local
```bash
# Backend
cd src/RpgQuestManager.Api
dotnet run

# Frontend
cd frontend
npm run dev

# Docker
docker-compose up
```

### Variáveis de Ambiente
- `ConnectionStrings__Default` - String de conexão do banco
- Configurações no `appsettings.json`

## 🔄 FLUXO DE DESENVOLVIMENTO

### Para Novas Funcionalidades
1. **Analise** requisitos e arquitetura existente
2. **Planeje** implementação (backend + frontend)
3. **Implemente** seguindo padrões existentes
4. **Teste** completamente
5. **Documente** mudanças
6. **Atualize** tutorial se necessário

### Para Correções
1. **Identifique** causa raiz
2. **Teste** solução localmente
3. **Implemente** correção mínima
4. **Verifique** não quebrar outras funcionalidades
5. **Documente** correção

## 🎯 FOCO EM QUALIDADE

### Código Limpo
- Nomes descritivos
- Funções pequenas e focadas
- Comentários quando necessário
- Evite duplicação
- Siga princípios SOLID

### Performance
- Use cache quando apropriado
- Otimize queries de banco
- Minimize re-renders no React
- Use lazy loading quando possível

### Segurança
- Valide inputs
- Use autenticação JWT
- Implemente autorização
- Sanitize dados

## 🆘 EM CASO DE DÚVIDAS

### Antes de Implementar
1. **Leia** documentação existente
2. **Analise** código similar
3. **Pesquise** padrões do projeto
4. **Consulte** este guia
5. **Pergunte** se ainda tiver dúvidas

### Recursos de Referência
- README.md principal
- Código existente
- Documentação de tecnologias
- Este guia de diretrizes

## 📝 EXEMPLOS DE BOAS PRÁTICAS

### ✅ Correto
```typescript
// Componente reutilizável
export function Button({ variant = 'primary', children, ...props }: ButtonProps) {
  return (
    <button 
      className={`btn btn-${variant}`}
      {...props}
    >
      {children}
    </button>
  );
}

// Hook customizado
export function useCharacter() {
  const [character, setCharacter] = useState<Character | null>(null);
  // ... lógica
  return { character, setCharacter };
}
```

### ❌ Incorreto
```typescript
// Duplicação de código
function Button1() { return <button className="btn-primary">Click</button>; }
function Button2() { return <button className="btn-primary">Click</button>; }

// Sem tipagem
function getData() {
  return fetch('/api/data').then(r => r.json());
}
```

## 🎮 CONTEXTO DO JOGO

### Sistema de Combate
- Turnos alternados
- Dados por dificuldade
- Sistema de moral
- Status effects com duração
- Habilidades especiais

### Sistema de Progressão
- Níveis e experiência
- Equipamentos e stats
- Conquistas
- Missões e recompensas

### Multiplayer
- Grupos e convites
- Combate cooperativo
- Chat em tempo real (futuro)

## 🔮 FUTURAS EXPANSÕES

### Funcionalidades Planejadas
- Sistema de guilds
- Eventos especiais
- Mais ambientes
- Novos tipos de monstros
- Sistema de crafting

### Considerações
- Manter compatibilidade
- Planejar escalabilidade
- Documentar mudanças
- Testar thoroughly

---

## ⚡ RESUMO EXECUTIVO

**REGRA DE OURO**: Se você não tem certeza, **PERGUNTE**. É melhor ser cauteloso do que quebrar algo.

**FOCO**: Manter a qualidade, consistência e funcionalidade do projeto.

**OBJETIVO**: Expandir o RPG Quest Manager de forma segura e eficiente.

**LEMBRE-SE**: Este é um projeto em produção. Toda mudança deve ser pensada, testada e documentada.
