using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

// using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private const float secondsPerTick = 1f/60f;
    private PlayerController otherPlayer;
    private Animator animator;
    [Header("Set in inspector")]
    [Header("Action timings")]
    [Header("Frames in this context refer to game ticks, using frames because thats the standard term in fighting games")]
    [SerializeField]
    private int crouchStartupFrames; //The number of frames before the 
    [SerializeField]
    private int crouchEndFrames;
    [SerializeField]
    private int jumpStartupFrames;

    [SerializeField]
    private State[] cancellableIntoCrouch; //What states can be cancelled into crouch
    [SerializeField]
    private State[] cancellableIntoIdle;
    [SerializeField]
    private State[] cancellableIntoJump;
    [SerializeField]
    private State[] cancellableIntoWalk;



    [Header("Character stats")]
    [SerializeField]
    private float gravity;
    [SerializeField]
    private float jumpVel;
    [SerializeField]
    private float baseHealth;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;


    
    [HideInInspector]
    public bool actionable = false;
    [SerializeField]
    public enum State{
        Idle, // 0
        CrouchStart, //1
        Crouch, // 2
        CrouchEnd, // 3
        JumpStart, // 4
        Airborn, // 5
        WalkRight, // 6
        WalkLeft, // 7
        BlockReady // 8
    }


    public State state {
        get {
            return _state;
        }
        set {
            if(stateAnimation[(int)value] == "Hold"){
                //Ignore animation
            } else {
                print("Changing animation to: " + stateAnimation[(int)value]);
                //Get transition time
                float transTime = 0f;
                foreach(var transitionTime in stateAnimationTransition){
                    if(transitionTime.fromState == state && transitionTime.toState == value){
                        transTime = transitionTime.transitionTime;
                    }
                }
                animator.CrossFade(stateAnimation[(int)value], transTime);
                //Play the proper animation
            }
            _state = value;
        }
    }
    [Header("Indexes correspond to State enum, see code for indexes")]
    [Header("Use \"Hold\" when an animation will be the same as the previous state, these will be ignored when the state changes")]
    
    [SerializeField] private string[] stateAnimation = new string[Enum.GetValues(typeof(State)).Length]; 
    [Serializable]
    struct AnimationTransitionTime{
        public State fromState;
        public State toState;
        public float transitionTime;
    }
    [SerializeField] private AnimationTransitionTime[] stateAnimationTransition = new AnimationTransitionTime[Enum.GetValues(typeof(State)).Length]; 
    [SerializeField]

    private State _state = State.Idle;
    /// <summary>
    /// The state that is next to come up. Typically is based on what button is being held
    /// </summary>
    public State bufferState = State.Idle;
    private int framesActive = 0;
    public float yVel = 0;
    
    private Quaternion defaultRot;
    private Quaternion otherRot;
    private bool Airborn {
        get{
            return transform.position.y != -2.5f;
        }
    }
    public float xVel = 0f;
    private float bufferXVel = 0f;
    private void Update() {
        if(transform.position.y < -2.5f){
            print("Went out of bounds");
            print(transform.position.y);
            transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
        }
    }
    private void Start() {
        animator = GetComponentInChildren<Animator>();
        actionable = false;
        print("Creating a new player");
        if(MultiplayerController.s.player1 == null){
            MultiplayerController.s.player1 = gameObject;
            transform.position = MultiplayerController.s.GetStartPosition(1);
            transform.rotation = MultiplayerController.s.GetStartRotation(1);
            defaultRot = transform.rotation;
            otherRot = MultiplayerController.s.GetStartRotation(2);
            MultiplayerController.s.PlayerReady(1);
        } else if(MultiplayerController.s.player2 == null){
            MultiplayerController.s.player2 = gameObject;
            transform.position = MultiplayerController.s.GetStartPosition(2);
            transform.rotation = MultiplayerController.s.GetStartRotation(2);
            otherRot = transform.rotation;
            defaultRot = MultiplayerController.s.GetStartRotation(1);
            MultiplayerController.s.PlayerReady(2);
        } else {
            throw new System.Exception("Error: Attempted to have a third player join");
        }
    }
    public void SetOtherPlayer(PlayerController player){
        otherPlayer = player;
    }
    public void moveInput(InputAction.CallbackContext context){
        Vector2 input = context.ReadValue<Vector2>();
        //Up down should take priority in state over left/right
        // Left/right movements depend on up/down state but not vice versa
        //We never set our State here, only the bufferState
        State startBuffer = bufferState;
        if(input.x == 0f){
            
            if(input.y == 0f){
                if(state == State.Crouch || state == State.CrouchStart){
                    bufferState = State.CrouchEnd;
                } else {
                    bufferState = State.Idle;
                }
            } else if(input.y < 0f){
                // print("Buffering crouch");
                bufferState = State.CrouchStart;
            } else {
                //Y > 0
                bufferState = State.JumpStart;
            }
            
        } else if(input.x > 0f){
            // if(otherPlayer.gameObject.transform.position.x > transform.position.x){
                //If the other player is on the right of this player
                if(input.y == 0f){
                    if(state == State.Crouch || state == State.CrouchStart){
                        state = State.CrouchEnd;
                        framesActive = 0;
                        bufferState = State.WalkRight;
                        bufferXVel = 0f;
                    } else {
                        bufferState = State.WalkRight;
                        bufferXVel = walkSpeed;
                    }
                } else if(input.y < 0){
                    // print("Buffering crouch");
                    bufferState = State.CrouchStart;
                } else {
                    //Y > 0
                    bufferState = State.JumpStart;
                    bufferXVel = walkSpeed;

                }
            // } else {
                // bufferState = State.BlockReady;
            // }
        } else if(input.x < 0){
            if(input.y == 0f){
                    if(state == State.Crouch || state == State.CrouchStart){
                        state = State.CrouchEnd;
                        framesActive = 0;
                        bufferState = State.WalkLeft;
                        bufferXVel = 0f;
                    } else {
                        bufferState = State.WalkLeft;
                        bufferXVel = -walkSpeed;
                    }
                } else if(input.y < 0){
                    // print("Buffering crouch");
                    bufferState = State.CrouchStart;
                } else {
                    //Y > 0
                    bufferState = State.JumpStart;
                    bufferXVel = -walkSpeed;
                }

        }
        if(startBuffer == bufferState){

        }
    
    }
    private void setPos(){
        if(!Airborn){
            // We don't turn around in the air
            if(otherPlayer.gameObject.transform.position.x > transform.position.x){
                transform.rotation = defaultRot;
            } else {
                transform.rotation = otherRot;
            }
        }
        
        if(transform.position.y > -2.5f){
            yVel -= gravity;
            transform.position = new Vector3(transform.position.x + xVel, transform.position.y + yVel, transform.position.z);
        } else{

            transform.position = new Vector3(transform.position.x + xVel, -2.5f, transform.position.z);
        }
    }
    private void FixedUpdate() {
        
        //Clamp the position
        //Setting position in fixedUpdate so that game ticks will always see the proper position of the player
        // This is going to be used to create game "ticks" where every action takes a certain number of ticks to complete
        setPos();
        if(!actionable){
            //Prevent the player from having control if they aren't actionable
            return;
        }
        switch (state)
        {
            case State.CrouchStart:
                framesActive += 1;
                yVel = 0;
                xVel = 0;
                bufferXVel = 0;
                if(framesActive == crouchStartupFrames){
                    framesActive = 0;
                    state = State.Crouch;
                    
                }
                break;
            case State.CrouchEnd:
                framesActive += 1;

                if(framesActive == crouchEndFrames){
                    // print("Crouch ended");
                    framesActive = 0;
                    state = State.Idle;
                }
                break;
            case State.JumpStart:
                framesActive += 1;
                if(framesActive == jumpStartupFrames){
                    print("Going arborn");
                    framesActive = 0;
                    state = State.Airborn;
                    transform.position = new Vector3(transform.position.x + bufferXVel, -2.499f, transform.position.z);
                    xVel = bufferXVel;
                    yVel = jumpVel;
                }
                break;
            case State.Airborn:
                print("Went airborn");
                // The bottom of the fraction determines max height somehow
                // The 20x determines the frames in the air
                if(Airborn){
                    framesActive += 1;
                } else {
                    print("Landed");
                    framesActive = 0;
                    state = State.Idle;
                }
                break;
            case State.Idle:
                xVel = 0;
                bufferXVel = 0;
                yVel = 0;
                break;
            case State.WalkLeft:
                yVel = 0;
                xVel = -walkSpeed;
                break;
            case State.WalkRight:
                yVel = 0;
                xVel = walkSpeed;
                break;
            

            default:
                
                break;
        }
        switch (bufferState)
        {
            case State.CrouchStart:
                if(cancellableIntoCrouch.Contains(state)){
                    state = bufferState;
                    framesActive = 0;
                }
                break;
            case State.Idle:
                if(cancellableIntoIdle.Contains(state)){
                    state = bufferState;
                    framesActive = 0;
                }
                break;
            case State.CrouchEnd:
                //This doesn't need to check if it is cancellable since it will always come from a crouch
                state = bufferState;
                
                bufferState = State.Idle;
                framesActive = 0;
                break;
            case State.JumpStart:
                if(cancellableIntoJump.Contains(state)){
                    state = bufferState;
                    framesActive = 0;
                }
                break;
            case State.WalkRight:
                if(cancellableIntoWalk.Contains(state) && state != State.WalkRight){
                    xVel = walkSpeed;
                    state = State.WalkRight;
                }
                break;
            case State.WalkLeft:
                if(cancellableIntoWalk.Contains(state) && state != State.WalkLeft){
                    xVel = -walkSpeed;
                    state = State.WalkLeft;
                }
                break;
            default:
                break;
        }
    }
}
