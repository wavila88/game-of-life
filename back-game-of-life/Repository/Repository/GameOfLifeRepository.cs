using Domain.Models;
using Domain.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infra.Repository
{
    public class GameOfLifeRepository : IGameOfLifeRepository
    {
        private readonly IDatabase _redisDb;

        public GameOfLifeRepository()
        {
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            _redisDb = redis.GetDatabase();
        }

        public async Task<GameOfLife> SaveGameOfLife(GameOfLife gameOfLife)
        {
            var value = JsonSerializer.Serialize(gameOfLife);
            // Only set if the key does not exist
            bool created =await _redisDb.StringSetAsync(gameOfLife.Id.ToString(), value, when: When.NotExists);

            if (!created)
            {
                // Already exists
                throw new InvalidOperationException($"A board with Id {gameOfLife.Id} already exists.");
            }

            var storedValue = _redisDb.StringGet(gameOfLife.Id.ToString());
            if (storedValue.IsNullOrEmpty)
            {
                throw new Exception("Failed to save the GameOfLife instance in Redis.");
            }
            return JsonSerializer.Deserialize<GameOfLife>(storedValue)!;
        }

        public async Task<GameOfLife?> GetState(Guid id)
        {
            var value = await _redisDb.StringGetAsync(id.ToString());
            if (value.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<GameOfLife>(value);
        }

        public async Task<GameOfLife?> Update(GameOfLife gameOfLife)
        {
            // Only update if the key exists
            if (!await _redisDb.KeyExistsAsync(gameOfLife.Id.ToString()))
            {
                throw new KeyNotFoundException($"No board found with Id {gameOfLife.Id} to update.");
            }

            var value = JsonSerializer.Serialize(gameOfLife);
            await _redisDb.StringSetAsync(gameOfLife.Id.ToString(), value);

            var storedValue = await _redisDb.StringGetAsync(gameOfLife.Id.ToString());
            if (storedValue.IsNullOrEmpty)
            {
                return null;
            }
            return JsonSerializer.Deserialize<GameOfLife>(storedValue);
        }

    }
}
