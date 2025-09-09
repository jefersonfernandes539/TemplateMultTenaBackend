# TemplateMultTenaBackend

Este projeto é um **template backend** desenvolvido em **.NET 8** que adota os princípios da **Clean Architecture** e suporta **Multi-Tenancy** (multi-inquilino), permitindo a criação de aplicações escaláveis, modulares e organizadas.

---

## 🏗️ Arquitetura

A solução segue os princípios da **Clean Architecture**, garantindo separação de responsabilidades, baixo acoplamento e alta testabilidade:

- **Domain**  
  Contém as **entidades de negócio** e **interfaces**. Não possui dependências de outras camadas.

- **Application**  
  Implementa os **casos de uso** (application services) e regras de negócio da aplicação.  
  Depende apenas da camada **Domain**.

- **Infrastructure**  
  Contém a implementação de persistência, repositórios, integrações externas e acesso a dados.  
  Depende de **Application** e **Domain**.

- **WebApi**  
  Exposição dos serviços via **API REST**.  
  Depende das demais camadas e funciona como **ponto de entrada** da aplicação.

---

## 🌍 Multi-Tenancy

O projeto é **multi-tenant**, permitindo atender múltiplos clientes (ou inquilinos) em uma mesma aplicação.  
Isso pode ser feito de diferentes formas:
- **Tenant por banco de dados**  
- **Tenant por schema**  
- **Tenant por coluna (discriminator)**  

A implementação já está preparada para identificar o **tenant** a partir do **contexto da requisição** (ex.: `Header`, `JWT` ou `ConnectionString` dinâmica).

---

## 🚀 Tecnologias Utilizadas

- **.NET 8**  
- **ASP.NET Core Web API**  
- **Entity Framework Core** (migrations e persistência)  
- **Clean Architecture**  
- **Multi-Tenancy strategy**  
- **Dependency Injection (DI)** nativa do .NET  
- **MediatR** (para organização de casos de uso) *(se estiver usando)*  
- **FluentValidation** (para validações) *(se estiver usando)*  

---

## 📂 Estrutura do Projeto

