using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int ticksAlive = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ticksAlive -= 1;
        if(ticksAlive == 0){
            Destroy(gameObject);
        }
    }
}
