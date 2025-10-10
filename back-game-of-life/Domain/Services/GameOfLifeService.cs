using Domain.Core;
using Domain.Models;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    public class GameOfLifeService : IGameOfLifeService
    {
        protected readonly IGameOfLifeRepository _gameOfLifeRepository;
        private const int MAX_GENERATIONS = 3000;
        private readonly ILogger<GameOfLifeService> _logger;
        public GameOfLifeService( IGameOfLifeRepository gameOfLifeRepository, ILogger<GameOfLifeService> logger) 
        {
            _gameOfLifeRepository = gameOfLifeRepository;
            _logger = logger;
        }

        public async Task<GameOfLife> SaveGameOfLife(GameOfLife gameOfLife)
        {
            ValidateLiveCellsCoordinates(gameOfLife.LiveCells);
            //Validate if Hash already exist in DB to avoid save duplicated boards
            //Ensure idempotence in repeated operations
            var gameOfLifeDuplicate =await _gameOfLifeRepository.GetByPatternHash(CycleDetector.ComputePatternHash(gameOfLife.LiveCells.ToHashSet()));
            if (gameOfLifeDuplicate != null)
            { 
                return gameOfLifeDuplicate;
            }
            gameOfLife.Id = Guid.NewGuid();
            gameOfLife.Generation = 0;
            gameOfLife.PatternHash = CycleDetector.ComputePatternHash(gameOfLife.LiveCells.ToHashSet());
            return await _gameOfLifeRepository.SaveGameOfLife(gameOfLife);
        }

        public async Task<GameOfLife> GetNGerationsStateAsync(Guid boardId, int numGenerations, List<Coords> LiveCells)
        {
            var currentBoard = await _gameOfLifeRepository.GetState(boardId);
            currentBoard.LiveCells = LiveCells;

            OperationValidations(currentBoard, numGenerations);
            // Excecute in a background thread to avoid blocking
            var responseBoard =  GameOfLifeEngine.CalculateNGerations(currentBoard.LiveCells.ToHashSet(), numGenerations);
            //Get generation and patternHash from response
            currentBoard.Generation = currentBoard.Generation + responseBoard.Generation;
            currentBoard.PatternHash = responseBoard.PatternHash;
            currentBoard.isCycleDetected = responseBoard.isCycleDetected;
            currentBoard.LiveCells = responseBoard.LiveCells.ToList();

            
            if(currentBoard.isCycleDetected)
            {
                _logger.LogInformation("Cycle detected for board {BoardId} at generation {Generation}", boardId, currentBoard.Generation);
            }

            // save on persistence new state.
            currentBoard = await _gameOfLifeRepository.Update(currentBoard);

            return currentBoard;
        }


        private void OperationValidations(GameOfLife currentBoard, int numGenerations)
        {
            if (currentBoard == null)
            {
                throw new InvalidOperationException("No board foound with this id");
            }
            if (currentBoard.LiveCells.Count == 0)
            {
                throw new InvalidOperationException("No live cells remain on the board.");
            }
            if (numGenerations > MAX_GENERATIONS)
            {
                throw new KeyNotFoundException($"Is not allowed more than 3000 Generations");
            }
        }

        private void ValidateLiveCellsCoordinates(IEnumerable<Coords> liveCells)
        {
            const int MIN_COORD = -10000;
            const int MAX_COORD = 10000;

            if (liveCells == null || !liveCells.Any())
                throw new InvalidOperationException("The board must have at least one live cell.");

            foreach (var cell in liveCells)
            {
                if (cell.X < MIN_COORD || cell.X > MAX_COORD || cell.Y < MIN_COORD || cell.Y > MAX_COORD)
                {
                    throw new InvalidOperationException(
                        $"Cell coordinate ({cell.X}, {cell.Y}) is out of allowed range [{MIN_COORD}, {MAX_COORD}].");
                }
            }
        }
    }
}
