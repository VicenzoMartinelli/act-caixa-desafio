# Act.Caixa
![alt text](https://github.com/VicenzoMartinelli/act-caixa-desafio/blob/main/ACT%20-%20Consolida%C3%A7%C3%A3o.png?raw=true)

## Estrutura do projeto

Projetos:

- **BuildingBlocks**: biblioteca de componentes reutilizáveis e utilitários comuns a todos os projetos.
- **Act.Caixa.Domain**: camada de domínio contendo entidades, objetos de valor e lógica de negócio principal.
- **Act.Caixa.Services.Consolidacao**: serviço responsável pela lógica de **Consolidação** de dados.
- **Act.Caixa.Services.Lancamento**: serviço responsável pela lógica de **Lançamento** de dados.
- **Act.Caixa.Gateway**: API Gateway que centraliza e roteia as requisições para os microsserviços apropriados.
- **Act.Caixa.Tests**: projeto de testes, contendo testes de integração utilizando xUnit e Testcontainers.

### Padrões de Projeto
- **Command Query Responsibility Segregation (CQRS)**: Separação entre comandos (escrita) e consultas (leitura)
- **Arquitetura Orientada a Eventos (Event-Driven)**: Comunicação assíncrona baseada em eventos
- **API Gateway Pattern**: Centralização e roteamento de requisições
- **Mediator**: Implementado via MediatR para comunicação entre componentes
- **Feature Focused Structure **: Organização do código baseada em funcionalidades do negócio

### Frameworks e Ferramentas

- **.NET 9.0**: Framework base
- **Ocelot**: Framework para API Gateway
- **MassTransit**: Framework de mensageria com RabbitMQ
- **Entity Framework Core**: ORM para PostgreSQL
- **MediatR**: Implementação do padrão Mediator
- **RabbitMQ**: Message broker para comunicação assíncrona
- **Scalar**: Documentação de API
- **Testcontainers**: Containers para testes de integração
- **xUnit**: Framework de testes
- **Docker**: Containers para infraestrutura
- **PostgreSQL**: Banco de dados relacional

## Como Rodar Localmente

### Pré-requisitos
- Docker Desktop
- .NET 9.0 SDK
- Visual Studio 2022/Rider (recomendado)

### Configuração do Ambiente

1. **Iniciar containers de infraestrutura**:
   ```bash
   docker-compose up -d
   ```

2. **Iniciar os serviços**:
    Terminal 1 (Lançamento):
   ```bash
   cd Act.Caixa.Services.Lancamento
   dotnet run
   ```
   
   Terminal 2 (Consolidação):
   ```bash
   cd Act.Caixa.Services.Consolidacao
   dotnet run
   ```

   Terminal 3 (Gateway):
   ```bash
   cd Act.Caixa.Gateway
   dotnet run
   ```
     
### Acessando os Serviços

**Rotas disponíveis no Gateway**:
- Todas rotas lancamentos: `/lancamentos/{*}` ex: `/lancamentos/api/v1/lancamentos/{id}`
- Todas rotas consolidação: `/consolidacoes/{*}` ex: `/consolidacoes/api/v1/consolidacoes`

**Rotas disponíveis serviço Lancamentos**:
- Lançamentos: `/api/v1/lancamentos/*`
- Documentação: `/scalar`

**Rotas disponíveis serviço Lancamentos**:
- Consolidações: `/api/v1/consolidacoes/*`
- Documentação: `/scalar`
   
### Portas dos Serviços

- **Gateway**: 5155
- **Serviço de Lançamento**: 5168
- **Serviço de Consolidação**: 5259

## Como Rodar os Testes

Os testes são baseados em xUnit e utilizam Testcontainers para criar ambientes isolados.

```bash
cd Act.Caixa.Tests
dotnet test
```

**Observações**:
- Os testes de integração utilizam containers Docker isolados
- PostgreSQL e RabbitMQ são instanciados automaticamente via Testcontainers
- Os testes são executados em ambiente isolado, não afetando o ambiente de desenvolvimento

### Notas Importantes
- Os serviços utilizam cache em memória para otimização
- A comunicação entre serviços é feita de forma assíncrona via eventos
- As configurações de conexão podem ser ajustadas nos arquivos `appsettings.Development.json`
- O Gateway utiliza Ocelot para roteamento de requisições
- A documentação da API é gerada automaticamente pelo Scalar