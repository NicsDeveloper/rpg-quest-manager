import { api } from './api';

// AchievementType values (from backend)
export const AchievementType = {
  Combat: 0,
  Quest: 1,
  Exploration: 2,
  Collection: 3,
  Social: 4,
  Progression: 5,
  Special: 6
} as const;

// AchievementCategory values (from backend)
export const AchievementCategory = {
  Bronze: 0,
  Silver: 1,
  Gold: 2,
  Platinum: 3,
  Legendary: 4,
  Mythic: 5
} as const;

export interface Achievement {
  id: number;
  name: string;
  description: string;
  iconUrl: string;
  type: number; // AchievementType enum
  category: number; // AchievementCategory enum
  requiredValue: number;
  experienceReward: number;
  goldReward: number;
  itemRewardId?: number;
  isHidden: boolean;
  isRepeatable: boolean;
  sortOrder: number;
  createdAt: string;
}

export interface UserAchievement {
  id: number;
  userId: number;
  achievementId: number;
  progress: number;
  isCompleted: boolean;
  isClaimed: boolean;
  completedAt: string;
  claimedAt: string;
  createdAt: string;
  achievement: Achievement;
}

class AchievementService {
  async getAllAchievements(): Promise<Achievement[]> {
    const response = await api.get('/achievements');
    return response.data;
  }

  async getUserAchievements(userId: number): Promise<UserAchievement[]> {
    const response = await api.get(`/achievements/user/${userId}`);
    return response.data;
  }

  async getAvailableAchievements(userId: number): Promise<Achievement[]> {
    const response = await api.get(`/achievements/user/${userId}/available`);
    return response.data;
  }

  async getCompletedAchievements(userId: number): Promise<UserAchievement[]> {
    const response = await api.get(`/achievements/user/${userId}/completed`);
    return response.data;
  }

  async getClaimedAchievements(userId: number): Promise<UserAchievement[]> {
    const response = await api.get(`/achievements/user/${userId}/claimed`);
    return response.data;
  }

  async getUserAchievement(userId: number, achievementId: number): Promise<UserAchievement | null> {
    try {
      const response = await api.get(`/achievements/user/${userId}/achievement/${achievementId}`);
      return response.data;
    } catch (error) {
      return null;
    }
  }

  async updateAchievementProgress(userId: number, type: string, value: number): Promise<void> {
    await api.post('/achievements/update-progress', {
      userId,
      type,
      value
    });
  }

  async claimAchievementReward(userId: number, achievementId: number): Promise<void> {
    await api.post('/achievements/claim-reward', {
      userId,
      achievementId
    });
  }
}

export const achievementService = new AchievementService();
