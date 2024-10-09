## Admin ESP

Simple admin ESP plugin

## Installation
Like any other CSS plugin; Don't forget to add the `AdminESP.json` from the `gamedata` folder to your server's gamedata folder

## Commands
- `css_esp spec` - `Toggle ESP` - toggle seeing players behind walls while spec(allowed only for flag @css/ban and spectators team)
- `css_esp dead` - `Toggle ESP` - toggle seeing players behind walls while dead(allowed only if AllowDeadAdminESP is set to true)

## Configuration (`counterstrikesharp/configs/plugins/AdminESP` and auto-generated)
- "debug_mode" - `not used`
- "chat_prefix" - The chat prefix that will appear when using the plugin's command/s
- "admin_flag" - default `@css/ban`
- "hide_from_spectator_list" - Whether to hide the Admin that is using the ESP from a cheats spectator's list(tested on neverlose)
- "allow_esp_when_dead" - Wether to allow the dead admins to enter esp
