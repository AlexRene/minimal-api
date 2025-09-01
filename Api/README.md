# Minimal API - Sistema de Gestão de Veículos

## 🔐 Melhorias de Segurança Implementadas

### 1. **Hash de Senhas com BCrypt**
- Todas as senhas são agora hasheadas usando BCrypt com salt de 12 rounds
- Senhas nunca são armazenadas em texto plano
- Verificação segura de senhas durante login

### 2. **Configuração JWT Segura**
- Chave JWT configurável via appsettings.json
- Validação de Issuer e Audience
- Expiração configurável
- ClockSkew zero para maior precisão

### 3. **CORS Configurado Adequadamente**
- Origem, métodos e headers restritos
- Suporte a credenciais
- Configuração via appsettings.json

### 4. **Tratamento de Erros**
- Try-catch em todas as operações críticas
- Mensagens de erro padronizadas
- Não exposição de informações sensíveis

## 🏗️ Melhorias de Arquitetura Implementadas

### 1. **Endpoints Organizados**
- Separação de endpoints em arquivos específicos
- Uso de extension methods para organização
- Documentação Swagger melhorada
- Código mais limpo e manutenível

### 2. **Padrão Repository**
- Implementação do padrão Repository genérico
- Repositórios específicos para cada entidade
- Separação de responsabilidades
- Facilita testes unitários

### 3. **Tratamento Global de Exceções**
- Middleware para captura de exceções não tratadas
- Respostas de erro padronizadas
- Logging estruturado de erros
- Melhor experiência do usuário

### 4. **Async/Await**
- Todas as operações de banco são assíncronas
- Melhor performance e escalabilidade
- Uso correto de async/await em toda a aplicação

### 5. **Injeção de Dependência**
- Registro adequado de todos os serviços
- Repositórios registrados no container DI
- Facilita testes e manutenção

## 🚀 Funcionalidades Avançadas Implementadas

### 1. **Sistema de Paginação Avançado**
- Paginação com metadados completos
- Informações sobre páginas anterior/próxima
- Contagem total de itens
- Tamanho de página configurável

### 2. **Filtros Avançados para Veículos**
- Filtro por nome (busca parcial)
- Filtro por marca (busca parcial)
- Filtro por faixa de anos (mínimo e máximo)
- Ordenação por múltiplos campos
- Combinação de filtros

### 3. **Health Checks**
- Endpoint `/health` para monitoramento
- Verificação de conectividade com banco
- Status do sistema em tempo real
- Ideal para monitoramento e alertas

### 4. **Documentação Swagger Aprimorada**
- Informações detalhadas da API
- Exemplos de uso
- Documentação de todos os endpoints
- Interface organizada por tags

### 5. **Endpoints de Estatísticas**
- Estatísticas gerais do sistema
- Contagem de administradores e veículos
- Base para dashboards e relatórios
- Monitoramento de crescimento

## 🧪 Melhorias de Qualidade e Testes Implementadas

### 1. **Validação com FluentValidation**
- Validação robusta de DTOs
- Mensagens de erro personalizadas
- Validação automática de entrada
- Regras de validação configuráveis

### 2. **Logging Estruturado com Serilog**
- Logs estruturados e legíveis
- Logs em arquivo com rotação diária
- Logs no console para desenvolvimento
- Enriquecimento de logs com contexto

### 3. **Testes Unitários**
- Testes com xUnit e Moq
- Cobertura de código básica
- Testes de serviços principais
- Estrutura para expansão de testes

### 4. **Análise Estática de Código**
- Configuração .editorconfig para padronização
- Análise de código com .NET Analyzers
- Tratamento de warnings como erros
- Padrões de código consistentes

### 5. **Middleware de Logging**
- Logging de todas as requisições
- Medição de tempo de resposta
- Captura de informações de contexto
- Logs de erro estruturados

## 🚀 Como Executar

### Pré-requisitos
- .NET 7.0 SDK
- MySQL Server
- Visual Studio 2022 ou VS Code

### Configuração
1. Clone o repositório
2. Configure a string de conexão no `appsettings.json`
3. Execute as migrações: `dotnet ef database update`
4. Execute o projeto: `dotnet run`

### Executar Testes
```bash
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~VeiculoServicoTests"
```

### Configurações de Segurança
```json
{
  "Jwt": {
    "Key": "sua-chave-super-secreta-aqui",
    "Issuer": "MinimalApi",
    "Audience": "MinimalApiUsers",
    "ExpirationHours": 24
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["Content-Type", "Authorization"]
  }
}
```

