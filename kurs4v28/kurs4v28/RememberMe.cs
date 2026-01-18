using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kurs4v28
{
    public static class RememberMe
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "kurs4v28",
            "rememberme.dat");
        public static void SaveCredentials(string login, string password)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                string data = $"{login}|{password}";
                string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
                File.WriteAllText(FilePath, encoded);

                Console.WriteLine($"Saved to: {FilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Save error: {ex.Message}");
            }
        }
        public static (string login, string password) LoadCredentials()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return (null, null);
                string encoded = File.ReadAllText(FilePath);
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                var parts = decoded.Split('|');
                if (parts.Length == 2)
                {
                    Console.WriteLine($"Loaded: {parts[0]}");
                    return (parts[0], parts[1]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Load error: {ex.Message}");
            }
            return (null, null);
        }
        public static void ClearCredentials()
        {
            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
            }
            catch { }
        }
    }
}
