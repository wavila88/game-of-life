import { HubConnectionBuilder } from "@microsoft/signalr";
import type { RequestApiNext } from "../types";

const HUB_URL = `${import.meta.env.VITE_API_URL}/gameoflifehub`;

const connection = new HubConnectionBuilder()
  .withUrl(HUB_URL, { withCredentials: true })
  .withAutomaticReconnect()
  .build();

// Allows registering a callback for when the board state is received
export function onReceiveBoardState(callback: (board: any) => void) {
  connection.on("ReceiveBoardState", (board) => {
    callback(board);
  });
}

export async function startConnection() {
  if (connection.state === "Disconnected") {
    await connection.start();
  }
}

export async function closeWebSocketConnection() {
  if (connection.state === "Connected") {
    await connection.stop();
  }
}

export async function sendNextGeneration(payload: RequestApiNext) {
  await startConnection();
  await connection.invoke("Play", payload);
}

export function isExpectedWebSocketError(error: any) {
  return (
    error?.message?.includes("Invocation canceled")
  );
}

export default connection;