using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProjetoBanco.API.Models
{
    public abstract class Cliente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public int AgenciaId { get; set; }
        [JsonIgnore]
        public Agencia Agencia { get; set; }
        public ICollection<Contratacao> Contratacoes { get; set; } = new List<Contratacao>();
    }

    public class PessoaFisica : Cliente
    {
        [Required]
        public string CPF { get; set; }
        [Required]
        public DateTime DataNascimento { get; set; }
    }

    public class PessoaJuridica : Cliente
    {
        [Required]
        public string CNPJ { get; set; }
        [Required]
        public string RazaoSocial { get; set; }
    }

    public class Agencia
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Codigo { get; set; }
        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }

    public abstract class Produto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        public abstract string DescricaoRegraNegocio { get; }
    }

    public class MaquinaDeCartao : Produto
    {
        public override string DescricaoRegraNegocio => "Taxa MDR variável conforme faturamento.";
    }

    public class ReceberSalario : Produto
    {
        public override string DescricaoRegraNegocio => "Isenção de tarifas para portabilidade.";
    }

    public class Emprestimo : Produto
    {
        public override string DescricaoRegraNegocio => "Cálculo de score para aprovação de crédito.";
    }

    public class Contratacao
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int ClienteId { get; set; }
        [JsonIgnore]
        public Cliente Cliente { get; set; }
        [Required]
        public int ProdutoId { get; set; }
        public string Status { get; set; } // Pendente, Processado, Falha
        public DateTime DataSolicitacao { get; set; } = DateTime.Now;
    }
}
