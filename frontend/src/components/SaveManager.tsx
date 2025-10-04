import { useState, useEffect } from 'react';
import { saveSystem, type SaveData } from '../services/saveSystem';
import { Card } from './ui/Card';
import { Button } from './ui/Button';
import { Modal } from './ui/Modal';
import { useToast } from './Toast';
import { 
  Save, 
  Download, 
  Upload, 
  Trash2, 
  Clock, 
  User,
  Play,
  FileText
} from 'lucide-react';

interface SaveManagerProps {
  isOpen: boolean;
  onClose: () => void;
  onLoadGame: (saveData: SaveData) => void;
  characterId?: number;
  characterData?: any;
  gameState?: any;
}

export function SaveManager({ 
  isOpen, 
  onClose, 
  onLoadGame, 
  characterId, 
  characterData, 
  gameState 
}: SaveManagerProps) {
  const { showToast } = useToast();
  const [saves, setSaves] = useState<SaveData[]>([]);
  const [showImportModal, setShowImportModal] = useState(false);
  const [importData, setImportData] = useState('');
  const [saveName, setSaveName] = useState('');
  const [showSaveModal, setShowSaveModal] = useState(false);

  useEffect(() => {
    if (isOpen) {
      loadSaves();
    }
  }, [isOpen]);

  const loadSaves = () => {
    const saveList = saveSystem.getSaveList();
    setSaves(saveList);
  };

  const handleSaveGame = async () => {
    if (!characterId || !characterData) {
      showToast({
        type: 'error',
        title: 'Dados não disponíveis',
        message: 'Dados do personagem não estão disponíveis para salvar.'
      });
      return;
    }

    try {
      await saveSystem.saveGame(characterId, characterData, gameState || {}, saveName);
      setSaveName('');
      setShowSaveModal(false);
      loadSaves();
      showToast({
        type: 'success',
        title: 'Jogo salvo!',
        message: 'Seu progresso foi salvo com sucesso.'
      });
    } catch (error) {
      console.error('Erro ao salvar:', error);
      showToast({
        type: 'error',
        title: 'Erro ao salvar',
        message: 'Não foi possível salvar o jogo. Tente novamente.'
      });
    }
  };

  const handleLoadGame = (save: SaveData) => {
    onLoadGame(save);
    onClose();
  };

  const handleDeleteSave = (saveId: string) => {
    if (confirm('Tem certeza que deseja deletar este save?')) {
      if (saveSystem.deleteSave(saveId)) {
        loadSaves();
      }
    }
  };

  const handleExportSave = (save: SaveData) => {
    const exportData = saveSystem.exportSave(save.id);
    if (exportData) {
      const blob = new Blob([exportData], { type: 'application/json' });
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${save.name.replace(/[^a-z0-9]/gi, '_').toLowerCase()}.json`;
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }
  };

  const handleImportSave = () => {
    try {
      const save = saveSystem.importSave(importData);
      if (save) {
        setImportData('');
        setShowImportModal(false);
        loadSaves();
        showToast({
          type: 'success',
          title: 'Save importado!',
          message: 'O save foi importado com sucesso.'
        });
      } else {
        showToast({
          type: 'error',
          title: 'Erro ao importar',
          message: 'Verifique se os dados do save estão corretos.'
        });
      }
    } catch (error) {
      console.error('Erro ao importar:', error);
      showToast({
        type: 'error',
        title: 'Erro ao importar',
        message: 'Não foi possível importar o save.'
      });
    }
  };

  const formatDate = (timestamp: number) => {
    return new Date(timestamp).toLocaleString('pt-BR');
  };

  const formatPlayTime = (playTime: number) => {
    const hours = Math.floor(playTime / 3600);
    const minutes = Math.floor((playTime % 3600) / 60);
    return `${hours}h ${minutes}m`;
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-4xl max-h-[90vh] overflow-hidden">
        <div className="p-6 border-b border-gray-200">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-bold text-gray-900">Gerenciador de Saves</h2>
            <div className="flex space-x-2">
              <Button
                onClick={() => setShowSaveModal(true)}
                disabled={!characterId}
                variant="secondary"
              >
                <Save className="h-4 w-4 mr-2" />
                Salvar
              </Button>
              <Button
                onClick={() => setShowImportModal(true)}
                variant="secondary"
              >
                <Upload className="h-4 w-4 mr-2" />
                Importar
              </Button>
              <Button
                onClick={onClose}
                variant="secondary"
              >
                Fechar
              </Button>
            </div>
          </div>
        </div>

        <div className="p-6 overflow-y-auto max-h-[calc(90vh-120px)]">
          {saves.length === 0 ? (
            <div className="text-center py-12">
              <FileText className="h-12 w-12 text-gray-400 mx-auto mb-4" />
              <p className="text-gray-500">Nenhum save encontrado</p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {saves.map((save) => (
                <Card key={save.id} className="relative">
                  <div className="flex items-start justify-between mb-3">
                    <div>
                      <h3 className="font-semibold text-gray-900">{save.name}</h3>
                      <p className="text-sm text-gray-600">
                        Personagem ID: {save.characterId}
                      </p>
                    </div>
                    <div className="flex space-x-1">
                      <button
                        onClick={() => handleExportSave(save)}
                        className="p-1 text-gray-400 hover:text-gray-600"
                        title="Exportar"
                      >
                        <Download className="h-4 w-4" />
                      </button>
                      <button
                        onClick={() => handleDeleteSave(save.id)}
                        className="p-1 text-gray-400 hover:text-red-600"
                        title="Deletar"
                      >
                        <Trash2 className="h-4 w-4" />
                      </button>
                    </div>
                  </div>

                  <div className="space-y-2 text-sm text-gray-600 mb-4">
                    <div className="flex items-center space-x-2">
                      <Clock className="h-4 w-4" />
                      <span>{formatDate(save.timestamp)}</span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <Play className="h-4 w-4" />
                      <span>{formatPlayTime(save.metadata.playTime)}</span>
                    </div>
                    <div className="flex items-center space-x-2">
                      <User className="h-4 w-4" />
                      <span>Nível {save.characterData.level}</span>
                    </div>
                  </div>

                  <div className="flex space-x-2">
                    <Button
                      onClick={() => handleLoadGame(save)}
                      className="flex-1"
                    >
                      Carregar
                    </Button>
                  </div>
                </Card>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Save Modal */}
      <Modal
        isOpen={showSaveModal}
        onClose={() => setShowSaveModal(false)}
        title="Salvar Jogo"
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Nome do Save
            </label>
            <input
              type="text"
              value={saveName}
              onChange={(e) => setSaveName(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Digite o nome do save"
            />
          </div>
          
          <div className="flex space-x-2">
            <Button
              onClick={handleSaveGame}
              className="flex-1"
            >
              Salvar
            </Button>
            <Button
              onClick={() => setShowSaveModal(false)}
              variant="secondary"
              className="flex-1"
            >
              Cancelar
            </Button>
          </div>
        </div>
      </Modal>

      {/* Import Modal */}
      <Modal
        isOpen={showImportModal}
        onClose={() => setShowImportModal(false)}
        title="Importar Save"
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Dados do Save (JSON)
            </label>
            <textarea
              value={importData}
              onChange={(e) => setImportData(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              rows={10}
              placeholder="Cole os dados do save aqui..."
            />
          </div>
          
          <div className="flex space-x-2">
            <Button
              onClick={handleImportSave}
              className="flex-1"
            >
              Importar
            </Button>
            <Button
              onClick={() => setShowImportModal(false)}
              variant="secondary"
              className="flex-1"
            >
              Cancelar
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}
