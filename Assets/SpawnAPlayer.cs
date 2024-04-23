using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
   // Start is called before the first frame update
    public void pressed(){
        if(MultiplayerController.s.player2 == null){
            Instantiate(player);
        } else {
            Destroy(gameObject);
        }
    }
}
