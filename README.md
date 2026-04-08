# D.AI-Chain

## Objetivo

O D.AI-Chain e um prototipo de uma Layer 2 experimental chamada Aethos Ledger, construida principalmente em .NET 8, com componentes auxiliares em Vue/Vite, Solidity e Python.

A proposta do sistema e combinar:

- execucao de transacoes em ambiente compativel com Ethereum
- trilhas auditaveis de inferencia chamadas de Proof of Reasoning (PoR)
- governanca operacional e modo de contingencia
- checkpoints e integracao planejada entre L2 e L1

Hoje, o repositorio representa uma POC avancada com varias pecas implementadas, mas ainda com partes importantes simuladas, mockadas ou em fase de scaffold.

## Estado Atual do Projeto

O ponto de entrada principal do sistema e o projeto `src/Aethos.Node`, que concentra o bootstrap do no, o worker de sequenciamento, a exposicao de endpoints e a integracao entre os modulos centrais.

O repositorio nao deve ser lido como uma implementacao completa de uma L2 pronta para producao. O estado atual esta mais proximo de:

- uma base arquitetural funcional para experimentacao
- uma demonstracao de conceitos de consenso, PoR, governanca e bridge
- uma fundacao tecnica com testes e modulos separados por responsabilidade

Os arquivos de sprint em `src/Sprints/` e o script `src/generate_sprints.py` ajudam a entender a visao e a evolucao planejada, mas nao devem ser usados como prova de que cada etapa ja esta concluida no codigo.

## Arquitetura em Alto Nivel

### Backend principal

- `src/Aethos.Node`: no principal ASP.NET Core com RPC, WebSockets, gRPC, health checks, metricas e workers de segundo plano
- `src/Aethos.Presentation.RPC`: camada de JSON-RPC, roteadores de metodo e hubs SignalR
- `src/Aethos.Application`: camada application, hoje ainda muito proxima do template inicial
- `src/Aethos.Presentation.Admin`: API/admin app separado, ainda em estado basico de template

### Core de dominio e execucao

- `src/Aethos.Domain`: entidades, interfaces, value objects e excecoes
- `src/Aethos.Math.FixedPoint`: aritmetica deterministica com `FixedPointInt128`, vetores e matrizes
- `src/Aethos.Core.EVM`: processamento de transacao, wallet com thresholds e fluxo simplificado de execucao
- `src/Aethos.Core.AI.LSTM`: rede LSTM, trace de ativacao, importacao ONNX e geracao de PoR
- `src/Aethos.Core.Consensus`: sequencer, servico gRPC de consenso, slashing e guardas de divergencia
- `src/Aethos.Core.Governance`: pausa global, shadow mode e mecanismos de governanca
- `src/Aethos.Core.Persistence`: persistencia de estado por meio da abstracao `IStateDb`
- `src/Aethos.Core.SmartContracts`: registros e componentes ligados a account abstraction

### Infraestrutura e L1

- `src/Aethos.Infrastructure.Cache`: cache de inferencia em Redis
- `src/Aethos.Infrastructure.Bridge`: cliente de bridge L1/L2 e job de publicacao de state root
- `src/Aethos.Contracts.L1`: contratos Solidity para bridge, token e escape hatch

### Frontends e observabilidade

- `src/Aethos.Presentation.Dashboard`: dashboard Vue/Vite com componentes visuais de PoR, MetaMask e controles administrativos
- `src/Aethos.UI.Dashboard`: dashboard Vue/Vite separado, focado em telemetria e Network Health

### Testes e automacao

- `src/Aethos.Tests.Unit`: testes unitarios e alguns testes de integracao leve dentro do projeto
- `src/Aethos.Tests.Integration`: testes com Redis real via Testcontainers
- `src/Aethos.Tests.Performance`: projeto de performance ainda sem benchmark real implementado
- `src/.github/workflows/ci.yml`: pipeline CI declarada para build, testes e etapa de dockerizacao simulada

## Funcionalidades Encontradas no Codigo

### 1. No ASP.NET Core para a L2

O projeto `Aethos.Node` sobe um host ASP.NET Core com:

- health check em `/health`
- scraping de metricas Prometheus
- Swagger em ambiente de desenvolvimento
- autenticacao JWT e policy `AdminOnly`
- pipeline WebSocket
- pipeline JSON-RPC
- servico gRPC de consenso
- hubs SignalR para eventos de rede e transacoes

### 2. Camada JSON-RPC e WebSocket

Existe uma camada RPC publica no projeto `Aethos.Presentation.RPC` com:

- middleware dedicado para JSON-RPC
- extensoes para habilitar JSON-RPC e WebSockets
- controller HTTP para metodos estilo Ethereum
- hubs SignalR para notificacoes de administracao e saude da rede

