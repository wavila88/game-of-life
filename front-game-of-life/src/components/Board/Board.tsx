
import type { Coords } from '../types';
import "./Board.css";

const ROWS = 20;
const COLS = 45;

interface BoardProps {
    aliveCells: Coords[];
    setAliveCells: React.Dispatch<React.SetStateAction<Coords[]>>;
}

const Board = ({ aliveCells, setAliveCells }: BoardProps) => {
    const isAlive = (row: number, col: number) =>
        aliveCells.some(cell => cell.x === row && cell.y === col);

    const toggleCell = (row: number, col: number) => {
        setAliveCells(prev => {
            const idx = prev.findIndex(cell => cell.x === row && cell.y === col);
            if (idx !== -1) {
                // Si ya está viva, la quitamos
                return prev.filter((_, i) => i !== idx);
            } else {
                // Si está muerta, la agregamos
                return [...prev, { x: row, y: col }];
            }
        });
    };
    
    return (
        <div className="gol-board">
            {Array.from({ length: ROWS }).map((_, rowIdx) => (
                <div className="gol-row" key={rowIdx}>
                    {Array.from({ length: COLS }).map((_, colIdx) => (
                        <div
                            key={colIdx}
                            className={`gol-cell${isAlive(rowIdx, colIdx) ? ' gol-cell-alive' : ''}`}
                            onClick={() => toggleCell(rowIdx, colIdx)}
                        />
                    ))}
                </div>
            ))}
        </div>
    );
};

export default Board;
