using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LocalPlay : MonoBehaviour
{
    [SerializeField] private GameObject[] destroyButtons;
    [SerializeField] private GameObject playerPrefab;
    public void Pressed(){
        
        foreach(var d in destroyButtons){
            Destroy(d);
        }
        
        destroyButtons = null;
        Instantiate(playerPrefab);
        print("Button pressed");
        
    }
}
