/**
 * Tipos principais do jogo VidaPixel
 */

export interface Character {
  id: string;
  firstName: string;
  lastName: string;
  gender: 'male' | 'female';
  country: string;
  age: number;
  isAlive: boolean;
  stats: CharacterStats;
  relationships: Relationship[];
  career: Career;
  education: Education;
  events: GameEvent[];
  createdAt: Date;
  lastSaved: Date;
}

export interface CharacterStats {
  happiness: number; // 0-100
  health: number; // 0-100
  money: number; // Pode ser negativo
  intelligence: number; // 0-100
  looks: number; // 0-100
  popularity: number; // 0-100
}

export interface Relationship {
  id: string;
  name: string;
  type: 'family' | 'friend' | 'romantic' | 'colleague';
  status: 'good' | 'neutral' | 'bad';
  age: number;
  gender: 'male' | 'female';
  isAlive: boolean;
}

export interface Career {
  job: string | null;
  salary: number;
  experience: number;
  isEmployed: boolean;
}

export interface Education {
  level: 'none' | 'elementary' | 'high_school' | 'college' | 'university' | 'phd';
  institution: string | null;
  graduationYear: number | null;
}

export interface GameEvent {
  id: string;
  title: string;
  description: string;
  age: number;
  type: 'positive' | 'negative' | 'neutral';
  effects: EventEffects;
  requirements?: EventRequirements;
  timestamp: Date;
}

export interface EventEffects {
  happiness?: number;
  health?: number;
  money?: number;
  intelligence?: number;
  looks?: number;
  popularity?: number;
}

export interface EventRequirements {
  minAge?: number;
  maxAge?: number;
  minHappiness?: number;
  minHealth?: number;
  minMoney?: number;
  minIntelligence?: number;
  isEmployed?: boolean;
  hasEducation?: boolean;
  hasRelationship?: boolean;
}

export interface EventTemplate {
  id: string;
  title: string;
  description: string;
  type: 'positive' | 'negative' | 'neutral';
  effects: EventEffects;
  requirements?: EventRequirements;
  probability: number; // 0-100
  ageRange?: {
    min: number;
    max: number;
  };
}

export interface GameState {
  character: Character | null;
  currentScreen: 'start' | 'game' | 'status' | 'events';
  isLoading: boolean;
  error: string | null;
}

export interface Country {
  name: string;
  code: string;
  currency: string;
}

export interface NameData {
  male: string[];
  female: string[];
  surnames: string[];
} 