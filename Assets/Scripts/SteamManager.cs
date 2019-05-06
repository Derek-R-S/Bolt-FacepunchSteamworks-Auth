using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UdpKit;

public class SteamManager : Bolt.GlobalEventListener
{
    public static SteamManager instance;
    public uint appID = 480;
    private Dictionary<ulong, UdpEndPoint> pendingConnections = new Dictionary<ulong, UdpEndPoint>();

    void Awake(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
            SteamClient.Init(appID);
            if(!SteamClient.IsValid || !SteamClient.IsLoggedOn){
                Debug.LogError("Failed starting steam.");
                // Display a message or stop the user from playing.
            }else{
                SteamUser.OnValidateAuthTicketResponse += OnTicketResponse;
            }
        }else{
            Destroy(gameObject);
        }
    }

    void OnDestroy(){
        SteamClient.Shutdown();
    }

    void Update(){
        if(SteamClient.IsValid)
            SteamClient.RunCallbacks();
    }

    void OnTicketResponse(SteamId steamid, SteamId owner, AuthResponse response){
        if(pendingConnections.ContainsKey(steamid)){
            if(response == AuthResponse.OK){
                BoltNetwork.Accept(pendingConnections[steamid], new SteamToken(steamid));
                pendingConnections.Remove(steamid);
                BoltLog.Info("Ticket valid, accepted connection.");
            }else{
                BoltNetwork.Refuse(pendingConnections[steamid]);
                pendingConnections.Remove(steamid);
                BoltLog.Info("Refused user for reason: " + response);
            }
        }
    }

    public unsafe override void ConnectRequest(UdpEndPoint endpoint, Bolt.IProtocolToken token){
        SteamConnectToken connectToken = token as SteamConnectToken;

        BeginAuthResult result = SteamUser.BeginAuthSession(connectToken.ticket, connectToken.steamid);

        if(result == BeginAuthResult.OK){
            if(pendingConnections.ContainsKey(connectToken.steamid))
                pendingConnections.Remove(connectToken.steamid);
                    
            pendingConnections.Add(connectToken.steamid, endpoint);
        }else{
            BoltNetwork.Refuse(endpoint);
            BoltLog.Info("Refused user with invalid ticket.");
        }
    }

    public SteamConnectToken GetConnectToken(){
        if(!SteamClient.IsValid){
            Debug.LogError("Error, Steam is not initialized.");
            return null;
        }

        SteamConnectToken token = new SteamConnectToken(SteamUser.GetAuthSessionTicket().Data, SteamClient.SteamId.Value);
        return token;
    }
}