using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using PasswordManager.Models;
using PasswordManager.Results;

namespace PasswordManager.Services; 

class PasswordServices
{
    private readonly string _filePath;
    private readonly byte[] _key;

    public PasswordServices(string filePath, byte[] key)
    {
        _filePath = filePath;
        _key = key;

        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public List<PasswordEntry> GetAll()
    {
        var entries = new List<PasswordEntry>();
        if (!File.Exists(_filePath)) return entries;

        var lines = File.ReadAllLines(_filePath);
        foreach (var line in lines)
        {
            try
            {
                var entry = JsonSerializer.Deserialize<PasswordEntry>(line);

                if (entry != null)
                {
                    entry.Password = Decrypt(entry.EncryptedPasswordForJson);
                    entries.Add(entry);
                }
            }
            catch { }

        }
        return entries;
    }

    public OperationResult Add(PasswordEntry entry)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(entry.Service))
                return OperationResult.Fail("Название сервиса не может быть пустым");

            if (entry.Password.Length < 3)
                return OperationResult.Fail("Пароль слишком короткий");

            if (entry.Password.Length > 32)
                return OperationResult.Fail("Пароль слишком длинный");

            var allServices = GetAll();
            if (!allServices.Any(
                e => e.Service.Equals(entry.Service, StringComparison.OrdinalIgnoreCase) &&
                e.Password.Equals(entry.Password, StringComparison.Ordinal)
            ))
            {
                entry.EncryptedPasswordForJson = Encrypt(entry.Password);

                string jsonLine = JsonSerializer.Serialize(entry);
                File.AppendAllLines(_filePath, [jsonLine]);

                return OperationResult.Ok("Пароль успешно сохранён");
            }

            return OperationResult.Fail("В базе уже есть такой сервис с таким паролем");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail($"Системная ошибка: {ex.Message}");
        }

    }

    public OperationResult DeleteByPassword(string service, string password)
    {
        var entries = GetAll();
        var removed = entries.RemoveAll(e => e.Service == service && e.Password == password);
        try
        {
            if (removed > 0)
            {
                var lines = entries.Select(e => {
                    e.EncryptedPasswordForJson = Encrypt(e.Password);
                    return JsonSerializer.Serialize(e);
                });
                File.WriteAllLines(_filePath, lines);
                return OperationResult.Ok("Сервис успешно удалён из базы");
            }
            return OperationResult.Fail("Неверное название или пароль сервиса");
        }
        catch(Exception ex)
        {
            return OperationResult.Fail($"Системная ошибка: {ex.Message}");
        }  
    }

    public List<PasswordEntry>? SearchByPassword(string password)
    {
        return [.. GetAll().Where(e => e.Password == password)];
    }

    public OperationResult DeleteDataBase()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
                return OperationResult.Ok("База данных успешно удалена");
            }
            return OperationResult.Fail("Ошибка удаления базы данных");
        }
        catch (Exception ex)
        {
            return OperationResult.Fail($"Системная ошибка: {ex.Message}");
        }
    }

    private string Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] encrpytedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

        byte[] result = new byte[aes.IV.Length + encrpytedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encrpytedBytes, 0, result, aes.IV.Length, encrpytedBytes.Length);

        return Convert.ToBase64String(result);
    }

    private string Decrypt(string cipherText)
    {
        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            using Aes aes = Aes.Create();
            aes.Key = _key;

            byte[] iv = new byte[aes.BlockSize / 8];
            byte[] cipher = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            
            return Encoding.UTF8.GetString(decryptedBytes);

        }
        catch { return "Ошибка расшифроки"; }
    }
}