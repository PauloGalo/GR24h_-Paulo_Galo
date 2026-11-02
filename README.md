1. Plataforma GaloReceitas24horas (GR24h)

Plataforma web completa para partilha, gestão e avaliação interativa de receitas culinárias, desenvolvida em C# com .NET e Razor Pages.
O projeto foi concebido focando em escalabilidade e manutenção, aplicando uma arquitetura em camadas bem definida, padrões de software modernos (DDD, Repository Pattern, DTOs) e uma integração robusta com base de dados SQL Server.

Demonstração da Interface

Página Principal (Homepage):

<img width="1900" height="4873" alt="localhost_7123_ Homepage" src="https://github.com/user-attachments/assets/c3785450-8b89-4483-b0dd-7f732736dd63" />


Página de Pesquisa (Search):

<img width="1900" height="2110" alt="localhost_7123_Search_Filters SearchTerm= SearchPage" src="https://github.com/user-attachments/assets/b2772561-7821-4db3-806d-ba6c6d2a0528" />


Página de Receita (RecipePage):

<img width="1900" height="4005" alt="localhost_7123_RecipePage_id=118" src="https://github.com/user-attachments/assets/8828385a-a78a-4555-9818-b05d9b2bb160" />


Área Pessoal de Utilizador (UserProfile):

<img width="1900" height="2123" alt="localhost_7123_PersonalAreaManagement_UserPersonalManagement_UserProfile_id=10" src="https://github.com/user-attachments/assets/72df2c04-4279-4b49-aede-c2a79500a0fd" />


Área Pessoal de Admin (AdminProfile):

<img width="1900" height="1519" alt="localhost_7123_PersonalAreaManagement_AdminPersonalManagement_AdminProfile_id=3" src="https://github.com/user-attachments/assets/9dc1a8c2-657e-4af4-ac00-dfdb0d96483d" />


2. Principais Funcionalidades (Features)

Gestão Completa de Receitas: Criar, editar, listar, detalhar e apagar receitas, com suporte flexível para upload de imagens locais ou inserção por link da web.

Autenticação, Sessões & Dados Pessoais: Sistema de autenticação de utilizadores com controlo seguro de sessões, permitindo a cada utilizador gerir e atualizar os seus próprios dados pessoais e perfil.

Sistema de Avaliações: Classificação de receitas de 1 a 5 estrelas, implementando validações de unicidade e regras temporais.

Comentários & Interação: Sistema robusto de comentários e respostas encadeadas.

Favoritos & Popularidade: Gestão de receitas favoritas e cálculo automático do nível de popularidade.

Pesquisa Avançada: Barra de pesquisa com filtros por categoria, dificuldade, autor e ingredientes, suportando pesquisa parcial e tratamento de acentos.

Painel de Moderação & Gestão de Utilizadores: Painel dedicado para administradores aprovarem receitas pendentes e gerirem contas e acessos de utilizadores.

3. Arquitetura e Tecnologias

Linguagem & Framework: C#, .NET, ASP.NET Core Razor Pages.

Base de Dados & Persistência: Microsoft SQL Server, consultas relacionais e execução de scripts SQL customizados (script.sql).

Front-End: HTML, CSS, JavaScript, Bootstrap.

Arquitetura: Arquitetura em Camadas (Domain/Models, Repositories, Services, Controllers/Pages).

Padrões & Lógica: DTOs (Data Transfer Objects), Repository Pattern, Injeção de Dependências, princípios de Domain-Driven Design (DDD) com validações e regras de negócio na camada de serviço, e tratamento robusto de nulos e exceções.

4. Estrutura do Projeto

GR24h_PauloGalo/

GR24h_Controller/ (Camada de Apresentação e Razor Pages)

GR24h_Models/ (Entidades, DTOs e View Models)

GR24h_Repos/ (Acesso a Dados e Repositórios SQL)

GR24h_Services/ (Lógica de Negócio e Serviços)

script.sql (Script SQL para criação e povoamento da base de dados)

README.md (Documentação do projeto)

5. Autor

Paulo Galo

Desenvolvedor C# / .NET

GitHub: https://github.com/PauloGalo