O repositorio tenta se posicionar como uma interface compativel com ecossistema Ethereum, incluindo integracao planejada com MetaMask.

### 3. Processamento de transacoes e smart wallet

O modulo `Aethos.Core.EVM` possui:

- `EvmTransactionProcessor` que valida gas minimo e grava um estado simples no banco abstrato
- `AiSmartWallet` que bloqueia transacoes acima de um limite autonomo
- builder e estruturas auxiliares ligadas a account abstraction

Mesmo simplificado, esse trecho ja mostra as regras basicas de protecao por threshold e o fluxo de execucao de transacoes.

### 4. Motor LSTM e Proof of Reasoning

O repositorio contem implementacao para:

- `LstmCell`, `LstmState` e `LstmNetwork`
- rastreamento de ativacoes via `ActivationTrace`
- geracao de hash de prova via `ProofOfReasoning`
- importacao inicial de modelo ONNX com validacao por `Microsoft.ML.OnnxRuntime`

Essa parte e um dos diferenciais conceituais do projeto: atrelar a decisao/inferencia a uma prova resumida e auditavel.

### 5. Consenso e sequenciamento

No modulo `Aethos.Core.Consensus`, existem:

- `Sequencer` como `BackgroundService`
- servico gRPC `ConsensusService`
- `DivergenceGuard`
- `SlashingService`

O sequencer roda em loop e simula a forja de blocos, enquanto o servico gRPC recebe propostas e votos de bloco.

### 6. Governanca operacional

No modulo `Aethos.Core.Governance`, foram identificados:

- pausa global de inferencia
- retomada de execucao
- registro de shadow models
- processamento de inferencia em modo observacional
- estrutura para votacao e governanca

Esses modulos sustentam a ideia de um control plane para uma rede com componentes autonomos e supervisionados.

### 7. Cache de inferencia com Redis

O projeto `Aethos.Infrastructure.Cache` oferece:

- armazenamento de decisoes por hash de payload
- TTL de 1 hora
- invalidacao ampla por script Lua

Essa parte esta mais concreta e possui teste de integracao com Redis real via Testcontainers.

### 8. Bridge L1/L2 e contratos Solidity

O repositorio inclui:

- cliente de bridge L1 em C# com Nethereum
- `StateRootPublisher` em background
- contrato `AethosBridge.sol`
- contrato `EscapeHatch.sol`
- contrato `AethosToken.sol`

O desenho arquitetural aponta para publicacao de state root e PoR na L1, alem de um mecanismo de saque emergencial.

### 9. Dashboards e UX de operacao

Foram encontrados dois frontends distintos:

- `Aethos.Presentation.Dashboard`: inclui grafo D3 da arquitetura neural, painel de emergencia e tentativa de integracao com MetaMask
- `Aethos.UI.Dashboard`: exibe telemetria, eventos de bloco/PoR e usa SignalR para atualizacao em tempo real

Na pratica, isso mostra duas frentes de UI: uma mais institucional/operacional e outra mais focada em monitoramento.

### 10. Testes automatizados

O repositorio possui testes cobrindo:

- value objects de dominio
- aritmetica deterministica
- consistencia do motor LSTM
- bloqueio de smart wallet por limite
- cache Redis com Testcontainers

Ha base de testes real, mas a cobertura ainda esta longe de fechar o sistema ponta a ponta.

## Limitacoes e Pontos Incompletos

Esta e a parte mais importante para interpretar corretamente o repositorio.

### Funcionalidades ainda parciais ou mockadas

- `src/Aethos.Application/Program.cs` ainda e o template padrao de weather forecast do ASP.NET Core.
- `src/Aethos.Presentation.Admin/Program.cs` tambem permanece como template basico.
- O controller RPC retorna resultados mockados para metodos como `eth_sendRawTransaction`, `eth_chainId` e `aethos_getpor`.
- O `Sequencer` forja hashes pseudoaleatorios e descreve o pipeline real apenas em comentarios.
- O `ConsensusService` aprova blocos imediatamente com voto simulado e assinatura falsa.
- O `RocksDbHealthCheck` sempre considera o banco saudavel sem validar I/O real.
- O `StateRootPublisher` publica dados ficticios periodicamente.
- O `L1BridgeClient` usa placeholders de ABI, chave privada e endereco de contrato.
- O contrato `EscapeHatch.sol` ainda deixa a verificacao de prova e a transferencia efetiva como scaffolding.

### Persistencia nao corresponde ao nome atual

Apesar da nomenclatura indicar RocksDB, a implementacao atual em `RocksDbStore` usa um `ConcurrentDictionary<string, byte[]>` em memoria. Ou seja:

- nao ha persistencia real em RocksDB
- nao ha sobrevivencia de dados entre reinicios
- o nome da classe sugere uma maturidade maior do que a implementacao entrega

### Compatibilidade Ethereum ainda incompleta

