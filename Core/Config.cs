using CounterStrikeSharp.API.Core;

namespace AdminESP;

public class Config : BasePluginConfig
{
    public bool DebugMode {get; set;} = true;
    public int VersionCfg { get; set; } = 1;
}