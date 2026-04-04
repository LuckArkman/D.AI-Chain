# Sprint 41: Application: Policies de Resiliência (Polly)

**Descrição**: Retry comportamental para I/O RocksDB/Cache.

## Pacotes/Ferramentas Inclusas
- `Polly`

## Classes/Objetos a Implementar
- `RetryBehavior`

## Detalhamento Técnico Minucioso
Caso RocksDB acuse Timeout/ReadBlock, refazer query 3 vezes até ceder.

---
*Gerado como parte da especificação arquitetural abrangente Aethos Ledger do ciclo Abril/2026.*
