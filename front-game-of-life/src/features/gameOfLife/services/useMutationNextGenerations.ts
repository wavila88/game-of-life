import { useMutation } from "@tanstack/react-query";
import type { ApiResponseDTO, GameOfLife, RequestApiNext } from "../types";

// Replace with your actual API endpoint
const CREATE_BOARD_URL = `${import.meta.env.VITE_API_URL}/GameOfLife`;

async function nextGeneration(payload: RequestApiNext): Promise<ApiResponseDTO<GameOfLife>> {
        const response = await fetch(`${CREATE_BOARD_URL}/boards/next`, {
            method: "POST",
            headers: { "Content-Type": "application/json", "Access-Control-Allow-Origin": "*" },
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

const useMutationNextGenerations = () => {
    return useMutation<ApiResponseDTO<GameOfLife>, Error, any>({
        mutationKey: ["nextGenerations"],
        mutationFn: nextGeneration,
    });
};

export default useMutationNextGenerations;