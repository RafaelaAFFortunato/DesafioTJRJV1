using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

[Route("api/[controller]")]
[ApiController]
public class AutorController : ControllerBase
{
    private readonly AppDbContext _context;
    public AutorController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Autor>>> GetAutores() => await _context.Autores.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Autor>>> GetAutor(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
        {
            return Ok(new ApiResponse<Autor>
            {
                Sucesso = false,
                Mensagem = "Autor não encontrado.",
                // opcional: dado nulo
            });
        }

        return Ok(new ApiResponse<Autor>
        {
            Sucesso = true,
            Mensagem = "Autor encontrado."
   
        });
    }


    [HttpPost]
    public async Task<ActionResult<ApiResponse<Autor>>> PostAutor(AutorDTO dto)
    {
        bool existe = await _context.Autores
            .AnyAsync(a => a.Nome.ToLower() == dto.Nome.ToLower());

        if (existe)
        {
            return Ok(new ApiResponse<Autor>
            {
                Sucesso = false,
                Mensagem = "Já existe um autor com esta descrição."
            });
        }

        var autor = new Autor { Nome = dto.Nome };
        _context.Autores.Add(autor);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Autor>
        {
            Sucesso = true,
            Mensagem = "Autor criado com sucesso!",
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> PutAutor(int id, AutorDTO dto)
    {
        if (dto.CodAu.HasValue && dto.CodAu != id)
            return Ok(new ApiResponse<object> { Sucesso = false, Mensagem = "ID inválido." });

        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
            return Ok(new ApiResponse<object> { Sucesso = false, Mensagem = "Autor não encontrado." });

        // Verifica se já existe outro autor com a mesma descrição
        bool existe = await _context.Autores
            .AnyAsync(a => a.CodAu != id && a.Nome.ToLower() == dto.Nome.ToLower());

        if (existe)
        {
            return Ok(new ApiResponse<object> { Sucesso = false, Mensagem = "Já existe outro autor com este nome." });
        }

        autor.Nome = dto.Nome;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object> { Sucesso = true, Mensagem = "Autor atualizado com sucesso." });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAutor(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
            return Ok(new ApiResponse<object> { Sucesso = false, Mensagem = "Autor não encontrado." });

        _context.Autores.Remove(autor);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object> { Sucesso = true, Mensagem = "Autor excluído com sucesso." });
    }
}
