using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator animator;
    int horizontal;
    int vertical;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        #region If I want to waste more time
        //#region Snapping Horizontal

        ////Animation Snapping
        //float snappedHorizontal;

        //if (horizontalMovement > 0 && horizontalMovement < 0.55f) { snappedHorizontal = 0.5f; }
        //else if (horizontalMovement > 0.55f) { snappedHorizontal = 1f; }
        //else if (horizontalMovement < 0f && horizontalMovement > -0.55f) { snappedHorizontal = -0.5f; }
        //else if (horizontalMovement < -0.55f) { snappedHorizontal = -1f; }
        //else { snappedHorizontal = 0f; }

        //#endregion
        //#region Snapping Vertical

        //float snappedVertical;

        //if (verticalMovement > 0 && verticalMovement < 0.55f) { snappedVertical = 0.5f; }
        //else if (verticalMovement > 0.55f) { snappedVertical = 1f; }
        //else if (verticalMovement < 0f && verticalMovement > -0.55f) { snappedVertical = -0.5f; }
        //else if (verticalMovement < -0.55f) { snappedVertical = -1f; }
        //else { snappedVertical = 0f; }
        //#endregion


        #endregion

        animator.SetFloat(horizontal, horizontalMovement, 0.1f, Time.deltaTime);

        if (isSprinting) { animator.SetFloat(vertical, 2, 0.1f, Time.deltaTime);}
        else { animator.SetFloat(vertical, verticalMovement, 0.1f, Time.deltaTime); }
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}
