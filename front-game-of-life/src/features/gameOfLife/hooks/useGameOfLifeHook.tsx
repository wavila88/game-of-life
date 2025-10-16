// hooks/useGameOfLifeLogic.js

import { useState, useEffect, type Dispatch, type SetStateAction, } from 'react';
import type { ApiResponseDTO, Coords, GameOfLife, GameOfLifeState, RequestNextGenerations } from '../types';
import useMutationCreateBoard from '../services/useMutationCreateBoard';
import useMutationNextGenerations from '../services/useMutationNextGenerations';

// Banner type
export type BannerType = 'error' | 'success' | 'warning';
export interface BannerProps {
  message: string;
  showModal: boolean;
  bannerType: BannerType;
  onClose: () => void;
}

export interface UseGameOfLifeLogic {
  autoMode: boolean;
  handleToggleAuto: () => void;
  onNextGen: (request: RequestNextGenerations) => void;
  gameOfLifeState: GameOfLifeState;
  setGameOfLifeState: React.Dispatch<React.SetStateAction<GameOfLifeState>>;
  gameOfLife: GameOfLife | null;
  handleSearchBoard: (boardId: string) => Promise<void>;
  banner: BannerProps;
  onClose: () => void;
}

export interface UseGameOfLifeHookProps  { 
  aliveCells: Set<string>;
  setAliveCells: Dispatch<SetStateAction<Set<string>>>;
 }

export function useGameOfLifeLogic({ aliveCells, setAliveCells }: UseGameOfLifeHookProps): UseGameOfLifeLogic {
  const [autoMode, setAutoMode] = useState<boolean>(false);
  const [gameOfLife, setGameOfLife] = useState<GameOfLife | null>(null);
  const [banner, setBanner] = useState<BannerProps>({
    message: '',
    showModal: false,
    bannerType: 'error',
    onClose: () => onClose(),
  });
  const [gameOfLifeState, setGameOfLifeState] = useState<GameOfLifeState>({
    currentGeneration: 0,
    expectedGeneration: 1,
    autoMode: false,
  });

  const mutationCreateBoard = useMutationCreateBoard();
  const mutationNextGeneration = useMutationNextGenerations();
  const mutationQueryBoard = useMutationCreateBoard();
  const SET_INTERVAL_MS = 500;

  const onClose = () => setBanner({ ...banner, showModal: false });

  const setToCoordsArray = (set: Set<string>): Coords[] =>
    Array.from(set).map((s) => {
      const [x, y] = s.split(',').map(Number);
      return { x, y };
    });

    

  const coordsArrayToSet = (arr: Coords[]): Set<string> =>
    new Set(arr.map(({ x, y }) => `${x},${y}`));

  const handleNextXGen = (gameOfLifeObj: GameOfLife) => {
    mutationNextGeneration.mutate(
      {
        BoardId: gameOfLifeObj.id || '',
        Generations: gameOfLifeObj.generation,
        LiveCells: setToCoordsArray(aliveCells),
      },
      {
        onSuccess: (data: ApiResponseDTO<GameOfLife>) => {
          setAliveCells(coordsArrayToSet(data.data?.liveCells || []));
          setGameOfLifeState((prev) => ({
            ...prev,
            currentGeneration: data.data?.generation || 0,
          }));
          setGameOfLife(data.data || null);

          if (data.message) {
            setBanner({
              message: data.message,
              showModal: true,
              bannerType: 'success',
              onClose,
            });
          }
        },
        onError: (ex: Error) =>
          setBanner({
            message: ex.message || 'An unexpected error occurred.',
            showModal: true,
            bannerType: 'error',
            onClose,
          }),
      }
    );
  };

  

  const onNextGen = (request: RequestNextGenerations) => {
    if (!gameOfLife) {
      mutationCreateBoard.mutate(setToCoordsArray(aliveCells), {
        onSuccess: (data: ApiResponseDTO<GameOfLife>) => {
          setAliveCells(coordsArrayToSet(data.data?.liveCells || []));
          let nextGen: GameOfLife = { ...data.data, generation: request.expectedGeneration || 1 } as GameOfLife;
          setGameOfLife(nextGen);
          handleNextXGen(nextGen);
        },
        onError: (ex: Error) =>
          setBanner({
            message: ex.message || 'An unexpected error occurred.',
            showModal: true,
            bannerType: 'error',
            onClose,
          }),
      });
    } else {
      const next = { ...gameOfLife, generation: request.expectedGeneration || 1 };
      handleNextXGen(next);
    }
  };

  const handleToggleAuto = () => setAutoMode((a) => !a);

  useEffect(() => {
    let interval : any;
    if (autoMode) {
      interval = setInterval(() => {
        if (gameOfLife) {
          handleNextXGen({ ...gameOfLife, generation: 1 });
        } else {
          onNextGen({
            initialState: setToCoordsArray(aliveCells),
            generations: 1,
          });
        }
      }, SET_INTERVAL_MS);
    }
    return () => interval && clearInterval(interval);
  }, [autoMode, gameOfLife, aliveCells]);

const handleSearchBoard = async (boardId: string) => {

    const result = await mutationQueryBoard.mutateAsync(boardId)
    if (result.success) {
      debugger;
      setGameOfLife(result.data || null);
      setAliveCells(coordsArrayToSet(result.data?.liveCells || []));
      setGameOfLifeState(prev => ({
        ...prev,
        currentGeneration: result.data?.generation || 0,
      }));
    }

    if (result.success === false) {
      setBanner({
        message: result.message || "An unexpected error occurred.",
        showModal: true,
        bannerType: 'error',
        onClose: () => onClose()
      });
    }
  }


  return {
    autoMode,
    handleToggleAuto,
    onNextGen,
    gameOfLifeState,
    gameOfLife,
    setGameOfLifeState,
    handleSearchBoard,
    banner,
    onClose,
  };
}
