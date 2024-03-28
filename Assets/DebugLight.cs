using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DebugLight : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Light2D light;
    public bool lightingEnabled = false;
    public void Toggled(){
        lightingEnabled = !lightingEnabled;
        if(lightingEnabled){
            light.intensity = 1f;
        } else {
            light.intensity = 0.05f;
        }
    }
}
