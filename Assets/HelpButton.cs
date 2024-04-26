using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    // Start is called before the first frame update
    private bool pressed = false;
    [SerializeField]
    private TextMeshProUGUI text;
    public void Pressed(){
        pressed = !pressed;
        text.enabled = pressed;
    }
}
