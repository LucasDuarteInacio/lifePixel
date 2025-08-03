import { NameData } from '@/types/game';

export const names: NameData = {
  male: [
    'João', 'Pedro', 'Lucas', 'Gabriel', 'Matheus', 'Rafael', 'Bruno', 'Carlos',
    'Daniel', 'André', 'Thiago', 'Felipe', 'Marcos', 'Leonardo', 'Alexandre',
    'Diego', 'Ricardo', 'Eduardo', 'Fernando', 'Roberto', 'Paulo', 'Marcelo',
    'Rodrigo', 'Antônio', 'José', 'Francisco', 'Luiz', 'Miguel', 'Arthur',
    'Heitor', 'Bernardo', 'Davi', 'Theo', 'Enzo', 'Valentim', 'Benjamin',
    'Nicolas', 'Guilherme', 'Rafael', 'Isaac', 'Samuel', 'Henrique', 'Caio',
    'Lorenzo', 'João Pedro', 'Gustavo', 'Murilo', 'Pedro Henrique', 'Pietro',
    'Lucca', 'Anthony', 'Gabriel', 'Bento', 'Joaquim', 'Antonella', 'Isabella'
  ],
  female: [
    'Maria', 'Ana', 'Julia', 'Sofia', 'Isabella', 'Manuela', 'Giovanna',
    'Alice', 'Laura', 'Luiza', 'Valentina', 'Beatriz', 'Gabriela', 'Rafaela',
    'Carolina', 'Mariana', 'Clara', 'Ana Clara', 'Ana Luiza', 'Letícia',
    'Yasmin', 'Isabela', 'Lívia', 'Helena', 'Lorena', 'Ana Beatriz',
    'Maria Eduarda', 'Maria Clara', 'Maria Luiza', 'Maria Fernanda',
    'Ana Júlia', 'Ana Sofia', 'Ana Carolina', 'Ana Paula', 'Ana Lívia',
    'Ana Helena', 'Ana Lorena', 'Ana Gabriela', 'Ana Rafaela', 'Ana Beatriz',
    'Ana Carolina', 'Ana Paula', 'Ana Lívia', 'Ana Helena', 'Ana Lorena',
    'Ana Gabriela', 'Ana Rafaela', 'Ana Beatriz', 'Ana Carolina', 'Ana Paula'
  ],
  surnames: [
    'Silva', 'Santos', 'Oliveira', 'Souza', 'Rodrigues', 'Ferreira',
    'Alves', 'Pereira', 'Lima', 'Gomes', 'Costa', 'Ribeiro', 'Martins',
    'Carvalho', 'Almeida', 'Lopes', 'Soares', 'Fernandes', 'Vieira',
    'Barbosa', 'Rocha', 'Dias', 'Cavalcanti', 'Nascimento', 'Melo',
    'Cardoso', 'Teixeira', 'Moura', 'Santana', 'Castro', 'Araújo',
    'Correia', 'Sousa', 'Machado', 'Andrade', 'Dantas', 'Lima',
    'Cunha', 'Pinto', 'Monteiro', 'Moreira', 'Medeiros', 'Nunes',
    'Freitas', 'Barros', 'Azevedo', 'Costa', 'Souza', 'Oliveira'
  ]
};

export function getRandomName(gender: 'male' | 'female'): string {
  const firstName = gender === 'male' 
    ? names.male[Math.floor(Math.random() * names.male.length)]
    : names.female[Math.floor(Math.random() * names.female.length)];
  
  const surname = names.surnames[Math.floor(Math.random() * names.surnames.length)];
  
  return `${firstName} ${surname}`;
}

export function getRandomGender(): 'male' | 'female' {
  return Math.random() > 0.5 ? 'male' : 'female';
} 