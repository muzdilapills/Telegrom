using System.Collections.Generic;

namespace TelegromV4.Models;

public class PrivateChat
{
    public int Id { get; set; }
    public string User1 { get; set; } = string.Empty;
    public string User2 { get; set; } = string.Empty;
    public List<Message> Messages { get; set; } = new List<Message>();
    
    public string GetOtherUser(string currentUser)
    {
        return currentUser == User1 ? User2 : User1;
    }
    
    // Добавьте это свойство для привязки в XAML
    public string OtherUser => User2; // Временно, для отображения в списке
}