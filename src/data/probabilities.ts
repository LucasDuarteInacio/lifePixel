/**
 * Centralized probabilities configuration for the LifePixel game
 * All probability values are defined here for easy maintenance and balancing
 */

export interface ProbabilityConfig {
  // Probabilidades de eventos por categoria
  events: {
    // Eventos da infância (0-12 anos)
    childhood: {
      born: 100;                    // Nascimento - evento obrigatório
      firstWords: 80;               // Primeiras palavras - marco importante do desenvolvimento
      firstSteps: 85;               // Primeiros passos - marco motor fundamental
      kindergarten: 90;             // Jardim de infância - início da socialização
      childhoodIllness: 30;         // Doença infantil - problemas de saúde comuns
      talentDiscovery: 20;          // Descoberta de talento - habilidades especiais
      familySupport: 60;            // Apoio da família - suporte emocional
      familyConflict: 30;           // Conflito familiar - tensões domésticas
      siblingBirth: 15;             // Nascimento de irmão - expansão da família
      familyTrip: 25;               // Viagem em família - momentos especiais
    };
    
    // Eventos da adolescência (13-17 anos)
    adolescence: {
      puberty: 100;                 // Puberdade - mudanças físicas obrigatórias
      firstCrush: 70;               // Primeira paixão - despertar romântico
      schoolBully: 25;              // Bullying na escola - problemas sociais
      academicExcellence: 15;       // Excelência acadêmica - destaque nos estudos
      teenRebellion: 40;            // Rebeldia adolescente - fase de contestação
    };
    
    // Eventos da juventude (18-25 anos)
    youngAdult: {
      highSchoolGraduation: 95;     // Formatura do ensino médio - conclusão educacional
      collegeAdmission: 60;         // Admissão na faculdade - continuidade dos estudos
      firstJob: 70;                 // Primeiro emprego - entrada no mercado de trabalho
      collegeDropout: 15;           // Abandono da faculdade - desistência dos estudos
      seriousRelationship: 50;      // Relacionamento sério - compromisso romântico
    };
    
    // Eventos da vida adulta (26-50 anos)
    adult: {
      marriage: 40;                 // Casamento - união matrimonial
      childBirth: 30;               // Nascimento de filho - paternidade/maternidade
      jobPromotion: 25;             // Promoção no trabalho - crescimento profissional
      jobLoss: 10;                  // Perda do emprego - desemprego involuntário
      divorce: 15;                  // Divórcio - fim do casamento
      inheritance: 5;               // Herança - recebimento de bens familiares
    };
    
    // Eventos da meia-idade (51-65 anos)
    middleAge: {
      midlifeCrisis: 30;            // Crise da meia-idade - questionamentos existenciais
      careerPeak: 20;               // Pico da carreira - auge profissional
      healthIssues: 25;             // Problemas de saúde - declínio físico
    };
    
    // Eventos da terceira idade (66+ anos)
    senior: {
      retirement: 90;               // Aposentadoria - fim da vida profissional
      grandchildren: 40;            // Netos - nova geração familiar
      seriousIllness: 20;           // Doença grave - problemas de saúde severos
      death: 5;                     // Morte - fim da vida
    };
  };
  
  // Probabilidades de eventos especiais interativos
  specialEvents: {
    // Eventos de amizade
    newFriendSchool: 25;            // Novo amigo na escola - socialização estudantil
    newFriendWork: 20;              // Colega de trabalho - amizade profissional
    neighborFriend: 15;             // Vizinho amigável - relacionamento de vizinhança
    
    // Eventos românticos
    romanticInterest: 18;           // Interesse romântico - oportunidade amorosa
    
    // Eventos de conflito
    bullyConfrontation: 12;         // Confronto com valentão - situação de bullying
    
    // Eventos de oportunidade
    mentorOpportunity: 10;          // Oportunidade de mentoria - crescimento profissional
    
    // Eventos familiares
    distantRelative: 8;             // Parente distante - reconexão familiar
  };
  
