# Photon Bolt Free - Facepunch.Steamworks Ticket Authorization
A authorization example that shows accepting users that only have a valid ticket using Facepunch.Steamworks.

## Notes

This is intended for games where players host games are verify other players, it would need to be modified if it was a dedicated server verifying players.

This does not handle initializing steam fully, you should still stop the players from even trying to play if steam fails to initialize.

This was made a while ago and uses Facepunch.Steamworks.Unity, which seems to no longer be a thing. This still works if you use the Facepunch.Steamworks from inside this repo. but wont work with the latest Facepunch.Steamworks yet. Planning on updating it soon.

The only important files inside the repo. are inside the Assets/Scripts folder and the Assets/Facepunch.Steamworks folder, everything else is bolt files.

Also, this requires unsafe code to be turned on in the unity player settings.

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