import { NameData, Relationship } from '@/types/game';

export const names: NameData = {
  male: [
    'João', 'Pedro', 'Lucas', 'Gabriel', 'Matheus', 'Rafael', 'Bruno', 'Carlos',
    'Daniel', 'André', 'Thiago', 'Felipe', 'Marcos', 'Leonardo', 'Alexandre',
    'Diego', 'Ricardo', 'Eduardo', 'Fernando', 'Roberto', 'Paulo', 'Marcelo',
    'Rodrigo', 'Antônio', 'José', 'Francisco', 'Luiz', 'Miguel', 'Arthur',
    'Heitor', 'Bernardo', 'Davi', 'Theo', 'Enzo', 'Valentim', 'Benjamin',
    'Nicolas', 'Guilherme', 'Isaac', 'Samuel', 'Henrique', 'Caio',
    'Lorenzo', 'Gustavo', 'Murilo', 'Pietro', 'Lucca', 'Anthony', 'Bento', 'Joaquim'
  ],
  female: [
    'Maria', 'Ana', 'Julia', 'Sofia', 'Isabella', 'Manuela', 'Giovanna',
    'Alice', 'Laura', 'Luiza', 'Valentina', 'Beatriz', 'Gabriela', 'Rafaela',
    'Carolina', 'Mariana', 'Clara', 'Letícia', 'Yasmin', 'Isabela',
    'Lívia', 'Helena', 'Lorena', 'Eduarda', 'Fernanda',
    'Júlia', 'Paula', 'Antonella', 'Camila', 'Amanda', 'Vitória'
  ],
  surnames: [
    'Silva', 'Santos', 'Oliveira', 'Souza', 'Rodrigues', 'Ferreira',
    'Alves', 'Pereira', 'Lima', 'Gomes', 'Costa', 'Ribeiro', 'Martins',
    'Carvalho', 'Almeida', 'Lopes', 'Soares', 'Fernandes', 'Vieira',
    'Barbosa', 'Rocha', 'Dias', 'Cavalcanti', 'Nascimento', 'Melo',
    'Cardoso', 'Teixeira', 'Moura', 'Santana', 'Castro', 'Araújo',
    'Correia', 'Sousa', 'Machado', 'Andrade', 'Dantas',
    'Cunha', 'Pinto', 'Monteiro', 'Moreira', 'Medeiros', 'Nunes',
    'Freitas', 'Barros', 'Azevedo', 'Borges', 'Mendes', 'Ramos'
  ]
};

export function getRandomName(gender: 'male' | 'female'): { firstName: string; lastName: string } {
  const firstName = gender === 'male' 
    ? names.male[Math.floor(Math.random() * names.male.length)]
    : names.female[Math.floor(Math.random() * names.female.length)];
  
  const lastName = names.surnames[Math.floor(Math.random() * names.surnames.length)];
  
  return { firstName, lastName };
}

export function getRandomGender(): 'male' | 'female' {
  return Math.random() > 0.5 ? 'male' : 'female';
}

/**
 * Gerar um membro da família (pai, mãe ou irmão)
 */
export function generateFamilyMember(
  type: 'father' | 'mother' | 'sibling',
  characterAge: number,
  characterSurname: string
): Relationship {
  const id = `family_${type}_${typeof window !== 'undefined' ? Date.now() : 0}_${typeof window !== 'undefined' ? Math.random().toString(36).substr(2, 9) : 'temp'}`;
  
  let name: string;
  let age: number;
  let gender: 'male' | 'female';
  
  switch (type) {
    case 'father':
      gender = 'male';
      const fatherName = getRandomName('male');
      name = `${fatherName.firstName} ${characterSurname}`;
      // Pai tem entre 20 e 50 anos a mais que o personagem
      age = characterAge + 20 + Math.floor(Math.random() * 30);
      break;
    case 'mother':
      gender = 'female';
      const motherName = getRandomName('female');
      name = `${motherName.firstName} ${characterSurname}`;
      // Mãe tem entre 18 e 45 anos a mais que o personagem
      age = characterAge + 18 + Math.floor(Math.random() * 27);
      break;
    case 'sibling':
      gender = getRandomGender();
      const siblingName = getRandomName(gender);
      name = `${siblingName.firstName} ${characterSurname}`;
      // Irmão tem entre -5 e +10 anos de diferença
      age = Math.max(0, characterAge - 5 + Math.floor(Math.random() * 15));
      break;
  }
  
  return {
    id,
    name,
    type: 'family',
    status: 'good',
    age,
    gender,
    isAlive: true
  };
}

/**
 * Gerar família completa para um personagem
 */
export function generateFamily(characterAge: number, characterSurname: string): Relationship[] {
  const family: Relationship[] = [];
  
  // Sempre ter pelo menos um pai ou uma mãe
  const hasFather = Math.random() > 0.3; // 70% de chance de ter pai
  const hasMother = Math.random() > 0.1; // 90% de chance de ter mãe
  
  // Garantir que pelo menos um dos pais existe
  if (!hasFather && !hasMother) {
    if (Math.random() > 0.5) {
      family.push(generateFamilyMember('father', characterAge, characterSurname));
    } else {
      family.push(generateFamilyMember('mother', characterAge, characterSurname));
    }
  } else {
    if (hasFather) {
      family.push(generateFamilyMember('father', characterAge, characterSurname));
    }
    if (hasMother) {
      family.push(generateFamilyMember('mother', characterAge, characterSurname));
    }
  }
  
  // Adicionar irmãos com probabilidades específicas
  const siblingRandom = Math.random();
  let siblingCount = 0;
  
  if (siblingRandom <= 0.3) {
    // 30% de chance de não ter irmãos
    siblingCount = 0;
  } else if (siblingRandom <= 0.7) {
    // 40% de chance de ter 1 irmão (70% - 30%)
    siblingCount = 1;
  } else if (siblingRandom <= 0.9) {
    // 20% de chance de ter 2 irmãos (90% - 70%)
    siblingCount = 2;
  } else {
    // 10% de chance de ter 3 irmãos (100% - 90%)
    siblingCount = 3;
  }
  
  for (let i = 0; i < siblingCount; i++) {
    family.push(generateFamilyMember('sibling', characterAge, characterSurname));
  }
  
  return family;
} 