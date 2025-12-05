import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LivroService } from '../../services/livro.service';
import { AutorService } from '../../services/autor.service';
import { AssuntoService } from '../../services/assunto.service';
import { Livro } from '../../models/livro.model';
import { Autor } from '../../models/autor.model';
import { Assunto } from '../../models/assunto.model';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'app-livro-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './livro-form.component.html',
})
export class LivroFormComponent implements OnInit {
  form: FormGroup;
  id?: number;
  mensagem: string = '';
  erro: string = '';          // Mensagem do backend
  enviando: boolean = false;  // Flag para desabilitar botão

  autores: Autor[] = [];
  assuntos: Assunto[] = [];

  constructor(
    private fb: FormBuilder,
    private livroService: LivroService,
    private autorService: AutorService,
    private assuntoService: AssuntoService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
  titulo: ['', Validators.required],
  editora: ['', Validators.required],
  edicao: [1, Validators.required],
  anoPublicacao: ['', [Validators.required, Validators.pattern('^[0-9]{4}$')]],
  valor: [0],
  autorIds: [[], Validators.required],
  assuntoIds: [[], Validators.required]
});

  }

  ngOnInit(): void {
    this.id = this.route.snapshot.params['id'];
    this.loadAutores();
    this.loadAssuntos();

    if (this.id) {
      this.livroService.getLivro(this.id).subscribe(livro => {
        const autorIds = livro.livroAutores?.map(a => a.autorCodAu) ?? [];
        const assuntoIds = livro.livroAssuntos?.map(a => a.assuntoCodAs) ?? [];

        this.form.patchValue({
          titulo: livro.titulo,
          editora: livro.editora,
          edicao: livro.edicao,
          anoPublicacao: livro.anoPublicacao,
          valor: livro.valor,
          autorIds,
          assuntoIds
        });
      });
    }
  }

  onValorChange(event: any) {
  let value = event.target.value;

  // Remove tudo que não for número ou vírgula
  value = value.replace(/[^0-9,]/g, '');

  // Substitui vírgula por ponto para número decimal
  const numericValue = parseFloat(value.replace(',', '.'));

  if (!isNaN(numericValue)) {
    // Atualiza o formControl com número
    this.form.controls['valor'].setValue(numericValue, { emitEvent: false });
    // Atualiza o campo com formatação de moeda
    event.target.value = numericValue.toLocaleString('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    });
  } else {
    this.form.controls['valor'].setValue(0, { emitEvent: false });
    event.target.value = '';
  }
}


  loadAutores() {
    this.autorService.loadAutores();
    this.autorService.autores$.subscribe(a => this.autores = a);
  }

  loadAssuntos() {
    this.assuntoService.loadAssuntos();
    this.assuntoService.assuntos$.subscribe(a => this.assuntos = a);
  }

  save() {
    this.erro = '';

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.enviando = true;
    const livroDTO = {
      cod: this.id,
      titulo: this.form.value.titulo,
      editora: this.form.value.editora,
      edicao: this.form.value.edicao,
      anoPublicacao: this.form.value.anoPublicacao,
      valor: this.form.value.valor,
      autorIds: this.form.value.autorIds ?? [],
      assuntoIds: this.form.value.assuntoIds ?? []
    };

    const request$ = this.id
      ? this.livroService.updateLivro(this.id, livroDTO)
      : this.livroService.createLivro(livroDTO);

    request$.pipe(
      finalize(() => this.enviando = false)
    ).subscribe({
      next: (res: any) => {
        if (!res.sucesso) {
          // Exibe mensagem de erro do backend
          this.enviando = false;
          alert(res.mensagem);
          return;
        }

        // Sucesso: navega para a lista de livros
        this.router.navigate(['/livros'], { state: { mensagem: res.mensagem || (this.id ? 'Livro alterado com sucesso!' : 'Livro incluído com sucesso!') } });
      },
      error: (err) => {
        this.erro = 'Erro inesperado.';
        console.error('Erro da API:', err);
      }
    });
  }
}
