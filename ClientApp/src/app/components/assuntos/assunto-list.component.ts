import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { AssuntoService } from '../../services/assunto.service';
import { Assunto } from '../../models/assunto.model';

@Component({
  selector: 'app-assunto-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './assunto-list.component.html'
})
export class AssuntoListComponent implements OnInit {

  assuntos$!: Observable<Assunto[]>;
  mensagem: string = ''; // variável para mensagem

  constructor(private assuntoService: AssuntoService) {}

  ngOnInit(): void {
    this.assuntos$ = this.assuntoService.assuntos$; // aqui agora ok
    this.assuntoService.loadAssuntos();             // carrega a lista

    // pega mensagem da navegação
    const estado = history.state;
    if (estado && estado.mensagem) {
      this.mensagem = estado.mensagem;
      setTimeout(() => this.mensagem = '', 3000);
    }
  }

  deleteAssunto(id: number) {    
    if (confirm('Deseja realmente excluir?')) {
      this.assuntoService.deleteAssunto(id).subscribe(() => {
        this.mensagem = 'Assunto excluído com sucesso!';
        setTimeout(() => this.mensagem = '', 3000);
      });
    }
  }
}
