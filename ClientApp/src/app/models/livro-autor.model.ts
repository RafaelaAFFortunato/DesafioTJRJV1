import { Autor } from "./autor.model";

export interface LivroAutor {
  livroCod: number;
  autorCodAu: number;
  autor?: Autor;
}