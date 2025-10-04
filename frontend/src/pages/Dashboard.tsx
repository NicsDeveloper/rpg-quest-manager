import { useCharacter } from '../contexts/CharacterContext';
import { Card } from '../components/ui/Card';
import { ProgressBar } from '../components/ui/ProgressBar';
import { soundService } from '../services/sound';
import { FadeIn, SlideIn } from '../components/animations';
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
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
        {error}
      </div>
    );
  }

  if (!character) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Nenhum personagem encontrado</p>
      </div>
    );
  }

  const moraleColor = character.morale >= 70 ? 'green' : character.morale >= 40 ? 'yellow' : 'red';

  return (
    <div className="space-y-6">
      <FadeIn delay={0}>
        <div data-tutorial="dashboard">
          <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
          <p className="text-gray-600 mt-2">Bem-vindo de volta, {character.name}!</p>
        </div>
      </FadeIn>

      {/* Character Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <SlideIn direction="left" delay={100}>
          <Card>
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <User className="h-6 w-6 text-blue-600" />
              </div>
              <div>
                <p className="text-sm text-gray-600">Nível</p>
                <p className="text-2xl font-bold text-gray-900">{character.level}</p>
              </div>
            </div>
          </Card>
        </SlideIn>

        <SlideIn direction="left" delay={200}>
          <Card>
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-red-100 rounded-lg">
                <Heart className="h-6 w-6 text-red-600" />
              </div>
              <div>
                <p className="text-sm text-gray-600">Vida</p>
                <p className="text-2xl font-bold text-gray-900">{character.health}</p>
              </div>
            </div>
          </Card>
        </SlideIn>

        <SlideIn direction="left" delay={300}>
          <Card>
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-yellow-100 rounded-lg">
                <Coins className="h-6 w-6 text-yellow-600" />
              </div>
              <div>
                <p className="text-sm text-gray-600">Ouro</p>
                <p className="text-2xl font-bold text-gray-900">{character.gold}</p>
              </div>
            </div>
          </Card>
        </SlideIn>

        <SlideIn direction="left" delay={400}>
          <Card>
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <Shield className="h-6 w-6 text-green-600" />
              </div>
              <div>
                <p className="text-sm text-gray-600">Moral</p>
                <p className="text-2xl font-bold text-gray-900">{character.morale}</p>
              </div>
            </div>
          </Card>
        </SlideIn>
      </div>

      {/* Character Stats */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card title="Estatísticas do Personagem">
          <div className="space-y-4">
            <div className="flex justify-between items-center">
              <span className="text-sm font-medium text-gray-700">Ataque</span>
              <span className="text-lg font-bold text-gray-900">
                {character.attack + (stats?.attack || 0)}
                {stats?.attack ? ` (+${stats.attack})` : ''}
              </span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-sm font-medium text-gray-700">Defesa</span>
              <span className="text-lg font-bold text-gray-900">
                {character.defense + (stats?.defense || 0)}
                {stats?.defense ? ` (+${stats.defense})` : ''}
              </span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-sm font-medium text-gray-700">Vida Máxima</span>
              <span className="text-lg font-bold text-gray-900">
                {character.maxHealth + (stats?.health || 0)}
                {stats?.health ? ` (+${stats.health})` : ''}
              </span>
            </div>
            <div className="flex justify-between items-center">
              <span className="text-sm font-medium text-gray-700">Moral Máxima</span>
              <span className="text-lg font-bold text-gray-900">
                100 + (stats?.morale || 0)
                {stats?.morale ? ` (+${stats.morale})` : ''}
              </span>
            </div>
          </div>
        </Card>

        <Card title="Progressão">
          <div className="space-y-4">
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="text-sm font-medium text-gray-700">Experiência</span>
                <span className="text-sm text-gray-600">
                  {character.experience}/{character.nextLevelExperience}
                </span>
              </div>
              <ProgressBar 
                value={character.experience} 
                max={character.nextLevelExperience} 
                color="blue"
                showPercentage={false}
              />
            </div>
            
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="text-sm font-medium text-gray-700">Vida</span>
                <span className="text-sm text-gray-600">
                  {character.health}/{character.maxHealth}
                </span>
              </div>
              <ProgressBar 
                value={character.health} 
                max={character.maxHealth} 
                color="red"
                showPercentage={false}
              />
            </div>
            
            <div>
              <div className="flex justify-between items-center mb-2">
                <span className="text-sm font-medium text-gray-700">Moral</span>
                <span className="text-sm text-gray-600">
                  {character.morale}/100
                </span>
              </div>
              <ProgressBar 
                value={character.morale} 
                max={100} 
                color={moraleColor as any}
                showPercentage={false}
              />
            </div>
          </div>
        </Card>
      </div>

      {/* Status Effects */}
      {character.statusEffects.length > 0 && (
        <Card title="Efeitos Ativos">
          <div className="flex flex-wrap gap-2">
            {character.statusEffects.map((effect, index) => (
              <span 
                key={index}
                className="px-3 py-1 bg-purple-100 text-purple-800 rounded-full text-sm font-medium"
              >
                {effect}
              </span>
            ))}
          </div>
        </Card>
      )}

      {/* Quick Actions */}
      <Card title="Ações Rápidas">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <button 
            onClick={() => soundService.playClick()}
            className="flex items-center space-x-3 p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
          >
            <Sword className="h-8 w-8 text-blue-600" />
            <div>
              <h3 className="font-medium text-gray-900">Combate</h3>
              <p className="text-sm text-gray-600">Lute contra monstros</p>
            </div>
          </button>
          
          <button 
            onClick={() => soundService.playClick()}
            className="flex items-center space-x-3 p-4 bg-green-50 rounded-lg hover:bg-green-100 transition-colors"
          >
            <ShoppingBag className="h-8 w-8 text-green-600" />
            <div>
              <h3 className="font-medium text-gray-900">Loja</h3>
              <p className="text-sm text-gray-600">Compre equipamentos</p>
            </div>
          </button>
          
          <button 
            onClick={() => soundService.playClick()}
            className="flex items-center space-x-3 p-4 bg-purple-50 rounded-lg hover:bg-purple-100 transition-colors"
          >
            <Target className="h-8 w-8 text-purple-600" />
            <div>
              <h3 className="font-medium text-gray-900">Missões</h3>
              <p className="text-sm text-gray-600">Complete objetivos</p>
            </div>
          </button>
        </div>
      </Card>
    </div>
  );
}


