import { Assunto } from "./assunto.model";

export interface LivroAssunto {
  livroCod: number;
  assuntoCodAs: number;
  assunto?: Assunto;
}