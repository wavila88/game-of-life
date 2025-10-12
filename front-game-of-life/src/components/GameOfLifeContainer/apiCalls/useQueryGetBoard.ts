import { useQuery } from "@tanstack/react-query";
import type { ApiResponseDTO, GameOfLife } from "./types";


// Replace with your actual API endpoint
const CREATE_BOARD_URL = "http://localhost:5082/GameOfLife";

async function getBoard(boardId: string): Promise<ApiResponseDTO<GameOfLife>> {
        const response = await fetch(`${CREATE_BOARD_URL}/boards/${boardId}`, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
            credentials: 'include',        });
        if (!response.ok) {
            // Try to parse error message from backend if available
            let message = "An unexpected error occurred.";
            try {
                const errorData = await response.json();
                if (errorData && errorData.message) {
                    message = errorData.message;
                }
            } catch {}
            return { success: false, message };
        }
        return response.json();
}

const useQueryGetBoard = (boardId: string) => {
    return useQuery<ApiResponseDTO<GameOfLife>, Error, any>({
        queryKey: ["getBoard", boardId],
        queryFn: () => getBoard(boardId),
    });
};

export default useQueryGetBoard;