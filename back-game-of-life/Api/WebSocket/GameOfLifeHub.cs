using Api.DTO;
using Application.UseCases;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;


namespace Api.WebSocket
{
    public class GameOfLifeHub : Hub
    {
        private readonly IGameOfLifeUseCase _gameOfLifeUseCase;
        private readonly int _generationDelayMs;

        public GameOfLifeHub(IGameOfLifeUseCase gameOfLifeUseCase, IConfiguration configuration)
        {
            _gameOfLifeUseCase = gameOfLifeUseCase;
            
            _generationDelayMs = configuration.GetValue<int>("GameOfLife:GenerationDelayMs");
        }

        public async Task Play(NextGenerationsHubRequestDTO request)
        {
            var currentRequest = request;
            //Is going to stop until socket be disconected.
            while (true)
            {
                var board = await _gameOfLifeUseCase.HandleHubNextGeneration(currentRequest);
                // Send the board state to the client
                await Clients.Caller.SendAsync("ReceiveBoardState", board);

                // Update the request for the next generation
                currentRequest = new NextGenerationsHubRequestDTO
                {
                    BoardId = board.Id.ToString(),
                    Generations =+ 1,
                    LiveCells = board.LiveCells
                };

                // Wait
                await Task.Delay(_generationDelayMs);

                // Optional: break the loop if the client disconnects
                if (Context.ConnectionAborted.IsCancellationRequested)
                    break;
            }
        }

       
    }
}
