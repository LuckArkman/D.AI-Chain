# Sprint 17: EVM: Commit-Reveal Anti-MEV (Fase 1)

**Descrição**: Proteção do mempool.

## Pacotes/Ferramentas Inclusas
- *(Nenhuma dependência externa - Camada Pura)*

## Classes/Objetos a Implementar
- `CommitRevealEngine`

## Detalhamento Técnico Minucioso
Salvar Hashes comutáveis (TransactionData || Salt) para evitar front-running. Exigirá TTL de 256 blocos.

---
*Gerado como parte da especificação arquitetural abrangente Aethos Ledger do ciclo Abril/2026.*
