import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AssuntoService } from '../../services/assunto.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-autor-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './assunto-form.component.html'
})
export class AssuntoFormComponent implements OnInit {
  form: FormGroup;
  id?: number;
  erro: string = '';          // Mensagem do backend
  enviando: boolean = false;  // Flag para desabilitar botão

  constructor(
    private fb: FormBuilder,
    private assuntoService: AssuntoService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      descricao: ['', [Validators.required, Validators.maxLength(20)]]
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.assuntoService.getAssuntoById(this.id)
        .subscribe(a => this.form.patchValue(a));
    }
  }

  save() {
    this.erro = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando = true;
    const assunto = this.form.value;

    const request$ = this.id
      ? this.assuntoService.updateAssunto(this.id, assunto)
      : this.assuntoService.createAssunto(assunto);

    request$.pipe(
      finalize(() => this.enviando = false)  // garante que botão volte ao normal
    ).subscribe({
      next: (res: any) => {
        if (!res.sucesso) {
          // Mensagem de falha do backend
          this.enviando = false;
          alert(res.mensagem);
          return;
        }

        // Sucesso: navega para a lista de autores
        this.router.navigate(['/assuntos']);
      },
      error: (err) => {
        // Erro inesperado (ex: servidor offline)
        this.erro = 'Erro inesperado.';
        console.error('Erro da API:', err);
      }
    });
  }
}
