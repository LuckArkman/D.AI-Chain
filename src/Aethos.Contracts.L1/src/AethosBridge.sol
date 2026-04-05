// SPDX-License-Identifier: MIT
pragma solidity 0.8.24;

contract AethosBridge {
    address public immutable guardianSequencer;
    mapping(uint256 => bytes32) public stateRoots;
    mapping(uint256 => bytes32) public porHashes;
    uint256 public latestL2BlockNumber;

    event StateCommitted(uint256 indexed l2BlockNumber, bytes32 stateRoot, bytes32 porHash);

    modifier onlySequencer() {
        require(msg.sender == guardianSequencer, "Unauthorized");
        _;
    }

    constructor(address _sequencer) {
        guardianSequencer = _sequencer;
    }

    function commitState(uint256 l2BlockNumber, bytes32 stateRoot, bytes32 porHash) external onlySequencer {
        require(l2BlockNumber > latestL2BlockNumber, "Invalid sequence");
        stateRoots[l2BlockNumber] = stateRoot;
        porHashes[l2BlockNumber] = porHash;
        latestL2BlockNumber = l2BlockNumber;
        emit StateCommitted(l2BlockNumber, stateRoot, porHash);
    }
}
