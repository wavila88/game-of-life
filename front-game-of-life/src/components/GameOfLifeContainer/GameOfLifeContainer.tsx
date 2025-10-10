
import React, { useState, type JSX } from 'react';

import type { Coords, GameOfLifeState, RequestApiNext, RequestNextGenerations } from '../types';
import Board from '../Board/Board';
import Operations from '../Operations';
import useMutationCreateBoard from './apiCalls/useMutationCreateBoard';
import Banner, { type BannerProps } from '../Banner';
import useMutationNextGenerations from './apiCalls/useMutationNextGenerations';
import type { GameOfLife } from './apiCalls/types';


const GameOfLifeContainer = () => {
    const [aliveCells, setAliveCells] = useState<Coords[]>([]);
    const [autoMode, setAutoMode] = useState(false);
    const [gameOfLife, setGameOfLife] = useState<GameOfLife | null>(null);

    const mutationCreateBoard = useMutationCreateBoard();
    const mutationextGeneration = useMutationNextGenerations();
    const [banner, setBanner] = useState<BannerProps>({
        message: '',
        showModal: false,
        bannerType: 'error',
        onClose: () => onClose()
    });
    const onClose = () => setBanner({ ...banner, showModal: false });

     const [gameOfLifeState, setGameOfLifeState] = React.useState<GameOfLifeState>({
        currentGeneration: 0,
        expectedGeneration: 0,
        autoMode: false,
      });

    // Handlers (sin lógica real de avance, solo UI)
    const handleGenerationsChange = (e: React.ChangeEvent<HTMLInputElement>) => {

    };

    const onNextGen = (request: RequestNextGenerations) => {
        debugger;
        if (gameOfLife === null) {
            mutationCreateBoard.mutate(request.initialState, {
                onSuccess: (data) => {
                    debugger;
                    setAliveCells(data.data?.liveCells || []);
                    let nextGeneration: GameOfLife = data.data as GameOfLife;
                    nextGeneration.generation = request.expectedGeneration  || 1;
                    setGameOfLife(nextGeneration);
                    handleNextXGen(nextGeneration);
                },
                onError: (ex: any) => {
                    setBanner({
                        message: ex.message || "An unexpected error occurred.",
                        showModal: true,
                        bannerType: 'error',
                        onClose: () => onClose()
                    });
                }
            });
        }else{
            gameOfLife.generation = request.expectedGeneration  || 1;
            handleNextXGen(gameOfLife);
        }
    }

    const handleNextXGen = (gameOfLife: GameOfLife) => {
        mutationextGeneration.mutate({
            BoardId: gameOfLife.id || '',
            Generations: gameOfLife.generation,
            LiveCells: aliveCells
        } as RequestApiNext,
            {
                onSuccess: (data) => {
                    debugger;
                    setAliveCells(data.data?.liveCells || []);
                    setGameOfLifeState(prev => ({
                        ...prev,
                        currentGeneration: data.data?.generation || 0,
                    }));
                    setGameOfLife(data.data || null);
                    //If there is a message from the backend, show it
                    if (data.message) {
                        debugger;
                        setBanner({
                            message: data.message,
                            showModal: true,
                            bannerType: 'success',
                            onClose: () => onClose()
                        });
                    }
                },
                onError: (ex: any) => {
                    setBanner({
                        message: ex.message || "An unexpected error occurred.",
                        showModal: true,
                        bannerType: 'error',
                        onClose: () => onClose()
                    });
                }
            }
        );
    }

    const handleToggleAuto = () => {
        setAutoMode(a => !a);
    };

    return (
        <div>
            <Banner message={banner.message} showModal={banner.showModal} onClose={banner.onClose} bannerType={banner.bannerType} />
            <h1>Game of Life</h1>
            <Operations
                aliveCells={aliveCells}
                setAliveCells={setAliveCells}
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