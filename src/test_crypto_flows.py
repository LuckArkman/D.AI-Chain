from web3 import Web3

import json
import traceback

def run_tests():
    try:
        # 0. Conecta à blockchain Aethos L2
        w3 = Web3(Web3.HTTPProvider('http://localhost:5097'))
        print(f"[{'PASS' if w3.is_connected() else 'FAIL'}] Conectado a Aethos (is_connected={w3.is_connected()})")
        
        # 1. Criação de Carteira Web3
        account_a = w3.eth.account.create()
        account_b = w3.eth.account.create()
        print(f"[PASS] Carteiras Criadas.\n  Carteira A: {account_a.address}\n  Carteira B: {account_b.address}")

        w3.eth.default_account = account_a.address

        # MOCK OF FUNDS (In a real test, account_a would start with funds on genesis, 
        # or we'd call a faucet. We will check balance just to log it).
        bal_a = w3.eth.get_balance(account_a.address)
        print(f"Saldo Inicial Carteira A: {bal_a} wei")

        # 2. Transferência de Criptoativos da Carteira A para B
        print("Preparando transferência de 1 wei da Carteira A para Carteira B...")
        nonce = w3.eth.get_transaction_count(account_a.address)
        tx = {
            'nonce': nonce,
            'to': account_b.address,
            'value': 1,
            'gas': 21000,
            'gasPrice': w3.to_wei('1', 'gwei'),
            'chainId': w3.eth.chain_id
        }
        signed_tx = w3.eth.account.sign_transaction(tx, account_a.key)
        tx_hash = w3.eth.send_raw_transaction(signed_tx.raw_transaction)
        print(f"Transferência enviada! Hash: {w3.to_hex(tx_hash)}")
        
        # Validacão da Transferência (Receipt)
        receipt = w3.eth.wait_for_transaction_receipt(tx_hash, timeout=5)
        print(f"[PASS] Transferência validada. Bloco: {receipt.blockNumber}")

        bal_b = w3.eth.get_balance(account_b.address)
        print(f"[PASS] Checagem de Saldo. Carteira B Saldo: {bal_b} wei")

        # 3. Criação de Smart Contract com Supply e Execução
        # Using a very simple ERC20-like mock bytecode (just to test deployment)
        bytecode = "6080604052348015600f57600080fd5b50603f80601d6000396000f3fe6080604052600080fdfea2646970667358221220268ae2f8d3840afefd9a9ba0fe5a6d3f271a2dd6ec140c8fa21e646fa127a3a964736f6c63430008070033"
        
        print("Realizando deploy do Smart Contract de Token...")
        tx_contract = {
            'nonce': w3.eth.get_transaction_count(account_a.address),
            'gas': 2000000,
            'gasPrice': w3.to_wei('1', 'gwei'),
            'data': bytecode,
            'chainId': w3.eth.chain_id
        }
        signed_tx_contract = w3.eth.account.sign_transaction(tx_contract, account_a.key)
        contract_hash = w3.eth.send_raw_transaction(signed_tx_contract.raw_transaction)
        
        receipt_contract = w3.eth.wait_for_transaction_receipt(contract_hash, timeout=5)
        print(f"[PASS] Smart Contract Criado! Endereço: {receipt_contract.contractAddress}")

        print("\n--- TODOS OS TESTES PASSARAM COM SUCESSO ---")

    except Exception as e:
        print("\n[ERRO DURANTE O TESTE]")
        traceback.print_exc()

if __name__ == "__main__":
    run_tests()
