using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facepunch.Steamworks;
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
            new Client(appID);

            if(Client.Instance == null || !Client.Instance.IsValid){
                Debug.LogError("Failed starting steam.");
                // Display a message or stop the user from playing.
            }else{
                Client.Instance.RegisterCallback<SteamNative.ValidateAuthTicketResponse_t>(OnTicketResponse);
            }
        }else{
            Destroy(gameObject);
        }
    }

    void OnDestroy(){
        Client.Instance.Dispose();
    }

    void Update(){
        if(Client.Instance != null && Client.Instance.IsValid)
            Client.Instance.RunCallbacks();
    }

    void OnTicketResponse(SteamNative.ValidateAuthTicketResponse_t data){
        if(pendingConnections.ContainsKey(data.SteamID)){
            if(data.AuthSessionResponse == SteamNative.AuthSessionResponse.OK){
                BoltNetwork.Accept(pendingConnections[data.SteamID], new SteamToken(data.SteamID));
                pendingConnections.Remove(data.SteamID);
                BoltLog.Info("Ticket valid, accepted connection.");
            }else{
                BoltNetwork.Refuse(pendingConnections[data.SteamID]);
                pendingConnections.Remove(data.SteamID);
                BoltLog.Info("Refused user for reason: " + data.AuthSessionResponse);
            }
        }
    }

    public unsafe override void ConnectRequest(UdpEndPoint endpoint, Bolt.IProtocolToken token){
        SteamConnectToken connectToken = token as SteamConnectToken;

        fixed(byte* ticket = connectToken.ticket){
            SteamNative.BeginAuthSessionResult result = Client.Instance.native.user.BeginAuthSession((IntPtr)ticket, connectToken.ticket.Length, connectToken.steamid);
            
            if(result != SteamNative.BeginAuthSessionResult.OK){
                BoltNetwork.Refuse(endpoint);
                BoltLog.Info("Refused user with invalid ticket.");
            }else{
                if(pendingConnections.ContainsKey(connectToken.steamid))
                    pendingConnections.Remove(connectToken.steamid);
                    
                pendingConnections.Add(connectToken.steamid, endpoint);
            }
        }
    }

    public SteamConnectToken GetConnectToken(){
        if(Client.Instance == null || !Client.Instance.IsValid){
            Debug.LogError("Error, Steam is not initialized.");
            return null;
        }

        SteamConnectToken token = new SteamConnectToken(Client.Instance.Auth.GetAuthSessionTicket().Data, Client.Instance.SteamId);
        return token;
    }
}