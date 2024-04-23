using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    public PlayerController pc;
    [SerializeField] private bool isPlayer1;
    void Start(){
        GetComponent<Image>().enabled = false;
    }
    public void setVisible(){
        GetComponent<Image>().enabled = true;
    }
    public float health {
        set {
            if(pc != null){
                GetComponent<RectTransform>().localScale = new Vector3(value / PlayerController.maxHealth, 1, 1);
            }
        }
    }
}
