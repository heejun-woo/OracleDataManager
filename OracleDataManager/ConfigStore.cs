using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

namespace OracleDataManager
{
    public sealed class DbConnectionStore
    {
        public List<DbConnectionProfile> Profiles { get; set; } = new();
        public string? LastSelectedName { get; set; }
    }

    public static class ConfigStore
    {
        private static string PathConfig =>
            System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "OraTableEditor", "connections.json");

        public static DbConnectionStore LoadStore()
        {
            if (!File.Exists(PathConfig)) return new DbConnectionStore();
            var json = File.ReadAllText(PathConfig, Encoding.UTF8);
            return JsonSerializer.Deserialize<DbConnectionStore>(json) ?? new DbConnectionStore();
        }

        public static void SaveStore(DbConnectionStore store)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PathConfig)!);
            var json = JsonSerializer.Serialize(store, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(PathConfig, json, Encoding.UTF8);
        }

        public static string Encrypt(string plain)
        {
            var bytes = Encoding.UTF8.GetBytes(plain ?? "");
            var enc = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(enc);
        }

        public static string Decrypt(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return "";
            var enc = Convert.FromBase64String(base64);
            var bytes = ProtectedData.Unprotect(enc, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string BuildConnStr(DbConnectionProfile p, string? overridePassword = null)
        {
            var pw = overridePassword ?? Decrypt(p.PasswordEnc);
            return $"User Id={p.UserId};Password={pw};Data Source={p.DataSource};";
        }
    }

}
