import requests
import json
import time

BASE_URL = "http://localhost:5097/api"

def print_result(route, res):
    print(f"--- TEST: {route} ---")
    print(f"STATUS CODE: {res.status_code}")
    try:
        print(f"RESPONSE FORMAT RESULT: \n{json.dumps(res.json(), indent=2)}\n")
    except:
        print(f"TEXT: {res.text}\n")

def run_tests():
    print("Iniciando bateria de testes das rotas REST...\n" + "="*50)

    # ==========================
    # 1. AI Wallet Controller
    # ==========================
    print(">>> 1. AI Wallet Controller")
    res = requests.post(f"{BASE_URL}/ai-wallet/create")
    print_result("POST /ai-wallet/create", res)
    wallet_address = res.json().get('address') if res.status_code == 200 else "0xMock"

    res = requests.get(f"{BASE_URL}/ai-wallet/{wallet_address}/balance")
    print_result(f"GET /ai-wallet/{wallet_address}/balance", res)

    res = requests.post(f"{BASE_URL}/ai-wallet/transfer", json={"from": wallet_address, "to": "0xDest", "amount": "0.5"})
    print_result("POST /ai-wallet/transfer", res)

    res = requests.post(f"{BASE_URL}/ai-wallet/deploy", json={"ownerAddress": wallet_address})
    print_result("POST /ai-wallet/deploy", res)

    res = requests.get(f"{BASE_URL}/ai-wallet/{wallet_address}/debug")
    print_result(f"GET /ai-wallet/{wallet_address}/debug", res)

    # ==========================
    # 2. AI Contract Controller
    # ==========================
    print(">>> 2. AI Contract Controller")
    res = requests.post(f"{BASE_URL}/ai-contract/create", json={"contractName": "NeuralNetCore", "neuralModelUri": "ipfs://xyz"})
    print_result("POST /ai-contract/create", res)

    res = requests.post(f"{BASE_URL}/ai-contract/deploy", json={"contractName": "NeuralNetCore", "deployerAddress": wallet_address, "bytecode": "0xABCDEF"})
    print_result("POST /ai-contract/deploy", res)
    contract_address = res.json().get('contractAddress') if res.status_code == 200 else "0xMockSC"

    res = requests.post(f"{BASE_URL}/ai-contract/execute", json={"contractAddress": contract_address, "method": "infer", "payload": [0.1, 0.5, 0.9]})
    print_result("POST /ai-contract/execute", res)

    res = requests.get(f"{BASE_URL}/ai-contract/{contract_address}/debug")
    print_result(f"GET /ai-contract/{contract_address}/debug", res)

    # ==========================
    # 3. Testnet Controller
    # ==========================
    print(">>> 3. Testnet Controller")
    res = requests.get(f"{BASE_URL}/testnet/status")
    print_result("GET /testnet/status", res)

    res = requests.post(f"{BASE_URL}/testnet/faucet", json={"address": wallet_address})
    print_result("POST /testnet/faucet", res)

    res = requests.post(f"{BASE_URL}/testnet/transactions/simulate", json={"from": wallet_address, "to": "0xTesting", "amount": "100.0"})
    print_result("POST /testnet/transactions/simulate", res)

    res = requests.post(f"{BASE_URL}/testnet/contracts/deploy", json={"deployerAddress": wallet_address, "bytecode": "0x00FF00"})
    print_result("POST /testnet/contracts/deploy", res)

    res = requests.post(f"{BASE_URL}/testnet/contracts/execute", json={"contractAddress": "0xMock", "method": "testFunction", "payload": []})
    print_result("POST /testnet/contracts/execute", res)

    # ==========================
    # 4. Mainnet Controller
    # ==========================
    print(">>> 4. Mainnet Controller")
    res = requests.get(f"{BASE_URL}/mainnet/status")
    print_result("GET /mainnet/status", res)

    res = requests.get(f"{BASE_URL}/mainnet/accounts/{wallet_address}/balance")
    print_result(f"GET /mainnet/accounts/{wallet_address}/balance", res)

    res = requests.post(f"{BASE_URL}/mainnet/transactions/transfer", json={"from": wallet_address, "to": "0xRealDest", "amount": "999.50"})
    print_result("POST /mainnet/transactions/transfer", res)

    res = requests.post(f"{BASE_URL}/mainnet/contracts/deploy", json={"deployerAddress": wallet_address, "bytecode": "0xREALDATA"})
    print_result("POST /mainnet/contracts/deploy", res)

    res = requests.post(f"{BASE_URL}/mainnet/contracts/execute", json={"contractAddress": "0xRealContract", "method": "executeMain", "payload": ["param1"]})
    print_result("POST /mainnet/contracts/execute", res)

    # ==========================
    # 5. Asset Controller (Supply Logic Verification)
    # ==========================
    print(">>> 5. Asset Controller")
    res = requests.post(f"{BASE_URL}/asset/create", json={"name": "Aethos Core Token", "symbol": "ACT", "supply": 1000.0})
    print_result("POST /asset/create", res)
    asset_address = res.json().get('contractAddress') if res.status_code == 200 else "0xErr"

    res = requests.get(f"{BASE_URL}/asset/{asset_address}")
    print_result(f"GET /asset/{asset_address}", res)

    print("--- EMITINDO 400 TOKENS (Válido) ---")
    res = requests.post(f"{BASE_URL}/asset/mint", json={"contractAddress": asset_address, "amount": 400.0})
    print_result("POST /asset/mint (400 ACT)", res)

    print("--- EMITINDO 700 TOKENS (Inválido - Ultrapassa limite de 1000 supply) ---")
    res = requests.post(f"{BASE_URL}/asset/mint", json={"contractAddress": asset_address, "amount": 700.0})
    print_result("POST /asset/mint (700 ACT) EXPECT OOVERFLOW ERROR", res)

    print("--- EMITINDO 600 TOKENS (Válido - Crava os 1000 limit) ---")
    res = requests.post(f"{BASE_URL}/asset/mint", json={"contractAddress": asset_address, "amount": 600.0})
    print_result("POST /asset/mint (600 ACT)", res)

    print("--- VERIFICANDO DADO REFLETIDO ---")
    res = requests.get(f"{BASE_URL}/asset/{asset_address}")
    print_result(f"GET /asset/{asset_address}", res)

    print("SUITE DE TESTES CONCLUIDA")

if __name__ == '__main__':
    run_tests()
