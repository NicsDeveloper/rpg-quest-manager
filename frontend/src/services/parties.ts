import { api } from './api';

export interface Party {
  id: number;
  name: string;
  description: string;
  leaderId: number;
  maxMembers: number;
  isPublic: boolean;
  status: string;
  createdAt: string;
  lastActivityAt: string;
  leader: {
    id: number;
    username: string;
  };
  members: Array<{
    id: number;
    userId: number;
    characterId: number;
    role: string;
    joinedAt: string;
    lastActiveAt: string;
    user: {
      id: number;
      username: string;
    };
    character: {
      id: number;
      name: string;
      level: number;
    };
  }>;
}

export interface PartyInvite {
  id: number;
  partyId: number;
  inviterId: number;
  inviteeId: number;
  status: string;
  message: string;
  sentAt: string;
  respondedAt?: string;
  expiresAt: string;
  party: {
    id: number;
    name: string;
    description: string;
  };
  inviter: {
    id: number;
    username: string;
  };
}

class PartyService {
  async getPublicParties(): Promise<Party[]> {
    const response = await api.get('/parties/public');
    return response.data;
  }

  async getPartyById(partyId: number): Promise<Party> {
    const response = await api.get(`/parties/${partyId}`);
    return response.data;
  }

  async getUserParty(userId: number): Promise<Party | null> {
    try {
      const response = await api.get(`/parties/user/${userId}`);
      return response.data;
    } catch (error: any) {
      // Se for 404, significa que o usuário não está em nenhum grupo
      if (error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  }

  async getUserInvites(userId: number): Promise<PartyInvite[]> {
    try {
      const response = await api.get(`/parties/user/${userId}/invites`);
      return response.data;
    } catch (error: any) {
      // Se for 404, significa que não há convites
      if (error.response?.status === 404) {
        return [];
      }
      throw error;
    }
  }

  async createParty(userId: number, name: string, description: string, isPublic: boolean): Promise<Party> {
    const response = await api.post('/parties/create', {
      userId,
      name,
      description,
      isPublic
    });
    return response.data.party;
  }

  async joinParty(userId: number, partyId: number): Promise<void> {
    await api.post('/parties/join', {
      userId,
      partyId
    });
  }

  async leaveParty(userId: number, partyId: number): Promise<void> {
    await api.post('/parties/leave', {
      userId,
      partyId
    });
  }

  async inviteToParty(inviterId: number, inviteeId: number, partyId: number, message: string): Promise<void> {
    await api.post('/parties/invite', {
      inviterId,
      inviteeId,
      partyId,
      message
    });
  }

  async respondToInvite(userId: number, inviteId: number, accept: boolean): Promise<void> {
    await api.post('/parties/respond-invite', {
      userId,
      inviteId,
      accept
    });
  }

  async kickMember(leaderId: number, memberId: number, partyId: number): Promise<void> {
    await api.post('/parties/kick-member', {
      leaderId,
      memberId,
      partyId
    });
  }

  async transferLeadership(currentLeaderId: number, newLeaderId: number, partyId: number): Promise<void> {
    await api.post('/parties/transfer-leadership', {
      currentLeaderId,
      newLeaderId,
      partyId
    });
  }

  async disbandParty(leaderId: number, partyId: number): Promise<void> {
    await api.post('/parties/disband', {
      leaderId,
      partyId
    });
  }
}

export const partyService = new PartyService();
