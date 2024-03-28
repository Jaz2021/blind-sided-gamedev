using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RpcTest : MonoBehaviour
{
    // public override void OnNetworkSpawn(){
    //     if(!IsServer && IsOwner){
    //         TestServerRpc(0, NetworkObjectId);
    //     }
    // }
    [Rpc(SendTo.ClientsAndHost)]
    void TestClientRpc(int value, ulong sourceNetworkObjectId){
        Debug.Log($"Client received the RPC #{value} on NetworkObject {sourceNetworkObjectId}");
    //     if(IsOwner){
    //         TestServerRpc(value + 1, sourceNetworkObjectId);
    //     }
    }
    [Rpc(SendTo.Server)]
    void TestServerRpc(int value, ulong sourceNetworkObjectId){
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
}
