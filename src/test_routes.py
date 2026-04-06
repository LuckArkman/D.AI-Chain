import requests
import json
import time
import sys

# Configuração da URL do nó Aethos (Docker)
BASE_URL = "http://localhost:5000"
RPC_URL = BASE_URL
HEALTH_URL = f"{BASE_URL}/health"
METRICS_URL = f"{BASE_URL}/metrics"
HUB_HEALTH = f"{BASE_URL}/hubs/health"

tests_run = 0
tests_failed = 0

def log_test(name, success, response=None, error=None):
    global tests_run, tests_failed
    tests_run += 1
    status = "SUCCESS" if success else "FAILED"
    print(f"[{status}] {name}")
    if not success:
        tests_failed += 1
        if response: print(f"      Response: {response}")
        if error: print(f"      Error: {error}")

def test_health_endpoint():
    try:
        r = requests.get(HEALTH_URL, timeout=10)
        log_test("Health Check Endpoint (/health)", r.status_code == 200, r.text)
    except Exception as e:
        log_test("Health Check Endpoint (/health)", False, error=str(e))

def test_metrics_endpoint():
    try:
        r = requests.get(METRICS_URL, timeout=10)
        log_test("Metrics Endpoint (/metrics)", r.status_code == 200, r.text[:50] + "...")
    except Exception as e:
        log_test("Metrics Endpoint (/metrics)", False, error=str(e))

def test_json_rpc_method(method, params=[]):
    payload = {
        "jsonrpc": "2.0",
        "id": tests_run + 1,
        "method": method,
        "params": params
    }
    try:
        r = requests.post(RPC_URL, json=payload, timeout=10)
        data = r.json()
        success = r.status_code == 200 and "result" in data
        log_test(f"JSON-RPC: {method}", success, data)
    except Exception as e:
        log_test(f"JSON-RPC: {method}", False, error=str(e))

def run_all_tests():
    print("--- INICIANDO BATERIA DE TESTES DAS ROTAS AETHOS LEDGER ---")
    
    # 1. Testes de Infraestrutura / Health
    test_health_endpoint()
    test_metrics_endpoint()
    
    # 2. Testes JSON-RPC (Standard Web3)
    test_json_rpc_method("web3_clientVersion")
    test_json_rpc_method("eth_chainId")
    test_json_rpc_method("eth_blockNumber")
    test_json_rpc_method("eth_getBalance", ["0x1234567890123456789012345678901234567890", "latest"])
    
    # 3. Testes JSON-RPC (Aethos Specific)
    test_json_rpc_method("aethos_getPoR", [1])
    test_json_rpc_method("aethos_getActivationTrace", ["0xtxhash123"])

    print("---------------------------------------------------------")
    print(f"RESUMO: {tests_run} testes executados, {tests_failed} falhas.")
    if tests_failed > 0:
        sys.exit(1)

if __name__ == "__main__":
    # Aguardar o nó subir no Docker
    print("Aguardando o serviço Aethos-Node estabilizar no Docker...")
    time.sleep(15) 
    run_all_tests()
