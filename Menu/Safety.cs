using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Parrot.client.Menu
{
    internal class Safetyy
    {
        private static float RPCDelay = 0.3f;
        public static void RPCProtection()
        {
            if (!PhotonNetwork.InRoom)
                return;
        
            try
            {
                
                var p = PhotonNetwork.NetworkingClient?.LoadBalancingPeer;
                if (p != null)
                {
                    p.SentCountAllowance = int.MaxValue;
                    p.QuickResendAttempts = 3;
                    p.CrcEnabled = false;
                    p.UseByteArraySlicePoolForEvents = false;
                    p.TrafficStatsEnabled = false;
                    p.TrafficStatsReset();
                    p.SendOutgoingCommands();
                    try
                    {
                        var t = p.GetType();
                        var q = t.GetField("outgoingStreamQueue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var q2 = q?.GetValue(p) as System.Collections.IList;
                        q2?.Clear();
                        var c = t.GetField("commandList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var c2 = c?.GetValue(p) as System.Collections.IList;
                        c2?.Clear();
                        var resentField = t.GetField("resentCommandsCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        resentField?.SetValue(p, 0);
                    }
                    catch { }
                }
                
                MonkeAgent.instance.rpcErrorMax = int.MaxValue;
                MonkeAgent.instance.rpcCallLimit = int.MaxValue;
                MonkeAgent.instance.logErrorMax = int.MaxValue;

                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;

                PhotonNetwork.SendAllOutgoingCommands();
                if (Time.time > RPCDelay)
                {
                    MonkeAgent.instance.userRPCCalls.Clear();
                    PhotonNetwork.RaiseEvent(200, new object[] { }, new Photon.Realtime.RaiseEventOptions
                    {
                        CachingOption = Photon.Realtime.EventCaching.RemoveFromRoomCache,
                        Flags = new Photon.Realtime.WebFlags(1),
                        Receivers = Photon.Realtime.ReceiverGroup.All,
                    }, new ExitGames.Client.Photon.SendOptions
                    {
                        DeliveryMode = ExitGames.Client.Photon.DeliveryMode.UnreliableUnsequenced,
                        Encrypt = false,
                        Reliability = false,
                    });
                    MonkeAgent.RPCCallTracker rpcct = new MonkeAgent.RPCCallTracker();
                    rpcct.RPCCallsMax = int.MaxValue;
                     
                    
                        rpcct.RPCCalls = 0; 
                    
                    
                }
            }
            catch {}
            
        }
    }
}
