# Grupo13Fiap - Plataforma de Jogos API

API desenvolvida em .NET 10 para gerenciamento de uma plataforma de jogos, com autenticação de usuários e controle de acesso por perfil (Admin e User).

# Objetivo

Esta API tem como objetivo:

- Gerenciar usuários da plataforma
- Controlar autenticação via JWT
- Permitir que administradores cadastrem jogos no catalogo
- Disponibilizar endpoints REST para consumo

# Tecnologias Utilizadas

- .NET (10)
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT (JSON Web Token)
- Swagger (OpenAPI)
- Scalar

# Pré-requisitos

Antes de executar o projeto, você precisa ter:

- .NET 10 instalado
- SQL Server (local ou Docker)


# Configuração do Banco de Dados

## 1. Criar o banco

CREATE DATABASE Grupo13FiapDb;

## 2. Configurar a connection string no appsettings.json

"ConnectionStrings": {
  "DefaultConnection": "SUA_STRING_DE_CONEXÃO"
}

## 3. Aplicar Migrations

- dotnet ef migrations add UpdateDescriptionMaxLength --project src\Grupo13Fiap.Infrastructure --startup-project src\Grupo13Fiap.WebApi --context DBContextGrupo13Fiap 2>&1

- dotnet ef migrations add InitialCreate --project src\Grupo13Fiap.Identity --startup-project src\Grupo13Fiap.WebApi --context IdentityDataContext 2>&1

- dotnet ef database update --project src\Grupo13Fiap.Infrastructure --startup-project src\Grupo13Fiap.WebApi --context DBContextGrupo13Fiap 2>&1

- dotnet ef database update --project src\Grupo13Fiap.Identity --startup-project src\Grupo13Fiap.WebApi --context IdentityDataContext 2>&1
  

# Pós execução

- Abrir o localhost Swagger ou Scalar
- Após isso é necessario se autenticar na URL (/api/v1/user/login)
   pode se autenticar com o usuario ADMIN " Email: admin@grupo13.com / senha: Admin@123! "
   ou pode criar um usuario na URL (/api/v1/user/register) e depois se autenticar na URL anterior
- Após a autenticação vai ser gerado um token para que consiga acessar as outras URL's através do Authorize 
  
# Autores

- Alessandro dos Santos Santana – Discord: alee___ 
-
- Gutemberg Nascimento da Silva – Discord: gutemberg1328 
- Guilherme Monteiro Massi – Discord: guimassi 
- Kevin Castro de Oliveira – Discord: kevincastrodev 





