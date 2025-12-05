import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AutorService } from '../../services/autor.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-autor-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './autor-form.component.html'
})
export class AutorFormComponent implements OnInit {
  form: FormGroup;
  id?: number;
  erro: string = '';          // Mensagem do backend
  enviando: boolean = false;  // Flag para desabilitar botão

  constructor(
    private fb: FormBuilder,
    private autorService: AutorService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      nome: ['', [Validators.required, Validators.maxLength(40)]]
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.autorService.getAutorById(this.id)
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
    const autor = this.form.value;

    const request$ = this.id
      ? this.autorService.updateAutor(this.id, autor)
      : this.autorService.createAutor(autor);

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
        this.router.navigate(['/autores']);
      },
      error: (err) => {
        // Erro inesperado (ex: servidor offline)
        this.erro = 'Erro inesperado.';
        console.error('Erro da API:', err);
      }
    });
  }
}
