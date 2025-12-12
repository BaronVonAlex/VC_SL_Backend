using System.Text.Json;

namespace VC_SL.Services;

public static class UsernameHistoryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static string AddUsername(string currentHistoryJson, string newUsername)
    {
        var history = ParseHistory(currentHistoryJson);
        var newEntries = ParseNewUsername(newUsername);

        foreach (var entry in newEntries.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            var trimmed = entry.Trim();
            if (!history.Contains(trimmed))
                history.Add(trimmed);
        }

        return JsonSerializer.Serialize(history);
    }

    public static List<string> DeserializeHistoryToList(string historyJson)
    {
        return ParseHistory(historyJson);
    }

    private static List<string> ParseHistory(string historyJson)
    {
        if (string.IsNullOrWhiteSpace(historyJson))
            return [];

        return TryDeserialize<List<string>>(historyJson)
               ?? (TryDeserialize<string>(historyJson) is string single && !string.IsNullOrEmpty(single)
                   ? [single]
                   : []);
    }

    private static List<string> ParseNewUsername(string newUsername)
    {
        if (string.IsNullOrWhiteSpace(newUsername))
            return [];

        var trimmed = newUsername.Trim();

        if (IsJsonLike(trimmed))
            return TryDeserialize<List<string>>(newUsername)
                   ?? (TryDeserialize<string>(newUsername) is string single && !string.IsNullOrEmpty(single)
                       ? [single]
                       : [GetCleanedValue(trimmed)]);

        return [newUsername];
    }

    private static T? TryDeserialize<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch
        {
            return default;
        }
    }

    private static bool IsJsonLike(string value)
    {
        return (value.StartsWith('[') && value.EndsWith(']')) ||
               (value.StartsWith('"') && value.EndsWith('"'));
    }

    private static string GetCleanedValue(string value)
    {
        return value.Trim('"', '[', ']');
    }
}