import React from 'react';
import type { Coords, GameOfLifeState, RequestNextGenerations } from './types';

interface OperationsProps {
  autoMode: boolean;
  onNextGen: (request: RequestNextGenerations) => void;
  onToggleAuto: () => void;
  aliveCells: Coords[];
  setAliveCells: React.Dispatch<React.SetStateAction<Coords[]>>;
  gameOfLifeState: GameOfLifeState;
  setGameOfLifeState: React.Dispatch<React.SetStateAction<GameOfLifeState>>;
}

const Operations: React.FC<OperationsProps> = ({
  autoMode,
  onNextGen,
  onToggleAuto,
  aliveCells,
  setAliveCells,
  gameOfLifeState,
  setGameOfLifeState,
}) => {
 
 

  const handleGenerationsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setGameOfLifeState(prev => ({
      ...prev,
      expectedGeneration: Number(e.target.value),
    }));
  };

  const handleNextGen = () => {
    onNextGen({
      initialState: aliveCells,
      generations: gameOfLifeState.currentGeneration,
      expectedGeneration: gameOfLifeState.expectedGeneration,
    });
  }

  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 8, marginBottom: 16 }}>
    
      <div style={{ display: 'flex', gap: '2rem', alignItems: 'center' }}>
        <div>
          <label>
            Next state (x generations):
            <input
              type="number"
              min={1}
              value={gameOfLifeState.expectedGeneration}
              onChange={handleGenerationsChange}
              style={{ width: 60, marginLeft: 8 }}
            />
          </label>
          <button onClick={handleNextGen} style={{ marginLeft: 8 }}>
            Next
          </button>
        </div>
        <div>
          <button onClick={onToggleAuto} style={{ fontSize: 20, padding: '4px 12px' }}>
            {autoMode ? '⏸️' : '▶️'}
          </button>
          <span style={{ marginLeft: 8 }}>
            Constant Advance (Auto Mode)
          </span>
        </div>
      </div>
        <div style={{ display: 'flex', gap: '2rem', marginBottom: 4 }}>
        <h3>Generation: <b>{gameOfLifeState.currentGeneration}</b></h3>
        <h3>Population: <b>{aliveCells.length}</b></h3>
      </div>
    </div>
  );
};

export default Operations;