O projeto fala em compatibilidade com MetaMask e RPC estilo Ethereum, mas:

- os metodos RPC implementados sao poucos
- respostas ainda sao simuladas
- o `chainId` usado no frontend esta inconsistente com o valor retornado pelo controller
- o `wallet_addEthereumChain` usa um `chainId` hexadecimal invalido para o padrao Ethereum (`0xAE7H05`)

### CI e operacao local precisam ajustes

O workflow em `src/.github/workflows/ci.yml` usa caminhos absolutos Windows como `d:/D.AI-Chain/...`, mas roda em `ubuntu-latest`. Do jeito que esta:

- a pipeline nao e portavel
- o CI provavelmente falha fora do ambiente local em que foi escrito

### Repositorio contem artefatos gerados

Ha varios artefatos versionados que normalmente nao deveriam estar no Git:

- `bin/`
- `obj/`
- `node_modules/`
- arquivos de IDE

Isso aumenta ruido de manutencao e dificulta distinguir codigo-fonte real de output de build.

### Cobertura funcional desigual

- `Aethos.Tests.Unit` e `Aethos.Tests.Integration` tem conteudo util.
- `Aethos.Tests.Performance` ainda esta praticamente vazio.
- parte do dashboard principal esta mais demonstrativa do que acoplada a APIs de verdade.

## O Que Ja Funciona Melhor Hoje

Se fosse resumir o que esta mais solido no estado atual, eu destacaria:

- separacao arquitetural dos modulos
- modelagem de dominio e tipos de valor
- aritmetica deterministica em `Aethos.Math.FixedPoint`
- implementacao base do motor LSTM e da geracao de PoR
- cache Redis com teste de integracao real
- estrutura do no ASP.NET Core central
- presenca de contratos L1 coerentes com a proposta do sistema

## O Que Ainda E Principalmente Conceitual

- finalizacao de blocos em rede distribuida real
- persistencia RocksDB real
- bridge L1/L2 funcional
- compatibilidade JSON-RPC mais ampla
- governanca operacional integrada de ponta a ponta
- fluxo completo entre inferencia, consenso, persistencia, bridge e dashboards
- CI verdadeiramente executavel em ambiente padrao GitHub Actions

## Como Executar Localmente

### Requisitos

- .NET SDK 8
- Node.js 20 ou superior
- Docker Desktop
- Redis local se quiser subir partes que dependem dele fora dos testes

### Backend principal

O ponto de entrada mais relevante hoje e `src/Aethos.Node`.

```powershell
cd src
dotnet run --project Aethos.Node
```

Endpoints esperados no estado atual:

- `http://localhost:5000/health`
- `http://localhost:5000/metrics`
- JSON-RPC na raiz HTTP
- hubs SignalR expostos pelo no

### Dashboard Vue principal

```powershell
cd src/Aethos.Presentation.Dashboard
npm install
npm run dev
```

### Dashboard alternativo

```powershell
cd src/Aethos.UI.Dashboard
npm install
npm run dev
```

### Docker Compose

Existem arquivos `src/docker-compose.yml` e `src/docker-compose.infra.yml`, mas eles devem ser tratados como base inicial de orquestracao, nao como ambiente de producao pronto.

## Como Validar

### Testes .NET

```powershell
cd src
dotnet test Aethos.Tests.Unit/Aethos.Tests.Unit.csproj
dotnet test Aethos.Tests.Integration/Aethos.Tests.Integration.csproj
```

### Teste de rotas via Python

Existe um script util em `src/test_routes.py` para validar:

- `/health`
- `/metrics`
- alguns metodos JSON-RPC

```powershell
cd src
python test_routes.py
```

Observacao: esse script assume o no rodando localmente em `http://localhost:5000`.

## Resumo Honesto

O D.AI-Chain ja tem uma base tecnica interessante para uma L2 experimental com elementos de IA auditavel, consenso, governanca e bridge. O valor atual do repositorio esta mais na arquitetura, nos conceitos implementados e nos blocos funcionais isolados do que em uma plataforma completa pronta para uso real.

Em outras palavras:

- e um repositorio com direcao tecnica clara
- ja possui modulos e testes que mostram intencao de engenharia real
- ainda depende de bastante consolidacao para sair de POC e virar um sistema operacional completo

## Recomendacoes Imediatas

Se a proxima etapa do projeto for consolidacao tecnica, as prioridades mais claras seriam:

1. remover artefatos gerados do versionamento e criar um `.gitignore` adequado
2. tornar a CI portavel e executavel no GitHub Actions
3. substituir a persistencia fake por RocksDB real
4. transformar o RPC mockado em implementacao funcional
5. alinhar dashboards, SignalR e endpoints reais
6. preencher as camadas ainda em template (`Aethos.Application` e `Aethos.Presentation.Admin`)
