import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

import { AppComponent } from './app/app.component';

// Componentes de livros
import { LivroListComponent } from './app/components/livros/livro-list.component';
import { LivroFormComponent } from './app/components/livros/livro-form.component';

// Componentes de autores
import { AutorListComponent } from './app/components/autores/autor-list.component';
import { AutorFormComponent } from './app/components/autores/autor-form.component';

// Componentes de assuntos
import { AssuntoListComponent } from './app/components/assuntos/assunto-list.component';
import { AssuntoFormComponent } from './app/components/assuntos/assunto-form.component';

// Relatórios
import { RelatorioLivrosComponent } from './app/components/relatorios/relatorio-livros.component';

bootstrapApplication(AppComponent, {
  providers: [
    provideRouter([
      // Raiz já carrega lista de livros diretamente
      { path: '', component: LivroListComponent },

      // Rotas de livros
      { path: 'livros', component: LivroListComponent },
      { path: 'livros/novo', component: LivroFormComponent },
      { path: 'livros/:id', component: LivroFormComponent },

      // Rotas de autores
      { path: 'autores', component: AutorListComponent },
      { path: 'autores/novo', component: AutorFormComponent },
      { path: 'autores/:id', component: AutorFormComponent },

      // Rotas de assuntos
      { path: 'assuntos', component: AssuntoListComponent },
      { path: 'assuntos/novo', component: AssuntoFormComponent },
      { path: 'assuntos/:id', component: AssuntoFormComponent },

      // 👉 NOVA ROTA DO RELATÓRIO
      { path: 'relatorio/livros', component: RelatorioLivrosComponent }
    ]),
    provideHttpClient()
  ]
});
