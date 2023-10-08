using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    Animator animator;
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
        #region Snapping Horizontal
        //Animation Snapping
        float snappedHorizontal;

        if (horizontalMovement > 0 && horizontalMovement < 0.55f) { snappedHorizontal = 0.5f; }
        else if (horizontalMovement > 0.55f) { snappedHorizontal = 1f; }
        else if (horizontalMovement < 0f && horizontalMovement > -0.55f) { snappedHorizontal = -0.5f; }
        else if (horizontalMovement < -0.55f) { snappedHorizontal = -1f; }
        else { snappedHorizontal = 0f; }

        #endregion
        #region Snapping Vertical

        float snappedVertical;

        if (verticalMovement > 0 && verticalMovement < 0.55f) { snappedVertical = 0.5f; }
        else if (verticalMovement > 0.55f) { snappedVertical = 1f; }
        else if (verticalMovement < 0f && verticalMovement > -0.55f) { snappedVertical = -0.5f; }
        else if (verticalMovement < -0.55f) { snappedVertical = -1f; }
        else { snappedVertical = 0f; }
        #endregion

        if (isSprinting) { snappedVertical = 2; snappedHorizontal = 2; }

        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        animator.SetFloat(vertical, snappedVertical,0.1f, Time.deltaTime);
    }

}
