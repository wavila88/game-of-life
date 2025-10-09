using Domain.Core;
using Domain.Models;
using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class GameOfLifeService : IGameOfLifeService
    {
        protected readonly IGameOfLifeRepository _gameOfLifeRepository;
        public GameOfLifeService( IGameOfLifeRepository gameOfLifeRepository) 
        {
            _gameOfLifeRepository = gameOfLifeRepository;
        }

        public async Task<GameOfLife> SaveGameOfLife(GameOfLife gameOfLife)
        {
            gameOfLife.Id = Guid.NewGuid();
            gameOfLife.Generation = 0;
            return await _gameOfLifeRepository.SaveGameOfLife(gameOfLife);
        }

        public async Task<GameOfLife> GetNGerationsStateAsync(Guid boardId, int numGenerations)
        {
            var currentBoard = await _gameOfLifeRepository.GetState(boardId);

            OperationValidations(currentBoard, numGenerations);
            // Excecute in a background thread to avoid blocking
            var finalLiveCellsHashSet =
                GameOfLifeEngine.CalculateNGerations(currentBoard.LiveCells.ToHashSet(), numGenerations);
           

            var finalBoardState = new GameOfLife
            {
                Id = currentBoard.Id,
                Generation = currentBoard.Generation + numGenerations,
                LiveCells = finalLiveCellsHashSet.ToList(),
            };

            // save on persistence new state.
            finalBoardState = await _gameOfLifeRepository.Update(finalBoardState);

            return finalBoardState;
        }


        private void OperationValidations(GameOfLife currentBoard, int numGenerations)
        {
            if (currentBoard.LiveCells.Count == 0)
            {
                throw new InvalidOperationException("No live cells remain on the board.");
            }
            if (numGenerations > 3000)
            {
                throw new KeyNotFoundException($"Is not allowed more than 3000 Generations");
            }
        }
    }
}
