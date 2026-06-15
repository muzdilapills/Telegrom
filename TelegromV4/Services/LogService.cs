using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class LogService
{
    private const string LogsFile = "logstelegrom.json";
    private List<LogEntry> _logs = new List<LogEntry>();
    private int _nextId = 1;

    public LogService()
    {
        LoadLogs();
    }

    private void LoadLogs()
    {
        if (File.Exists(LogsFile))
        {
            var json = File.ReadAllText(LogsFile);
            _logs = JsonSerializer.Deserialize<List<LogEntry>>(json) ?? new List<LogEntry>();
            if (_logs.Any())
                _nextId = _logs.Max(l => l.Id) + 1;
        }
    }

    private void SaveLogs()
    {
        var json = JsonSerializer.Serialize(_logs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(LogsFile, json);
    }

    public void AddLog(string userNickname, string action, string target = "", string details = "")
    {
        var log = new LogEntry
        {
            Id = _nextId++,
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            UserNickname = userNickname,
            Action = action,
            Target = target,
            Details = details
        };
        _logs.Add(log);
        SaveLogs();
    }

    public List<LogEntry> GetAllLogs()
    {
        return _logs.OrderByDescending(l => l.Timestamp).ToList();
    }

    public List<LogEntry> GetUserLogs(string nickname)
    {
        return _logs.Where(l => l.UserNickname == nickname).OrderByDescending(l => l.Timestamp).ToList();
    }
}