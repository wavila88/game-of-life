import { useMutation } from "@tanstack/react-query";
import type { ApiResponseDTO, Coords, GameOfLife } from "../types";


// Replace with your actual API endpoint
const CREATE_BOARD_URL = "http://localhost:5082/GameOfLife/boards";

async function createBoard(payload: Coords[]): Promise<ApiResponseDTO<GameOfLife>> {
        const response = await fetch(CREATE_BOARD_URL, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: 'include',
            body: JSON.stringify(payload),
        });
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

const useMutationCreateBoard = () => {
    return useMutation<ApiResponseDTO<GameOfLife>, Error, any>({
        mutationKey: ["createBoard"],
        mutationFn: createBoard,
    });
};

export default useMutationCreateBoard;