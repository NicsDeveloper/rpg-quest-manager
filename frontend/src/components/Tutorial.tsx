import React, { useState } from 'react';
import { Button } from './Button';
import { userService } from '../services/userService';

interface TutorialProps {
  onComplete: () => void;
  onSkip: () => void;
}

const tutorialSteps = [
  {
    title: '🎮 Bem-vindo ao RPG Quest Manager!',
    content: 'Prepare-se para uma aventura épica de RPG estratégico! Este tutorial vai te ensinar tudo sobre o jogo: criação de heróis, combate com dados, party e muito mais!',
    icon: '🎉',
    color: 'from-purple-600 to-purple-700'
  },
  {
    title: '⚔️ Crie Seu Primeiro Herói',
    content: 'Você começa sem heróis! Acesse "Heróis" no menu para criar seu primeiro aventureiro. Escolha a classe (Guerreiro, Mago, Arqueiro, Paladino, Ladino) e distribua estrategicamente 30 pontos entre Força, Inteligência e Destreza. Mínimo 3 pontos em cada atributo para balanceamento!',
    icon: '🧙',
    color: 'from-blue-600 to-blue-700'
  },
  {
    title: '💰 Ouro Compartilhado',
    content: 'Você começa com 100 moedas de ouro! O ouro é do PLAYER (você), não do herói, então é compartilhado entre todos os seus heróis. Use-o para comprar dados mágicos na loja!',
    icon: '💎',
    color: 'from-amber-600 to-orange-600'
  },
  {
    title: '🎲 Sistema de Combate com Dados',
    content: 'As batalhas funcionam com dados! Compre dados D6, D10, D12 e D20 na loja. Cada inimigo requer um tipo de dado e uma rolagem mínima. Role os dados para vencer! Dados são consumidos ao usar, então compre com sabedoria!',
    icon: '⚡',
    color: 'from-red-600 to-red-700'
  },
  {
    title: '🛡️ Sistema de Party',
    content: 'Seu primeiro herói entra automaticamente na party. Você pode ter até 3 heróis ativos! Mais heróis = mais força no combate, mas as recompensas diminuem. Para criar mais heróis, seu primeiro precisa atingir nível 5+.',
    icon: '👥',
    color: 'from-green-600 to-green-700'
  },
  {
    title: '📚 Aceite Missões e Lute!',
    content: 'No Catálogo de Missões, aceite quests adequadas ao seu nível e classe. Clique em "Ir para Missão" para lutar! Escolha o dado certo, role e derrote os inimigos. Ganhe XP, ouro e itens épicos! Bosses dropam itens raros e lendários!',
    icon: '⚔️',
    color: 'from-purple-600 to-purple-700'
  },
  {
    title: '📦 Inventário e Equipamentos',
    content: 'Itens que você ganhar vão para o inventário. Acesse "Meu Perfil" para equipar e desequipar itens nos seus heróis. Itens dão bônus de atributos e melhoram seu poder de combate!',
    icon: '🎒',
    color: 'from-indigo-600 to-indigo-700'
  },
  {
    title: '📈 Progressão e Níveis',
    content: 'Seu herói começa no nível 0! A cada missão completada, ganha experiência. Ao subir de nível, seus atributos melhoram e novas missões ficam disponíveis. O nível máximo é 20!',
    icon: '⭐',
    color: 'from-pink-600 to-pink-700'
  },
  {
    title: '🔔 Notificações e Dicas',
    content: 'O sino no topo te avisa sobre level ups, novas quests disponíveis e recompensas. O widget do herói no canto superior esquerdo mostra suas informações em tempo real. Passe o mouse sobre ele para ver mais detalhes!',
    icon: '💡',
    color: 'from-cyan-600 to-cyan-700'
  },
  {
    title: '🚀 Pronto para a Aventura!',
    content: 'Agora você sabe tudo! Comece criando seu herói, compre alguns dados, aceite sua primeira missão e mostre sua estratégia em combate. Boa sorte, aventureiro! 🎯',
    icon: '🏆',
    color: 'from-amber-500 to-orange-600'
  }
];

export const Tutorial: React.FC<TutorialProps> = ({ onComplete, onSkip }) => {
  const [currentStep, setCurrentStep] = useState(0);
  const [isClosing, setIsClosing] = useState(false);

  const step = tutorialSteps[currentStep];
  const isLastStep = currentStep === tutorialSteps.length - 1;
  const progress = ((currentStep + 1) / tutorialSteps.length) * 100;

  const handleNext = async () => {
    if (isLastStep) {
      setIsClosing(true);
      await userService.completeTutorial();
      setTimeout(onComplete, 300);
    } else {
      setCurrentStep(currentStep + 1);
    }
  };

  const handlePrevious = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleSkip = async () => {
    if (window.confirm('Tem certeza que deseja pular o tutorial? Você pode revê-lo depois nas configurações.')) {
      setIsClosing(true);
      await userService.skipTutorial();
      setTimeout(onSkip, 300);
    }
  };

  return (
    <div className={`fixed inset-0 z-50 flex items-center justify-center bg-black/90 backdrop-blur-sm transition-opacity duration-300 ${isClosing ? 'opacity-0' : 'opacity-100'}`}>
      <div className={`relative w-full max-w-3xl mx-4 transition-transform duration-300 ${isClosing ? 'scale-95' : 'scale-100'}`}>
        <div className="bg-gradient-to-br from-gray-800 to-gray-900 rounded-3xl shadow-2xl border-2 border-gray-700 overflow-hidden">
          <div className={`bg-gradient-to-r ${step.color} px-8 py-6`}>
            <div className="flex items-center justify-between mb-2">
              <div className="flex items-center gap-4">
                <span className="text-6xl animate-float">{step.icon}</span>
                <div>
                  <h2 className="text-3xl font-black text-white">{step.title}</h2>
                  <p className="text-white/80 text-sm">Passo {currentStep + 1} de {tutorialSteps.length}</p>
                </div>
              </div>
              <button
                onClick={handleSkip}
                className="text-white/60 hover:text-white text-sm underline"
              >
                Pular Tutorial
              </button>
            </div>
            
            <div className="w-full bg-white/20 rounded-full h-2 mt-4">
              <div
                className="bg-white h-2 rounded-full transition-all duration-300"
                style={{ width: `${progress}%` }}
              />
            </div>
          </div>

          <div className="p-8">
            <div className="bg-gray-800/50 rounded-2xl p-8 mb-8 min-h-[200px] flex items-center">
              <p className="text-gray-200 text-xl leading-relaxed">
                {step.content}
              </p>
            </div>

            <div className="flex justify-between items-center">
              <Button
                variant="secondary"
                onClick={handlePrevious}
                disabled={currentStep === 0}
                className="px-8"
              >
                ← Anterior
              </Button>

              <div className="flex gap-2">
                {tutorialSteps.map((_, index) => (
                  <div
                    key={index}
                    className={`w-3 h-3 rounded-full transition-all ${
                      index === currentStep
                        ? 'bg-amber-500 w-8'
                        : index < currentStep
                        ? 'bg-green-500'
                        : 'bg-gray-600'
                    }`}
                  />
                ))}
              </div>

              <Button
                variant="primary"
                onClick={handleNext}
                className="px-8"
              >
                {isLastStep ? '🎉 Começar!' : 'Próximo →'}
              </Button>
            </div>
          </div>
        </div>

        <div className="mt-4 text-center">
          <p className="text-gray-400 text-sm">
            💡 Dica: Use as setas do teclado para navegar entre os passos
          </p>
        </div>
      </div>
    </div>
  );
};

