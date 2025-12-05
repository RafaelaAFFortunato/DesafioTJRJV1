export interface LivroDTO {
  cod?: number;         // opcional no POST
  titulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  valor: number;

  autorIds: number[];   // IDs dos autores selecionados
  assuntoIds: number[]; // IDs dos assuntos selecionados
}