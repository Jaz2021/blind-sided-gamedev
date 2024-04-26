using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    [SerializeField] private GameObject mainMenu, winScreen;
    
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
            p1.healthbar = GameObject.Find("HealthbarP1").GetComponent<Healthbar>();
            p2.healthbar = GameObject.Find("HealthbarP2").GetComponent<Healthbar>();
            p1.healthbar.setVisible();
            p1.healthbar.pc = p1;
            p2.healthbar.pc = p2;
            p2.healthbar.setVisible();
            var c = Camera.main.GetComponent<CameraFollow>();
            c.player1 = player1;
            c.player2 = player2;
            
        }
    }
    private GameObject win;
    public void GameOver(int winner){
        Time.timeScale = 0.01f;
        win = Instantiate(winScreen);
        win.transform.SetParent(GameObject.Find("MenuHome").transform);
        win.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        win.GetComponent<TextMeshProUGUI>().text = "Player " + winner + "wins!";
        Invoke("gameOverNextStep", 0.1f);
        

    }
    public void gameOverNextStep(){
        Destroy(win);
        var mm = Instantiate(mainMenu);
        mm.transform.SetParent(GameObject.Find("MenuHome").transform);
        player1.GetComponent<PlayerController>().healthbar.health = PlayerController.maxHealth;
        player2.GetComponent<PlayerController>().healthbar.health = PlayerController.maxHealth;

        Destroy(player1.gameObject);
        Destroy(player2.gameObject);
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
