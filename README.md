# Agenda de Contatos – API + Frontend

Projeto desenvolvido como parte de um desafio técnico em .NET, consistindo em uma **API REST** para gerenciamento de contatos e um **frontend em Vue.js** para consumo dessa API.

---

##  Preview da Aplicação

<img width="913" height="461" alt="Tela Inicial" src="https://github.com/user-attachments/assets/0121b42c-8225-4f2c-908a-b7573e52bfef" />


##  Visão Geral

A aplicação permite:

- Cadastrar contatos
- Listar contatos
- Editar contatos
- Excluir contatos

### Arquitetura

O projeto foi organizado em **múltiplas camadas**:

- `Agenda.Domain` – Entidades de domínio (ex: `Contact`)
- `Agenda.Infrastructure` – Acesso a dados (Entity Framework Core, repositórios, migrations)
- `Agenda.Api` – API REST em ASP.NET Core
- `Agenda.Tests` – Testes unitários com xUnit + Moq
- `agenda-frontend` – Frontend em Vue 3 + PrimeVue

---

## Tecnologias Utilizadas

### Backend

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core (Code First)**
- **SQL Server / LocalDB**
- **AutoMapper**
- **FluentValidation**
- **xUnit** + **Moq** para testes
- **Swagger / Swashbuckle** para documentação

### Frontend

- **Vue 3 (Composition API)**
- **Vite**
- **Axios**
- **PrimeVue**
- **PrimeIcons**

---

## Como Clonar o Repositório

```bash
git clone https://github.com/Deyvidsk10/TesteAgenda
cd TesteAgenda


⚙️ Configuração do Backend (API)
1️⃣ Acessar o projeto da API
cd Agenda.Api

2️⃣ Configurar a Connection String

No arquivo appsettings.json da API:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AgendaDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}


Se estiver usando outra instância do SQL Server (ex.: SQLEXPRESS), basta ajustar a Server=.

3️⃣ Aplicar Migrations

No Package Manager Console ou terminal, dentro do projeto da API/Infra:

dotnet ef database update


Isso criará o banco AgendaDb com a tabela de contatos.

4️⃣ Rodar a API
dotnet run


A API ficará disponível em algo como:

https://localhost:52203

🔍 A documentação do Swagger fica em:
https://localhost:52203/swagger/index.html

🌐 Endpoints Principais

Todos os endpoints principais estão sob o controller:

/api/contacts

🔹 GET /api/contacts

Retorna todos os contatos.

🔹 GET /api/contacts/{id}

Retorna um contato específico por id (Guid).

🔹 POST /api/contacts

Cria um novo contato.

Request body (JSON):

{
  "name": "João da Silva",
  "email": "joao@teste.com",
  "phone": "81999999999"
}

🔹 PUT /api/contacts/{id}

Atualiza um contato existente.

🔹 DELETE /api/contacts/{id}

Remove um contato existente.

🧪 Testes de Backend

Os testes estão no projeto:

Agenda.Tests

Foram criados testes unitários para o ContactService utilizando:

xUnit

Moq

Para rodar os testes:

cd Agenda.Tests
dotnet test

🖥️ Configuração do Frontend (Vue 3 + PrimeVue)
1️⃣ Acessar a pasta do frontend
cd agenda-frontend

2️⃣ Instalar dependências
npm install

3️⃣ Arquivo main.js

Exemplo de configuração com PrimeVue e Toast:

import { createApp } from 'vue'
import App from './App.vue'

import PrimeVue from 'primevue/config'
import ToastService from 'primevue/toastservice'

// Estilos PrimeVue
import 'primevue/resources/themes/saga-blue/theme.css'
import 'primevue/resources/primevue.min.css'
import 'primeicons/primeicons.css'

const app = createApp(App)

app.use(PrimeVue)
app.use(ToastService)

app.mount('#app')

4️⃣ Rodar o frontend
npm run dev


O frontend estará disponível em:

http://localhost:5173

🔗 Integração Frontend + Backend

No arquivo App.vue, o Axios foi configurado para consumir a API:

const api = axios.create({
  baseURL: 'https://localhost:52203/api/contacts'
})


📌 Ajuste a URL se a porta da API mudar.

 CORS

Para permitir que o frontend (Vite em http://localhost:5173) acesse a API, foi configurado CORS em Program.cs:

var corsPolicyName = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(corsPolicyName);

🧠 Regras de Negócio e Validações

Não permite cadastro de contatos com e-mail duplicado.

Validações de entrada feitas com FluentValidation:

CreateContactDto

UpdateContactDto

A API retorna erros de validação em formato amigável (400 – BadRequest).

📸 Telas (Frontend)

A tela principal inclui:

Formulário para criar/editar contatos

Tabela com:

Nome

E-mail

Telefone

Ações (Editar / Excluir)

Feedback visual via Toast (PrimeVue) para:

Sucesso

Erros

Avisos

✅ Status do Projeto

✅ API funcionando com CRUD completo

✅ Integração com banco via EF Core

✅ Validações com FluentValidation

✅ Testes unitários com xUnit + Moq

✅ Frontend em Vue 3 integrado com a API

✅ Toasts e tabela estilizada com PrimeVue

📌 Como Rodar Tudo Junto

Subir a API

cd Agenda.Api
dotnet run


Subir o frontend

cd agenda-frontend
npm run dev


Acessar o frontend em:

http://localhost:5173

Confirmar que os contatos estão sendo carregados e manipulados pela API em:

https://localhost:52203/api/contacts

📚 Melhorias Futuras (Sugestões)

Paginação e filtros na listagem de contatos

Autenticação/JWT para proteger a API

Dockerfile para facilitar deploy

Deploy em algum serviço (Azure, Railway, etc.)

👨‍💻 Autor: @deyvidsk10
