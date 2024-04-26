using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAPlayer : MonoBehaviour
{
    [SerializeField] GameObject player;
   // Start is called before the first frame update
    public void pressed(){
        if(MultiplayerController.s.player2 == null){
            
            var p = Instantiate(player);
            var controller = p.GetComponent<PlayerController>();
            controller.npc = true;  

        } else {
            Destroy(gameObject);
        }
    }
}
