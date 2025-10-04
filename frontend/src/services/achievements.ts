import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api';

export interface Achievement {
  id: number;
  name: string;
  description: string;
  iconUrl: string;
  type: string;
  category: string;
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
    const response = await axios.get(`${API_BASE_URL}/achievements`);
    return response.data;
  }

  async getUserAchievements(userId: number): Promise<UserAchievement[]> {
    const response = await axios.get(`${API_BASE_URL}/achievements/user/${userId}`);
    return response.data;
  }

  async getAvailableAchievements(userId: number): Promise<Achievement[]> {
    const response = await axios.get(`${API_BASE_URL}/achievements/user/${userId}/available`);
    return response.data;
  }

  async getCompletedAchievements(userId: number): Promise<UserAchievement[]> {
    const response = await axios.get(`${API_BASE_URL}/achievements/user/${userId}/completed`);
    return response.data;
  }

  async getClaimedAchievements(userId: number): Promise<UserAchievement[]> {
    const response = await axios.get(`${API_BASE_URL}/achievements/user/${userId}/claimed`);
    return response.data;
  }

  async getUserAchievement(userId: number, achievementId: number): Promise<UserAchievement | null> {
    try {
      const response = await axios.get(`${API_BASE_URL}/achievements/user/${userId}/achievement/${achievementId}`);
      return response.data;
    } catch (error) {
      return null;
    }
  }

  async updateAchievementProgress(userId: number, type: string, value: number): Promise<void> {
    await axios.post(`${API_BASE_URL}/achievements/update-progress`, {
      userId,
      type,
      value
    });
  }

  async claimAchievementReward(userId: number, achievementId: number): Promise<void> {
    await axios.post(`${API_BASE_URL}/achievements/claim-reward`, {
      userId,
      achievementId
    });
  }
}

export const achievementService = new AchievementService();
