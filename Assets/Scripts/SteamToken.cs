using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UdpKit;
using Bolt;

public class SteamToken : Bolt.IProtocolToken
{
    public SteamId steamid;

    public void Read(UdpPacket packet){
        steamid = packet.ReadULong();
    }

    public void Write(UdpPacket packet){
        packet.WriteULong(steamid);
    }

    public SteamToken(){}
    public SteamToken(ulong steamid){
        this.steamid = steamid;
    }
}
