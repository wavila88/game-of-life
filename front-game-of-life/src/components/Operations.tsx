import React, { use, useEffect } from 'react';
import TextField from '@mui/material/TextField';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import SearchIcon from '@mui/icons-material/Search';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import RefreshIcon from '@mui/icons-material/Refresh';
import PauseIcon from '@mui/icons-material/Pause';
import type { GameOfLifeState, RequestNextGenerations, Coords } from './types';
import type { GameOfLife } from './GameOfLifeContainer/apiCalls/types';


interface OperationsProps {
  autoMode: boolean;
  onNextGen: (request: RequestNextGenerations) => void;
  onToggleAuto: () => void;
  aliveCells: Set<string>;
  gameOfLifeState: GameOfLifeState;
  gameOfLife: GameOfLife | null;
  setGameOfLifeState: React.Dispatch<React.SetStateAction<GameOfLifeState>>;
}

const Operations: React.FC<OperationsProps> = ({
  autoMode,
  onNextGen,
  onToggleAuto,
  aliveCells,
  gameOfLifeState,
  gameOfLife,
  setGameOfLifeState,
}) => {
 
  const [searchBoardId, setSearchBoardId] = React.useState<string>('');

  const handleGenerationsChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setGameOfLifeState(prev => ({
      ...prev,
      expectedGeneration: Number(e.target.value),
    }));
  };

  /** Update the searchBoardId when gameOfLife changes */
  useEffect(() => { 
    setSearchBoardId(gameOfLife?.id || '');
  }, [gameOfLife]);

  // Convert Set<string> to Coords[] for API
  const setToCoordsArray = (set: Set<string>): Coords[] => {
    return Array.from(set).map((s) => {
      const [x, y] = s.split(',').map(Number);
      return { x, y };
    });
  };

  const handleNextGen = () => {
    onNextGen({
      initialState: setToCoordsArray(aliveCells),
      generations: gameOfLifeState.currentGeneration,
      expectedGeneration: gameOfLifeState.expectedGeneration,
    });
  };

  return (
  
    <Grid container spacing={2} alignItems={"center"}>
         {/* BoardId input and search button */}
          <Grid container size={12}>
          <TextField
            id="boardIdInput"
            label="BoardId"
            variant="outlined"
            size="small"
            value={searchBoardId}
            onChange={(e) => setSearchBoardId(e.target.value)}
            placeholder="Enter BoardId"
            disabled={gameOfLife !== null}
            sx={{
              width: 280,
              color: gameOfLife !== null ? '#888' : undefined,
            }}
          />
          <IconButton color="primary" aria-label="search board" size="large">
            <SearchIcon />
          </IconButton>
        </Grid>
       <Grid container size={12}>
        <Grid itemType='div'>
          <label>
            Next state (x generations):
            <TextField
              type="number"
              size="small"
              value={gameOfLifeState.expectedGeneration}
              onChange={handleGenerationsChange}
              variant="outlined"
              sx={{ width: 90, marginLeft: 1, marginRight: 1 }}
            />
          </label>
          <IconButton onClick={handleNextGen} color="primary" aria-label="next state" size="large">
            <PlayArrowIcon />
          </IconButton>
        </Grid>
         <Grid itemType='div'>
          <IconButton
            color="primary"
            aria-label="refresh"
            onClick={() => window.location.reload()}
            size="large"
            sx={{ marginRight: 2 }}
          >
            <RefreshIcon />
          </IconButton>
          <span style={{ marginLeft: 8, marginRight: 16 }}>
            Refresh
          </span>
          <IconButton
            color={autoMode ? 'secondary' : 'primary'}
            aria-label={autoMode ? 'pause' : 'auto'}
            onClick={onToggleAuto}
            size="large"
          >
            {autoMode ? <PauseIcon /> : <PlayArrowIcon />}
          </IconButton>
          <span style={{ marginLeft: 8 }}>
            Constant Advance (Auto Mode)
          </span>
        </Grid>
      </Grid>
     
      <Grid container size={12}>
        <Grid itemType='div'>
          <h3 style={{ margin: 0 }}>Generation: <b>{gameOfLifeState.currentGeneration}</b></h3>
        </Grid>
        <Grid itemType='div'>
          <h3 style={{ margin: 0 }}>Population: <b>{aliveCells.size}</b></h3>
        </Grid>
      </Grid>
    </Grid>

  );
};

export default Operations;
