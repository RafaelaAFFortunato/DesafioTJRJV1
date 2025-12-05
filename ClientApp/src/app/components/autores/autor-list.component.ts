import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AutorService } from '../../services/autor.service';
import { Autor } from '../../models/autor.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-autor-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './autor-list.component.html'
})
export class AutorListComponent implements OnInit {

  autores$!: Observable<Autor[]>;  // <-- inicializada no ngOnInit
  mensagem: string = ''; // variável para mensagem
  
  constructor(private autorService: AutorService) {}

  ngOnInit(): void {
    this.autores$ = this.autorService.autores$; // <-- OK agora!
    this.autorService.loadAutores();            // carrega os dados

    // pega mensagem da navegação
    const estado = history.state;
    if (estado && estado.mensagem) {
      this.mensagem = estado.mensagem;
      setTimeout(() => this.mensagem = '', 3000);
    }
  }

  deleteAutor(id: number) {    
    if (confirm('Deseja realmente excluir?')) {
      this.autorService.deleteAutor(id).subscribe(() => {
        this.mensagem = 'Autor excluído com sucesso!';
        setTimeout(() => this.mensagem = '', 3000);
      });
    }
  }
}
