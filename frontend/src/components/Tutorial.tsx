import { useState, useEffect } from 'react';
import { Button } from './ui/Button';
import { 
  ArrowRight, 
  ArrowLeft, 
  X, 
  Play, 
  Sword, 
  Backpack, 
  ShoppingBag, 
  Map, 
  Star, 
  Users,
  Volume2
} from 'lucide-react';

interface TutorialStep {
  id: string;
  title: string;
  content: string;
  target?: string;
  position?: 'top' | 'bottom' | 'left' | 'right';
  action?: () => void;
}

interface TutorialProps {
  isOpen: boolean;
  onClose: () => void;
  onComplete: () => void;
}

export function Tutorial({ isOpen, onClose, onComplete }: TutorialProps) {
  const [currentStep, setCurrentStep] = useState(0);

  const tutorialSteps: TutorialStep[] = [
    {
      id: 'welcome',
      title: 'Bem-vindo ao RPG Quest Manager!',
      content: 'Este tutorial irá te guiar pelas principais funcionalidades do jogo. Vamos começar!',
      position: 'bottom'
    },
    {
      id: 'dashboard',
      title: 'Dashboard',
      content: 'Aqui você pode ver as informações do seu personagem: nível, vida, ouro e moral. Use as barras de progresso para acompanhar seu desenvolvimento.',
      target: '[data-tutorial="dashboard"]',
      position: 'bottom'
    },
    {
      id: 'navigation',
      title: 'Navegação',
      content: 'Use o menu lateral para navegar entre as diferentes seções do jogo: Inventário, Loja, Missões, Combate, Conquistas e Grupos.',
      target: '[data-tutorial="navigation"]',
      position: 'right'
    },
    {
      id: 'inventory',
      title: 'Inventário',
      content: 'Gerencie seus itens, equipamentos e use poções. Equipe itens para aumentar suas estatísticas.',
      target: '[data-tutorial="inventory"]',
      position: 'right'
    },
    {
      id: 'shop',
      title: 'Loja',
      content: 'Compre novos equipamentos e itens. Venda itens que não precisa mais para ganhar ouro.',
      target: '[data-tutorial="shop"]',
      position: 'right'
    },
    {
      id: 'quests',
      title: 'Missões',
      content: 'Complete missões para ganhar experiência, ouro e itens. Cada missão tem diferentes dificuldades e recompensas.',
      target: '[data-tutorial="quests"]',
      position: 'right'
    },
    {
      id: 'combat',
      title: 'Combate',
      content: 'Lute contra monstros em turnos. Use habilidades especiais e itens para vencer batalhas épicas.',
      target: '[data-tutorial="combat"]',
      position: 'right'
    },
    {
      id: 'achievements',
      title: 'Conquistas',
      content: 'Complete objetivos para desbloquear conquistas e ganhar recompensas especiais.',
      target: '[data-tutorial="achievements"]',
      position: 'right'
    },
    {
      id: 'parties',
      title: 'Grupos',
      content: 'Junte-se a outros jogadores, forme grupos e complete missões em equipe.',
      target: '[data-tutorial="parties"]',
      position: 'right'
    },
    {
      id: 'settings',
      title: 'Configurações',
      content: 'Ajuste o volume dos efeitos sonoros e gerencie seus saves do jogo.',
      target: '[data-tutorial="settings"]',
      position: 'bottom'
    },
    {
      id: 'complete',
      title: 'Tutorial Concluído!',
      content: 'Agora você está pronto para começar sua aventura! Explore o mundo, complete missões e torne-se um herói lendário.',
      position: 'bottom'
    }
  ];

  useEffect(() => {
    if (isOpen) {
      setCurrentStep(0);
    }
  }, [isOpen]);

  const nextStep = () => {
    if (currentStep < tutorialSteps.length - 1) {
      setCurrentStep(currentStep + 1);
    } else {
      onComplete();
    }
  };

  const prevStep = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const skipTutorial = () => {
    onComplete();
  };

  const getStepIcon = (stepId: string) => {
    switch (stepId) {
      case 'welcome':
        return Play;
      case 'dashboard':
        return Play;
      case 'navigation':
        return Play;
      case 'inventory':
        return Backpack;
      case 'shop':
        return ShoppingBag;
      case 'quests':
        return Map;
      case 'combat':
        return Sword;
      case 'achievements':
        return Star;
      case 'parties':
        return Users;
      case 'settings':
        return Volume2;
      case 'complete':
        return Star;
      default:
        return Play;
    }
  };

  if (!isOpen) return null;

  const currentStepData = tutorialSteps[currentStep];
  const StepIcon = getStepIcon(currentStepData.id);

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md mx-4">
        <div className="p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <StepIcon className="h-6 w-6 text-blue-600" />
              </div>
              <div>
                <h2 className="text-lg font-semibold text-gray-900">
                  {currentStepData.title}
                </h2>
                <p className="text-sm text-gray-500">
                  Passo {currentStep + 1} de {tutorialSteps.length}
                </p>
              </div>
            </div>
            <button
              onClick={onClose}
              className="text-gray-400 hover:text-gray-600"
            >
              <X className="h-5 w-5" />
            </button>
          </div>

          <div className="mb-6">
            <p className="text-gray-700 leading-relaxed">
              {currentStepData.content}
            </p>
          </div>

          {/* Progress Bar */}
          <div className="mb-6">
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div
                className="bg-blue-600 h-2 rounded-full transition-all duration-300"
                style={{ width: `${((currentStep + 1) / tutorialSteps.length) * 100}%` }}
              />
            </div>
          </div>

          <div className="flex items-center justify-between">
            <div className="flex space-x-2">
              {currentStep > 0 && (
                <Button
                  onClick={prevStep}
                  variant="secondary"
                  size="sm"
                >
                  <ArrowLeft className="h-4 w-4 mr-1" />
                  Anterior
                </Button>
              )}
            </div>

            <div className="flex space-x-2">
              <Button
                onClick={skipTutorial}
                variant="secondary"
                size="sm"
              >
                Pular
              </Button>
              <Button
                onClick={nextStep}
                size="sm"
              >
                {currentStep === tutorialSteps.length - 1 ? 'Concluir' : 'Próximo'}
                {currentStep < tutorialSteps.length - 1 && (
                  <ArrowRight className="h-4 w-4 ml-1" />
                )}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
