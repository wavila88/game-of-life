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

