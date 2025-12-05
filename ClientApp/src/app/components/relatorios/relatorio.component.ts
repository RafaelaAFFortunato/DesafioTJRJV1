import { Component } from '@angular/core';
import { RelatorioService } from '../../services/relatorio.service';

@Component({
  selector: 'app-relatorio',
  templateUrl: './relatorio.component.html'
})
export class RelatorioComponent {

  constructor(private relatorioService: RelatorioService) {}

  baixarPDF() {
    this.relatorioService.baixarRelatorioLivros().subscribe((arquivo: Blob) => {
      
      const url = window.URL.createObjectURL(arquivo);
      const link = document.createElement('a');
      link.href = url;
      link.download = "RelatorioLivros.pdf";
      link.click();

      window.URL.revokeObjectURL(url);
    });
  }
}
