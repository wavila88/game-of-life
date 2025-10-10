using Domain.Models;
using Domain.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public class GameOfLifeUseCase : IGameOfLifeUseCase
    {
        public readonly IGameOfLifeService _service;
        public readonly IGameOfLifeRepository _repository;


        public GameOfLifeUseCase(IGameOfLifeService service, IGameOfLifeRepository gameOfLifeRepository) 
        {
            _service = service;
            _repository = gameOfLifeRepository;
        }

        public async Task<GameOfLife> SaveGameOfLife(GameOfLife gameOfLife)
        {
            return await _service.SaveGameOfLife(gameOfLife);
        }

        public async Task<GameOfLife?> GetState(Guid id)
        {
            return await _repository.GetState(id);
        }

        public async Task<GameOfLife> NextGenerations(Guid id, int generations, List<Coords> LiveCells)
        {
            return await _service.GetNGerationsStateAsync(id, generations, LiveCells);
        }
    }
}
