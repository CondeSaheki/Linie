# Linie
A simple Discord app for managing Final Fantasy XIV community events.
It allows you to create events, collect participant info and export data for raid planning.

## Features
- `/event_create` – Create a new event with interactive menus.
- `/event_list` – List all currently tracked events.
- `/event_info` – Show detailed info of an event (job counts, participants, open/closed status).
- `/event_remove` – Close an event and delete its messages.
- `/event_export` – Export event participants as CSV.
- `/ping` – Get bot latency.

## Installation

### Requirements
- Lastest [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
- A Discord app token (see [Discord Developer Portal](https://discord.com/developers/applications))

### Setup

1. Clone this repository:
    ```bash
    git clone https://github.com/yourname/linie.git
    cd linie
    ```

2. Create a `Linie.cfg` file in the root folder with the following structure:
    ```json
    {
        "DiscordToken": "YOUR_DISCORD_BOT_TOKEN"
    }
    ```

3. Run the bot:
    ```bash
    dotnet run
    ```

4. Invite the bot to your Discord server using its OAuth2 link with the following scopes:
    - `bot`
    - `applications.commands`

## Usage
* Once the bot is online, use `/event_create` to start a new event.
* Use `/event_info` to see job distribution, `/event_list` to check active events, and `/event_export` to download CSVs.

## File Structure

```bash
Linie/
 ├─ Command/          # Slash command
 ├─ Component/        # Select menus & buttons
 ├─ Modal/            # Modal (popup form)
 ├─ Event/            # Persistence, Event & participant structures
 ├─ Commands.cs       # Command dispatcher
 ├─ Components.cs     # Component dispatcher
 ├─ Modals.cs         # Modal dispatcher
 ├─ Logger.cs         # Logging to console & file
 ├─ Program.cs        # Main bot entrypoint
```
