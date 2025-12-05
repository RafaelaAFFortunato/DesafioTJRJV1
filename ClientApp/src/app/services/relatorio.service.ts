import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RelatorioService {
  private baseUrl = 'https://localhost:7133/api/relatorio';

  constructor(private http: HttpClient) { }

  // Método para baixar PDF do relatório de livros
  baixarRelatorioLivros(): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/livros`, { responseType: 'blob' });
  }
}
