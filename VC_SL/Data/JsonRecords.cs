using System.Text.Json.Serialization;

namespace VC_SL.Data;

/// <summary>Shape of a row in Data/UserDataPublic/Users.json.</summary>
public class UserJsonRecord
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username_history")]
    public string UsernameHistory { get; set; } = "[]";

    [JsonPropertyName("createdAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("updatedAt")]
    public string UpdatedAt { get; set; } = string.Empty;
}

/// <summary>Shape of a row in Data/UserDataPublic/Winrates.json.</summary>
public class WinrateJsonRecord
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("month")]
    public int Month { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }

    [JsonPropertyName("baseAttackWinrate")]
    public float? BaseAttackWinrate { get; set; }

    [JsonPropertyName("baseDefenceWinrate")]
    public float? BaseDefenceWinrate { get; set; }

    [JsonPropertyName("fleetWinrate")]
    public float? FleetWinrate { get; set; }

    [JsonPropertyName("baseAttackWin")]
    public int? BaseAttackWin { get; set; }

    [JsonPropertyName("baseAttackDraw")]
    public int? BaseAttackDraw { get; set; }

    [JsonPropertyName("baseAttackLoss")]
    public int? BaseAttackLoss { get; set; }

    [JsonPropertyName("baseDefenceWin")]
    public int? BaseDefenceWin { get; set; }

    [JsonPropertyName("baseDefenceDraw")]
    public int? BaseDefenceDraw { get; set; }

    [JsonPropertyName("baseDefenceLoss")]
    public int? BaseDefenceLoss { get; set; }

    [JsonPropertyName("fleetWin")]
    public int? FleetWin { get; set; }

    [JsonPropertyName("fleetDraw")]
    public int? FleetDraw { get; set; }

    [JsonPropertyName("fleetLoss")]
    public int? FleetLoss { get; set; }
}
