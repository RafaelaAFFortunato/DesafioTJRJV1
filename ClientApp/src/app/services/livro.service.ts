import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, tap } from 'rxjs';
import { Livro } from '../models/livro.model';

export interface ApiResponse<T> {
  sucesso: boolean;
  mensagem: string;
  dados?: T;
}

@Injectable({
  providedIn: 'root'
})
export class LivroService {
  private apiUrl = 'https://localhost:5001/api/Livro';

  private livrosSubject = new BehaviorSubject<Livro[]>([]);
  livros$ = this.livrosSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadLivros() {
    this.http.get<Livro[]>(this.apiUrl).subscribe(data => {
      // Se o backend retorna $values (Entity Framework), usa ele
      this.livrosSubject.next((data as any).$values ?? data);
    });
  }

  getLivro(id: number) {
    return this.http.get<Livro>(`${this.apiUrl}/${id}`);
  }

  createLivro(livro: Livro) {
    return this.http.post<ApiResponse<Livro>>(this.apiUrl, livro);
  }

  updateLivro(id: number, livro: Livro) {
    return this.http.put<ApiResponse<Livro>>(`${this.apiUrl}/${id}`, livro);
  }

  deleteLivro(id: number) {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }
  
}
