# 🚀 DeskFlow API — Sistema de Gestão de Chamados e Helpdesk de TI

A **DeskFlow API** é uma Web API RESTful construída em **.NET 10** utilizando **Entity Framework Core** e persistência de dados. O sistema foi desenvolvido como Projeto Final Avaliativo para automatizar o gerenciamento de chamados de suporte técnico, histórico de interações técnicas e o acompanhamento rigoroso do ciclo de vida dos atendimentos de TI.

---

## 🛠️ Tecnologias Utilizadas

- **.NET 10 (Web API)**
- **Entity Framework Core 10** (Persistência e ORM)
- **Banco de Dados em Memória (InMemory)** (Para homologação rápida e testes fluidos)
- **Swagger / OpenAPI** (Documentação e Teste de Endpoints)

---

## 📐 Arquitetura do Projeto

O projeto adota uma arquitetura em camadas estruturada de forma desacoplada para garantir a separação de responsabilidades e **Injeção de Dependência (IoC)**:

- **`Controllers`**: Exposição das rotas HTTP, recebimento de requisições e definição semântica dos Status Codes (200, 201, 204, 400, 404).
- **`Services`**: Camada isolada contendo as regras de negócio cruciais e validações obrigatórias das transições de status.
- **`Repositories`**: Camada responsável exclusivamente pelo acesso a dados e comandos de forma assíncrona, separando contratos (Interfaces) das implementações.
- **`Models/Entities`**: Representação das entidades do sistema (`Chamado`, `Categoria`, `Interacao`) e seus relacionamentos (1:N).
- **`Middlewares`**: Pipeline customizado (`ExceptionHandlingMiddleware`) para captura global de exceções e tratamento de erros sem vazamento de stack trace.
- **`Data`**: Centralização e configuração do contexto do banco de dados (`AppDbContext`).

---

## 🔄 Regras de Negócio Implementadas (Ciclo de Vida)

1. **Abertura de Chamado:** Registro automatizado com status inicial definido como `Aberto` e carimbo de data/hora atual (`DateTime.UtcNow`).
2. **Início do Atendimento:** Validação rigorosa que permite a transição de status para `EmAndamento` apenas se o chamado estiver previamente `Aberto`.
3. **Encerramento de Chamado:** Exigência obrigatória de preenchimento do texto descritivo da solução técnica para a conclusão com status `Fechado`.
4. **Histórico de Interações:** Bloqueio sumário que impede a inclusão de novas notas ou comentários em chamados que já se encontram com status `Fechado`.

---

## ⚙️ Como Executar a Aplicação

### Pré-requisitos
- .NET SDK 10 (ou superior)

### Passo a Passo

1. **Acesse a pasta principal da API no terminal:**
   ```bash
   cd DeskFlow.API
   ```

2. **Restaure as dependências e compile o projeto:**
   ```bash
   dotnet build
   ```

3. **Execute a API:**
   ```bash
   dotnet run
   ```

4. **Acesse a documentação do Swagger para interagir com as rotas:**
   Abra no seu navegador o endereço indicado no console acompanhado de `/swagger` (Ex: `http://localhost:5041/swagger`).
