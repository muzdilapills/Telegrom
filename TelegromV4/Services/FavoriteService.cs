using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class FavoriteService
{
    private const string FavoritesFile = "Favorites.json";
    private List<FavoriteMessage> _favorites = new List<FavoriteMessage>();
    private int _nextId = 1;

    public FavoriteService()
    {
        LoadFavorites();
    }

    private void LoadFavorites()
    {
        if (File.Exists(FavoritesFile))
        {
            var json = File.ReadAllText(FavoritesFile);
            _favorites = JsonSerializer.Deserialize<List<FavoriteMessage>>(json) ?? new List<FavoriteMessage>();
            if (_favorites.Any())
                _nextId = _favorites.Max(f => f.Id) + 1;
        }
    }

    private void SaveFavorites()
    {
        var json = JsonSerializer.Serialize(_favorites, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FavoritesFile, json);
    }

    public void AddToFavorites(string userNickname, string message)
    {
        _favorites.Add(new FavoriteMessage
        {
            Id = _nextId++,
            UserNickname = userNickname,
            Message = message,
            Timestamp = DateTime.Now
        });
        SaveFavorites();
    }

    public List<FavoriteMessage> GetUserFavorites(string userNickname)
    {
        return _favorites.Where(f => f.UserNickname == userNickname).OrderByDescending(f => f.Timestamp).ToList();
    }
}