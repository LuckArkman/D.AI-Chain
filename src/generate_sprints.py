import os
import json

sprints_dir = r"d:\D.AI-Chain\src\Sprints"
if not os.path.exists(sprints_dir):
    os.makedirs(sprints_dir)

sprints = [
    {
        "id": 1,
        "title": "Configuração Base e Ambiente",
        "description": "Configuração inicial da Solution e diretórios de Clean Architecture.",
        "packages": "['.NET 8 SDK', 'Testcontainers', 'xUnit']",
        "classes": "['Directory.Packages.props', 'docker-compose.infra.yml']",
        "details": "Scaffold da Solution AethosLedger.sln. Criação dos projetos base vazios para Domain, Math, Persistence, EVM, AI, Network, Consensus, Governance, SmartContracts, Application, Cache, Bridge, RPC, Admin e SDK."
    },
    {
        "id": 2,
        "title": "Domain: Value Objects",
        "description": "Implementação dos Value Objects cruciais para a rede.",
        "packages": "[]",
        "classes": "['ContractAddress', 'BlockHash', 'ResultHash', 'HardwareThreshold']",
        "details": "Validações internas de wrappers (ex: ContractAddress validando '0x' + 40 hex). Escrita de testes unitários para confirmar invariantes do domínio."
    },
    {
        "id": 3,
        "title": "Domain: Interfaces Core",
        "description": "Definição das Interfaces primárias sem dependência externa.",
        "packages": "[]",
        "classes": "['IAiContract', 'ISmartWallet', 'ITransaction', 'IBlock', 'IValidator', 'IStateDb']",
        "details": "Elaboração de contratos puros. IAiContract conterá a assinatura de ExecuteInferenceAsync."
    },
    {
        "id": 4,
        "title": "Domain: Entities e Exceptions",
        "description": "Estágio final de Domain com mapeamento das Entidades.",
        "packages": "[]",
        "classes": "['AiContractEntity', 'BlockEntity', 'TransactionEntity', 'AiContractExecutionException']",
        "details": "Conclusão das Entidades da rede e Mapeamento de Exceções de Domínio que serão devolvidas pela Camada Application."
    },
    {
        "id": 5,
        "title": "Math: Foundation FixedPointInt128",
        "description": "Construção do wrapper determinístico principal Int128.",
        "packages": "['BenchmarkDotNet']",
        "classes": "['FixedPointInt128']",
        "details": "Implementação Q20.44. Adição e subtração. Prevenção de perda fracionária."
    },
    {
        "id": 6,
        "title": "Math: Operadores Avançados e Overflow Guard",
        "description": "Multiplicação trigada para Float sem perda de determinismo.",
        "packages": "[]",
        "classes": "['FixedPointInt128', 'Int128OverflowException']",
        "details": "CheckedAdd e CheckedMultiply. Adicionar cobertura de testes para simular limite do Q20.44 excedente."
    },
    {
        "id": 7,
        "title": "Math: Vetores e Matrizes Determinísticos",
        "description": "Estruturas de Dados Matriciais Fixas.",
        "packages": "[]",
        "classes": "['FixedPointVector', 'FixedPointMatrix']",
        "details": "Implementação do MatVecMul, essencial para inferência neural. Serialização para rocksdb em blob array."
    },
    {
        "id": 8,
        "title": "Math: Activation Look-Up Tables (LUT)",
        "description": "Tabela de indexação constante para funções Sigmoid e Tanh.",
        "packages": "[]",
        "classes": "['ActivationLUT']",
        "details": "Geração do _tanhLUT e _sigmoidLUT com 65536 entradas estáticas. Hash de validação de tabela."
    },
    {
        "id": 9,
        "title": "Persistence: RocksDB Setup",
        "description": "Configuração fundamental de persistência local da rede.",
        "packages": "['RocksDb', 'MessagePack']",
        "classes": "['RocksDbStateDb', 'RocksDbOptionsBuilder']",
        "details": "Criação de Column Family Handles para accounts, blocks, transactions, ai_contracts."
    },
    {
        "id": 10,
        "title": "Persistence: State Commit e Atomicidade",
        "description": "Processamento e salvamento em batch.",
        "packages": "[]",
        "classes": "['RocksDbStateDb', 'StateSnapshot']",
        "details": "WriteBatch do Rocksdb para comitar TransactionEntity e Update no AccountState sem estados parciais."
    },
    {
        "id": 11,
        "title": "Persistence: Merkle State Root",
        "description": "Implementação nativa do Cálculo Merkle",
        "packages": "['BouncyCastle.Cryptography']",
        "classes": "['MerkleStateRoot']",
        "details": "Cálculo da Trie simplificada para Root Hashing exigido pela Bridge da Layer 1 (Ethereum)."
    },
    {
        "id": 12,
        "title": "Persistence: Event Logs DB",
        "description": "Gravação de traces e PoR em base auxiliar permanente",
        "packages": "['MessagePack']",
        "classes": "['EventLogEntry', 'ActivationTraceEntity']",
        "details": "Separar column family exclusiva para Logs gigantes de Metadados de Decisão (PoR). Evita inchaço do Block DB."
    },
    {
        "id": 13,
        "title": "EVM: Configuração Base Nethereum",
        "description": "Acoplamento do interpretador Ethereum.",
        "packages": "['Nethereum.EVM', 'Nethereum.Web3']",
        "classes": "['AethosEvm']",
        "details": "Execução in-process da Layer 2 para Opcodes genéricos de envio de ETH e Tokens ERC-20."
    },
    {
        "id": 14,
        "title": "EVM: Assinaturas e Decodificação",
        "description": "Sistemas de conversão ABI do Ethereum.",
        "packages": "['Nethereum.ABI', 'Nethereum.Signer']",
        "classes": "['AethosEvm', 'TransactionDecoder']",
        "details": "Decodificação de transação Raw `eth_sendRawTransaction`. Validação de secp256k1 Signature Ethereum."
    },
    {
        "id": 15,
        "title": "EVM: Opcodes Aethos (L2)",
        "description": "Integração customizada para Inteligência Artificial.",
        "packages": "[]",
        "classes": "['AethosOpcode']",
        "details": "Mapeamento dos códigos 0xF0 (AETHOS_INFER) a 0xF3 (AETHOS_REVEAL) para chamar a Célula LSTM da L2."
    },
    {
        "id": 16,
        "title": "EVM: Dynamics Gas Calculation",
        "description": "Custo de computação dinâmico e burn de $AETH.",
        "packages": "[]",
        "classes": "['GasCalculator']",
        "details": "EstimateAiFee combinando Base Cost + Penalty pelas Camadas Ocultas (0.3x)."
    },
    {
        "id": 17,
        "title": "EVM: Commit-Reveal Anti-MEV (Fase 1)",
        "description": "Proteção do mempool.",
        "packages": "[]",
        "classes": "['CommitRevealEngine']",
        "details": "Salvar Hashes comutáveis (TransactionData || Salt) para evitar front-running. Exigirá TTL de 256 blocos."
    },
    {
        "id": 18,
        "title": "AI: Célula Unitária LSTM",
        "description": "A célula núcleo de matemática da LSTM.",
        "packages": "[]",
        "classes": "['LstmCell', 'LstmCellOutput']",
        "details": "Forget, Input e Output Gates utilizando o nosso FixedPointMatrix, resolvendo com LUT."
    },
    {
        "id": 19,
        "title": "AI: Network Assembly 4-Layers",
        "description": "Pipeline serializado das 4 camadas congeladas.",
        "packages": "[]",
        "classes": "['LstmNetwork']",
        "details": "Passando o Hidden State como input sequencial por 4 Cells consecutivas no LstmNetwork."
    },
    {
        "id": 20,
        "title": "AI: Geração do Activation Trace",
        "description": "As trilhas para explicabilidade auditável.",
        "packages": "['MessagePack']",
        "classes": "['ActivationTrace']",
        "details": "Cada LstmCell arquiva no LstmCellOutput os pesos influentes e estado interno com Timestamp."
    },
    {
        "id": 21,
        "title": "AI: Proof of Reasoning",
        "description": "Compactação e Hash da prova auditável.",
        "packages": "['HashLib4CSharp']",
        "classes": "['ProofOfReasoning']",
        "details": "Aethos gera o ResultHash da rede em Keccak256 sobre a resposta probabilística da softmax final."
    },
    {
        "id": 22,
        "title": "AI: Importador ONNX Model",
        "description": "Conversor float-to-Int128 para Cientistas de Dados.",
        "packages": "['Microsoft.ML.OnnxRuntime']",
        "classes": "['LstmNetwork']",
        "details": "Método ImportFromOnnx para receber um modelo gerado no PyTorch e consolidá-lo no RocksDB como Aethos Bytecode."
    },
    {
        "id": 23,
        "title": "RPC: Servidor Base e Kestrel",
        "description": "Listener Público da Blockchain.",
        "packages": "['StreamJsonRpc', 'Nethereum.JsonRpc.Client']",
        "classes": "['JsonRpcServerExtensions', 'JsonRpcProcessor']",
        "details": "Pipeline ASP.NET de JsonRpc processual com Rate Limiting e Endpoint Mapping /"
    },
    {
        "id": 24,
        "title": "RPC: Web3 Standard Methods",
        "description": "Endpoints para compatibilidade total MetaMask.",
        "packages": "[]",
        "classes": "['EthMethodRouter']",
        "details": "eth_chainId, eth_blockNumber, eth_getBalance, eth_estimateGas e eth_getTransactionCount."
    },
    {
        "id": 25,
        "title": "RPC: Endpoints Específicos $AETH",
        "description": "Recursos fechados do Ledger Determinístico.",
        "packages": "[]",
        "classes": "['AethosMethodRouter']",
        "details": "aethos_getActivationTrace e aethos_getPoR acionados direto pelo Kestrel."
    },
    {
        "id": 26,
        "title": "RPC: WebSockets Subscriptions",
        "description": "Streaming real-time de blocos Ethereum-like.",
        "packages": "[]",
        "classes": "['WebSocketSubscriptionManager']",
        "details": "eth_subscribe acoplado à injeção de dependência para notificar novas Heads à MetaMask."
    },
    {
        "id": 27,
        "title": "Consensus: Network & Protobufs",
        "description": "Estruturação dos nós P2P.",
        "packages": "['Grpc.AspNetCore', 'Google.Protobuf']",
        "classes": "['ConsensusService']",
        "details": "Criação do consensus.proto. Envio de BlockProposal e recebimento de BlockVote."
    },
    {
        "id": 28,
        "title": "Consensus: Sequencer Worker",
        "description": "O executor mestre de blocos.",
        "packages": "['Microsoft.Extensions.Hosting']",
        "classes": "['Sequencer']",
        "details": "O Nó Principal (Sequencer) faz um loop a cada tempo alvo extraindo transações do Mempool para processá-las em massa."
    },
    {
        "id": 29,
        "title": "Consensus: State-Root Rollup",
        "description": "Otimizando a validação leve (Light Nodes).",
        "packages": "[]",
        "classes": "['RollupValidator']",
        "details": "Design dos Lightweight Validators para assinar blocos apenas com o State Root do Sequencer ou exigindo recálculo total em caso desonesto."
    },
    {
        "id": 30,
        "title": "Consensus: Divergence Guard",
        "description": "Mecanismo penal de execução da rede Neural.",
        "packages": "[]",
        "classes": "['DivergenceGuard', 'DivergenceResult']",
        "details": "Compara localmente o ResultHash. Caso dê disparidade, abre evidência cryptográfica de trapaça (Jailing automatico)."
    },
    {
        "id": 31,
        "title": "Consensus: Slashing de Infraestrutura",
        "description": "Sanções ativas sob penalidades P2P.",
        "packages": "[]",
        "classes": "['SlashingService']",
        "details": "Perda de fundos configuracional sobre nós que falharem as 4 horas de uptime ou fraudarem o Root Hash."
    },
    {
        "id": 32,
        "title": "SmartContracts: Account Registry",
        "description": "Indexador de carteiras do ledger.",
        "packages": "['Nethereum.Contracts']",
        "classes": "['AccountRegistry']",
        "details": "O mapeamento nativo Address -> Type (SmartWallet, AiWallet, ExternallyOwnedAccount)."
    },
    {
        "id": 33,
        "title": "SmartContracts: ERC-4337 Foundation",
        "description": "Account Abstraction Base.",
        "packages": "['Nethereum.ERC4337']",
        "classes": "['UserOperationHandler']",
        "details": "Delega chamadas baseadas no ERC-4337 encapsulado para paymasters da Aethos."
    },
    {
        "id": 34,
        "title": "SmartContracts: AI Wallet e Threshoold",
        "description": "Carteira gerenciada por Inteligencia Artificial.",
        "packages": "[]",
        "classes": "['AiWalletLogic']",
        "details": "Verificador GuardianThreshold. Liberação automática de transação por Inference (AETHOS_INFER)."
    },
    {
        "id": 35,
        "title": "SmartContracts: Panic Button e Override",
        "description": "Retenção soberana humana.",
        "packages": "[]",
        "classes": "['PanicButtonExecute']",
        "details": "Função especial de revogação de todas as permissões da IAM Model Wallet em uma única transação de mempool prioritário."
    },
    {
        "id": 36,
        "title": "Governance: Controle do Control Plane",
        "description": "Backend para uso do super admin.",
        "packages": "[]",
        "classes": "['GovernanceService']",
        "details": "Permitir EmergencyPauseAiContractsAsync apenas ao Admin oficial na RootDB e suspensão temporária de inferência."
    },
    {
        "id": 37,
        "title": "Governance: Shadow Mode Deploy",
        "description": "Execução A/B segura no blockchain L2.",
        "packages": "[]",
        "classes": "['ShadowModeManager']",
        "details": "Roda nova versão dos Weights do LSTM mas as execuções terminam apenas em logging simulado não processado via AethosEvm."
    },
    {
        "id": 38,
        "title": "Governance: Votação On-Chain ($AETH)",
        "description": "PoS com peso ponderado.",
        "packages": "[]",
        "classes": "['GovernanceVoting']",
        "details": "Atualização das variáveis paramétricas da Base Fee pela aprovação da maioria percentual ($AETH Stakes)."
    },
    {
        "id": 39,
        "title": "Application: CQRS Pipelines",
        "description": "Setup MediatR.",
        "packages": "['MediatR', 'AutoMapper']",
        "classes": "['ProcessTransactionCommand', 'ProcessTransactionHandler']",
        "details": "Montagem dos fluxos Mediator para coordenar Kestrel, EVM e RocksDb."
    },
    {
        "id": 40,
        "title": "Application: FluentValidation Integrado",
        "description": "Validador de input estrito do JSON-RPC antes de tocar EVM.",
        "packages": "['FluentValidation']",
        "classes": "['ValidationBehavior']",
        "details": "Rejeição sumária com Payload Error limpo contra scripts e bots ruins."
    },
    {
        "id": 41,
        "title": "Application: Policies de Resiliência (Polly)",
        "description": "Retry comportamental para I/O RocksDB/Cache.",
        "packages": "['Polly']",
        "classes": "['RetryBehavior']",
        "details": "Caso RocksDB acuse Timeout/ReadBlock, refazer query 3 vezes até ceder."
    },
    {
        "id": 42,
        "title": "Application: AI Guardrail Behaviors",
        "description": "Muro protetor na pipeline do Mediator.",
        "packages": "[]",
        "classes": "['AiContractGuardBehavior']",
        "details": "Antes de aprovar ProcessTransactionCommand, intercepta e paralisa se status=Paused na Rocksdb Governance."
    },
    {
        "id": 43,
        "title": "Cache: Instalação Redis Memoization",
        "description": "Cache em memória para evitar redudância.",
        "packages": "['StackExchange.Redis']",
        "classes": "['InferenceCache']",
        "details": "Calcula Cache Key com Hash of (Payload + TxData) devolvendo DecisionResult imediatamente do Redis na rede local."
    },
    {
        "id": 44,
        "title": "Cache: Invalidação de Shadow Mode",
        "description": "Limpeza do pub/sub redis por versão.",
        "packages": "['StackExchange.Redis.Extensions.MsgPack']",
        "classes": "['InferenceCache']",
        "details": "Comandos LUA via redis multiplexer rodando limpeza total quando patch update de AI Model for deferido."
    },
    {
        "id": 45,
        "title": "Bridge: Aethos Bridge Smart Contract",
        "description": "Solidity L1 Contract Finality.",
        "packages": "[]",
        "classes": "['AethosBridge.sol']",
        "details": "Contrato deployado na Sepolia/Ethereum via Nethereum com funcao PublishStateRoot restrita ao relayer da Layer 2."
    },
    {
        "id": 46,
        "title": "Bridge: Forced Withdrawal e Merkle",
        "description": "Escape Hatch na Ethereum.",
        "packages": "[]",
        "classes": "['AethosBridge.sol', 'EscapeHatch.sol']",
        "details": "Funcao Solidity initiateWithdrawal validada pelo array de hashes root provando a posessão de saques presos."
    },
    {
        "id": 47,
        "title": "Bridge: .NET State Publisher Job",
        "description": "Hosted Service do Relayer L2 para L1.",
        "packages": "['Microsoft.Extensions.Hosting']",
        "classes": "['StateRootPublisher']",
        "details": "Um job assíncrono rodando de 1h/1h para enviar Hash consolidado Root Ethereum Mainnet usando infra-chave Secp256."
    },
    {
        "id": 48,
        "title": "Public RPC: Observabilidade Padrão",
        "description": "Camada Métrica para Promethues e Gráficos.",
        "packages": "['OpenTelemetry.Exporter.Prometheus.AspNetCore', 'Serilog.Sinks.OpenTelemetry']",
        "classes": "['Program.cs']",
        "details": "Exportar throughput de RPC Calls com Grafana Exporter injetado na DI Container (.NET 8 OTEL)."
    },
    {
        "id": 49,
        "title": "Public RPC: Health Check e Throttling",
        "description": "Proteções de fronteira API.",
        "packages": "['Microsoft.Extensions.Diagnostics.HealthChecks', 'Swashbuckle.AspNetCore']",
        "classes": "['Program.cs', 'RocksDbHealthCheck']",
        "details": "Limitação global de RPS Kestrel e health endpoints nativos com Swagger para aethos-* actions."
    },
    {
        "id": 50,
        "title": "Admin RPC: Painel WebSockets SignalR",
        "description": "APIs interativas ao painel do Admin L2.",
        "packages": "['Microsoft.AspNetCore.SignalR.StackExchangeRedis']",
        "classes": "['NetworkHealthHub', 'TransactionFeedHub']",
        "details": "Streaming do log em tempo real `HealthUpdate` enviando JSON a cada 1000ms do estado Consensus Divergence Rate."
    },
    {
        "id": 51,
        "title": "Admin RPC: gRPC JWT Authorization",
        "description": "Isolamento Zero-knowledge Server.",
        "packages": "['Microsoft.AspNetCore.Authentication.JwtBearer']",
        "classes": "['Program.cs']",
        "details": "Ativar regras Restritas [Authorize(Roles='Admin')] em Hub SignalR para Controle e Emergency Pauses, ignorando chaves de usuario."
    },
    {
        "id": 52,
        "title": "Admin RPC: Network Background Pusher",
        "description": "Disparador de métricas proativo.",
        "packages": "[]",
        "classes": "['NetworkHealthBroadcaster']",
        "details": "Service Worker infinito colhendo dados de RocksDB + CPU + Validators list → Emite no grupo Todos via HubContext."
    },
    {
        "id": 53,
        "title": "SDK: Pacote para Devs",
        "description": "Kit de integração para terceiros.",
        "packages": "['Nethereum.Web3']",
        "classes": "['AethosClient']",
        "details": "DeployAiContractAsync e SimulateInferenceAsync formatados num pacote NuGet e JS. Simplificam chamadas remotas de RPC."
    },
    {
        "id": 54,
        "title": "SDK: Aethos Compiler Toolchain",
        "description": "O utilitario que compila para fixed point bytecode.",
        "packages": "[]",
        "classes": "['AethosContractCompiler']",
        "details": "Classe compiladora do arquivo base ONNX que expõe validações fixas da LSTM limitando neurons size < Governance Cap."
    },
    {
        "id": 55,
        "title": "Frontend: Vue 3 Setup & Vite",
        "description": "Dashboard Web de gerenciamento.",
        "packages": "['vue', 'vite', 'pinia', 'vue-router']",
        "classes": "['package.json', 'router/index.ts', 'stores/networkStore.ts']",
        "details": "Pinia Network state manager controlando as visualizações de Nodes e Latências de Consenso e Variáveis estáticas."
    },
    {
        "id": 56,
        "title": "Frontend: MetaMask ethers.js Integration",
        "description": "Login Auth e Web3 Actions.",
        "packages": "['ethers', '@metamask/sdk']",
        "classes": "['useMetaMask.ts']",
        "details": "Hook de conexão com metamask alterando programmaticamente RPC Chain `0xAE7H05`. WalletDashboard Views criadas."
    },
    {
        "id": 57,
        "title": "Frontend: Gráficos de Network e PoR (D3.js)",
        "description": "Visual Data Analysis Audit",
        "packages": "['chart.js', 'd3']",
        "classes": "['DecisionGraph.vue', 'NetworkHealth.vue']",
        "details": "Renderização do D3JS das 4 Layers da LSTM (Explicability de Neuronio atômico). Atualização na GUI por eventos SignalR."
    },
    {
        "id": 58,
        "title": "Frontend: Shadow Mode & Panic UX",
        "description": "Operacional Admin e User UI.",
        "packages": "['@headlessui/vue', '@heroicons/vue']",
        "classes": "['EmergencyControls.vue', 'GuardianConfig.vue']",
        "details": "Admin painel (Suspender, Rollback, Deploy A-B model comparison). Painel do usuário (Panic Threshold Limits, Override Manual Modal)."
    },
    {
        "id": 59,
        "title": "Quality: Suite de Teste Unitario Massivo 1",
        "description": "TDD forçado no Domain e Core Math",
        "packages": "['xunit', 'FluentAssertions', 'AutoFixture']",
        "classes": "['FixedPointInt128Tests', 'LstmCellTests']",
        "details": "Testes paralelos verificando convergência hash. 98% coverage requirement strict enforced with FluentAssertions."
    },
    {
        "id": 60,
        "title": "Quality: Suite de Teste Unitario Massivo 2",
        "description": "TDD forçado no Consenso e EVM",
        "packages": "['Moq', 'Bogus']",
        "classes": "['SequencerTests', 'DivergenceGuardTests']",
        "details": "Mocking e Simulação do Nethereum EVM Opcode Injection (Ockam Razor logic mocks). Slashing falsos positivos."
    },
    {
        "id": 61,
        "title": "Quality: Integration e Benchmark Tests",
        "description": "Execução completa de L2 -> DB.",
        "packages": "['Testcontainers', 'Testcontainers.Redis']",
        "classes": "['AethosFullIntegrationTests']",
        "details": "MetaMask Fake → RPC → EVM → LSTM execution → RocksDB Commit validation. Setup Testcontainers para run Dockerized em Pipeline Git."
    },
    {
        "id": 62,
        "title": "CI/CD: GitHub Actions Pipeline",
        "description": "Entrega Contínua Segura do Aethos.",
        "packages": "[]",
        "classes": "['ci.yml']",
        "details": "Job Runner configurando Testes .NET E2E, Frontend playwright UI E2E, Docker Image Build step Push e CodeCov validation (90%+ guard)."
    },
    {
        "id": 63,
        "title": "Security: SAST, Slither e Pentest",
        "description": "Auditoria Automatizada Fria.",
        "packages": "['SecurityCodeScan.VS2019', 'SonarAnalyzer.CSharp']",
        "classes": "['slither-analyzer', 'OWASP ZAP']",
        "details": "Scans no backend (Int Overflow, Null Pointers). Slither correndo sobre o Solidity Bridge Contract preventivamente contra Reentrancy Attacks."
    },
    {
        "id": 64,
        "title": "Testnet: Bootstrapping P2P Nodes e Faucet",
        "description": "Rede publicamente acessável Beta.",
        "packages": "[]",
        "classes": "['AethosFaucetBot']",
        "details": "5 Nodes em Data Centers distribuídos para testnet RPC publica. Faucet Telegram bot que dispensa 100 $AETH."
    },
    {
        "id": 65,
        "title": "Auditoria Final e Rollup Mainnet",
        "description": "Congelamento de código L2/L1 e Go Live.",
        "packages": "[]",
        "classes": "['$AETH Token Contract']",
        "details": "Publicação definitiva da Genesis L2 block e TGE (Token Generation Event). Onboarding de novos Validators PoS."
    }
]

for sprint in sprints:
    filename = f"Sprint_{sprint['id']:02d}.md"
    filepath = os.path.join(sprints_dir, filename)
    with open(filepath, "w", encoding="utf-8") as f:
        f.write(f"# Sprint {sprint['id']:02d}: {sprint['title']}\n\n")
        f.write(f"**Descrição**: {sprint['description']}\n\n")
        f.write("## Pacotes/Ferramentas Inclusas\n")
        packages = eval(sprint['packages'])
        if packages:
            for pkg in packages:
                f.write(f"- `{pkg}`\n")
        else:
            f.write("- *(Nenhuma dependência externa - Camada Pura)*\n")
        f.write("\n")
        f.write("## Classes/Objetos a Implementar\n")
        classes = eval(sprint['classes'])
        for cls in classes:
            f.write(f"- `{cls}`\n")
        f.write("\n")
        f.write("## Detalhamento Técnico Minucioso\n")
        f.write(sprint['details'] + "\n")
        f.write("\n---\n*Gerado como parte da especificação arquitetural abrangente Aethos Ledger do ciclo Abril/2026.*\n")

print(f"{len(sprints)} sprints gerados com sucesso no diretorio: {sprints_dir}")
