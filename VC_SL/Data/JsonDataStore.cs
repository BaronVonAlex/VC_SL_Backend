using System.Globalization;
using System.Text.Json;
using VC_SL.Models.Entities;

namespace VC_SL.Data;

/// <summary>
/// Loads Users/Winrates from the exported JSON files in Data/UserDataPublic and serves
/// them from memory. This replaces the old MySQL-backed ApplicationDbContext now that the
/// source database no longer exists.
///
/// Reads are served straight from the in-memory lists. Writes (CreateUser/UpdateUser/
/// UpdateWinrate) mutate the in-memory lists and are written back to the same JSON files,
/// so changes survive an app restart. Note: on hosts where the deployed files get replaced
/// on every deploy (e.g. a fresh Azure Web App publish), in-place edits won't survive a
/// redeploy - only a restart of the same deployment.
/// </summary>
public class JsonDataStore
{
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    private readonly object _gate = new();
    private readonly string _usersFilePath;
    private readonly string _winratesFilePath;

    private List<User> _users = [];
    private List<Winrate> _winrates = [];
    private int _nextWinrateId = 1;

    public JsonDataStore(IWebHostEnvironment env)
    {
        var dataDir = Path.Combine(env.ContentRootPath, "Data", "UserDataPublic");
        _usersFilePath = Path.Combine(dataDir, "Users.json");
        _winratesFilePath = Path.Combine(dataDir, "Winrates.json");

        Load();
    }

    private void Load()
    {
        _users = LoadUsers();
        _winrates = LoadWinrates();
        _nextWinrateId = _winrates.Count == 0 ? 1 : _winrates.Max(w => w.Id) + 1;
    }

    private List<User> LoadUsers()
    {
        if (!File.Exists(_usersFilePath))
            return [];

        var json = File.ReadAllText(_usersFilePath);
        var records = JsonSerializer.Deserialize<List<UserJsonRecord>>(json, ReadOptions) ?? [];

        return records.Select(r => new User
        {
            Id = r.Id,
            UsernameHistory = r.UsernameHistory,
            CreatedAt = ParseDate(r.CreatedAt),
            UpdatedAt = ParseDate(r.UpdatedAt)
        }).ToList();
    }

    private List<Winrate> LoadWinrates()
    {
        if (!File.Exists(_winratesFilePath))
            return [];

        var json = File.ReadAllText(_winratesFilePath);
        var records = JsonSerializer.Deserialize<List<WinrateJsonRecord>>(json, ReadOptions) ?? [];

        return records.Select(r => new Winrate
        {
            Id = r.Id,
            UserId = r.UserId,
            Month = r.Month,
            Year = r.Year,
            BaseAttackWinrate = r.BaseAttackWinrate,
            BaseDefenceWinrate = r.BaseDefenceWinrate,
            FleetWinrate = r.FleetWinrate,
            BaseAttackWin = r.BaseAttackWin,
            BaseAttackDraw = r.BaseAttackDraw,
            BaseAttackLoss = r.BaseAttackLoss,
            BaseDefenceWin = r.BaseDefenceWin,
            BaseDefenceDraw = r.BaseDefenceDraw,
            BaseDefenceLoss = r.BaseDefenceLoss,
            FleetWin = r.FleetWin,
            FleetDraw = r.FleetDraw,
            FleetLoss = r.FleetLoss
        }).ToList();
    }

    private static DateTime ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return DateTime.UtcNow;

        if (DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var exact))
            return exact;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : DateTime.UtcNow;
    }

    public List<User> GetUsers()
    {
        lock (_gate) return [.._users];
    }

    public User? GetUser(int id)
    {
        lock (_gate) return _users.FirstOrDefault(u => u.Id == id);
    }

    public void AddUser(User user)
    {
        lock (_gate)
        {
            _users.Add(user);
            SaveUsers();
        }
    }

    /// <summary>Call after mutating a User instance returned by GetUser to persist it.</summary>
    public void UpdateUser(User user)
    {
        lock (_gate) SaveUsers();
    }

    public List<Winrate> GetWinrates()
    {
        lock (_gate) return [.._winrates];
    }

    public Winrate? GetWinrate(int userId, int year, int month)
    {
        lock (_gate) return _winrates.FirstOrDefault(w => w.UserId == userId && w.Year == year && w.Month == month);
    }

    public Winrate AddWinrate(Winrate winrate)
    {
        lock (_gate)
        {
            winrate.Id = _nextWinrateId++;
            _winrates.Add(winrate);
            SaveWinrates();
            return winrate;
        }
    }

    /// <summary>Call after mutating a Winrate instance returned by GetWinrate to persist it.</summary>
    public void UpdateWinrate(Winrate winrate)
    {
        lock (_gate) SaveWinrates();
    }

    private void SaveUsers()
    {
        var records = _users.Select(u => new UserJsonRecord
        {
            Id = u.Id,
            UsernameHistory = u.UsernameHistory,
            CreatedAt = u.CreatedAt.ToString(DateFormat, CultureInfo.InvariantCulture),
            UpdatedAt = u.UpdatedAt.ToString(DateFormat, CultureInfo.InvariantCulture)
        }).ToList();

        File.WriteAllText(_usersFilePath, JsonSerializer.Serialize(records, WriteOptions));
    }

    private void SaveWinrates()
    {
        var records = _winrates.Select(w => new WinrateJsonRecord
        {
            Id = w.Id,
            UserId = w.UserId,
            Month = w.Month,
            Year = w.Year,
            BaseAttackWinrate = w.BaseAttackWinrate,
            BaseDefenceWinrate = w.BaseDefenceWinrate,
            FleetWinrate = w.FleetWinrate,
            BaseAttackWin = w.BaseAttackWin,
            BaseAttackDraw = w.BaseAttackDraw,
            BaseAttackLoss = w.BaseAttackLoss,
            BaseDefenceWin = w.BaseDefenceWin,
            BaseDefenceDraw = w.BaseDefenceDraw,
            BaseDefenceLoss = w.BaseDefenceLoss,
            FleetWin = w.FleetWin,
            FleetDraw = w.FleetDraw,
            FleetLoss = w.FleetLoss
        }).ToList();

        File.WriteAllText(_winratesFilePath, JsonSerializer.Serialize(records, WriteOptions));
    }
}
