namespace Kaffee.Settings
{
    public interface IDarkSkySettings
    {
        string Url { get; set; }
        string Token { get; set; }
    }

    public class DarkSkySettings
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }
}