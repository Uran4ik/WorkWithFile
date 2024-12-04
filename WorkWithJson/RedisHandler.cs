using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgresRedisDocker
{
    public class RedisHandler
    {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:6379");
            IDatabase bd;

            public RedisHandler()
                => bd = redis.GetDatabase();
            public async Task SetData(string index, string value)
                => await bd.StringSetAsync(index, value);
            
            public async Task<string> GetData(string index)
                => await bd.StringGetAsync(index);
        }
    }
