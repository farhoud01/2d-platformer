using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation_logic : MonoBehaviour
{
    private input_manager input;
    private movementBehavior moveRef;
    private Animator animator;
    private string currentAni = "";
    void Start()
    {
        animator = GetComponent<Animator>();
        input = input_manager.Instance;
        moveRef = GetComponent<movementBehavior>();
    }

    void changeAnimation(string animation, float crossFade = 0.2f)
    {
        if(currentAni != animation)
        {
            currentAni = animation;
            animator.CrossFade(animation, crossFade);
        }
    }
    void handleIdle()
    {
        if (moveRef.groundTimer>0 && input.MoveInput.x == 0)
        {
            changeAnimation("idle");
        }
    }
  
    void handleJumping()
    {
        if (moveRef.groundTimer>0 && moveRef.jumpTimer>0)
        {
            changeAnimation("jumping");
        }
    }
  
    void Update()
    {
        handleIdle();
        handleJumping();
        hanleDash();
        handleFalling();
    }
    void handleFalling()
    {
        if (moveRef.groundTimer<0 && currentAni != "jumping" && currentAni != "Dashing")
        {
            changeAnimation("Falling");
        }
        else if (currentAni == "Falling" && moveRef.isDashing)
        {

            StartCoroutine(DashDelay());
        }
     
    }

    private void hanleDash()
    {
        if (moveRef.isDashing && currentAni != "Dashing")
        {
            changeAnimation("Dashing");
        }
      else if(moveRef.isDashing == false && currentAni == "Dashing" && moveRef.groundTimer<0)
        {
            StartCoroutine(FallDelay());
         

        }
    }

    IEnumerator FallDelay()
    {
        yield return new WaitForSeconds(1);
        changeAnimation("Falling");
    }
    IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(1);
        changeAnimation("Dashing");
    }
}
