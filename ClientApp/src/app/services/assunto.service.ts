import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, tap } from "rxjs";
import { Assunto } from "../models/assunto.model";

export interface ApiResponse<T> {
  sucesso: boolean;
  mensagem: string;
  dados?: T;
}

@Injectable({ providedIn: "root" })
export class AssuntoService {
  private apiUrl = "https://localhost:5001/api/Assunto";

  private assuntosSubject = new BehaviorSubject<Assunto[]>([]);
  assuntos$ = this.assuntosSubject.asObservable();

  constructor(private http: HttpClient) {}

  loadAssuntos() {
        this.http.get<Assunto[]>(this.apiUrl)
          .subscribe(assuntos => this.assuntosSubject.next(assuntos));
      }
  
    getAssuntoById(id: number) {
      return this.http.get<ApiResponse<Assunto>>(`${this.apiUrl}/${id}`);
    }
  
    createAssunto(autor: Assunto) {
      return this.http.post<ApiResponse<Assunto>>(this.apiUrl, autor);
    }
  
    updateAssunto(id: number, autor: Assunto) {
      return this.http.put<ApiResponse<Assunto>>(`${this.apiUrl}/${id}`, autor);
    }
  
    deleteAssunto(id: number) {
      return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
    }
}
