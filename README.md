# iATeste - Plataforma de Sinais de Investimento (IA + RAG)

Projeto web em **.NET 9 + Blazor** com arquitetura em camadas para gerar sinais de ativos a partir de notícias geopolíticas, políticas e naturais.

## Arquitetura

- `src/IATeste.Domain`: entidades e contratos de domínio
- `src/IATeste.Application`: casos de uso e abstrações
- `src/IATeste.Infrastructure`: implementação de IA (heurística inicial) e RAG (base histórica em memória)
- `src/IATeste.Web`: interface Blazor Server
- `tests/IATeste.Application.Tests`: testes unitários do caso de uso

## Como executar

```bash
dotnet restore IATeste.slnx
dotnet build IATeste.slnx -c Release
dotnet test IATeste.slnx -c Release
cd src/IATeste.Web
dotnet run
```

Acesse: `https://localhost:xxxx`

## Sobre IA e RAG

- **IA (análise):** `IMarketSignalAnalyzer`
- **RAG (contexto histórico):** `IHistoricalContextRetriever`

A implementação atual usa provedores gratuitos/simulados para validação rápida:
- `HeuristicMarketSignalAnalyzer`
- `InMemoryHistoricalContextRetriever`

Para trocar de tecnologia, implemente novas classes dessas interfaces e registre no DI.

## Deploy para teste (alternativas web)

Sem servidor próprio, você pode usar:
- **Azure App Service (free/trial)** para hospedar o Blazor Server
- **Render** com plano gratuito quando disponível
- **Fly.io** para laboratório com baixo custo

## Próximos passos recomendados

1. Integrar coleta real de notícias (APIs/RSS)
2. Substituir IA heurística por provedor LLM configurável (OpenAI, Azure OpenAI, Ollama, etc.)
3. Persistir contexto histórico em banco vetorial para RAG real
4. Adicionar autenticação, trilha de auditoria e monitoramento
5. Criar pipeline CI/CD com deploy automático
