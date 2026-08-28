# TaskTracker

Um sistema web prático para gerenciamento de tarefas e contas de usuários, desenvolvido utilizando o padrão de arquitetura MVC (Model-View-Controller).

## 🚀 Funcionalidades

* **Gerenciamento de Contas:** Registro e controle de acesso de usuários (via `ContaController`).
* **Controle de Tarefas (CRUD):** Criação, visualização, edição e exclusão de tarefas diárias (via `TarefaController`).
* **Interface Dinâmica:** Telas modulares renderizadas no servidor utilizando o motor Razor.
* **Persistência de Dados:** Mapeamento objeto-relacional automatizado utilizando Entity Framework Core, incluindo histórico de migrações.

## 🛠️ Tecnologias Utilizadas

* **Linguagem:** C#
* **Framework:** ASP.NET Core MVC
* **ORM:** Entity Framework Core
* **Front-end:** HTML, CSS e Razor Views (`.cshtml`)
* **Banco de Dados:** Gerenciado via `AppDbContext` (Configuração definida no `appsettings.json`)

## 📁 Estrutura do Projeto

* `Controllers/`: Contém a lógica de controle e roteamento da aplicação (`HomeController`, `ContaController`, `TarefaController`).
* `Models/`: Define as entidades principais do domínio e regras de dados (`Tarefa`, `Usuario`).
* `Views/`: Contém as interfaces de usuário agrupadas por controlador, além de layouts compartilhados (`_Layout.cshtml`).
* `Data/`: Configuração de contexto do banco de dados (`AppDbContext`).
* `Migrations/`: Arquivos de versionamento do esquema do banco de dados (ex: `InitialCreate`).

## ⚙️ Como Executar o Projeto

1. Clone o repositório para o seu ambiente local.
2. Certifique-se de ter o [.NET SDK](https://dotnet.microsoft.com/download) instalado.
3. Abra o terminal na raiz do projeto e restaure as dependências:
   ```bash
   dotnet restore

   dotnet ef database update

   dotnet run
