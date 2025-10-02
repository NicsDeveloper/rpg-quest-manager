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
    content: 'Prepare-se para uma aventura épica! Vou te mostrar como funciona tudo por aqui.',
    icon: '🎉',
    color: 'from-purple-600 to-purple-700'
  },
  {
    title: '👤 Seu Perfil de Herói',
    content: 'Todo jogador possui um herói! Acesse "Meu Perfil" para ver seus atributos, nível, inventário e missões aceitas. Conforme você completa missões, seu herói ganha experiência e sobe de nível!',
    icon: '⚔️',
    color: 'from-blue-600 to-blue-700'
  },
  {
    title: '📚 Catálogo de Missões',
    content: 'No menu "Missões", você encontra o catálogo completo! Aceite missões que correspondam ao seu nível e classe. Missões bloqueadas 🔒 ficam disponíveis quando você evoluir!',
    icon: '📖',
    color: 'from-amber-600 to-orange-600'
  },
  {
    title: '🎯 Completando Missões',
    content: 'Após aceitar uma missão, um administrador irá completá-la para você. Você receberá ouro, experiência e itens como recompensa!',
    icon: '✨',
    color: 'from-green-600 to-green-700'
  },
  {
    title: '🎊 Level Up!',
    content: 'Quando subir de nível, você ganha: +2 em todos os atributos, ouro extra e acesso a novas missões! Uma notificação te avisará sobre as novidades.',
    icon: '📈',
    color: 'from-pink-600 to-pink-700'
  },
  {
    title: '🔔 Notificações',
    content: 'Fique de olho no sino no topo da tela! Ele te avisa sobre level ups e novas missões disponíveis. Clique nele para ver suas notificações.',
    icon: '🔔',
    color: 'from-red-600 to-red-700'
  },
  {
    title: '🚀 Pronto para Começar!',
    content: 'Agora você está pronto para sua jornada épica! Explore o mundo, aceite missões e torne-se uma lenda!',
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

