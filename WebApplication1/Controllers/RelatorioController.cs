using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using System.IO;
using System.Linq;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/relatorio")]
    public class RelatorioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RelatorioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("livros")]
        public IActionResult GerarRelatorioLivrosTabela()
        {
            // Obter todos os livros com autores e assuntos
            var livros = _context.Livros
                .Select(l => new
                {
                    Livro = l,
                    Autores = l.LivroAutores.Select(la => la.Autor.Nome).ToList(),
                    Assuntos = l.LivroAssuntos.Select(ls => ls.Assunto.Descricao).ToList()
                })
                .ToList();

            using (var ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                PdfFont fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                PdfFont fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Título do relatório
                Paragraph titulo = new Paragraph("Relatório de Livros, Autores e Assuntos")
                    .SetFont(fontBold)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(titulo);

                // Criar tabela com colunas proporcionais
                Table tabela = new Table(UnitValue.CreatePercentArray(new float[] { 3, 3, 4, 3, 1 }))
                    .UseAllAvailableWidth()
                    .SetTextAlignment(TextAlignment.LEFT);

                // Cabeçalho
                tabela.AddHeaderCell(new Cell().Add(new Paragraph("Autor(es)").SetFont(fontBold)).SetTextAlignment(TextAlignment.CENTER));
                tabela.AddHeaderCell(new Cell().Add(new Paragraph("Assunto(s)").SetFont(fontBold)).SetTextAlignment(TextAlignment.CENTER));
                tabela.AddHeaderCell(new Cell().Add(new Paragraph("Título").SetFont(fontBold)).SetTextAlignment(TextAlignment.CENTER));
                tabela.AddHeaderCell(new Cell().Add(new Paragraph("Editora").SetFont(fontBold)).SetTextAlignment(TextAlignment.CENTER));
                tabela.AddHeaderCell(new Cell().Add(new Paragraph("Ano").SetFont(fontBold)).SetTextAlignment(TextAlignment.CENTER));

                // Adicionar linhas
                foreach (var item in livros)
                {
                    string autores = string.Join(", ", item.Autores);
                    string assuntos = string.Join(", ", item.Assuntos);

                    tabela.AddCell(new Cell().Add(new Paragraph(autores).SetFont(fontRegular))
                        .SetPadding(5).SetTextAlignment(TextAlignment.LEFT).SetKeepTogether(true));
                    tabela.AddCell(new Cell().Add(new Paragraph(assuntos).SetFont(fontRegular))
                        .SetPadding(5).SetTextAlignment(TextAlignment.LEFT).SetKeepTogether(true));
                    tabela.AddCell(new Cell().Add(new Paragraph(item.Livro.Titulo ?? "").SetFont(fontRegular))
                        .SetPadding(5).SetTextAlignment(TextAlignment.LEFT));
                    tabela.AddCell(new Cell().Add(new Paragraph(item.Livro.Editora ?? "").SetFont(fontRegular))
                        .SetPadding(5).SetTextAlignment(TextAlignment.LEFT));
                    tabela.AddCell(new Cell().Add(new Paragraph(item.Livro.AnoPublicacao ?? "").SetFont(fontRegular))
                        .SetPadding(5).SetTextAlignment(TextAlignment.CENTER));
                }

                document.Add(tabela);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "RelatorioLivros.pdf");
            }
        }
    }
}
