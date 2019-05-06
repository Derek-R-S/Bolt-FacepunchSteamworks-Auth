# Photon Bolt - Facepunch.Steamworks Ticket Auth
A authorization example that shows accepting users that only have a valid ticket using Facepunch.Steamworks.

## What

Tickets verify users actually own the game, are who they say they are, and are not banned. Tickets are essential in almost every steam game.

This is an example which handles accepting users and validating their tickets with Facepunch.Steamworks.

## Notes

This is intended for games where players host games and verify other players, it would need to be modified if it was a dedicated server verifying players.

This does not handle failed steam inits, you should still stop the players from even trying to play if steam fails to initialize.

The only important files inside the repo. are inside the Assets/Scripts, everything else is Bolt/Facepunch.Steamworks files.

## Use

To use this, simply create a new gameobject and attach the SteamManager script to it and put in the correct appid.
Make sure to also set your Accept Mode to manual in the bolt settings, the SteamManager will handle accepting clients.

When connecting to a server all you need to do is include the SteamConnectToken, you can get an already setup token by calling
```c#
SteamManager.instance.GetConnectToken()
```
example connecting:
```c#
BoltNetwork.Connect(match, SteamManager.instance.GetConnectToken());
```

Accepted connections get an acceptToken containing a SteamToken which has the steamid of the user.
