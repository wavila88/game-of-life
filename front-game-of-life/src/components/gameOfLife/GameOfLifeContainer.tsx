
import React, { useState } from 'react';

import Board from '../Board/Board';
import type { Coords, GameOfLifeState } from '../types';
import Operations from '../Operations';


const GameOfLifeContainer = () => {
    const [aliveCells, setAliveCells] = useState<Coords[]>([]);
  
    const [autoMode, setAutoMode] = useState(false);
    const [gameOfLife, setGameOfLife] = useState<GameOfLifeState>(
        {   currentGeneration: 0, expectedGeneration: 0, autoMode: false}
    );





    // Handlers (no real advance logic yet, just UI)
    const handleGenerationsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        // setGenerations(Number(e.target.value));
    };

    const handleNextGen = () => {
        // Here would go the logic to advance N generations
        // alert(`Advance ${generations} generations (not implemented)`);
    };

    const handleToggleAuto = () => {
        setAutoMode(a => !a);
    };

    return (
        <div>
            <h1>Game of Life</h1>
            <Operations
                aliveCells={aliveCells}
                setAliveCells={setAliveCells}
                autoMode={autoMode}
                onNextGen={handleNextGen}
                onToggleAuto={handleToggleAuto}
                gameOfLifeState={gameOfLife}
                setGameOfLifeState={setGameOfLife}
            />
            
            <Board aliveCells={aliveCells} setAliveCells={setAliveCells} />
        </div>
    );
}

export default GameOfLifeContainer;