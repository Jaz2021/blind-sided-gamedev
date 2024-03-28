using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject[] destroyButtons;
    public void Pressed(){
        NetworkManager.Singleton.StartClient();
        // foreach(var d in destroyButtons){
        //     Destroy(d);
        // }
        
        destroyButtons = null;
        
    }
}
