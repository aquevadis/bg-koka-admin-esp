using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace AdminESP;

public class Config : BasePluginConfig
{
    [JsonPropertyName("debug_mode")] public bool DebugMode { get; set; } = true;
    [JsonPropertyName("chat_prefix")] public string ChatPrefix { get; set; } = "{GREEN}[AdminESP]";
    [JsonPropertyName("hide_from_spectator_list")] public bool HideAdminSpectators { get; set; } = true;

}