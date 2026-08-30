# TaskTracker

Aplicação web para gerenciamento de tarefas pessoais, construída em **ASP.NET Core MVC** com **PostgreSQL**. Cada usuário cria sua conta, gerencia suas próprias tarefas, e um painel administrativo separado permite gerenciar todos os usuários e suas tarefas.

## Funcionalidades

### Usuário
- Cadastro de conta com nome, e-mail (único) e senha (armazenada com hash via `PasswordHasher`)
- Login/logout com autenticação por cookie
- CRUD completo de tarefas: criar, editar, ver detalhes e excluir
- Cada tarefa tem título, descrição, status (`Pendente`, `Em andamento`, `Concluída`) e prazo
- Cada usuário só acessa e gerencia suas próprias tarefas

### Painel Administrativo
- Login separado em `/Admin/Login`, com um esquema de autenticação independente do usuário comum
- Primeiro administrador criado automaticamente na primeira execução (via *seed*, configurável por User Secrets)
- Listagem de todos os usuários cadastrados, com contagem de tarefas
- Editar ou excluir contas de usuário
- Visualizar, criar, editar e excluir as tarefas de qualquer usuário

## Tecnologias

- ASP.NET Core MVC (.NET)
- Entity Framework Core + Npgsql (PostgreSQL)
- Autenticação por Cookie (dois esquemas: usuário e admin)
- Bootstrap 5
- Google Fonts (Poppins)

## Como rodar o projeto

### Pré-requisitos
- [.NET SDK](https://dotnet.microsoft.com/download) instalado
- PostgreSQL instalado e rodando (local ou em nuvem)

### 1. Clonar o repositório
```bash
git clone https://github.com/Arthur-Cardoso0/TaskTracker.git
cd TaskTracker
```

### 2. Restaurar os pacotes
```bash
dotnet restore
```

### 3. Configurar a string de conexão e os segredos

O projeto usa [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) para não expor credenciais no repositório:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:AppDbConnectionString" "Host=localhost;Database=tasktracker;Username=postgres;Password=SUASENHA"
dotnet user-secrets set "AdminSeed:Username" "admin"
dotnet user-secrets set "AdminSeed:Password" "UmaSenhaForte123"
```

`AdminSeed:Username` e `AdminSeed:Password` definem as credenciais do primeiro administrador, criado automaticamente na primeira execução se a tabela de admins estiver vazia.

### 4. Aplicar as migrations
```bash
dotnet ef database update
```

### 5. Rodar a aplicação
```bash
dotnet run
```

Acesse `http://localhost:<porta>` para a área do usuário, ou `http://localhost:<porta>/Admin/Login` para o painel administrativo.

## Estrutura do projeto

```
TaskTracker/
├── Controllers/
│   ├── ContaController.cs      # Cadastro, login e logout de usuário
│   ├── TarefaController.cs     # CRUD de tarefas do usuário logado
│   ├── AdminController.cs      # Login, gerenciamento de usuários e tarefas (admin)
│   └── HomeController.cs
├── Models/
│   ├── Usuario.cs
│   ├── Tarefa.cs
│   └── Admin.cs
├── Data/
│   └── AppDbContext.cs
├── Views/
│   ├── Conta/
│   ├── Tarefa/
│   ├── Admin/
│   └── Shared/                 # _Layout.cshtml (site) e _AdminLayout.cshtml (painel)
├── Migrations/
└── wwwroot/css/site.css
```

## Segurança

- Senhas de usuários e administradores nunca são salvas em texto puro — usam `PasswordHasher` do ASP.NET Core.
- Sessões de usuário e de administrador usam esquemas de cookie **separados** (`AdminScheme` vs. o padrão), evitando qualquer mistura entre as duas áreas.
- Todas as ações do painel administrativo exigem autenticação (`[Authorize(AuthenticationSchemes = "AdminScheme")]`).
- Credenciais e strings de conexão ficam fora do controle de versão, via User Secrets.