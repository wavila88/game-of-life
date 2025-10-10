export interface Coords {
  x: number;
  y: number;
}

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
