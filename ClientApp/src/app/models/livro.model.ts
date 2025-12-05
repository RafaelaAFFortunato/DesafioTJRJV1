import { Autor } from './autor.model';
import { Assunto } from './assunto.model';

export interface Livro {
  cod?: number;
  titulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  valor: number;
  // Propriedades para relacionamentos
  livroAutores?: { autorCodAu: number; autor: Autor }[];
  livroAssuntos?: { assuntoCodAs: number; assunto: Assunto }[];
}

