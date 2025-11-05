# Obracore API

Descrição
---------
Obracore é uma API REST feita em ASP.NET Core (versão 10) usando Entity Framework Core e MySQL. A API expõe CRUDs para entidades de obras, etapas, usuários, perfis, custos, documentos e finanças. O projeto inclui um AppDbContext com mapeamentos detalhados e controllers básicos para cada entidade.

Tecnologias
-----------
- .NET / ASP.NET Core 10
- Entity Framework Core
- MySQL (ou MariaDB)
- Swashbuckle / Swagger (para documentação)
- Opcional: Pomelo.EntityFrameworkCore.MySql (driver recomendado para MySQL)

Resumo das features
-------------------
- Endpoints CRUD (GET all, GET by id, POST, PUT, DELETE) para as entidades do modelo.
- Configuração de mapeamento detalhada no AppDbContext.
- Swagger para inspeção e teste dos endpoints.

Pré-requisitos
--------------
- .NET SDK (compatível com ASP.NET Core 10)
- MySQL ou MariaDB em execução
- (opcional) dotnet-ef tool para criação/aplicação de migrations:
  - dotnet tool install --global dotnet-ef

Dependências recomendadas (nuget)
--------------------------------
- Microsoft.EntityFrameworkCore
- Pomelo.EntityFrameworkCore.MySql (ou outro provedor MySQL compatível)
- Swashbuckle.AspNetCore
- (opcional) AutoMapper e AutoMapper.Extensions.Microsoft.DependencyInjection

Configuração (passo a passo)
----------------------------

1) Clonar o repositório
   - git clone <seu-repo-url>
   - cd <pasta-do-projeto>   (pasta que contém o .csproj com AppDbContext)

2) Configurar connection string
   - No arquivo appsettings.json (ou em variáveis de ambiente) adicione/edite a seção ConnectionStrings:

   Exemplo appsettings.json:
   {
     "ConnectionStrings": {
       "DefaultConnection": "server=localhost;port=3306;database=obracore;user=root;password=MinhaSenha;"
     },
     "Logging": { ... }
   }

   Observação: prefira armazenar credenciais em variáveis de ambiente ou cofre de segredos em produção.

3) Instalar provider MySQL e ferramentas EF (se ainda não estiverem instalados)
   - dotnet add package Pomelo.EntityFrameworkCore.MySql
   - dotnet tool install --global dotnet-ef

   Certifique-se de escolher versão do Pomelo que seja compatível com a versão do EF Core usada no projeto.

4) Criar e aplicar migrations
   - No diretório do projeto (onde está o .csproj e AppDbContext):
     - dotnet ef migrations add InitialCreate
     - dotnet ef database update

   Se o projeto tem separação entre projeto de startup e projeto com DbContext, use `--startup-project` e `--project` conforme necessário:
   - dotnet ef migrations add InitialCreate --project Obracore.Data --startup-project Obracore.Api

5) Ajustes para o Swagger & Serialização (importante)
   - Se no Swagger você encontrar erros como "Could not resolve reference" por causa de referências circulares entre entidades com navegações bidirecionais, aplique uma das soluções abaixo.

   Solução rápida (recomendada para desenvolvimento): configurar serializer para ignorar ciclos:
   - Em Program.cs (top-level) configure os controllers assim:

   using System.Text.Json.Serialization;

   var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddControllers()
       .AddJsonOptions(opts =>
       {
           // evita referências circulares no JSON (Swagger não cria $ref recursivos)
           opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
           opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
       });

   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();

   var app = builder.Build();
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }
   app.UseAuthorization();
   app.MapControllers();
   app.Run();

   Alternativa: usar Newtonsoft.Json em vez do System.Text.Json:
   - dotnet add package Swashbuckle.AspNetCore.Newtonsoft
   - builder.Services.AddControllers().AddNewtonsoftJson(opts => { opts.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
   - builder.Services.AddSwaggerGen(); builder.Services.AddSwaggerGenNewtonsoftSupport();

   Melhor prática (a longo prazo): evite expor as entidades EF diretamente — use DTOs. Isso resolve ciclos, evita over-posting e gera schemas OpenAPI previsíveis.

6) Rodar a aplicação localmente
   - dotnet run
   - ou para hot-reload: dotnet watch run

   Por padrão a Swagger UI estará disponível em:
   - https://localhost:5001/swagger (ou http://localhost:5000 dependendo da sua configuração)

7) Testar endpoints
   - Usando Swagger UI, Postman ou curl.
   - Rotas padrão esperadas: /api/Usuarios, /api/Obras, /api/Etapas, /api/CustosObra, etc.

Exemplo de comandos úteis
-------------------------
- Restaurar pacotes:
  dotnet restore

- Build:
  dotnet build

- Rodar:
  dotnet run

- Criar migration:
  dotnet ef migrations add NomeDaMigration

- Aplicar migration:
  dotnet ef database update

Rodando com Docker (MySQL + app)
-------------------------------
Exemplo simples de docker-compose.yml:

version: "3.8"
services:
  db:
    image: mysql:8.0
    environment:
      MYSQL_ROOT_PASSWORD: MinhaSenha
      MYSQL_DATABASE: obracore
    ports:
      - "3306:3306"
    volumes:
      - db_data:/var/lib/mysql
  app:
    build: .
    depends_on:
      - db
    environment:
      ConnectionStrings__DefaultConnection: "server=db;port=3306;database=obracore;user=root;password=MinhaSenha;"
    ports:
      - "5000:80"

volumes:
  db_data:

Observação: ajuste Dockerfile e nome do projeto conforme sua estrutura.