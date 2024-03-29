using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private State[] cancellableIntoCrouch; //What states can be cancelled into crouch
    [SerializeField]
    private State[] cancellableIntoIdle;
    
    [Header("Set in runtime")]
    public bool actionable = true;
    [SerializeField]
    public enum State{
        Idle,
        CrouchStart,
        Crouch,
        CrouchEnd,
        JumpStart,
    }


    public State state {
        get {
            return _state;
        }
        set {
            if(stateAnimation[(int)value] == "Hold"){
                //Ignore animation
            } else {
                animator.CrossFade(stateAnimation[(int)value], stateAnimationTransition[(int)value]);
                //Play the proper animation
            }
            _state = value;
        }
    }
    [Header("Indexes correspond to State enum, see code for indexes")]
    [Header("Use \"Hold\" when an animation will be the same as the previous state, these will be ignored when the state changes")]
    
    [SerializeField] private string[] stateAnimation = new string[Enum.GetValues(typeof(State)).Length]; 
    [SerializeField] private float[] stateAnimationTransition = new float[Enum.GetValues(typeof(State)).Length]; 

    private State _state = State.Idle;
    /// <summary>
    /// The state that is next to come up. Typically is based on what button is being held
    /// </summary>
    public State bufferState = State.Idle;
    private int framesActive = 0;

    private bool Airborn {
        get{
            return transform.position.y == -2.5f;
        }
    }

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
        transform.position = (transform.position.y < -2.5) ? new Vector3(transform.position.x, -2.5f, transform.position.z) : transform.position; 
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
                    framesActive = 0;
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
                }
                break;
            case State.Idle:
                if(cancellableIntoIdle.Contains(state)){
                    state = bufferState;
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
