using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private const float secondsPerTick = 1f/60f;
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



    [Header("Jump arc")]
    [SerializeField]
    private float jumpArcFrames;
    [SerializeField]
    private float jumpArcHeight;

    
    [Header("Set in runtime")]
    public bool actionable = true;
    [SerializeField]
    public enum State{
        Idle,
        CrouchStart,
        Crouch,
        CrouchEnd,
        JumpStart,
        Airborn
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

    private State _state = State.Idle;
    /// <summary>
    /// The state that is next to come up. Typically is based on what button is being held
    /// </summary>
    public State bufferState = State.Idle;
    private int framesActive = 0;

    private bool Airborn {
        get{
            return transform.position.y != -2.5f;
        }
    }
    private float xVel = 0;
    private void Start() {
        animator = GetComponentInChildren<Animator>();
        print("Creating a new player");
        if(MultiplayerController.s.player1 == null){
            MultiplayerController.s.player1 = gameObject;
            transform.position = MultiplayerController.s.GetStartPosition(1);
            transform.rotation = MultiplayerController.s.GetStartRotation(1);
        } else if(MultiplayerController.s.player2 == null){
            MultiplayerController.s.player2 = gameObject;
            transform.position = MultiplayerController.s.GetStartPosition(2);
            transform.rotation = MultiplayerController.s.GetStartRotation(2);
        } else {
            throw new System.Exception("Error: Attempted to have a third player join");
        }
    }
    public void moveInput(InputAction.CallbackContext context){
        Vector2 input = context.ReadValue<Vector2>();
        //Up down should take priority in state over left/right
        // Left/right movements depend on up/down state but not vice versa
        //We never set our State here, only the bufferState
        State startBuffer = bufferState;
        if(input.x == 0){
            
            if(input.y == 0){
                if(state == State.Crouch || state == State.CrouchStart){
                    bufferState = State.CrouchEnd;
                } else {
                    bufferState = State.Idle;
                }
            } else if(input.y < 0){
                // print("Buffering crouch");
                bufferState = State.CrouchStart;
            } else {
                //Y > 0
                bufferState = State.JumpStart;
            }
            
        } else if(input.x > 0){

        } else if(input.x < 0){

        }
        if(startBuffer == bufferState){

        }
    
    }
    private void FixedUpdate() {
        //Clamp the position
        // transform.position = (transform.position.y > -2.5) ? new Vector3(transform.position.x, -2.5f, transform.position.z) : transform.position; 
        if(transform.position.y < -2.5f){
            print("Went out of bounds");
            print(transform.position.y);
            transform.position = new Vector3(transform.position.x, -2.5f, transform.position.z);
        }
        // This is going to be used to create game "ticks" where every action takes a certain number of ticks to complete
        
        switch (state)
        {
            case State.CrouchStart:
                framesActive += 1;
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
                    transform.position = new Vector3(transform.position.x + xVel, -2.499f, transform.position.z);
                }
                break;
            case State.Airborn:
                print("Went airborn");
                //While airborn, we will move through a predetermined jump arc
                //I am using this instead of rigidbodies because we can use incredibly simple math and no collisions
                //Equation is (-x^2+20x)/40
                // The bottom of the fraction determines max height somehow
                // The 20x determines the frames in the air
                if(Airborn){
                    float h = jumpArcFrames / 2f;
                    float k = jumpArcHeight;
                    float yPos = -(h/(k*k)) * (framesActive - h) * (framesActive - h) + k - 2.499f;
                    transform.position = new Vector3(transform.position.x + xVel, yPos , transform.position.z);
                    print(transform.position.y);
                    framesActive += 1;
                } else {
                    print("Landed");
                    state = State.Idle;
                }
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
            default:
                break;
        }
    }
    public void crouch(){
        // Call to this player to go into crouch animation
        // currentState = State.Crouch;
        // animator?.Play("Crouch");
    }
    public void crouchRelease(){
        // animator?.CrossFade(prevState.ToString(), 0.25f);
        // State temp = currentState;
        // currentState = prevState;
        // prevState = temp;
    }
}
