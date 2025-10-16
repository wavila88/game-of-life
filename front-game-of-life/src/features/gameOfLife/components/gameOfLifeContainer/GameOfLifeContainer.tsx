
import  { useState } from 'react';
import Banner from '../Banner';
import Board from '../Board/Board';
import Operations from '../Operations';
import { useGameOfLifeLogic } from '../../hooks/useGameOfLifeHook';


const GameOfLifeContainer = () => {
    // Separate Cells from Hook to prevent re-renders and latency painting cells
    const [aliveCells, setAliveCells] = useState<Set<string>>(new Set());
    const {
    autoMode,
    handleToggleAuto,
    onNextGen,
    gameOfLifeState,
    setGameOfLifeState,
    gameOfLife,
    handleSearchBoard,
    banner,
    onClose,
  } = useGameOfLifeLogic({ aliveCells, setAliveCells });

  return (
    <div style={{ maxWidth: 1000, margin: '0 auto', padding: 20 }}>
      <Banner {...banner} onClose={onClose}  />
      <h1 style={{ textAlign: 'center' }}>Game of Life</h1>

      <Operations
        handleSearchBoard={handleSearchBoard}
        gameOfLife={gameOfLife}
        aliveCells={aliveCells}
        autoMode={autoMode}
        onNextGen={onNextGen}
        onToggleAuto={handleToggleAuto}
        gameOfLifeState={gameOfLifeState}
        setGameOfLifeState={setGameOfLifeState}
      />

      <Board aliveCells={aliveCells} setAliveCells={setAliveCells} />
    </div>
    );

}

export default GameOfLifeContainer;