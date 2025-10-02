import React, { useEffect, useState } from 'react';
import { Navbar } from '../components/Navbar';
import { Card } from '../components/Card';
import { Button } from '../components/Button';
import { attributesService, HeroAttributes, ClassInfo, AllocateAttributesRequest } from '../services/attributesService';
import { heroService } from '../services/heroService';

export const Attributes: React.FC = () => {
  const [heroes, setHeroes] = useState<any[]>([]);
  const [selectedHero, setSelectedHero] = useState<HeroAttributes | null>(null);
  const [classes, setClasses] = useState<ClassInfo[]>([]);
  const [loading, setLoading] = useState(true);
  const [allocating, setAllocating] = useState(false);
  const [allocationRequest, setAllocationRequest] = useState<AllocateAttributesRequest>({
    strengthPoints: 0,
    intelligencePoints: 0,
    dexterityPoints: 0
  });

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [heroesData, classesData] = await Promise.all([
        heroService.getAll(),
        attributesService.getAvailableClasses()
      ]);
      setHeroes(heroesData);
      setClasses(classesData);
    } catch (error) {
      console.error('Erro ao carregar dados:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleSelectHero = async (heroId: number) => {
    try {
      const heroAttributes = await attributesService.getHeroAttributes(heroId);
      setSelectedHero(heroAttributes);
      setAllocationRequest({
        strengthPoints: 0,
        intelligencePoints: 0,
        dexterityPoints: 0
      });
    } catch (error) {
      console.error('Erro ao carregar atributos do herói:', error);
    }
  };

  const handleAllocateAttributes = async () => {
    if (!selectedHero) return;

    const totalPoints = allocationRequest.strengthPoints + 
                       allocationRequest.intelligencePoints + 
                       allocationRequest.dexterityPoints;

    if (totalPoints > selectedHero.unallocatedPoints) {
      alert(`Você está tentando distribuir ${totalPoints} pontos, mas só tem ${selectedHero.unallocatedPoints} disponíveis.`);
      return;
    }

    if (totalPoints <= 0) {
      alert('Você deve distribuir pelo menos 1 ponto de atributo.');
      return;
    }

    try {
      setAllocating(true);
      const result = await attributesService.allocateAttributes(selectedHero.heroId, allocationRequest);
      
      if (result.success) {
        alert(result.message);
        // Recarregar atributos do herói
        await handleSelectHero(selectedHero.heroId);
      } else {
        alert(result.message);
      }
    } catch (error: any) {
      alert(error.response?.data?.message || 'Erro ao distribuir pontos de atributo');
    } finally {
      setAllocating(false);
    }
  };

  const getClassInfo = (className: string) => {
    return classes.find(c => c.name.toLowerCase() === className.toLowerCase());
  };


  if (loading) {
    return (
      <div className="min-h-screen bg-gray-900">
        <Navbar />
        <div className="container mx-auto px-4 py-8">
          <div className="text-center">
            <div className="text-white text-xl">Carregando...</div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-900">
      <Navbar />
      
      <div className="container mx-auto px-4 py-8">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-white mb-4">⚔️ Atributos dos Heróis</h1>
          <p className="text-gray-300 text-lg">
            Gerencie os atributos dos seus heróis e distribua pontos ao subir de nível
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          {/* Lista de Heróis */}
          <div>
            <h2 className="text-2xl font-bold text-white mb-4">Seus Heróis</h2>
            <div className="space-y-4">
              {heroes.map((hero) => (
                <Card key={hero.id}>
                  <div className="p-4">
                    <div className="flex items-center justify-between mb-3">
                      <div className="flex items-center gap-3">
                        <span className="text-2xl">⚔️</span>
                        <div>
                          <h3 className="text-lg font-bold text-white">{hero.name}</h3>
                          <p className="text-gray-400 text-sm">Nível {hero.level} - {hero.class}</p>
                        </div>
                      </div>
                      <div className="text-right">
                        {hero.unallocatedAttributePoints > 0 && (
                          <span className="bg-yellow-900 text-yellow-300 px-2 py-1 rounded text-xs">
                            {hero.unallocatedAttributePoints} pontos
                          </span>
                        )}
                      </div>
                    </div>
                    
                    <div className="grid grid-cols-3 gap-2 text-center text-sm mb-3">
                      <div className="bg-red-900/30 rounded py-1">
                        <p className="text-red-400 font-bold">{hero.strength}</p>
                        <p className="text-gray-500">FOR</p>
                      </div>
                      <div className="bg-blue-900/30 rounded py-1">
                        <p className="text-blue-400 font-bold">{hero.intelligence}</p>
                        <p className="text-gray-500">INT</p>
                      </div>
                      <div className="bg-green-900/30 rounded py-1">
                        <p className="text-green-400 font-bold">{hero.dexterity}</p>
                        <p className="text-gray-500">DEX</p>
                      </div>
                    </div>
                    
                    <Button
                      onClick={() => handleSelectHero(hero.id)}
                      variant="primary"
                      className="w-full text-sm"
                    >
                      Gerenciar Atributos
                    </Button>
                  </div>
                </Card>
              ))}
            </div>
          </div>

          {/* Detalhes do Herói Selecionado */}
          <div>
            {selectedHero ? (
              <div>
                <h2 className="text-2xl font-bold text-white mb-4">
                  {selectedHero.heroName} - {selectedHero.class}
                </h2>
                
                <Card>
                  <div className="p-6">
                    {/* Atributos Atuais */}
                    <div className="mb-6">
                      <h3 className="text-lg font-bold text-white mb-3">Atributos Atuais</h3>
                      <div className="grid grid-cols-3 gap-4">
                        <div className="bg-red-900/20 rounded-lg p-3 text-center">
                          <p className="text-red-400 font-bold text-xl">{selectedHero.strength}</p>
                          <p className="text-gray-400 text-sm">Força</p>
                          <p className="text-gray-500 text-xs">
                            Base: {selectedHero.baseStrength} + Bônus: {selectedHero.bonusStrength}
                          </p>
                          <p className="text-gray-500 text-xs">Ataque: {selectedHero.totalAttack}</p>
                        </div>
                        <div className="bg-blue-900/20 rounded-lg p-3 text-center">
                          <p className="text-blue-400 font-bold text-xl">{selectedHero.intelligence}</p>
                          <p className="text-gray-400 text-sm">Inteligência</p>
                          <p className="text-gray-500 text-xs">
                            Base: {selectedHero.baseIntelligence} + Bônus: {selectedHero.bonusIntelligence}
                          </p>
                          <p className="text-gray-500 text-xs">Magia: {selectedHero.totalMagic}</p>
                        </div>
                        <div className="bg-green-900/20 rounded-lg p-3 text-center">
                          <p className="text-green-400 font-bold text-xl">{selectedHero.dexterity}</p>
                          <p className="text-gray-400 text-sm">Destreza</p>
                          <p className="text-gray-500 text-xs">
                            Base: {selectedHero.baseDexterity} + Bônus: {selectedHero.bonusDexterity}
                          </p>
                          <p className="text-gray-500 text-xs">Defesa: {selectedHero.totalDefense}</p>
                        </div>
                      </div>
                    </div>

                    {/* Informações da Classe */}
                    {getClassInfo(selectedHero.class) && (
                      <div className="mb-6">
                        <h3 className="text-lg font-bold text-white mb-3">Informações da Classe</h3>
                        <div className="bg-gray-800 rounded-lg p-4">
                          <p className="text-gray-300 text-sm mb-2">
                            {getClassInfo(selectedHero.class)?.description}
                          </p>
                          <p className="text-gray-400 text-xs">
                            Foco: {getClassInfo(selectedHero.class)?.combatFocus} | 
                            Recomendado para: {getClassInfo(selectedHero.class)?.recommendedFor}
                          </p>
                        </div>
                      </div>
                    )}

                    {/* Distribuição de Pontos */}
                    {selectedHero.unallocatedPoints > 0 && (
                      <div>
                        <h3 className="text-lg font-bold text-white mb-3">
                          Distribuir Pontos Bônus ({selectedHero.unallocatedPoints} disponíveis)
                        </h3>
                        <p className="text-gray-400 text-sm mb-4">
                          ⚠️ Os atributos base da classe são fixos. Você só pode distribuir pontos ganhos ao subir de nível.
                        </p>
                        
                        <div className="space-y-3 mb-4">
                          <div className="flex items-center justify-between">
                            <label className="text-gray-300">Força:</label>
                            <div className="flex items-center gap-2">
                              <Button
                                onClick={() => setAllocationRequest(prev => ({
                                  ...prev,
                                  strengthPoints: Math.max(0, prev.strengthPoints - 1)
                                }))}
                                variant="secondary"
                                className="px-2 py-1 text-xs"
                                disabled={allocationRequest.strengthPoints <= 0}
                              >
                                -
                              </Button>
                              <span className="text-white font-bold w-8 text-center">
                                {allocationRequest.strengthPoints}
                              </span>
                              <Button
                                onClick={() => setAllocationRequest(prev => ({
                                  ...prev,
                                  strengthPoints: prev.strengthPoints + 1
                                }))}
                                variant="secondary"
                                className="px-2 py-1 text-xs"
                                disabled={allocationRequest.strengthPoints + allocationRequest.intelligencePoints + allocationRequest.dexterityPoints >= selectedHero.unallocatedPoints}
                              >
                                +
                              </Button>
                            </div>
                          </div>
                          
                          <div className="flex items-center justify-between">
                            <label className="text-gray-300">Inteligência:</label>
                            <div className="flex items-center gap-2">
                              <Button
                                onClick={() => setAllocationRequest(prev => ({
                                  ...prev,
                                  intelligencePoints: Math.max(0, prev.intelligencePoints - 1)
                                }))}
                                variant="secondary"
                                className="px-2 py-1 text-xs"
                                disabled={allocationRequest.intelligencePoints <= 0}
                              >
                                -
                              </Button>
                              <span className="text-white font-bold w-8 text-center">
                                {allocationRequest.intelligencePoints}
                              </span>
                              <Button
                                onClick={() => setAllocationRequest(prev => ({
                                  ...prev,
                                  intelligencePoints: prev.intelligencePoints + 1
                                }))}
                                variant="secondary"
                                className="px-2 py-1 text-xs"
                                disabled={allocationRequest.strengthPoints + allocationRequest.intelligencePoints + allocationRequest.dexterityPoints >= selectedHero.unallocatedPoints}
                              >
                                +
                              </Button>
                            </div>
                          </div>
                          
                          <div className="flex items-center justify-between">
                            <label className="text-gray-300">Destreza:</label>
                            <div className="flex items-center gap-2">
                              <Button
                                onClick={() => setAllocationRequest(prev => ({
                                  ...prev,
                                  dexterityPoints: Math.max(0, prev.dexterityPoints - 1)
                                }))}
                                variant="secondary"
                                className="px-2 py-1 text-xs"
                                disabled={allocationRequest.dexterityPoints <= 0}
                              >
                                -
                              </Button>
                              <span className="text-white font-bold w-8 text-center">
                                {allocationRequest.dexterityPoints}
                              </span>
                              <Button
                                onClick={() => setAllocationRequest(prev => ({
                                  ...prev,
                                  dexterityPoints: prev.dexterityPoints + 1
                                }))}
                                variant="secondary"
                                className="px-2 py-1 text-xs"
                                disabled={allocationRequest.strengthPoints + allocationRequest.intelligencePoints + allocationRequest.dexterityPoints >= selectedHero.unallocatedPoints}
                              >
                                +
                              </Button>
                            </div>
                          </div>
                        </div>
                        
                        <div className="text-center">
                          <p className="text-gray-400 text-sm mb-3">
                            Total: {allocationRequest.strengthPoints + allocationRequest.intelligencePoints + allocationRequest.dexterityPoints} / {selectedHero.unallocatedPoints}
                          </p>
                          <Button
                            onClick={handleAllocateAttributes}
                            disabled={allocating || (allocationRequest.strengthPoints + allocationRequest.intelligencePoints + allocationRequest.dexterityPoints) === 0}
                            variant="success"
                            className="w-full"
                          >
                            {allocating ? 'Distribuindo...' : 'Distribuir Pontos'}
                          </Button>
                        </div>
                      </div>
                    )}

                    {selectedHero.unallocatedPoints === 0 && (
                      <div className="text-center py-4">
                        <p className="text-gray-400">Este herói não possui pontos de atributo para distribuir.</p>
                        <p className="text-gray-500 text-sm">Ganhe XP completando quests para subir de nível!</p>
                      </div>
                    )}
                  </div>
                </Card>
              </div>
            ) : (
              <div className="text-center py-12">
                <div className="text-6xl mb-4">⚔️</div>
                <h3 className="text-xl font-bold text-gray-300 mb-2">Selecione um Herói</h3>
                <p className="text-gray-400">Escolha um herói da lista para gerenciar seus atributos</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};
