import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, of } from "rxjs";
import { catchError, tap } from "rxjs/operators";
import { Autor } from "../models/autor.model";

export interface ApiResponse<T> {
  sucesso: boolean;
  mensagem: string;
  dados?: T;
}

@Injectable({ providedIn: "root" })
export class AutorService {
  private apiUrl = "https://localhost:5001/api/Autor";

  private autoresSubject = new BehaviorSubject<Autor[]>([]);
  autores$ = this.autoresSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadAutores() {
      this.http.get<Autor[]>(this.apiUrl)
        .subscribe(autores => this.autoresSubject.next(autores));
    }

  getAutorById(id: number) {
    return this.http.get<ApiResponse<Autor>>(`${this.apiUrl}/${id}`);
  }

  createAutor(autor: Autor) {
    return this.http.post<ApiResponse<Autor>>(this.apiUrl, autor);
  }

  updateAutor(id: number, autor: Autor) {
    return this.http.put<ApiResponse<Autor>>(`${this.apiUrl}/${id}`, autor);
  }

  deleteAutor(id: number) {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
