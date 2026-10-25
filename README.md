1. Plataforma GaloReceitas24horas (GR24h)

Plataforma web completa para partilha, gestão e avaliação interativa de receitas culinárias, desenvolvida em C# com .NET e Razor Pages.
O projeto foi concebido focando em escalabilidade e manutenção, aplicando uma arquitetura em camadas bem definida, padrões de software modernos (DDD, Repository Pattern, DTOs) e uma integração robusta com base de dados SQL Server.

Demonstração da Interface

Página Principal (Homepage):

<img width="1876" height="773" alt="Captura de ecrã GR24hHomepage" src="https://github.com/user-attachments/assets/ad705802-1e3a-4e96-8cb6-8341c5361ef1" />

Página de Pesquisa (Search):

<img width="1820" height="902" alt="Captura de ecrã 2026-GR24hSearch" src="https://github.com/user-attachments/assets/777b2395-6814-4ea0-a429-36aeef924aba" />


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
