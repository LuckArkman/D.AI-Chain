import requests
import json
import traceback

BASE_URL = "http://localhost:5097"
RPC_URL = f"{BASE_URL}/"

def test_rpc_method(method, params=[]):
    payload = {
        "jsonrpc": "2.0",
        "method": method,
        "params": params,
        "id": 1
    }
    print(f"--- POST {method} ---")
    try:
        r = requests.post(RPC_URL, json=payload, timeout=5)
        print(f"Status Code: {r.status_code}")
        response_json = r.json()
        print(f"Response: {json.dumps(response_json, indent=2)}")
        
        if "error" in response_json:
            if method == "eth_nonExistentMethod" and response_json["error"]["code"] == -32601:
                print(f"-> Expected error caught for {method} (Correct Behavior)")
                return True
            print(f"-> ERROR detected for {method}: {response_json['error']}")
            return False
            
    except requests.exceptions.RequestException as e:
        print(f"-> Request FAILED to {method}: {e}")
        return False
    except ValueError:
        print(f"-> Invalid JSON response: {r.text}")
        return False
        
    return True

def run_tests():
    methods = [
        ("web3_clientVersion", []),
        ("eth_chainId", []),
        ("eth_blockNumber", []),
        ("eth_getBalance", ["0x1234567890123456789012345678901234567890", "latest"]),
        ("eth_estimateGas", [{"to": "0x1234567890123456789012345678901234567890"}]),
        ("eth_getTransactionCount", ["0x1234567890123456789012345678901234567890", "latest"]),
        ("aethos_getPoR", ["0xtxa1b2c3d4e5f607182930a1b2c3d4e5f607182930"]),
        ("aethos_getActivationTrace", ["0xtxa1b2c3d4e5f607182930a1b2c3d4e5f607182930"]),
        ("eth_nonExistentMethod", [])
    ]
    
    success_count = 0
    fail_count = 0
    
    for method, params in methods:
        success = test_rpc_method(method, params)
        if success:
            success_count += 1
        else:
            fail_count += 1
            
    print("---------------------------------")
    print(f"TESTS FINISHED: {success_count} SUCCESS, {fail_count} FAILED")

if __name__ == "__main__":
    try:
        # Primeiro, verifica se o Swagger está resolvendo
        print("Testing Swagger JSON...")
        r = requests.get(f"{BASE_URL}/swagger/v1/swagger.json", timeout=5)
        if r.status_code == 200:
            print("Swagger JSON is accessible.")
        else:
            print(f"Swagger JSON FAILED. Status: {r.status_code}")
            
        print("Testing Health Check...")
        r2 = requests.get(f"{BASE_URL}/health", timeout=5)
        print(f"Health Check Status: {r2.status_code} - {r2.text}")
        
    except Exception as e:
        print(f"Swagger/Health test failed: {e}")

    run_tests()
