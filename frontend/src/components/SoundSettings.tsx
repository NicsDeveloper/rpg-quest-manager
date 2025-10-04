import { useState, useEffect } from 'react';
import { soundService } from '../services/sound';
import { Button } from './ui/Button';
import { Card } from './ui/Card';
import { Volume2, VolumeX, Play } from 'lucide-react';

interface SoundSettingsProps {
  isOpen: boolean;
  onClose: () => void;
}

export function SoundSettings({ isOpen, onClose }: SoundSettingsProps) {
  const [enabled, setEnabled] = useState(soundService.isSoundEnabled());
  const [volume, setVolume] = useState(soundService.getVolume());

  useEffect(() => {
    if (isOpen) {
      setEnabled(soundService.isSoundEnabled());
      setVolume(soundService.getVolume());
    }
  }, [isOpen]);

  const handleToggleEnabled = () => {
    const newEnabled = !enabled;
    setEnabled(newEnabled);
    soundService.setEnabled(newEnabled);
    
    if (newEnabled) {
      soundService.playClick();
    }
  };

  const handleVolumeChange = (newVolume: number) => {
    setVolume(newVolume);
    soundService.setVolume(newVolume);
  };

  const handleTestSound = async () => {
    await soundService.playSuccess();
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <Card className="w-full max-w-md mx-4">
        <div className="p-6">
          <h2 className="text-xl font-bold text-gray-900 mb-6">Configurações de Áudio</h2>
          
          {/* Toggle de ativar/desativar */}
          <div className="mb-6">
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                {enabled ? (
                  <Volume2 className="h-5 w-5 text-green-600" />
                ) : (
                  <VolumeX className="h-5 w-5 text-gray-400" />
                )}
                <span className="font-medium text-gray-900">Efeitos Sonoros</span>
              </div>
              <button
                onClick={handleToggleEnabled}
                className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
                  enabled ? 'bg-blue-600' : 'bg-gray-200'
                }`}
              >
                <span
                  className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
                    enabled ? 'translate-x-6' : 'translate-x-1'
                  }`}
                />
              </button>
            </div>
          </div>

          {/* Controle de volume */}
          {enabled && (
            <div className="mb-6">
              <div className="flex items-center justify-between mb-2">
                <span className="font-medium text-gray-900">Volume</span>
                <span className="text-sm text-gray-600">{Math.round(volume * 100)}%</span>
              </div>
              <input
                type="range"
                min="0"
                max="1"
                step="0.1"
                value={volume}
                onChange={(e) => handleVolumeChange(parseFloat(e.target.value))}
                className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer slider"
              />
            </div>
          )}

          {/* Botão de teste */}
          {enabled && (
            <div className="mb-6">
              <Button
                onClick={handleTestSound}
                variant="secondary"
                className="w-full"
              >
                <Play className="h-4 w-4 mr-2" />
                Testar Som
              </Button>
            </div>
          )}

          {/* Botões de ação */}
          <div className="flex space-x-3">
            <Button
              onClick={onClose}
              className="flex-1"
            >
              Fechar
            </Button>
          </div>
        </div>
      </Card>
    </div>
  );
}
