// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "@openzeppelin/contracts/token/ERC20/ERC20.sol";
import "@openzeppelin/contracts/token/ERC20/extensions/ERC20Burnable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

/**
 * @title AethosToken ($AETH)
 * @dev Token nativo da Aethos Ledger. 
 * Utilizado para:
 * 1. Pagamento de Gas para inferências neurais.
 * 2. Staking de validadores (Proof of Reasoning).
 * 3. Governança DAO.
 */
contract AethosToken is ERC20, ERC20Burnable, Ownable {
    
    // Sprint 65: Supply Gênesis: 1.000.000.000 $AETH
    uint256 public constant INITIAL_SUPPLY = 1_000_000_000 * 10**18;

    constructor(address initialOwner) 
        ERC20("Aethos Token", "AETH") 
        Ownable(initialOwner) 
    {
        _mint(initialOwner, INITIAL_SUPPLY);
    }

    /**
     * @dev Função para emissão controlada de recompensas de mineração neural.
     * Somente o contrato da Bridge ou a DAO pode disparar a emissão de novos tokens.
     */
    function mintRewards(address to, uint256 amount) external onlyOwner {
        _mint(to, amount);
    }
}
