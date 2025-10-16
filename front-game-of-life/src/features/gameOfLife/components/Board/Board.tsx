
import "./Board.css";

const INITIAL_ROWS = 150;
const INITIAL_COLS = 120;

interface BoardProps {
    aliveCells: Set<string>;
    setAliveCells: React.Dispatch<React.SetStateAction<Set<string>>>;
}


import React from 'react';


const Board = ({ aliveCells, setAliveCells }: BoardProps) => {
    const [scale, setScale] = React.useState(1);
    const [rows, setRows] = React.useState(INITIAL_ROWS);
    const [cols, setCols] = React.useState(INITIAL_COLS);
    const boardRef = React.useRef<HTMLDivElement>(null);

    const isAlive = (row: number, col: number) =>
        aliveCells.has(`${row},${col}`);

    const toggleCell = (row: number, col: number) => {
        setAliveCells(prev => {
            const key = `${row},${col}`;
            const newSet = new Set(prev);
            if (newSet.has(key)) {
                newSet.delete(key);
            } else {
                newSet.add(key);
            }
            return newSet;
        });
    };

    // Zoom con Ctrl + scroll
    const handleWheel = (e: React.WheelEvent<HTMLDivElement>) => {
        if (e.ctrlKey) {
            e.preventDefault();
            setScale(prev => {
                let next = prev - e.deltaY * 0.001;
                if (next < 0.2) next = 0.2;
                if (next > 3) next = 3;
                return next;
            });
        }
    };

    // Detectar scroll cerca del borde para expandir el tablero
    const handleScroll = (e: React.UIEvent<HTMLDivElement>) => {
        const el = e.currentTarget;
        // Si el usuario está a menos de 100px del borde derecho, agrega más columnas
        if (el.scrollWidth - el.scrollLeft - el.clientWidth < 100) {
            setCols(c => c + 20);
        }
        // Si el usuario está a menos de 100px del borde inferior, agrega más filas
        if (el.scrollHeight - el.scrollTop - el.clientHeight < 100) {
            setRows(r => r + 10);
        }
    };

    return (
        <div
            className="gol-board"
            style={{
                width: '100%',
                height: '500px',
                overflowX: 'scroll',
                overflowY: 'scroll',
                border: '2px solid #bbb',
                background: '#fff',
            }}
            onWheel={handleWheel}
            onScroll={handleScroll}
            ref={boardRef}
        >
            <div
                style={{
                    minWidth: 60 * 20 + 'px',
                    display: 'inline-block',
                    transform: `scale(${scale})`,
                    transformOrigin: '0 0',
                }}
            >
                {Array.from({ length: rows }).map((_, rowIdx) => (
                    <div className="gol-row" key={rowIdx} style={{ display: 'flex' }}>
                        {Array.from({ length: cols }).map((_, colIdx) => (
                            <div
                                key={colIdx}
                                className={`gol-cell${isAlive(rowIdx, colIdx) ? ' gol-cell-alive' : ''}`}
                                onClick={() => toggleCell(rowIdx, colIdx)}
                                style={{ minWidth: 24, minHeight: 20 }}
                            />
                        ))}
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Board;
