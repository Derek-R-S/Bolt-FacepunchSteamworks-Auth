using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdpKit;
using Bolt;

public class SteamConnectToken : Bolt.IProtocolToken
{
    public ulong steamid;
    public byte[] ticket;

    public void Read(UdpPacket packet){
        int ticketLength = packet.ReadInt();
        ticket = packet.ReadByteArray(ticketLength);
        steamid = packet.ReadULong();
    }

    public void Write(UdpPacket packet){
        packet.WriteInt(ticket.Length);
        packet.WriteByteArray(ticket);
        packet.WriteULong(steamid);
    }

    public SteamConnectToken(){}
	
    public SteamConnectToken(byte[] ticket, ulong steamid){
        this.ticket = ticket;
        this.steamid = steamid;
    }
}
