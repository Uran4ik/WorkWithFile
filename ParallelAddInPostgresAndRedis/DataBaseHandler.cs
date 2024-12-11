using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresRedisDocker
{
    public class PostgresDB
    {
        string postgresConnectionString = "Host=localhost;Username=postgres;Port=5432;Password=password;Database=mydatabase";

        public async Task CreateTable()
        {
            using (var connection = new NpgsqlConnection(postgresConnectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(@"CREATE TABLE IF NOT EXISTS Users 
                    (Id SERIAL PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Age INT,
                    IsActive BOOLEAN)", connection);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddInDB(int id, string name, string email, int age, bool isActive)
        {
            using (var connection = new NpgsqlConnection(postgresConnectionString))
            {
                await connection.OpenAsync();
                var command = new NpgsqlCommand(
                    @"INSERT INTO Users (Id, Name, Email, Age, IsActive)
                    VALUES (@id, @name, @email, @age, @isActive) ON CONFLICT (Id) DO NOTHING", connection);
                command.Parameters.AddWithValue("id", id);
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("email", email);
                command.Parameters.AddWithValue("age", age);
                command.Parameters.AddWithValue("isActive", isActive);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