## 📋 Endpoints Disponíveis

### Autenticação
- `POST /administradores/login` - Login de administrador

### Administradores (Requer role "Adm")
- `GET /administradores` - Listar administradores
- `GET /administradores/{id}` - Buscar administrador por ID
- `POST /administradores` - Criar novo administrador

### Veículos
- `GET /veiculos` - Listar veículos (Requer autenticação)
- `GET /veiculos/filtros` - Listar com filtros avançados (Requer autenticação)
- `GET /veiculos/marca/{marca}` - Buscar por marca (Requer autenticação)
- `GET /veiculos/ano/{ano}` - Buscar por ano (Requer autenticação)
- `GET /veiculos/nome/{nome}` - Buscar por nome (Requer autenticação)
- `GET /veiculos/{id}` - Buscar veículo por ID (Requer role "Adm,Editor")
- `POST /veiculos` - Criar novo veículo (Requer role "Adm,Editor")
- `PUT /veiculos/{id}` - Atualizar veículo (Requer role "Adm")
- `DELETE /veiculos/{id}` - Deletar veículo (Requer role "Adm")

### Estatísticas (Requer autenticação)
- `GET /estatisticas` - Estatísticas gerais do sistema
- `GET /estatisticas/veiculos` - Estatísticas dos veículos

### Monitoramento
- `GET /health` - Health check do sistema

## 🔍 Exemplos de Uso dos Filtros Avançados

### Filtro com Paginação
```
GET /veiculos/filtros?nome=civic&marca=honda&anoMinimo=2020&anoMaximo=2023&pagina=1&tamanhoPagina=20&ordenarPor=ano&ordenacaoAscendente=false
```

### Resposta com Metadados
```json
{
  "data": [...],
  "metadata": {
    "currentPage": 1,
    "totalPages": 5,
    "pageSize": 20,
    "totalCount": 95,
    "hasPrevious": false,
    "hasNext": true,
    "firstItemOnPage": 1,
    "lastItemOnPage": 20
  }
}
```

## 🏗️ Estrutura do Projeto

```
Api/
├── Dominio/
│   ├── Configuracoes/     # Classes de configuração
│   ├── DTOs/             # Data Transfer Objects
│   ├── Entidades/        # Entidades do domínio
│   ├── Enuns/           # Enumerações
│   ├── Interfaces/      # Interfaces dos serviços
│   ├── ModelViews/      # Modelos de visualização
│   ├── Servicos/        # Implementação dos serviços
│   └── Validators/      # Validadores FluentValidation
├── Endpoints/           # Endpoints organizados
├── Infraestrutura/
│   ├── Db/             # Contexto do Entity Framework
│   └── Repositories/   # Implementação do padrão Repository
├── Middleware/         # Middlewares customizados
├── Tests/              # Testes unitários
├── Migrations/         # Migrações do banco de dados
├── .editorconfig       # Configuração de estilo de código
└── Directory.Build.props # Configurações de análise estática
```

## 🔒 Considerações de Segurança

### ✅ Implementado
- Hash de senhas com BCrypt
- JWT com validação completa
- CORS configurado adequadamente
- Tratamento de erros
- Validação de entrada
- Tratamento global de exceções
- Validação robusta com FluentValidation
- Logging estruturado de todas as operações

### ⚠️ Recomendações para Produção
- Usar chave JWT de pelo menos 256 bits
- Configurar HTTPS
- Implementar rate limiting
- Adicionar logging de auditoria
- Configurar backup automático do banco
- Usar variáveis de ambiente para configurações sensíveis
- Implementar monitoramento de logs
- Configurar alertas de segurança

## 🧪 Testes
Execute os testes com: `dotnet test`

## 📚 Documentação
Acesse o Swagger em: `https://localhost:5001/swagger`

## 📊 Logs
Os logs são salvos em:
- Console: Durante a execução
- Arquivo: `logs/minimal-api-YYYY-MM-DD.log`

## 🔄 Funcionalidades Futuras Recomendadas

### Cache e Performance
- Cache Redis para melhor performance
- Compressão de respostas
- Paginação com cursor
- Lazy loading de entidades

### Funcionalidades Avançadas
- Upload de imagens para veículos
- Sistema de notificações
- Relatórios em PDF/Excel
- API versioning
- Rate limiting
- Auditoria de ações
- Webhooks para eventos

### Monitoramento e Observabilidade
- Métricas com Prometheus
- Tracing distribuído
- Dashboard de métricas
- Alertas automáticos
- Log aggregation
