namespace Kaffee.Models
{
    public class KaffeeDatabaseSettings : IKaffeeDatabaseSettings
    {
        public string CoffeeCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IKaffeeDatabaseSettings
    {
        string CoffeeCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}