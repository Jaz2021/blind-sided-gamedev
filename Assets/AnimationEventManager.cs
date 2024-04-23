using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState {
    Entrance,
    Idle,
    Crouch,
    Walk,
    Run,

}
// This half keeps me aware of what's going on between each class
public partial class AnimationEventManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject hitbox;
    [Serializable]
    public struct hitboxData{
        public Vector3 pos;
        public Vector3 size;
        public Vector3 knockback;
        public float damage;
        public int hitstun;
        public int lifespan;
    }
    [SerializeField]
    private hitboxData[] hitboxes;
    [SerializeField]
    private AudioClip[] soundEffects;
    private new AudioSource audio;
    PlayerController player;
    Animator animator;
    bool waitingForHit = false;
    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();
        audio = GetComponentInParent<AudioSource>();

    }
    /// <summary>
    /// Call this function from the animator in frames that the player should be actionable from a move
    /// </summary>
    /// <param name="a">Whether a player is actionable</param>
    public void setActionable(){
        // print("Should be actionable");
        if(player.gameStarted){
            player.actionable = true;

        }
    }
    public void playAudioEffectPanned(int index){
        // print("Playing audio");
        audio.panStereo = (player.transform.position.x - Camera.main.transform.position.x) / 15f;
        audio.PlayOneShot(soundEffects[index]);
    }
    public void setUnactionable(){
        // print("Should be non actionable");
        player.actionable = false;
    }
    public void waitForHit(){
        waitingForHit = true;
    }
    public void stopWaitingForHit(){
        waitingForHit = false;
    }
    public void zeroXVelocity(){
        player.zeroXVelocity();
    }
    public void CreateHitBox(int index){
        // I HATE how I had to set this up because it wouldn't let me put multiple items in one event
        // Not really a fan of the whole setup but it is all right I guess
        // If this needs to be optimized, create a list of gameobjects for each possible hitbox and activate them whenever needed
        var h = Instantiate(hitbox);

        var data = hitboxes[index];
        h.transform.parent = this.transform.parent;
        h.transform.localPosition = data.pos;
    
        h.transform.localScale = data.size;
        var hit = h.GetComponent<Hitbox>();
        hit.player = player;
        hit.damage = data.damage;
        hit.SetLifespan(data.lifespan);
        hit.knockback = data.knockback;
        hit.hitstun = data.hitstun;
        h.GetComponent<Rigidbody2D>().WakeUp();
    }
    public void EntranceStarted(){
        player.actionable = false;
        
    }
    public void EntranceFinished(){
        player.setReady();
        if(player.playerNum == 1){
            player.actionable = true;
            player.otherPlayer.actionable = true;
            player.ActivateIdleFunc();
            player.otherPlayer.ActivateIdleFunc();
        }
        // This will be called at the end of the entrance animation
    }
    public void EnterIdle(){
        // player.state = PlayerState.Idle;
        animator.ResetTrigger("idle");
        player.inputFunc = IdleInput;
        player.zeroXVelocity();
    }
    public void EnterRun(){

    }
    public void KnockedDown(){

    }
    public void StandUp(){

    }
    public void StandUpFinished(){

    }
    public void StandHeavy(){
        animator.ResetTrigger("heavyAttack");
        player.inputFunc = HeavyAttackInput;
    }
    public void StandHeavyEnd(){
        // print("Stand heavy ended");
        animator.SetTrigger("idle");
        player.inputFunc = IdleInput;
    }
    public void Walk(){
        animator.ResetTrigger("walk");
        player.addWalkXVel();
        player.inputFunc = WalkInput;
    }
    public void Crouch(){
        zeroXVelocity();
        animator.ResetTrigger("crouch");

        player.inputFunc = CrouchInput;
    }
    public void StandBlock(){
        zeroXVelocity();
        animator.ResetTrigger("block");
        player.addBlockXVel();
        player.inputFunc = BlockInput;
    }
    public void CrouchBlock(){
        zeroXVelocity();
    }
    public void CrouchHeavy(){
        zeroXVelocity();
        animator.ResetTrigger("crouchHeavy");

        player.inputFunc = CrouchHeavyInput;
    }
    public void CrouchHeavyEnd(){
        // print("Crouch heavy end");
        animator.SetTrigger("crouch");
        player.inputFunc = CrouchInput;
    }
    public void Jump(){
        animator.ResetTrigger("jump");

        player.inputFunc = null;
    }
    public void JumpEnd(){
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.04f);
        player.yVel = player.jumpVel;
        player.inputFunc = AirbornInput;
    }
    public void Falling(){
        animator.ResetTrigger("falling");
        player.inputFunc = FallingInput;
        
    }
    public void Land(){
        animator.ResetTrigger("idle");
        player.inputFunc = IdleInput;
    }
    public void StandLight(){
        zeroXVelocity();
        animator.ResetTrigger("lightAttack");
        player.inputFunc = lightAttackInput;
    }
    public void StandLightEnd(){
        // print("Stand light should be over");
        animator.SetTrigger("idle");
    }
    public void JumpHeavy(){
        animator.ResetTrigger("jumpHeavy");
        player.inputFunc = JumpHeavyInput;
    }
    public void JumpHeavyEnd(){

    }
    public void Hit(){
        player.inputFunc = null;
        // While in hitstun, no inputs
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
