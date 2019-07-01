namespace Kaffee.Settings
{
    public interface IRedisSettings 
    {
        string Url { get; set; }
        int Port { get; set; }
    }

    public class RedisSettings: IRedisSettings
    {
        public string Url { get; set; }
        public int Port { get; set; }
    }
}