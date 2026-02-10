using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OracleDataManager
{
    public static class ConfigStore
    {
        private static string ConfigPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "OracleDataManager", "config.json");

        public static DbConfig? Load()
        {
            if (!File.Exists(ConfigPath)) return null;
            var json = File.ReadAllText(ConfigPath, Encoding.UTF8);
            return JsonSerializer.Deserialize<DbConfig>(json);
        }

        public static void Save(DbConfig cfg)
        {
            var dir = Path.GetDirectoryName(ConfigPath)!;
            Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json, Encoding.UTF8);
        }

        public static string Encrypt(string plain)
        {
            var bytes = Encoding.UTF8.GetBytes(plain);
            var enc = ProtectedData.Protect(bytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(enc);
        }

        public static string Decrypt(string base64)
        {
            var enc = Convert.FromBase64String(base64);
            var bytes = ProtectedData.Unprotect(enc, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string BuildConnStr(DbConfig cfg)
        {
            var pw = string.IsNullOrEmpty(cfg.PasswordEnc) ? "" : Decrypt(cfg.PasswordEnc);
            return $"User Id={cfg.UserId};Password={pw};Data Source={cfg.DataSource};";
        }
    }
}
