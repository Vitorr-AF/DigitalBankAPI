using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoBanco.API.Data;
using ProjetoBanco.API.Models;
using ProjetoBanco.API.DTOs;
using ProjetoBanco.API.Messaging;

namespace ProjetoBanco.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("pf")]
        public async Task<IActionResult> CreatePF(ClientePFCreateDto dto)
        {
            if (await _context.PessoasFisicas.AnyAsync(p => p.CPF == dto.CPF))
                return BadRequest("CPF já cadastrado.");

            var agencia = await _context.Agencias.FindAsync(dto.AgenciaId);
            if (agencia == null) return BadRequest("Agência não encontrada.");

            var cliente = new PessoaFisica
            {
                Nome = dto.Nome,
                CPF = dto.CPF,
                DataNascimento = dto.DataNascimento,
                AgenciaId = dto.AgenciaId
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpPost("pj")]
        public async Task<IActionResult> CreatePJ(ClientePJCreateDto dto)
        {
            if (await _context.PessoasJuridicas.AnyAsync(p => p.CNPJ == dto.CNPJ))
                return BadRequest("CNPJ já cadastrado.");

            var agencia = await _context.Agencias.FindAsync(dto.AgenciaId);
            if (agencia == null) return BadRequest("Agência não encontrada.");

            var cliente = new PessoaJuridica
            {
                Nome = dto.Nome,
                CNPJ = dto.CNPJ,
                RazaoSocial = dto.RazaoSocial,
                AgenciaId = dto.AgenciaId
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, cliente);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cliente = await _context.Clientes.Include(c => c.Agencia).FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class AgenciasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AgenciasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(AgenciaCreateDto dto)
        {
            var agencia = new Agencia { Nome = dto.Nome, Codigo = dto.Codigo };
            _context.Agencias.Add(agencia);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = agencia.Id }, agencia);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var agencia = await _context.Agencias.FindAsync(id);
            if (agencia == null) return NotFound();
            return Ok(agencia);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ContratacoesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRabbitMqService _rabbitMq;

        public ContratacoesController(AppDbContext context, IRabbitMqService rabbitMq)
        {
            _context = context;
            _rabbitMq = rabbitMq;
        }

        [HttpPost]
        public async Task<IActionResult> Solicitar(ContratacaoCreateDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null) return NotFound("Cliente não encontrado.");

            var contratacao = new Contratacao
            {
                ClienteId = dto.ClienteId,
                ProdutoId = dto.ProdutoId,
                Status = "Pendente"
            };

            _context.Contratacoes.Add(contratacao);
            await _context.SaveChangesAsync();

            _rabbitMq.PublishContratacao(new { ContratacaoId = contratacao.Id, ClienteId = dto.ClienteId, ProdutoId = dto.ProdutoId });

            return AcceptedAtAction(nameof(GetStatus), new { id = contratacao.Id }, contratacao);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var contratacao = await _context.Contratacoes.FindAsync(id);
            if (contratacao == null) return NotFound();
            return Ok(new { contratacao.Id, contratacao.Status, contratacao.DataSolicitacao });
        }
    }
}
