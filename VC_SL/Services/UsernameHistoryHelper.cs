using System.Text.Json;

namespace VC_SL.Services;

public static class UsernameHistoryHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    }; // ignore case sensetivity here

    // here we pass current history json and new name
    public static string AddUsername(string currentHistoryJson, string newUsername)
    {
        var history = new List<string>();

        if (!string.IsNullOrWhiteSpace(currentHistoryJson))
        {
            try
            {
                history = JsonSerializer.Deserialize<List<string>>(currentHistoryJson, JsonOptions) ?? [];
            }
            catch
            {
                try
                {
                    var single = JsonSerializer.Deserialize<string>(currentHistoryJson, JsonOptions);
                    if (!string.IsNullOrEmpty(single))
                        history.Add(single);
                }
                catch
                {
                    history = [];
                }
            }
        } // if dis is valid json then it deserializes it into a List<string>, if fails then it's single string username "a" instead of ["a"], if both fail then empty ;d

        var newEntries = new List<string>();
        if (!string.IsNullOrWhiteSpace(newUsername))
        {
            var trimmed = newUsername.Trim();

            if ((trimmed.StartsWith($"[") && trimmed.EndsWith($"]")) || (trimmed.StartsWith($"\"") && trimmed.EndsWith($"\"")))
            {
                try
                {
                    var arr = JsonSerializer.Deserialize<List<string>>(newUsername, JsonOptions);
                    if (arr is { Count: > 0 })
                    {
                        newEntries.AddRange(arr);
                    }
                    else
                    {
                        var single = JsonSerializer.Deserialize<string>(newUsername, JsonOptions);
                        if (!string.IsNullOrEmpty(single))
                            newEntries.Add(single);
                    }
                }
                catch
                {
                    var cleaned = trimmed.Trim('\"', '[', ']');
                    if (!string.IsNullOrWhiteSpace(cleaned))
                        newEntries.Add(cleaned);
                }
            }
            else
            {
                newEntries.Add(newUsername);
            }
        }

        foreach (var entry in newEntries.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)))
        {
            if (!history.Contains(entry))
                history.Add(entry);
        } // dis here trimps spaces from each username, skip empties and add if it's unique

        return JsonSerializer.Serialize(history); //back to json
    }

    public static List<string> DeserializeHistoryToList(string historyJson) // dis thing just deserializes json to return string list
    {
        if (string.IsNullOrWhiteSpace(historyJson))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(historyJson, JsonOptions) ?? [];
        }
        catch
        {
            try
            {
                var single = JsonSerializer.Deserialize<string>(historyJson, JsonOptions);
                if (!string.IsNullOrEmpty(single))
                    return [single];
            }
            catch { }

            return [];
        }
    }
}