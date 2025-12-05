using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;


[Route("api/[controller]")]
[ApiController]
public class LivroController : ControllerBase
{
    private readonly AppDbContext _context;

    public LivroController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/livros
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Livro>>> GetLivros()
    {
        return await _context.Livros
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(ls => ls.Assunto)
            .ToListAsync();
    }

    // GET: api/livros/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Livro>> GetLivro(int id)
    {
        var livro = await _context.Livros
            .Include(l => l.LivroAutores)
                .ThenInclude(la => la.Autor)
            .Include(l => l.LivroAssuntos)
                .ThenInclude(ls => ls.Assunto)
            .FirstOrDefaultAsync(l => l.Cod == id);

        if (livro == null) return NotFound();
        return livro;
    }

    // POST: api/livros
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Livro>>> PostLivro(LivroDTO dto)
    {
        bool existe = await _context.Livros
            .AnyAsync(l => l.Titulo.ToLower() == dto.Titulo.ToLower()
                          && l.Editora.ToLower() == dto.Editora.ToLower());

        if (existe)
        {
            return Ok(new ApiResponse<Livro>
            {
                Sucesso = false,
                Mensagem = "Já existe um livro com este título e editora."
            });
        }

        var livro = new Livro
        {
            Titulo = dto.Titulo,
            Editora = dto.Editora,
            Edicao = dto.Edicao,
            AnoPublicacao = dto.AnoPublicacao,
            Valor = dto.Valor,
            LivroAutores = new List<LivroAutor>(),
            LivroAssuntos = new List<LivroAssunto>()
        };

        if (dto.AutorIds != null)
        {
            foreach (var autorId in dto.AutorIds)
            {
                livro.LivroAutores.Add(new LivroAutor { AutorCodAu = autorId, Livro = livro });
            }
        }

        if (dto.AssuntoIds != null)
        {
            foreach (var assuntoId in dto.AssuntoIds)
            {
                livro.LivroAssuntos.Add(new LivroAssunto { AssuntoCodAs = assuntoId, Livro = livro });
            }
        }

        // Adiciona preços por forma de compra
        if (dto.Precos != null)
        {
            foreach (var preco in dto.Precos)
            {
                livro.LivroPrecos.Add(new LivroPreco
                {
                    FormaCompra = preco.FormaCompra,
                    Valor = preco.Valor,
                    Livro = livro
                });
            }
        }

        _context.Livros.Add(livro);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Livro>
        {
            Sucesso = true,
            Mensagem = "Livro criado com sucesso."
        });
    }



    // PUT: api/livros/5
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Livro>>> PutLivro(int id, LivroDTO dto)
    {
        if (dto.Cod.HasValue && dto.Cod != id)
        {
            return BadRequest(new ApiResponse<Livro>
            {
                Sucesso = false,
                Mensagem = "ID inválido."
            });
        }

        // Verifica se outro livro já possui o mesmo título e editora
        bool existeOutro = await _context.Livros
            .AnyAsync(l => l.Cod != id
                           && l.Titulo.ToLower() == dto.Titulo.ToLower()
                           && l.Editora.ToLower() == dto.Editora.ToLower());
        if (existeOutro)
        {
            return BadRequest(new ApiResponse<Livro>
            {
                Sucesso = false,
                Mensagem = "Já existe outro livro com este título e editora."
            });
        }

        // Carrega o livro com relacionamentos
        var livro = await _context.Livros
            .Include(l => l.LivroAutores)
            .Include(l => l.LivroAssuntos)
            .Include(l => l.LivroPrecos)
            .FirstOrDefaultAsync(l => l.Cod == id);

        if (livro == null)
        {
            return NotFound(new ApiResponse<Livro>
            {
                Sucesso = false,
                Mensagem = "Livro não encontrado."
            });
        }

        // Atualiza campos básicos
        livro.Titulo = dto.Titulo;
        livro.Editora = dto.Editora;
        livro.Edicao = dto.Edicao;
        livro.AnoPublicacao = dto.AnoPublicacao;
        livro.Valor = dto.Valor;

        // --- Atualiza autores ---
        if (dto.AutorIds != null)
        {
            var autoresParaRemover = livro.LivroAutores
                .Where(la => !dto.AutorIds.Contains(la.AutorCodAu))
                .ToList();
            foreach (var autor in autoresParaRemover)
            {
                livro.LivroAutores.Remove(autor);
            }

            var autoresExistentesIds = livro.LivroAutores.Select(la => la.AutorCodAu).ToList();
            foreach (var autorId in dto.AutorIds)
            {
                if (!autoresExistentesIds.Contains(autorId))
                {
                    livro.LivroAutores.Add(new LivroAutor
                    {
                        LivroCod = livro.Cod,
                        AutorCodAu = autorId
                    });
                }
            }
        }
        else
        {
            livro.LivroAutores.Clear();
        }

        // --- Atualiza assuntos ---
        if (dto.AssuntoIds != null)
        {
            var assuntosParaRemover = livro.LivroAssuntos
                .Where(ls => !dto.AssuntoIds.Contains(ls.AssuntoCodAs))
                .ToList();
            foreach (var assunto in assuntosParaRemover)
            {
                livro.LivroAssuntos.Remove(assunto);
            }

            var assuntosExistentesIds = livro.LivroAssuntos.Select(ls => ls.AssuntoCodAs).ToList();
            foreach (var assuntoId in dto.AssuntoIds)
            {
                if (!assuntosExistentesIds.Contains(assuntoId))
                {
                    livro.LivroAssuntos.Add(new LivroAssunto
                    {
                        LivroCod = livro.Cod,
                        AssuntoCodAs = assuntoId
                    });
                }
            }
        }
        else
        {
            livro.LivroAssuntos.Clear();
        }

        // Atualiza preços
        livro.LivroPrecos.Clear();
        if (dto.Precos != null)
        {
            foreach (var preco in dto.Precos)
                livro.LivroPrecos.Add(new LivroPreco { LivroCod = livro.Cod, FormaCompra = preco.FormaCompra, Valor = preco.Valor });
        }


        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Livro>
        {
            Sucesso = true,
            Mensagem = "Livro atualizado com sucesso."
        });
    }


    // DELETE: api/livros/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLivro(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null)
        {
            return Ok(new ApiResponse<object>
            {
                Sucesso = false,
                Mensagem = "Livro não encontrado."
            });
        }

        _context.Livros.Remove(livro);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Sucesso = true,
            Mensagem = "Livro excluído com sucesso."
        });
    }
}