  // Probabilidades de resultados de risco para eventos especiais
  riskOutcomes: {
    // Riscos de interesse romântico
    romanticInterestBetrayal: 15;           // Traição no relacionamento - descobrir infidelidade
    romanticInterestRejectionBacklash: 30;  // Reação negativa à rejeição - vingança social
    
    // Riscos de confronto com valentão
    bullyStandUpBackfire: 35;               // Enfrentar valentão dá errado - apanhar em público
    bullyAvoidanceBackfire: 25;             // Evitar conflito dá errado - humilhação pública
    bullySeekHelpBackfire: 20;              // Buscar ajuda dá errado - adulto não ajuda
    
    // Riscos de oportunidade de mentoria
    mentorDubiousPractices: 20;             // Mentor com práticas duvidosas - envolvimento em problemas
  };
  
  // Probabilidades de morte por faixa etária
  deathProbabilities: {
    young: number;       
    middleAged: number;  
    older: number;       
    elderly: number;    
    veryElderly: number; 
    ancient: number;     
  };
  
  // Probabilidades de mecânicas gerais do jogo
  mechanics: {
    specialEventChance: 0.1;  // Chance de evento especial: 10% por ano
  };
}

/**
 * Main probability configuration object
 */
export const PROBABILITIES: ProbabilityConfig = {
  events: {
    childhood: {
      born: 100,
      firstWords: 80,
      firstSteps: 85,
      kindergarten: 90,
      childhoodIllness: 30,
      talentDiscovery: 20,
      familySupport: 60,
      familyConflict: 30,
      siblingBirth: 15,
      familyTrip: 25,
    },
    
    adolescence: {
      puberty: 100,
      firstCrush: 70,
      schoolBully: 25,
      academicExcellence: 15,
      teenRebellion: 40,
    },
    
    youngAdult: {
      highSchoolGraduation: 95,
      collegeAdmission: 60,
      firstJob: 70,
      collegeDropout: 15,
      seriousRelationship: 50,
    },
    
    adult: {
      marriage: 40,
      childBirth: 30,
      jobPromotion: 25,
      jobLoss: 10,
      divorce: 15,
      inheritance: 5,
    },
    
    middleAge: {
      midlifeCrisis: 30,
      careerPeak: 20,
      healthIssues: 25,
    },
    
    senior: {
      retirement: 90,
      grandchildren: 40,
      seriousIllness: 20,
      death: 5,
    },
  },
  
  specialEvents: {
    newFriendSchool: 25,
    newFriendWork: 20,
    neighborFriend: 15,
    romanticInterest: 18,
    bullyConfrontation: 12,
    mentorOpportunity: 10,
    distantRelative: 8,
  },
  
  riskOutcomes: {
    romanticInterestBetrayal: 15,
    romanticInterestRejectionBacklash: 30,
    bullyStandUpBackfire: 35,
    bullyAvoidanceBackfire: 25,
    bullySeekHelpBackfire: 20,
    mentorDubiousPractices: 20,
  },
  
  deathProbabilities: {
    young: 0.001,       // Jovens (< 50 anos): baixo risco
    middleAged: 0.005,  // Meia-idade (50-59 anos): risco moderado
    older: 0.070,       // Idosos (60-69 anos): risco aumentado
    elderly: 0.100,     // Terceira idade (70-79 anos): risco alto
    veryElderly: 0.400, // Muito idosos (80-89 anos): risco muito alto
    ancient: 0.900,     // Anciãos (90+ anos): risco extremo
  },
  
  mechanics: {
    specialEventChance: 0.1,
  },
};

/**
 * Helper function to get death probability based on age
 */
export function getDeathProbabilityByAge(age: number): number {
  if (age < 50) return PROBABILITIES.deathProbabilities.young;
  if (age < 60) return PROBABILITIES.deathProbabilities.middleAged;
  if (age < 70) return PROBABILITIES.deathProbabilities.older;
  if (age < 80) return PROBABILITIES.deathProbabilities.elderly;
  if (age < 90) return PROBABILITIES.deathProbabilities.veryElderly;
  return PROBABILITIES.deathProbabilities.ancient;
}

/**
 * Helper function to get special event generation chance
 */
export function getSpecialEventChance(): number {
  return PROBABILITIES.mechanics.specialEventChance;
}