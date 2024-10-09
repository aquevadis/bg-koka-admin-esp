using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;

namespace AdminESP;
public partial class AdminESP {

    public enum PrintTo
    {
        Chat = 1,
        ChatAll,
        ChatAllNoPrefix,
        ConsoleError,
        ConsoleSucess,
        ConsoleInfo
    }

    public void SendMessageToSpecificChat(CCSPlayerController handle = null!, string msg = "",
        PrintTo print = PrintTo.Chat)
    {
        var chatPrefix = ReplaceColorTags($" {Config.ChatPrefix} {{DEFAULT}}\u226B");

        switch (print)
        {
            case PrintTo.Chat:
                if (!handle.IsValid) return;
                handle.PrintToChat($"{chatPrefix} {ReplaceColorTags(msg)}");
                return;
            case PrintTo.ChatAll:
                Server.PrintToChatAll($"{chatPrefix} {ReplaceColorTags(msg)}");
                return;
            case PrintTo.ChatAllNoPrefix:
                Server.PrintToChatAll($" {ReplaceColorTags(msg)}");
                return;
            case PrintTo.ConsoleError:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Debug] {msg}");
                Console.ResetColor();
                return;
            case PrintTo.ConsoleSucess:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[Debug] {msg}");
                Console.ResetColor();
                return;
            case PrintTo.ConsoleInfo:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[Debug] {msg}");
                Console.ResetColor();
                return;
        }
    }

    public string ReplaceColorTags(string input)
    {
        string[] colorPatterns =
        {
            "{DEFAULT}", "{WHITE}", "{GREEN}", "{LIGHTYELLOW}", "{LIGHTBLUE}", "{OLIVE}", "{LIME}",
            "{RED}", "{LIGHTPURPLE}", "{PURPLE}", "{GREY}", "{YELLOW}", "{GOLD}", "{SILVER}", "{BLUE}", "{DARKBLUE}",
            "{BLUEGREY}", "{MAGENTA}", "{LIGHTRED}", "{ORANGE}"
        };

        string[] colorReplacements =
        {
            $"{ChatColors.Default}", $"{ChatColors.White}", $"{ChatColors.Green}",
            $"{ChatColors.LightYellow}", $"{ChatColors.LightBlue}", $"{ChatColors.Olive}", $"{ChatColors.Lime}",
            $"{ChatColors.Red}", $"{ChatColors.LightPurple}", $"{ChatColors.Purple}", $"{ChatColors.Grey}",
            $"{ChatColors.Yellow}", $"{ChatColors.Gold}", $"{ChatColors.Silver}", $"{ChatColors.Blue}",
            $"{ChatColors.DarkBlue}", $"{ChatColors.BlueGrey}", $"{ChatColors.Magenta}", $"{ChatColors.LightRed}",
            $"{ChatColors.Orange}"
        };

        for (var i = 0; i < colorPatterns.Length; i++)
            input = input.Replace(colorPatterns[i], colorReplacements[i]);

        return input;
    }
    
}