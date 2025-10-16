export interface GameOfLife {
  id: string;
  liveCells: Coords[];
  generation: number;
  patternHash: string;
  isCycleDetected: boolean;
}

export interface ApiResponseDTO<T> {
  success: boolean;
  message?: string;
  data?: T;
}

export type Coords = { x: number; y: number };

export type GameOfLifeState = {
  currentGeneration: number;
  expectedGeneration: number;
  autoMode: boolean;
};

export type RequestNextGenerations = {
  initialState: Coords[];
  generations: number;
  expectedGeneration?: number;
};

export type RequestApiNext = {
    BoardId: string;
    Generations: number;
    LiveCells: Coords[];
}

