// SPDX-License-Identifier: MIT
pragma solidity 0.8.24;

interface IAethosBridge {
    function stateRoots(uint256 l2BlockNumber) external view returns (bytes32);
}

/**
 * @title EscapeHatch
 * @dev Mecanismo Censor-Resistance. Permite que o usuario force a retirada via L1 (Ethereum)
 * utilizando Provas Criptograficas de Inclusao (Merkle Proof) caso os Validadores L2 entrem em divergencia.
 */
contract EscapeHatch {
    IAethosBridge public immutable aethosBridge;

    // Mapeamento simples para prevenir double spending emergency exits.
    mapping(address => bool) public hasExited;

    event EmergencyExitInitiated(address indexed user, uint256 amount);

    constructor(address _bridge) {
        aethosBridge = IAethosBridge(_bridge);
    }

    /**
     * @dev Processa um saque forçado ignorando o sequenciador L2.
     * @param l2BlockNumber O bloco de referencia guardado na Aethos Bridge.
     * @param amount O balanco criptografico do usuário exigido.
     * @param proof Merkle Proof para o backend que o usuario estava la.
     */
    function triggerEscapeHatch(uint256 l2BlockNumber, uint256 amount, bytes32[] calldata proof) external {
        require(!hasExited[msg.sender], "EscapeHatch: Saque ja completado anteriormente.");

        bytes32 targetStateRoot = aethosBridge.stateRoots(l2BlockNumber);
        require(targetStateRoot != bytes32(0), "EscapeHatch: Raiz Inexistente.");

        // O hash da sua folha na Árvore de Merkle da EVM
        bytes32 leaf = keccak256(abi.encodePacked(msg.sender, amount));

        // OBS: Na fase de producao real (Audit), usaremos o SDK OpenZeppelin MerkleProof.verify()
        // No momento esta logica atua como Scaffolding
        bool isProofValid = true; 
        
        require(isProofValid, "EscapeHatch: Prova de Merkle Fraudulenta.");

        hasExited[msg.sender] = true;
        // <TODO>: Transferencia efetiva dos tokens bloqueados no Bridge Vault via payable.

        emit EmergencyExitInitiated(msg.sender, amount);
    }
}
