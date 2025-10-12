
import React, { useEffect, useState, type JSX } from 'react';

import type { Coords, GameOfLifeState, RequestApiNext, RequestNextGenerations } from '../types';
import Board from '../Board/Board';
import Operations from '../Operations';
import useMutationCreateBoard from './apiCalls/useMutationCreateBoard';
import Banner, { type BannerProps } from '../Banner';
import useMutationNextGenerations from './apiCalls/useMutationNextGenerations';
import type { GameOfLife } from './apiCalls/types';


const GameOfLifeContainer = () => {
    // Now using Set<string> for O(1) lookup
    const [aliveCells, setAliveCells] = useState<Set<string>>(new Set());
    const [autoMode, setAutoMode] = useState(false);
    const [gameOfLife, setGameOfLife] = useState<GameOfLife | null>(null);
    const mutationCreateBoard = useMutationCreateBoard();
    const mutationextGeneration = useMutationNextGenerations();

    const SET_INTERVAL_MS = 500; // 0.5 seconds
    const [banner, setBanner] = useState<BannerProps>({
        message: '',
        showModal: false,
        bannerType: 'error',
        onClose: () => onClose()
    });
    const onClose = () => setBanner({ ...banner, showModal: false });

     const [gameOfLifeState, setGameOfLifeState] = React.useState<GameOfLifeState>({
        currentGeneration: 0,
        expectedGeneration: 1,
        autoMode: false,
      });

    // Helper to convert Set<string> to Coords[]
    const setToCoordsArray = (set: Set<string>): Coords[] => {
        return Array.from(set).map((s) => {
            const [x, y] = s.split(',').map(Number);
            return { x, y };
        });
    };

    // Helper to convert Coords[] to Set<string>
    const coordsArrayToSet = (arr: Coords[]): Set<string> => {
        return new Set(arr.map(({ x, y }) => `${x},${y}`));
    };

    const onNextGen = (request: RequestNextGenerations) => {
        if (gameOfLife === null) {
            // Convert initialState (Set<string>) to Coords[] for API
            mutationCreateBoard.mutate(setToCoordsArray(aliveCells), {
                onSuccess: (data) => {
                    setAliveCells(coordsArrayToSet(data.data?.liveCells || []));
                    let nextGeneration: GameOfLife = data.data as GameOfLife;
                    nextGeneration.generation = request.expectedGeneration  || 1;
                    setGameOfLife(nextGeneration);
                    // Update URL with board ID
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
        } else {
            gameOfLife.generation = request.expectedGeneration  || 1;
            // Update URL with board ID if not already present
            if (gameOfLife.id) {
                window.history.pushState({}, '', `${gameOfLife.id}`);
            }
            handleNextXGen(gameOfLife);
        }
    };

    const handleNextXGen = (gameOfLife: GameOfLife) => {
        mutationextGeneration.mutate({
            BoardId: gameOfLife.id || '',
            Generations: gameOfLife.generation,
            LiveCells: setToCoordsArray(aliveCells)
        } as RequestApiNext,
            {
                onSuccess: (data) => {
                    setAliveCells(coordsArrayToSet(data.data?.liveCells || []));
                    setGameOfLifeState(prev => ({
                        ...prev,
                        currentGeneration: data.data?.generation || 0,
                    }));
                   
                    setGameOfLife(data.data || null);
                    //If there is a message from the backend, show it
                    if (data.message) {
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
    };

    const handleToggleAuto = () => {
        setAutoMode(a => !a);
    };
    /**
     * Auto mode effect call constantly calls next generation every SET_INTERVAL_MS milliseconds
     * until autoMode is turned off
     */
    React.useEffect(() => {
        let interval: any;
        if (autoMode ) {
            interval = setInterval(() => {
                if (gameOfLife != null){
                    handleNextXGen({ ...gameOfLife, generation: 1 });
                } else {
                    onNextGen({
                        initialState: setToCoordsArray(aliveCells),
                        generations: 1,
                    });
                }
            }, SET_INTERVAL_MS);
        }
        return () => {
            if (interval) clearInterval(interval);
        };
    }, [autoMode, gameOfLife, aliveCells]);



    return (
        <div style={{ maxWidth: 1000, margin: '0 auto', padding: 20 }}>
            <Banner message={banner.message} showModal={banner.showModal} onClose={banner.onClose} bannerType={banner.bannerType} />
            <h1 style={{ textAlign: 'center' }}>Game of Life</h1>
            <Operations
                aliveCells={aliveCells}
                autoMode={autoMode}
                onNextGen={onNextGen}
                onToggleAuto={handleToggleAuto}
                gameOfLifeState={gameOfLifeState}
                gameOfLife={gameOfLife}
                setGameOfLifeState={setGameOfLifeState}
            />
            <Board aliveCells={aliveCells} setAliveCells={setAliveCells} />
        </div>
    );

}

export default GameOfLifeContainer;