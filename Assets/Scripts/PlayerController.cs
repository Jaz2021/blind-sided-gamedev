using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;


// using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Windows;
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private const float secondsPerTick = 1f/60f;
    public float Health {
        get {
            return health;
        }
        set {
            healthbar.health = value;
            health = value;
            if(value <= 0){
                print("Game over!");
                
            }
        }
    }
    [SerializeField]
    private float health = maxHealth;
    public const float maxHealth = 100f;

    public PlayerController otherPlayer {
        get;
        private set;
    }
    public int playerNum;
    private Animator animator;
    [Header("Set in inspector")]
    [Header("Sound effects")]
    [SerializeField]
    private AudioClip walkSound;


    [Header("Character stats")]
    [SerializeField]
    private float gravity;
    [SerializeField]
    public float jumpVel;
    [SerializeField]
    private float baseHealth;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float hitstunFriction; //The amount that the x velocity should slow down each tick while in hitstun. Outside of hitstun, you won't slow down in the air

    public Healthbar healthbar;

    
    // [HideInInspector]
    public bool actionable = false;
    [HideInInspector]
    public float pitch;
    private Vector2 stick = Vector2.zero;
    private new bool light = false;
    private bool heavy = false;
    private bool special = false;
    private bool facingRight {
        get {
            if(otherPlayer != null){
                if(otherPlayer.transform.position.x > transform.position.x){
                    transform.rotation = MultiplayerController.s.GetStartRotation(1);
                    return true;
                } else {
                    transform.rotation = MultiplayerController.s.GetStartRotation(2);
                    return false;
                }
                // return otherPlayer.transform.position.x > transform.position.x;
            } else {
                return true;
            }
        }
    }
    public bool gameStarted = false;
    public delegate void Input(Vector2 movement, bool light, bool heavy, bool special, bool facingRight);
    public Input inputFunc{
        get{
            return _inputFunc;
        }
        set {
            _inputFunc = value;
            // print("Input func changed to " + value.ToString());
        }
    }
    private Input _inputFunc;

    public float yVel = 0;
    public float xVel = 0f;
    private new AudioSource audio;
    private int curHitstun = 0;
    
    private void Update() {
        
    }
    public void Hit(Vector3 knockback, float damage, int hitstun){
        yVel = knockback.y;
        print(playerNum + " got hit");
    }
    void OnTriggerEnter2D(Collider2D other){
        // print("Something entered the trigger");
        if(other.tag == "hitbox"){
            // print("Hitbox entered the trigger");
            
            var h = other.GetComponent<Hitbox>();
            if(h.player == this){
                // print("it was this player");
                return;
            }
            // print(h.knockback);
            Health -= h.damage;
            curHitstun = h.hitstun;
            if(facingRight){

                xVel = -h.knockback.x;

            } else {
                xVel = h.knockback.x;
            }
            yVel = h.knockback.y;
            var hitEffect = Instantiate(h.hitEffect);
            hitEffect.transform.position = h.transform.position;
            hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3(xVel, yVel));
        } else {
            // print(other.tag);
        }
    }
    public void setReady(){
        print("Setting ready");
        MultiplayerController.s.PlayerReady(playerNum);
    }
    private void Start() {
        animator = GetComponentInChildren<Animator>();
        if(animator == null){
            throw new System.Exception("No animator found!");
        }
        actionable = false;
        print("Creating a new player");
        if(MultiplayerController.s.player1 == null){
            playerNum = 0;
            MultiplayerController.s.player1 = gameObject;
            transform.position = MultiplayerController.s.GetStartPosition(1);
            transform.rotation = MultiplayerController.s.GetStartRotation(1);
            // MultiplayerController.s.PlayerReady(1);
        } else if(MultiplayerController.s.player2 == null){
            playerNum = 1;
            MultiplayerController.s.player2 = gameObject;
            transform.position = MultiplayerController.s.GetStartPosition(2);
            transform.rotation = MultiplayerController.s.GetStartRotation(2);
            // MultiplayerController.s.PlayerReady(2);
        } else {
            throw new System.Exception("Error: Attempted to have a third player join");
        }
        audio = GetComponent<AudioSource>();
    }
    public void playAnimation(string animation, float transitionTime){
        animator.CrossFadeInFixedTime(animation, transitionTime);
    }
    public void EndSetup(){
        print("Exited setup");
        gameObject.layer = 6 + playerNum;
        gameStarted = true;
    }
    public void ActivateIdleFunc(){
        inputFunc = GetComponentInChildren<AnimationEventManager>().IdleInput;
    }
    public void SetOtherPlayer(PlayerController player){
        otherPlayer = player;
    }
    public void lightAttack(){
        light = true;
        heavy = false;
        special = false;
    }
    public void heavyAttack(){
        heavy = true;
        special = false;
        light = false;
    }
    public void playAudio(AudioClip audio, bool directional = true){
        if(directional){
            this.audio.panStereo = (this.transform.position.x - Camera.main.transform.position.x) * 0.25f;
        } else {
            this.audio.panStereo = 0f;
        }
        this.audio.PlayOneShot(audio);
    }
    public void moveInput(InputAction.CallbackContext context){
        stick = context.ReadValue<Vector2>();

    }
    public void zeroXVelocity(){
        xVel = 0;
    }
    /// <summary>
    /// Sets the players velocity to the speed they walk backwards while blocking
    /// </summary>
    /// <param name="facingRight">The direction the motion should be in, true for right</param>
    public void addBlockXVel(){
        // Block velocity will be half of walk vel
        if(!facingRight){
            xVel = 0.5f * walkSpeed;
        }
        else {
            xVel = -0.5f * walkSpeed;
        }
    }
    /// <summary>
    /// Sets the player's x velocity to the speed they walk forwards/jump
    /// </summary>
    /// <param name="facingRight">The direction the motion should be in, true for right</param>
    public void addWalkXVel(){
        // print("Adding walking velocity");
        if(facingRight){
            xVel = walkSpeed;
        } else {
            xVel = -walkSpeed;
        }
    }
    public void zeroXVel(){
        xVel = 0;
    }
    private void setPos(){
        transform.position = new Vector3(transform.position.x + xVel, Mathf.Max(transform.position.y + yVel, -2.5f));
        yVel -= gravity;
        if(curHitstun != 0){
            xVel -= hitstunFriction;
        }
        
    }
    private void FixedUpdate() {
        setPos();
        if(actionable && curHitstun <= 0){
            inputFunc?.Invoke(stick, light, heavy, special, facingRight);
        } else {
            curHitstun -= 1;
        }

        light = false;
        heavy = false;
        special = false;
        
    }
}
