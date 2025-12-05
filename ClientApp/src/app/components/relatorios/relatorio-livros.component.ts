import { Component } from '@angular/core';
import { RelatorioService } from '../../services/relatorio.service';

@Component({
  selector: 'app-relatorio-livros',
  templateUrl: './relatorio-livros.component.html'
})
export class RelatorioLivrosComponent {

  constructor(private relatorioService: RelatorioService) { }

  baixarPDF() {
    this.relatorioService.baixarRelatorioLivros().subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'RelatorioLivros.pdf';
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Erro ao baixar PDF:', err);
      }
    });
  }
}
