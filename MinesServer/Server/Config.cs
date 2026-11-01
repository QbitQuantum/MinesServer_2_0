using Newtonsoft.Json;

namespace MinesServer.Server
{
    public class Config
    {
        public const string DefaultWorldName = "MinesServer";

        public readonly string WorldName = DefaultWorldName;

        [JsonConstructor]
        public Config(string WorldName)
        {
            if (!string.IsNullOrWhiteSpace(WorldName))
                this.WorldName = WorldName;
        }

        public static Config Load(string configPath = "config.json")
        {
            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException("Config path must not be empty.", nameof(configPath));

            if (File.Exists(configPath))
            {
                var loaded = TryRead(configPath);
                if (loaded != null) return loaded;
            }

            var cfg = new Config("ff");
            TryWrite(configPath, cfg);
            return cfg;
        }

        private static Config? TryRead(string path)
        {
            try
            {
                var json = File.ReadAllText(path);
                if (string.IsNullOrWhiteSpace(json))
                    return null;

                return JsonConvert.DeserializeObject<Config>(json);
            }
            catch (JsonException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return null;
            }
        }

        private static void TryWrite(string path, Config cfg)
        {
            try
            {
                var json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (IOException) { /* нет доступа — работаем с конфигом в памяти */ }
            catch (UnauthorizedAccessException) { /* то же самое */ }
        }
    }
}