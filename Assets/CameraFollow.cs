using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private float stageMaxX;
    [SerializeField]
    private float followHeight; //The height at which the camera should follow a specific player rather than the center point between the two
    private Vector3 cameraMaxPoint;
    private Vector3 playerMaxPoint;
    public GameObject player1 = null, player2 = null;

    private void Start() {
        print(GetComponent<Camera>().orthographicSize);
        cameraMaxPoint = new Vector3(stageMaxX - GetComponent<Camera>().orthographicSize, 0, transform.position.z);
        
    }
    // Update is called once per frame
    void Update()
    {
        if(player1 == null || player2 == null){
            return;
        }

        transform.position = new Vector3(player1.transform.position.x * 0.5f + player2.transform.position.x * 0.5f,transform.position.y, transform.position.z);
        if(transform.position.x > cameraMaxPoint.x){
            print("Out of bounds, too high");
            transform.position = cameraMaxPoint;
        } else if(transform.position.x < -cameraMaxPoint.x){
            print("Out of bounds, too low");
            transform.position = -cameraMaxPoint;
        }
        if(player1.transform.position.x > stageMaxX){
            player1.transform.position = new Vector3(stageMaxX, player1.transform.position.y, player1.transform.position.z);
        } else if(player1.transform.position.x < -stageMaxX){
            player1.transform.position = new Vector3(-stageMaxX, player1.transform.position.y, player1.transform.position.z);
        }

        if(player2.transform.position.x > stageMaxX){
            player2.transform.position = new Vector3(stageMaxX, player2.transform.position.y, player2.transform.position.z);
        } else if(player2.transform.position.x < -stageMaxX){
            player2.transform.position = new Vector3(-stageMaxX, player2.transform.position.y, player2.transform.position.z);
        }
    }
}
