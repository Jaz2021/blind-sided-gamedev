using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
// This half of the class holds all the input functions
// It alows me to keep things seperated.
public partial class AnimationEventManager : MonoBehaviour {
    /// <summary>
    /// The input for a player when they are in the idle state.
    /// </summary>
    

    public void IdleInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        float xInput = motion.x;
        if(!facingRight){
            xInput = -xInput;
        }
        // print(motion);
        switch (xInput, motion.y, light, heavy, special)
        {
            case(-1, -1, false, false, false):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchBlock");
                break;
            case (_, -1, false, false, false):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouch");
                break;
            case(-1, 0, false, false, false):
                animator.ResetTrigger("idle");
                animator.SetTrigger("block");
                break;
            case(1, 0, false, false, false):
                animator.ResetTrigger("idle");
                animator.SetTrigger("walk");
                break;
            case(_, 1, _, _, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("jump");
                break;
            case (_, -1, true, _, _):
                //Crouch light
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchLight");
                break;
            case (_, 0, true, _, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("lightAttack");
                break;
            case(_, 0, _, true, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("heavyAttack");
                break;
            case(_, 0, _, _, true):
                animator.ResetTrigger("idle");
                animator.SetTrigger("specialAttack");
                break;
            case (_, -1, _, true, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, -1, _, _, true):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchSpecial");
                break;
            default:
                break;
        }
        
    }

    /// <summary>
    /// The input for a player while they are in the crouch state
    /// </summary>
    public void CrouchInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        float xInput = motion.x;
        if(!facingRight){
            xInput = -xInput;
        }
        switch (xInput, motion.y, light, heavy, special)
        {
            case(-1, -1, false, false, false):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchBlock");
                break;
            case(-1, 0, false, false, false):
                animator.ResetTrigger("idle");
                animator.SetTrigger("block");
                break;
            case(_, 1, _, _, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("jump");
                break;
            case (_, -1, true, _, _):
                //Crouch light
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchLight");
                break;
            case (_, 0, true, _, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("lightAttack");
                break;
            case(_, 0, _, true, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("heavyAttack");
                break;
            case(_, 0, _, _, true):
                animator.ResetTrigger("idle");
                animator.SetTrigger("specialAttack");
                break;
            case (_, -1, _, true, _):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, -1, _, _, true):
                animator.ResetTrigger("idle");
                animator.SetTrigger("crouchSpecial");
                break;
            case (0, 0, false, false ,false):
                animator.ResetTrigger("crouch");
                animator.SetTrigger("idle");
                break;
            default:
                break;
        }
    }
    public void AirbornInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        //While in the air, motion inputs don't matter
        // No air block in this game
        // Facing direction also shouldn't matter in this case
        if(light){
            animator.ResetTrigger("crouch");
            animator.SetTrigger("jumpLight");
        } else if (heavy){
            animator.ResetTrigger("crouch");
            animator.SetTrigger("jumpHeavy");
        } else if (special){
            animator.ResetTrigger("crouch");
            animator.SetTrigger("jumpSpecial");
        }
    }
    public void KnockedDownInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        if(light || heavy || special){
            animator.ResetTrigger("knockedDown");
            animator.SetTrigger("idle");
            //I'm just using an automatic animation for this one, no in between animation
        }
    }

    public void lightAttackInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        float xInput = motion.x;
        if(!facingRight){
            xInput = -xInput;
        }
        switch(xInput, light, heavy, special){
            case (-1, true, _, _):
                animator.ResetTrigger("lightAttack");
                animator.SetTrigger("crouchLight");
                break;
            case(_, true, _, _):
                animator.ResetTrigger("lightAttack");
                animator.SetTrigger("restart");
                break;
            case(-1, _, true, _):
                animator.ResetTrigger("lightAttack");
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, _, true, _):
                animator.ResetTrigger("lightAttack");
                animator.SetTrigger("heavyAttack");
                break;
            case(-1, _, _, true):
                animator.ResetTrigger("lightAttack");
                animator.SetTrigger("crouchSpecial");
                break;
            case(_, _, _, true):
                animator.ResetTrigger("lightAttack");
                animator.SetTrigger("specialAttack");
                break;
            default:
                break;
        }
    }
    public void HeavyAttackInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        // Heavy attacks can only cancel into specials
        // print("Heavy attack input");
        if(motion.y >= 0){
            if (special){
                animator.ResetTrigger("heavyAttack");
                animator.SetTrigger("specialAttack");
            }
        } else {
            if (special){
                animator.ResetTrigger("heavyAttack");
                animator.SetTrigger("crouchSpecial");
            }
        }
    }
    public void CrouchHeavyInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        if(motion.y == 0){
            if (special){
                animator.ResetTrigger("crouchHeavy");
                animator.SetTrigger("specialAttack");
            }
        } else if(motion.y > 0){
            animator.ResetTrigger("crouchHeavy");
            animator.SetTrigger("jump");
        } else if (motion.y < 0){
            animator.ResetTrigger("crouchHeavy");
            animator.SetTrigger("crouch");
        }
            else {
            if (special){
                animator.ResetTrigger("crouchHeavy");
                animator.SetTrigger("crouchSpecial");
            }
        }
    }
    
    public void SpecialAttackInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        //Can't cancel special attacks so nothing here
    }
    public void CrouchLightInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        switch(motion.y, light, heavy, special){
            case (-1, true, _, _):
                animator.ResetTrigger("crouchLight");
                animator.SetTrigger("restart");
                break;
            case(_, true, _, _):
                animator.ResetTrigger("crouchLight");
                animator.SetTrigger("lightAttack");
                break;
            case(-1, _, true, _):
                animator.ResetTrigger("crouchLight");
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, _, true, _):
                animator.ResetTrigger("crouchLight");
                animator.SetTrigger("heavyAttack");
                break;
            case(-1, _, _, true):
                animator.ResetTrigger("crouchLight");
                animator.SetTrigger("crouchSpecial");
                break;
            case(_, _, _, true):
                animator.ResetTrigger("crouchLight");
                animator.SetTrigger("specialAttack");
                break;
            default:
                break;
        }
    }
    public void CrouchSpecialInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){

    }
    public void JumpLightInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        if(heavy){
            animator.ResetTrigger("jumpLight");
            animator.SetTrigger("jumpHeavy");
        } else if (special){
            animator.ResetTrigger("jumpLight");
            animator.SetTrigger("jumpSpecial");
        }
    }
    public void JumpHeavyInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        if(special){
            animator.ResetTrigger("jumpHeavy");
            animator.SetTrigger("jumpSpecial");
        }
    }
    public void JumpSpecialInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        
    }

    public void BlockInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        float xInput = motion.x;
        if(!facingRight){
            xInput = -xInput;
        }
        const string cur = "block";
        switch (xInput, motion.y, light, heavy, special)
        {
            case(-1, 0, false, false, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("block");
                break;
            case(-1, -1, false, false, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchBlock");
                break;
            case(_, -1, false, false, false):
                // print("should crouch");
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouch");
                break;
            case(_, 1, _, _, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("jump");
                break;
            case (_, -1, true, _, _):
                //Crouch light
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchLight");
                break;
            case(1,0, false, false, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("walk");
                break;
            case (_, 0, true, _, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("lightAttack");
                break;
            case(_, 0, _, true, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("heavyAttack");
                break;
            case(_, 0, _, _, true):
                animator.ResetTrigger(cur);
                animator.SetTrigger("specialAttack");
                break;
            case (_, -1, _, true, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, -1, _, _, true):
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchSpecial");
                break;
            case (0, 0, false, false ,false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("idle");
                break;
            default:
                break;
        }
        
    }
    public void CrouchBlockInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        
        float xInput = motion.x;
        if(!facingRight){
            xInput = -xInput;
        }
        switch (xInput, motion.y, light, heavy, special)
        {
            case(-1, 0, false, false, false):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("block");
                break;
            case (_, -1, false, false, false):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("crouch");
                break;
            case(_, 1, _, _, _):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("jump");
                break;
            case (_, -1, true, _, _):
                //Crouch light
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("crouchLight");
                break;
            case (_, 0, true, _, _):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("lightAttack");
                break;
            case(_, 0, _, true, _):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("heavyAttack");
                break;
            case(_, 0, _, _, true):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("specialAttack");
                break;
            case (_, -1, _, true, _):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, -1, _, _, true):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("crouchSpecial");
                break;
            case (0, 0, false, false ,false):
                animator.ResetTrigger("crouchBlock");
                animator.SetTrigger("idle");
                break;
            default:
                break;
        }
    }
    public void WalkInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        float xInput = motion.x;
        if(!facingRight){
            xInput = -xInput;
        }
        const string cur = "walk";
        switch (xInput, motion.y, light, heavy, special)
        {
            case(-1, 0, false, false, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("block");
                break;
            case(-1, -1, false, false, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchBlock");
                break;
            case(_, -1, false, false, false):
                // print("should crouch");
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouch");
                break;
            case(_, 1, _, _, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("jump");
                break;
            case (_, -1, true, _, _):
                //Crouch light
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchLight");
                break;
            case (_, 0, true, _, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("lightAttack");
                break;
            case(_, 0, _, true, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("heavyAttack");
                break;
            case(_, 0, _, _, true):
                animator.ResetTrigger(cur);
                animator.SetTrigger("specialAttack");
                break;
            case (_, -1, _, true, _):
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchHeavy");
                break;
            case(_, -1, _, _, true):
                animator.ResetTrigger(cur);
                animator.SetTrigger("crouchSpecial");
                break;
            case (0, 0, false, false ,false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("idle");
                break;
            default:
                break;
        }
    }
    bool firstFallFrame = false;
    public void FallingInput(Vector2 motion, bool light, bool heavy, bool special, bool facingRight){
        // Theres currently a bug with the following code that sometimes makes it "hit the ground" midair
        // Fix that later
        const string cur = "falling";
        if(player.transform.position.y + 2.5f == 0f){
            if(!firstFallFrame){
                firstFallFrame = true;
                return;
            }
            firstFallFrame = false;
            // print("Hit the ground");
            //Transition into landing
            animator.ResetTrigger(cur);
            animator.SetTrigger("landed");
            zeroXVelocity();
            return;
        }
        switch (light, heavy, special)
        {
            case (true, false, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("jumpLight");
                break;
            case (false, true, false):
                animator.ResetTrigger(cur);
                animator.SetTrigger("jumpHeavy");
                break;
            case(false, false, true):
                animator.ResetTrigger(cur);
                animator.SetTrigger("jumpSpecial");
                break;
            default:
                break;
        }
    }
} 
