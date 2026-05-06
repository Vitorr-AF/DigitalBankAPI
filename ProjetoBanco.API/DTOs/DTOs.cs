using System;

namespace ProjetoBanco.API.DTOs
{
    public class ClientePFCreateDto
    {
        public string Nome { get; set; }
        public string CPF { get; set; }
        public DateTime DataNascimento { get; set; }
        public int AgenciaId { get; set; }
    }

    public class ClientePJCreateDto
    {
        public string Nome { get; set; }
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public int AgenciaId { get; set; }
    }

    public class AgenciaCreateDto
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
    }

    public class ContratacaoCreateDto
    {
        public int ClienteId { get; set; }
        public int ProdutoId { get; set; }
    }
}
