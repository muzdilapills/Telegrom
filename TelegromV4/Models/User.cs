using System.Text.Json.Serialization;

namespace TelegromV4.Models;

public class User
{
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
    
    [JsonPropertyName("avatar")]
    public string? AvatarPath { get; set; }
}