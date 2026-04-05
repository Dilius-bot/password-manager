using System.Text.Json.Serialization;

namespace PasswordManager.Models;

public class PasswordEntry
{
    public string Service { get; set; } = "";
    [JsonIgnore]
    public string Password { get; set; } = "";

    [JsonPropertyName("Password")]
    public string EncryptedPasswordForJson { get; set; } = "";
    public override string ToString() => $"[Сервис: {Service}, Пароль: {Password}]";
}