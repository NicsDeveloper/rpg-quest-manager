import { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { partyService, type Party, type PartyInvite } from '../services/parties';
import { Card } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Modal } from '../components/ui/Modal';
import { 
  Users, 
  Plus, 
  Crown, 
  User, 
  Shield, 
  Check,
  X,
  Trash2,
  UserPlus
} from 'lucide-react';

export default function Parties() {
  const { user } = useAuth();
  const [parties, setParties] = useState<Party[]>([]);
  const [userParty, setUserParty] = useState<Party | null>(null);
  const [invites, setInvites] = useState<PartyInvite[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [newParty, setNewParty] = useState({ name: '', description: '', isPublic: true });

  useEffect(() => {
    if (user) {
      loadParties();
    }
  }, [user]);

  const loadParties = async () => {
    if (!user) return;
    
    try {
      setLoading(true);
      const [publicParties, userPartyData, invitesData] = await Promise.all([
        partyService.getPublicParties(),
        partyService.getUserParty(user.id),
        partyService.getUserInvites(user.id)
      ]);
      
      setParties(publicParties);
      setUserParty(userPartyData);
      setInvites(invitesData);
    } catch (error) {
      console.error('Erro ao carregar grupos:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateParty = async () => {
    if (!user) return;
    
    try {
      await partyService.createParty(user.id, newParty.name, newParty.description, newParty.isPublic);
      setShowCreateModal(false);
      setNewParty({ name: '', description: '', isPublic: true });
      await loadParties();
    } catch (error) {
      console.error('Erro ao criar grupo:', error);
    }
  };

  const handleJoinParty = async (partyId: number) => {
    if (!user) return;
    
    try {
      await partyService.joinParty(user.id, partyId);
      await loadParties();
    } catch (error) {
      console.error('Erro ao entrar no grupo:', error);
    }
  };

  const handleLeaveParty = async () => {
    if (!user || !userParty) return;
    
    try {
      await partyService.leaveParty(user.id, userParty.id);
      await loadParties();
    } catch (error) {
      console.error('Erro ao sair do grupo:', error);
    }
  };

  const handleRespondToInvite = async (inviteId: number, accept: boolean) => {
    if (!user) return;
    
    try {
      await partyService.respondToInvite(user.id, inviteId, accept);
      await loadParties();
    } catch (error) {
      console.error('Erro ao responder convite:', error);
    }
  };

  const handleKickMember = async (memberId: number) => {
    if (!user || !userParty) return;
    
    try {
      await partyService.kickMember(user.id, memberId, userParty.id);
      await loadParties();
    } catch (error) {
      console.error('Erro ao expulsar membro:', error);
    }
  };

  const handleTransferLeadership = async (newLeaderId: number) => {
    if (!user || !userParty) return;
    
    try {
      await partyService.transferLeadership(user.id, newLeaderId, userParty.id);
      await loadParties();
    } catch (error) {
      console.error('Erro ao transferir liderança:', error);
    }
  };

  const handleDisbandParty = async () => {
    if (!user || !userParty) return;
    
    try {
      await partyService.disbandParty(user.id, userParty.id);
      await loadParties();
    } catch (error) {
      console.error('Erro ao dissolver grupo:', error);
    }
  };

  const getRoleIcon = (role: string) => {
    switch (role.toLowerCase()) {
      case 'leader': return Crown;
      case 'officer': return Shield;
      default: return User;
    }
  };

  const getRoleColor = (role: string) => {
    switch (role.toLowerCase()) {
      case 'leader': return 'text-yellow-600';
      case 'officer': return 'text-blue-600';
      default: return 'text-gray-600';
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Grupos</h1>
          <p className="text-gray-600 mt-2">Junte-se a outros jogadores e forme grupos</p>
        </div>
        {!userParty && (
          <Button onClick={() => setShowCreateModal(true)}>
            <Plus className="h-4 w-4 mr-2" />
            Criar Grupo
          </Button>
        )}
      </div>

      {/* User Party */}
      {userParty && (
        <Card title="Seu Grupo">
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h3 className="text-lg font-semibold text-gray-900">{userParty.name}</h3>
                <p className="text-gray-600">{userParty.description}</p>
              </div>
              <div className="flex items-center space-x-2">
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                  userParty.isPublic ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                }`}>
                  {userParty.isPublic ? 'Público' : 'Privado'}
                </span>
                <span className="text-sm text-gray-500">
                  {userParty.members.length}/{userParty.maxMembers} membros
                </span>
              </div>
            </div>

            {/* Members */}
            <div>
              <h4 className="font-medium text-gray-900 mb-2">Membros</h4>
              <div className="space-y-2">
                {userParty.members.map((member) => {
                  const RoleIcon = getRoleIcon(member.role);
                  const roleColor = getRoleColor(member.role);
                  const isLeader = member.role === 'Leader';
                  const canManage = userParty.leaderId === user?.id && !isLeader;

                  return (
                    <div key={member.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                      <div className="flex items-center space-x-3">
                        <RoleIcon className={`h-5 w-5 ${roleColor}`} />
                        <div>
                          <p className="font-medium text-gray-900">{member.user.username}</p>
                          <p className="text-sm text-gray-600">
                            {member.character.name} (Nível {member.character.level})
                          </p>
                        </div>
                      </div>
                      <div className="flex items-center space-x-2">
                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${roleColor}`}>
                          {member.role}
                        </span>
                        {canManage && (
                          <div className="flex space-x-1">
                            {!isLeader && (
                              <Button
                                size="sm"
                                variant="secondary"
                                onClick={() => handleTransferLeadership(member.userId)}
                              >
                                <Crown className="h-4 w-4" />
                              </Button>
                            )}
                            <Button
                              size="sm"
                              variant="danger"
                              onClick={() => handleKickMember(member.userId)}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Actions */}
            <div className="flex space-x-2">
              <Button variant="secondary" onClick={handleLeaveParty}>
                Sair do Grupo
              </Button>
              {userParty.leaderId === user?.id && (
                <Button variant="danger" onClick={handleDisbandParty}>
                  Dissolver Grupo
                </Button>
              )}
            </div>
          </div>
        </Card>
      )}

      {/* Invites */}
      {invites.length > 0 && (
        <Card title="Convites Pendentes">
          <div className="space-y-3">
            {invites.map((invite) => (
              <div key={invite.id} className="flex items-center justify-between p-3 bg-blue-50 rounded-lg">
                <div>
                  <p className="font-medium text-gray-900">
                    Convite para {invite.party.name}
                  </p>
                  <p className="text-sm text-gray-600">
                    De: {invite.inviter.username}
                  </p>
                  {invite.message && (
                    <p className="text-sm text-gray-500 mt-1">"{invite.message}"</p>
                  )}
                </div>
                <div className="flex space-x-2">
                  <Button
                    size="sm"
                    variant="success"
                    onClick={() => handleRespondToInvite(invite.id, true)}
                  >
                    <Check className="h-4 w-4" />
                  </Button>
                  <Button
                    size="sm"
                    variant="danger"
                    onClick={() => handleRespondToInvite(invite.id, false)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </Card>
      )}

      {/* Public Parties */}
      {!userParty && (
        <Card title="Grupos Públicos">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {parties.map((party) => (
              <div key={party.id} className="p-4 border border-gray-200 rounded-lg">
                <div className="flex items-start justify-between mb-3">
                  <div>
                    <h3 className="font-semibold text-gray-900">{party.name}</h3>
                    <p className="text-sm text-gray-600">{party.description}</p>
                  </div>
                  <span className="text-sm text-gray-500">
                    {party.members.length}/{party.maxMembers}
                  </span>
                </div>
                
                <div className="flex items-center space-x-2 mb-3">
                  <Crown className="h-4 w-4 text-yellow-600" />
                  <span className="text-sm text-gray-600">{party.leader.username}</span>
                </div>
                
                <Button
                  className="w-full"
                  onClick={() => handleJoinParty(party.id)}
                >
                  <UserPlus className="h-4 w-4 mr-2" />
                  Entrar
                </Button>
              </div>
            ))}
          </div>
          
          {parties.length === 0 && (
            <div className="text-center py-8">
              <Users className="h-12 w-12 text-gray-400 mx-auto mb-4" />
              <p className="text-gray-500">Nenhum grupo público encontrado</p>
            </div>
          )}
        </Card>
      )}

      {/* Create Party Modal */}
      <Modal
        isOpen={showCreateModal}
        onClose={() => setShowCreateModal(false)}
        title="Criar Grupo"
        size="md"
      >
        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Nome do Grupo
            </label>
            <input
              type="text"
              value={newParty.name}
              onChange={(e) => setNewParty(prev => ({ ...prev, name: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Digite o nome do grupo"
            />
          </div>
          
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Descrição
            </label>
            <textarea
              value={newParty.description}
              onChange={(e) => setNewParty(prev => ({ ...prev, description: e.target.value }))}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
              rows={3}
              placeholder="Descreva o grupo"
            />
          </div>
          
          <div className="flex items-center">
            <input
              type="checkbox"
              id="isPublic"
              checked={newParty.isPublic}
              onChange={(e) => setNewParty(prev => ({ ...prev, isPublic: e.target.checked }))}
              className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
            />
            <label htmlFor="isPublic" className="ml-2 block text-sm text-gray-900">
              Grupo público (visível para outros jogadores)
            </label>
          </div>
          
          <div className="flex space-x-2">
            <Button
              onClick={handleCreateParty}
              className="flex-1"
            >
              Criar Grupo
            </Button>
            <Button
              variant="secondary"
              onClick={() => setShowCreateModal(false)}
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
