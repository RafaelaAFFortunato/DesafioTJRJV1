using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

[Route("api/[controller]")]
[ApiController]
public class AssuntoController : ControllerBase
{
    private readonly AppDbContext _context;
    public AssuntoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Assunto>>> GetAssuntos() => await _context.Assuntos.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Assunto>> GetAssunto(int id)
    {
        var assunto = await _context.Assuntos.FindAsync(id);
        return assunto == null ? NotFound() : assunto;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Assunto>>> PostAssunto(AssuntoDTO dto)
    {
        bool existe = await _context.Assuntos
            .AnyAsync(a => a.Descricao.ToLower() == dto.Descricao.ToLower());

        if (existe)
        {
            return Ok(new ApiResponse<Assunto>
            {
                Sucesso = false,
                Mensagem = "Já existe um assunto com esta descrição."
            });
        }

        var assunto = new Assunto { Descricao = dto.Descricao };
        _context.Assuntos.Add(assunto);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Assunto>
        {
            Sucesso = true,
            Mensagem = "Assunto criado com sucesso."
        });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Assunto>>> PutAssunto(int id, AssuntoDTO dto)
    {
        var assunto = await _context.Assuntos.FindAsync(id);
        if (assunto == null)
        {
            return Ok(new ApiResponse<Assunto>
            {
                Sucesso = false,
                Mensagem = "Assunto não encontrado."
            });
        }

        bool existe = await _context.Assuntos
            .AnyAsync(a => a.CodAs != id && a.Descricao.ToLower() == dto.Descricao.ToLower());

        if (existe)
        {
            return Ok(new ApiResponse<Assunto>
            {
                Sucesso = false,
                Mensagem = "Já existe outro assunto com esta descrição."
            });
        }

        assunto.Descricao = dto.Descricao;
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<Assunto>
        {
            Sucesso = true,
            Mensagem = "Assunto atualizado com sucesso."
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAssunto(int id)
    {
        var assunto = await _context.Assuntos.FindAsync(id);
        if (assunto == null)
        {
            return Ok(new ApiResponse<object>
            {
                Sucesso = false,
                Mensagem = "Assunto não encontrado."
            });
        }

        _context.Assuntos.Remove(assunto);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Sucesso = true,
            Mensagem = "Assunto excluído com sucesso."
        });
    }
}

