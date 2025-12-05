import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { LivroService } from '../../services/livro.service';
import { Livro } from '../../models/livro.model';

@Component({
  selector: 'app-livro-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './livro-list.component.html'
})
export class LivroListComponent implements OnInit {

  livros$!: Observable<Livro[]>;
  mensagem: string = ''; // variável para mensagem

  constructor(private livroService: LivroService) {}

  ngOnInit(): void {
    this.livros$ = this.livroService.livros$;
    this.livroService.loadLivros(); // carrega a lista

    // pega mensagem da navegação
    const estado = history.state;
    if (estado && estado.mensagem) {
      this.mensagem = estado.mensagem;
      setTimeout(() => this.mensagem = '', 3000);
    }
  }

  deleteLivro(id: number) {
    if (confirm('Deseja realmente excluir?')) {
      this.livroService.deleteLivro(id).subscribe(() => {
        this.mensagem = 'Livro excluído com sucesso!';
        setTimeout(() => this.mensagem = '', 3000);
      });
    }
  }
}
