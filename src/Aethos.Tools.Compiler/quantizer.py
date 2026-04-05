import numpy as np

# Aethos Ledger Q20.44 Fixed Point Config
# (20 bits na parte inteira, 44 na fracionária)
FRACTION_BITS = 44
SCALE = 1 << FRACTION_BITS

def float_to_int128(value: float) -> str:
    """
    Lida com a quebra de determinismo IEEE 754 de CPUs (x86 vs ARM).
    Ao receber um ponto flutuante do PyTorch, converte isso de forma absoluta e cega
    para um BigInteger simulado (string) garantindo 100% de paridade com o Int128 no Backend C#.
    """
    # Escala e força o arredondamento absoluto
    scaled_val = round(value * SCALE)
    # Retorna num formato string para não sofrer distorcoes em dumps de JSON
    return str(scaled_val)

def quantize_tensor(tensor: np.ndarray) -> np.ndarray:
    """
    Achata e itera uma Matriz Nd do SciPy/PyTorch transformando toda a matriz
    para a estrutura limpa Q20.44 do Aethos Mathematical Engine.
    """
    flat = tensor.flatten()
    int_matrix = [float_to_int128(float(val)) for val in flat]
    return np.array(int_matrix, dtype=object).reshape(tensor.shape)

if __name__ == "__main__":
    print("Aethos Quantizer Test\n-----------------------")
    test_val = 1.23456
    print(f"PyTorch Float : {test_val} \nAethos Q20.44 : {float_to_int128(test_val)}")
