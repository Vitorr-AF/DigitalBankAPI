# Alunos:
## Lucas Henzo Ide Yuki             RM: 554865
## Vitor Augusto França de Oliveira RM: 555469


[Projeto Banco Digital - API.md](https://github.com/user-attachments/files/27421250/Projeto.Banco.Digital.-.API.md)
# Projeto Banco Digital - API

Este projeto consiste em uma **Web API** desenvolvida em **ASP.NET Core 8.0**, atuando como o *backend* de um banco digital. Seu principal objetivo é gerenciar o cadastro de clientes (tanto pessoas físicas quanto jurídicas), vincular esses clientes a agências bancárias e processar a contratação de produtos financeiros de forma assíncrona.

## Funcionalidades Principais

*   **Cadastro de Clientes**: Permite o registro de Pessoas Físicas (PF) e Pessoas Jurídicas (PJ) com validações específicas.
*   **Gestão de Agências**: Funcionalidades para o cadastro e consulta de agências bancárias.
*   **Contratação de Produtos**: Habilita a solicitação de contratação de produtos bancários, com processamento assíncrono via RabbitMQ.
*   **Consulta de Status**: Permite verificar o status das contratações realizadas.

## Diagrama do draw.io
![Diagrama de Classes UML](https://github.com/Vitorr-AF/DigitalBankAPI/DiagramaCP2.drawio)

## Arquitetura e Tecnologias

A arquitetura do projeto segue os princípios de uma API RESTful, utilizando as seguintes tecnologias e *frameworks*:

*   **Runtime**: .NET 8.0
*   **Framework Web**: ASP.NET Core Web API
*   **ORM (Object-Relational Mapping)**: Entity Framework Core 8.0.10
    *   Configurado para usar um banco de dados **In-Memory** para facilitar o desenvolvimento e a execução local, mas preparado para integração com **Oracle** em um ambiente de produção.
*   **Mensageria**: RabbitMQ.Client 6.8.1
    *   Utilizado para desacoplar o processo de solicitação de contratação do seu processamento real, garantindo escalabilidade e resiliência.
*   **Documentação da API**: Swashbuckle.AspNetCore 6.9.0 (Swagger/OpenAPI)
    *   Fornece uma interface interativa para explorar e testar os *endpoints* da API.

## Modelo de Domínio

O modelo de domínio é composto pelas seguintes entidades principais:

*   **Cliente**: Uma classe abstrata que serve como base para `PessoaFisica` e `PessoaJuridica`, utilizando um discriminador para diferenciar os tipos de cliente. Possui um vínculo com `Agencia` e uma coleção de `Contratacao`.
    *   **PessoaFisica**: Herda de `Cliente`, adicionando propriedades como `CPF` e `DataNascimento`.
    *   **PessoaJuridica**: Herda de `Cliente`, adicionando propriedades como `CNPJ` e `RazaoSocial`.
*   **Agencia**: Representa uma agência bancária, com `Nome` e `Codigo`.
*   **Produto**: Uma classe abstrata para produtos bancários, com especializações como `MaquinaDeCartao`, `ReceberSalario` e `Emprestimo`.
    *   **Emprestimo**: Produto escolhido para implementação, com uma regra de negócio específica para cálculo de *score* de crédito.
*   **Contratacao**: Registra a solicitação de um cliente para um produto, incluindo `ClienteId`, `ProdutoId` e `Status` (Pendente, Processado, Falha).

## Endpoints da API

A API expõe os seguintes *endpoints*:

### Clientes

*   `POST /api/clientes/pf`
    *   **Descrição**: Cadastra uma nova Pessoa Física.
    *   **Corpo da Requisição (Exemplo)**:
        ```json
        {
          "nome": "João Silva",
          "cpf": "123.456.789-00",
          "dataNascimento": "1990-01-01T00:00:00",
          "agenciaId": 1
        }
        ```
    *   **Respostas**: `201 Created` (sucesso), `400 Bad Request` (CPF já cadastrado ou agência não encontrada).

*   `POST /api/clientes/pj`
    *   **Descrição**: Cadastra uma nova Pessoa Jurídica.
    *   **Corpo da Requisição (Exemplo)**:
        ```json
        {
          "nome": "Empresa XYZ Ltda",
          "cnpj": "11.222.333/0001-44",
          "razaoSocial": "Empresa XYZ Limitada",
          "agenciaId": 1
        }
        ```
    *   **Respostas**: `201 Created` (sucesso), `400 Bad Request` (CNPJ já cadastrado ou agência não encontrada).

*   `GET /api/clientes/{id}`
    *   **Descrição**: Busca um cliente pelo seu ID.
    *   **Respostas**: `200 OK` (cliente encontrado), `404 Not Found` (cliente não encontrado).

### Agências

*   `POST /api/agencias`
    *   **Descrição**: Cadastra uma nova agência bancária.
    *   **Corpo da Requisição (Exemplo)**:
        ```json
        {
          "nome": "Agência Central",
          "codigo": "0001"
        }
        ```
    *   **Respostas**: `201 Created` (sucesso).

*   `GET /api/agencias/{id}`
    *   **Descrição**: Busca uma agência pelo seu ID.
    *   **Respostas**: `200 OK` (agência encontrada), `404 Not Found` (agência não encontrada).

### Contratações

*   `POST /api/contratacoes`
    *   **Descrição**: Solicita a contratação de um produto por um cliente. A solicitação é enviada para uma fila RabbitMQ para processamento assíncrono.
    *   **Corpo da Requisição (Exemplo)**:
        ```json
        {
          "clienteId": 1,
          "produtoId": 3 
        }
        ```
    *   **Respostas**: `202 Accepted` (solicitação aceita para processamento), `404 Not Found` (cliente não encontrado).

*   `GET /api/contratacoes/{id}`
    *   **Descrição**: Consulta o status de uma contratação pelo seu ID.
    *   **Respostas**: `200 OK` (status da contratação), `404 Not Found` (contratação não encontrada).

## Como Executar o Projeto

Para colocar o projeto em funcionamento, siga os passos abaixo:

1.  **Pré-requisitos**:
    *   Certifique-se de ter o **SDK do .NET 8.0** instalado em sua máquina. Você pode baixá-lo em [dot.net](https://dotnet.microsoft.com/download/dotnet/8.0).
    *   (Opcional, para simular o ambiente de produção) Tenha uma instância do **RabbitMQ** em execução. Para desenvolvimento, a API continuará funcionando sem o RabbitMQ, mas as mensagens não serão publicadas.

2.  **Clonar o Repositório**:
    ```bash
    git clone https://github.com/Vitorr-AF/DigitalBankAPI/
    cd ProjetoBanco/ProjetoBanco.API
    ```

3.  **Restaurar Dependências**:
    ```bash
    dotnet restore
    ```

4.  **Executar a Aplicação**:
    ```bash
    dotnet run
    ```
    A API será iniciada, geralmente na porta `5000` (HTTP) e `5001` (HTTPS). O endereço exato será exibido no console.

5.  **Acessar a Documentação Swagger**:
    Após iniciar a aplicação, abra seu navegador e acesse a documentação interativa da API no Swagger UI:
    `http://localhost:5000/swagger` (substitua a porta se necessário).

## Testes

Para executar os testes do projeto:

```bash
dotnet test
```

**Nota**: Os fluxos críticos mencionados na atividade foram considerados na implementação dos *controllers*, incluindo validações de duplicidade de CPF/CNPJ e existência de entidades (clientes, agências) para garantir a integridade dos dados e o comportamento esperado da API.


