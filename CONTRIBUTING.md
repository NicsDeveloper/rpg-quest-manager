# 🤝 Guia de Contribuição - RPG Quest Manager

## 📋 Índice

- [Como Contribuir](#-como-contribuir)
- [Configuração do Ambiente](#-configuração-do-ambiente)
- [Padrões de Código](#-padrões-de-código)
- [Fluxo de Desenvolvimento](#-fluxo-de-desenvolvimento)
- [Testes](#-testes)
- [Documentação](#-documentação)
- [Pull Requests](#-pull-requests)
- [Code Review](#-code-review)

## 🚀 Como Contribuir

### 1. Fork e Clone
```bash
# Fork o repositório no GitHub
# Clone seu fork
git clone https://github.com/SEU_USUARIO/rpg-quest-manager.git
cd rpg-quest-manager
```

### 2. Configurar Remote
```bash
# Adicionar remote original
git remote add upstream https://github.com/ORIGINAL_OWNER/rpg-quest-manager.git

# Verificar remotes
git remote -v
```

### 3. Criar Branch
```bash
# Atualizar main
git checkout main
git pull upstream main

# Criar branch para feature
git checkout -b feature/nova-funcionalidade
```

## ⚙️ Configuração do Ambiente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://www.docker.com/) (opcional)
- [Git](https://git-scm.com/)

### Setup Local
```bash
# Backend
cd src/RpgQuestManager.Api
dotnet restore
dotnet ef database update
dotnet run

# Frontend
cd frontend
npm install
npm run dev

# Docker (alternativo)
docker-compose up
```

### Verificar Instalação
```bash
# Backend
dotnet --version
dotnet ef --version

# Frontend
node --version
npm --version

# Docker
docker --version
docker-compose --version
```

## 📝 Padrões de Código

### Backend (C#)

#### Nomenclatura
```csharp
// Classes: PascalCase
public class UserService { }

// Métodos: PascalCase
public async Task<User> GetUserAsync(int id) { }

// Propriedades: PascalCase
public string Name { get; set; }

// Variáveis locais: camelCase
var userData = await GetUserAsync(id);

// Constantes: PascalCase
public const string DefaultConnectionString = "...";

// Campos privados: _camelCase
private readonly ApplicationDbContext _context;
```

#### Estrutura de Arquivos
```
Controllers/
├── AuthController.cs
├── CharactersController.cs
└── ...

Services/
├── IAuthService.cs
├── AuthService.cs
└── ...

Models/
├── User.cs
├── Character.cs
└── ...
```

#### Exemplo de Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}
```

#### Exemplo de Service
```csharp
public interface IUserService
{
    Task<User> GetUserAsync(int id);
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<bool> UpdateUserAsync(int id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(int id);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Characters)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    // ... outros métodos
}
```

### Frontend (TypeScript/React)

#### Nomenclatura
```typescript
// Componentes: PascalCase
export function UserProfile() { }

// Hooks: camelCase
const useUserData = () => { };

// Funções: camelCase
const getUserData = async () => { };

// Variáveis: camelCase
const userData = await getUserData();

// Constantes: UPPER_SNAKE_CASE
const API_BASE_URL = 'http://localhost:5000';

// Interfaces: PascalCase
interface User {
  id: number;
  name: string;
}

// Types: PascalCase
type UserStatus = 'active' | 'inactive';
```

#### Estrutura de Componentes
```typescript
// UserProfile.tsx
import { useState, useEffect } from 'react';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { useUser } from '../contexts/UserContext';

interface UserProfileProps {
  userId: number;
  onUpdate?: (user: User) => void;
}

export function UserProfile({ userId, onUpdate }: UserProfileProps) {
  const { user, updateUser } = useUser();
  const [loading, setLoading] = useState(false);

  const handleUpdate = async () => {
    setLoading(true);
    try {
      await updateUser(userId, { name: 'New Name' });
      onUpdate?.(user);
    } catch (error) {
      console.error('Error updating user:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card title="User Profile">
      <div className="space-y-4">
        <p>Name: {user?.name}</p>
        <Button onClick={handleUpdate} loading={loading}>
          Update
        </Button>
      </div>
    </Card>
  );
}
```

#### Estrutura de Hooks
```typescript
// useUser.ts
import { useState, useEffect } from 'react';
import { userService } from '../services/user';

export function useUser() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadUser();
  }, []);

  const loadUser = async () => {
    try {
      setLoading(true);
      const userData = await userService.getCurrentUser();
      setUser(userData);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error');
    } finally {
      setLoading(false);
    }
  };

  const updateUser = async (id: number, data: Partial<User>) => {
    try {
      const updatedUser = await userService.updateUser(id, data);
      setUser(updatedUser);
      return updatedUser;
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Update failed');
      throw err;
    }
  };

  return {
    user,
    loading,
    error,
    updateUser,
    refreshUser: loadUser
  };
}
```

## 🔄 Fluxo de Desenvolvimento

### 1. Planejamento
- [ ] Analisar requisitos
- [ ] Verificar se já existe funcionalidade similar
- [ ] Planejar implementação (backend + frontend)
- [ ] Identificar dependências

### 2. Implementação
- [ ] Criar branch para feature
- [ ] Implementar backend primeiro
- [ ] Implementar frontend
- [ ] Testar integração
- [ ] Documentar mudanças

### 3. Testes
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Testes manuais
- [ ] Verificar performance
- [ ] Verificar acessibilidade

### 4. Documentação
- [ ] Atualizar README se necessário
- [ ] Documentar APIs
- [ ] Atualizar tutorial se necessário
- [ ] Comentar código complexo

### 5. Pull Request
- [ ] Criar PR descritivo
- [ ] Linkar issues relacionadas
- [ ] Adicionar screenshots se aplicável
- [ ] Solicitar review

## 🧪 Testes

### Backend
```bash
# Executar todos os testes
dotnet test

# Executar testes específicos
dotnet test --filter "UserService"

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Frontend
```bash
# Executar testes
npm run test

# Executar com cobertura
npm run test:coverage

# Executar em modo watch
npm run test:watch
```

### Testes Manuais
- [ ] Testar funcionalidade completa
- [ ] Verificar diferentes cenários
- [ ] Testar em diferentes navegadores
- [ ] Verificar responsividade
- [ ] Testar acessibilidade

## 📚 Documentação

### Obrigatório Atualizar
- **README.md** - Para mudanças estruturais
- **API Documentation** - Para novos endpoints
- **Tutorial** - Para novas funcionalidades
- **Comentários no código** - Para lógica complexa

### Exemplo de Documentação de API
```csharp
/// <summary>
/// Obtém um usuário pelo ID
/// </summary>
/// <param name="id">ID do usuário</param>
/// <returns>Dados do usuário</returns>
/// <response code="200">Usuário encontrado</response>
/// <response code="404">Usuário não encontrado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(User), 200)]
[ProducesResponseType(404)]
[ProducesResponseType(500)]
public async Task<ActionResult<User>> GetUser(int id)
{
    // Implementação
}
```

### Exemplo de Documentação de Componente
```typescript
/**
 * Componente de perfil do usuário
 * 
 * @param userId - ID do usuário a ser exibido
 * @param onUpdate - Callback chamado quando o usuário é atualizado
 * @param showActions - Se deve mostrar botões de ação
 */
export function UserProfile({ 
  userId, 
  onUpdate, 
  showActions = true 
}: UserProfileProps) {
  // Implementação
}
```

## 🔀 Pull Requests

### Template de PR
```markdown
## 📋 Descrição
Breve descrição das mudanças implementadas.

## 🎯 Tipo de Mudança
- [ ] Bug fix
- [ ] Nova funcionalidade
- [ ] Breaking change
- [ ] Documentação
- [ ] Refatoração
- [ ] Performance
- [ ] Testes

## 🧪 Como Testar
1. Passo 1
2. Passo 2
3. Passo 3

## 📸 Screenshots
(Se aplicável)

## ✅ Checklist
- [ ] Código segue os padrões do projeto
- [ ] Testes passam
- [ ] Documentação atualizada
- [ ] Não quebra funcionalidades existentes
- [ ] Performance verificada

## 🔗 Issues Relacionadas
Closes #123
```

### Regras para PR
- **Título descritivo** - Ex: "feat: adiciona sistema de notificações"
- **Descrição detalhada** - O que foi implementado e por quê
- **Screenshots** - Para mudanças visuais
- **Testes** - Como testar as mudanças
- **Checklist** - Verificações obrigatórias

## 👀 Code Review

### Critérios de Aprovação
- [ ] **Funcionalidade** - Implementa corretamente o requisito
- [ ] **Qualidade** - Código limpo e bem estruturado
- [ ] **Performance** - Não impacta negativamente
- [ ] **Segurança** - Não introduz vulnerabilidades
- [ ] **Testes** - Cobertura adequada
- [ ] **Documentação** - Atualizada e clara

### Processo de Review
1. **Revisar código** - Estrutura, lógica, padrões
2. **Testar funcionalidade** - Verificar se funciona
3. **Verificar testes** - Se cobrem os cenários
4. **Comentar** - Sugestões e melhorias
5. **Aprovar** - Quando estiver satisfeito

### Exemplo de Comentários
```typescript
// ❌ Evitar
// This is wrong

// ✅ Preferir
// Consider using useCallback here to prevent unnecessary re-renders
// or
// This could be extracted to a custom hook for reusability
```

## 🐛 Reportar Bugs

### Template de Bug Report
```markdown
## 🐛 Descrição do Bug
Descrição clara e concisa do bug.

## 🔄 Passos para Reproduzir
1. Vá para '...'
2. Clique em '...'
3. Role até '...'
4. Veja o erro

## ✅ Comportamento Esperado
O que deveria acontecer.

## ❌ Comportamento Atual
O que está acontecendo.

## 📸 Screenshots
(Se aplicável)

## 🖥️ Ambiente
- OS: [ex: Windows 10]
- Browser: [ex: Chrome 91]
- Version: [ex: 1.2.3]

## 📋 Informações Adicionais
Qualquer informação adicional relevante.
```

## 💡 Sugestões de Melhorias

### Template de Feature Request
```markdown
## 💡 Sugestão de Melhoria
Descrição clara da funcionalidade desejada.

## 🎯 Problema que Resolve
Qual problema esta funcionalidade resolveria.

## 💭 Solução Proposta
Como você imagina que deveria funcionar.

## 🔄 Alternativas Consideradas
Outras soluções que você considerou.

## 📋 Informações Adicionais
Qualquer informação adicional relevante.
```

## 🏷️ Versionamento

### Semântico (SemVer)
- **MAJOR** - Mudanças incompatíveis
- **MINOR** - Novas funcionalidades compatíveis
- **PATCH** - Correções de bugs

### Exemplos
- `1.0.0` - Versão inicial
- `1.1.0` - Nova funcionalidade
- `1.1.1` - Correção de bug
- `2.0.0` - Breaking change

## 📞 Suporte

### Canais de Comunicação
- **GitHub Issues** - Para bugs e feature requests
- **GitHub Discussions** - Para dúvidas e discussões
- **Pull Requests** - Para contribuições

### Resposta Esperada
- **Issues** - 48 horas
- **Pull Requests** - 72 horas
- **Discussions** - 24 horas

## 🎉 Reconhecimento

### Contribuidores
Todos os contribuidores são reconhecidos no README e releases.

### Tipos de Contribuição
- **Código** - Implementação de funcionalidades
- **Documentação** - Melhoria da documentação
- **Testes** - Adição de testes
- **Design** - Melhoria da interface
- **Ideias** - Sugestões e feedback

---

**Obrigado por contribuir! 🚀**
