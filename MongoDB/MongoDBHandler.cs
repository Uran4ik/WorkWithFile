using System;
using Newtonsoft.Json;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using StackExchange.Redis;
using System.Text.Json.Serialization;
using MongoDB.Driver;

namespace PostgresRedisDocker
{
    public class MongoDBHandler
    {
        private readonly IMongoCollection<UserClass> _usersCollection;

        public MongoDBHandler()
        {
            var client = new MongoClient("mongodb://localhost:27017"); // Убедитесь, что MongoDB запущен на этом адресе
            var database = client.GetDatabase("UserDatabase"); // Создаем/подключаемся к базе данных "UserDatabase"
            _usersCollection = database.GetCollection<UserClass>("Users"); // Создаем/подключаемся к коллекции "Users"
        }

        public async Task AddUser(UserClass user)
        {
            await _usersCollection.InsertOneAsync(user);
        }
    }
}
