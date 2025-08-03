import { Country } from '@/types/game';

export const countries: Country[] = [
  { name: 'Brasil', code: 'BR', currency: 'R$' },
  { name: 'Estados Unidos', code: 'US', currency: '$' },
  { name: 'Canadá', code: 'CA', currency: 'C$' },
  { name: 'Reino Unido', code: 'UK', currency: '£' },
  { name: 'Alemanha', code: 'DE', currency: '€' },
  { name: 'França', code: 'FR', currency: '€' },
  { name: 'Itália', code: 'IT', currency: '€' },
  { name: 'Espanha', code: 'ES', currency: '€' },
  { name: 'Portugal', code: 'PT', currency: '€' },
  { name: 'Japão', code: 'JP', currency: '¥' },
  { name: 'China', code: 'CN', currency: '¥' },
  { name: 'Coreia do Sul', code: 'KR', currency: '₩' },
  { name: 'Austrália', code: 'AU', currency: 'A$' },
  { name: 'Nova Zelândia', code: 'NZ', currency: 'NZ$' },
  { name: 'Argentina', code: 'AR', currency: 'AR$' },
  { name: 'México', code: 'MX', currency: 'MX$' },
  { name: 'Chile', code: 'CL', currency: 'CL$' },
  { name: 'Colômbia', code: 'CO', currency: 'CO$' },
  { name: 'Peru', code: 'PE', currency: 'S/' },
  { name: 'Venezuela', code: 'VE', currency: 'Bs' },
  { name: 'Uruguai', code: 'UY', currency: '$U' },
  { name: 'Paraguai', code: 'PY', currency: '₲' },
  { name: 'Bolívia', code: 'BO', currency: 'Bs' },
  { name: 'Equador', code: 'EC', currency: '$' },
  { name: 'Guiana', code: 'GY', currency: 'G$' },
  { name: 'Suriname', code: 'SR', currency: 'SR$' },
  { name: 'Guiana Francesa', code: 'GF', currency: '€' }
];

export function getRandomCountry(): Country {
  return countries[Math.floor(Math.random() * countries.length)];
} 