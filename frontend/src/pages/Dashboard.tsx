import { useCharacter } from '../contexts/CharacterContext';
import { soundService } from '../services/sound';
import { 
  User, 
  Heart, 
  Shield, 
  Sword, 
  Coins, 
  Target,
  ShoppingBag
} from 'lucide-react';

export default function Dashboard() {
  const { character, stats, isLoading, error } = useCharacter();

  if (isLoading) {
    return (
      <div className="container mx-auto px-6 py-8 flex items-center justify-center min-h-screen">
        <div className="text-center">
          <div className="inline-block p-6 bg-gradient-to-br from-amber-500 to-orange-600 rounded-full shadow-lg shadow-amber-500/50 animate-pulse mb-4">
            <svg className="w-16 h-16 text-white animate-spin" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
          </div>
          <p className="text-gray-400 text-lg">Carregando Aventura...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mx-auto px-6 py-8">
        <div className="card bg-gradient-to-br from-red-900/20 to-red-800/20 border-red-700/30">
          <div className="text-center">
            <div className="inline-block p-4 bg-red-600/20 rounded-full mb-4">
              <svg className="w-12 h-12 text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <p className="text-red-400 text-lg">{error}</p>
          </div>
        </div>
      </div>
    );
  }

  if (!character) {
    return (
      <div className="container mx-auto px-6 py-8">
        <div className="text-center py-12">
          <div className="inline-block p-4 bg-gray-800/50 rounded-full mb-4">
            <svg className="w-12 h-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4" />
            </svg>
          </div>
          <p className="text-gray-400 text-lg">Nenhum personagem encontrado</p>
        </div>
      </div>
    );
  }

  const moraleColor = character.morale >= 70 ? 'green' : character.morale >= 40 ? 'yellow' : 'red';

  return (
    <div className="container mx-auto px-6 py-8">
      <div className="mb-12 text-center">
        <h1 className="text-6xl font-black mb-4 hero-title animate-float">Dashboard</h1>
        <p className="text-gray-400 text-lg">Bem-vindo de volta, {character.name}!</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        <div className="stat-card group">
          <div className="flex items-center justify-between mb-3">
            <div className="p-3 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <User className="w-8 h-8 text-white" />
            </div>
            <span className="text-4xl font-black text-gradient">{character.level}</span>
          </div>
          <h3 className="text-gray-400 text-sm font-medium">Nível</h3>
        </div>

        <div className="stat-card group">
          <div className="flex items-center justify-between mb-3">
            <div className="p-3 bg-gradient-to-br from-red-500 to-red-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Heart className="w-8 h-8 text-white" />
            </div>
            <span className="text-4xl font-black text-gradient">{character.health}</span>
          </div>
          <h3 className="text-gray-400 text-sm font-medium">Vida</h3>
        </div>

        <div className="stat-card group">
          <div className="flex items-center justify-between mb-3">
            <div className="p-3 bg-gradient-to-br from-amber-500 to-orange-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform animate-glow">
              <Coins className="w-8 h-8 text-white" />
            </div>
            <span className="text-4xl font-black text-gradient">{character.gold}</span>
          </div>
          <h3 className="text-gray-400 text-sm font-medium">Ouro</h3>
        </div>

        <div className="stat-card group">
          <div className="flex items-center justify-between mb-3">
            <div className="p-3 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Shield className="w-8 h-8 text-white" />
            </div>
            <span className="text-4xl font-black text-gradient">{character.morale}</span>
          </div>
          <h3 className="text-gray-400 text-sm font-medium">Moral</h3>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <div className="card-hero hover:scale-[1.02] transition-transform">
          <h2 className="text-3xl font-bold text-gradient mb-6">Estatísticas do Personagem</h2>
          <div className="space-y-4">
            <div className="flex justify-between items-center p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30">
              <span className="text-gray-300 font-medium">Ataque</span>
              <span className="text-amber-400 font-bold text-lg">
                {character.attack + (stats?.attack || 0)}
                {stats?.attack ? ` (+${stats.attack})` : ''}
              </span>
            </div>
            <div className="flex justify-between items-center p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30">
              <span className="text-gray-300 font-medium">Defesa</span>
              <span className="text-amber-400 font-bold text-lg">
                {character.defense + (stats?.defense || 0)}
                {stats?.defense ? ` (+${stats.defense})` : ''}
              </span>
            </div>
            <div className="flex justify-between items-center p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30">
              <span className="text-gray-300 font-medium">Vida Máxima</span>
              <span className="text-amber-400 font-bold text-lg">
                {character.maxHealth + (stats?.health || 0)}
                {stats?.health ? ` (+${stats.health})` : ''}
              </span>
            </div>
            <div className="flex justify-between items-center p-4 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30">
              <span className="text-gray-300 font-medium">Moral Máxima</span>
              <span className="text-amber-400 font-bold text-lg">
                100 + (stats?.morale || 0)
                {stats?.morale ? ` (+${stats.morale})` : ''}
              </span>
            </div>
          </div>
        </div>

        <div className="card-quest hover:scale-[1.02] transition-transform">
          <h2 className="text-3xl font-bold text-gradient mb-6">Progressão</h2>
          <div className="space-y-6">
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="text-gray-300 font-medium">Experiência</span>
                <span className="text-gray-400 text-sm">
                  {character.experience}/{character.nextLevelExperience}
                </span>
              </div>
              <div className="progress-bar">
                <div 
                  className="progress-fill" 
                  style={{ width: `${(character.experience / character.nextLevelExperience) * 100}%` }}
                ></div>
              </div>
            </div>
            
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="text-gray-300 font-medium">Vida</span>
                <span className="text-gray-400 text-sm">
                  {character.health}/{character.maxHealth}
                </span>
              </div>
              <div className="progress-bar">
                <div 
                  className="h-full bg-gradient-to-r from-red-400 via-red-500 to-red-600 rounded-full transition-all duration-500 shadow-lg shadow-red-500/50" 
                  style={{ width: `${(character.health / character.maxHealth) * 100}%` }}
                ></div>
              </div>
            </div>
            
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="text-gray-300 font-medium">Moral</span>
                <span className="text-gray-400 text-sm">
                  {character.morale}/100
                </span>
              </div>
              <div className="progress-bar">
                <div 
                  className={`h-full rounded-full transition-all duration-500 shadow-lg ${
                    moraleColor === 'green' ? 'bg-gradient-to-r from-green-400 via-green-500 to-green-600 shadow-green-500/50' :
                    moraleColor === 'yellow' ? 'bg-gradient-to-r from-yellow-400 via-yellow-500 to-yellow-600 shadow-yellow-500/50' :
                    'bg-gradient-to-r from-red-400 via-red-500 to-red-600 shadow-red-500/50'
                  }`}
                  style={{ width: `${character.morale}%` }}
                ></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {character.statusEffects.length > 0 && (
        <div className="card mb-8">
          <h2 className="text-3xl font-bold text-gradient mb-6">Efeitos Ativos</h2>
          <div className="flex flex-wrap gap-2">
            {character.statusEffects.map((effect, index) => (
              <span 
                key={index}
                className="badge badge-info"
              >
                {effect}
              </span>
            ))}
          </div>
        </div>
      )}

      <div className="card">
        <h2 className="text-3xl font-bold text-gradient mb-6">Ações Rápidas</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <button 
            onClick={() => soundService.playClick()}
            className="group flex items-center space-x-4 p-6 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-blue-500/50 transition-all hover:scale-105"
          >
            <div className="p-3 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Sword className="h-8 w-8 text-white" />
            </div>
            <div>
              <h3 className="font-bold text-lg text-blue-400 group-hover:text-blue-300">Combate</h3>
              <p className="text-sm text-gray-400">Lute contra monstros</p>
            </div>
          </button>
          
          <button 
            onClick={() => soundService.playClick()}
            className="group flex items-center space-x-4 p-6 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-green-500/50 transition-all hover:scale-105"
          >
            <div className="p-3 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <ShoppingBag className="h-8 w-8 text-white" />
            </div>
            <div>
              <h3 className="font-bold text-lg text-green-400 group-hover:text-green-300">Loja</h3>
              <p className="text-sm text-gray-400">Compre equipamentos</p>
            </div>
          </button>
          
          <button 
            onClick={() => soundService.playClick()}
            className="group flex items-center space-x-4 p-6 bg-gradient-to-r from-gray-800/50 to-gray-900/50 rounded-xl border border-gray-700/30 hover:border-purple-500/50 transition-all hover:scale-105"
          >
            <div className="p-3 bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <Target className="h-8 w-8 text-white" />
            </div>
            <div>
              <h3 className="font-bold text-lg text-purple-400 group-hover:text-purple-300">Missões</h3>
              <p className="text-sm text-gray-400">Complete objetivos</p>
            </div>
          </button>
        </div>
      </div>
    </div>
  );
}


