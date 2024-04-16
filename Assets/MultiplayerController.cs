using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
    public static MultiplayerController s = null;
    [Header("Set in runtime")]
    public GameObject player1 = null;
    public GameObject player2 = null;
    [Header("Set in inspector")]
    [SerializeField] private Vector3[] startPositions;
    [SerializeField] private Quaternion[] startRotations;
    [SerializeField] private float pitchP1, pitchP2; 
    
    private bool[] playerReady = {false, false};
    // Start is called before the first frame update
    void Start()
    {
        if(s == null){
            s = this;
        } else {
            Destroy(gameObject);
            //There can be only one
        }
    }
    /// <summary>
    /// Returns the correct start position for each player
    /// </summary>
    /// <param name="player">The current player number (1 indexed, so Player 1 uses 1)</param>
    /// <returns></returns>
    public Vector3 GetStartPosition(int player){
        return startPositions[player - 1];
    }
    public Quaternion GetStartRotation(int player){
        return startRotations[player - 1];
    }
    public void PlayerReady(int player){
        playerReady[player] = true;
        print(player);
        if(playerReady[1]){
            print("Should end setup");
            PlayerController p1 = player1.GetComponent<PlayerController>();
            PlayerController p2 = player2.GetComponent<PlayerController>();
            p1.pitch = pitchP1;
            p2.pitch = pitchP2;
            p1.SetOtherPlayer(p2);
            p2.SetOtherPlayer(p1);
            p1.EndSetup();
            p2.EndSetup();
            var c = Camera.main.GetComponent<CameraFollow>();
            c.player1 = player1;
            c.player2 = player2;
        }
    }
}
