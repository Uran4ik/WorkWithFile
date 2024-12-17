using Npgsql;
using StackExchange;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json;
using System.IO;
using StackExchange.Redis;
using System.Threading.Tasks;

namespace PostgresRedisDocker
{
    public interface IReadJsonStrat
    {
        Task<UserClass[]> ReadJson(string path);
    }
    public class ReadJsonStrat : IReadJsonStrat
    {
        public async Task<UserClass[]> ReadJson(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = await reader.ReadToEndAsync();

                UserClass[] users = JsonConvert.DeserializeObject<UserClass[]>(content);
                return users;
            }
        }
    }

    public class JSONDelegate
    {
        public event Func<UserClass[], Task> DataRead;

        public async Task Read(string path, IReadJsonStrat strat)
        {
            UserClass[] users = await strat.ReadJson(path);
            if (DataRead != null)
                await DataRead(users);
        }
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const string jsonPath = @"C:\Users\Нурмухаммед\Downloads\Telegram Desktop\users2.json";

            var postgres = new PostgresDB();
            var redis = new RedisHandler();
            var mongo = new MongoDBHandler();

            await postgres.CreateTable();

            try
            {
                var reader = new JSONDelegate();
                reader.DataRead += async (users) =>
                {

                    Parallel.ForEach(users, async (user) =>
                    {
                        try
                        {

                            await Task.Run(async () =>
                            {

                                await postgres.AddInDB(user.Id, user.Name, user.Email, user.Age, user.IsActive); // Запись в PostgreSQL
                                await redis.SetData("Last", JsonConvert.SerializeObject(user)); // Обновление Redis
                                await mongo.AddUser(user); // Запись в MongoDB
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Ошибка: " + ex);
                        }
                    });

                    Console.WriteLine("\nДанные добавлены в PostgreSQL и MongoDB. Redis обновлен.");
                };

                IReadJsonStrat strat = new ReadJsonStrat();
                string last = await redis.GetData("Last");
                await reader.Read(jsonPath, strat);

                Console.WriteLine($"Последняя запись в Redis: {last}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.ToString());
            }
        }
    }
}
