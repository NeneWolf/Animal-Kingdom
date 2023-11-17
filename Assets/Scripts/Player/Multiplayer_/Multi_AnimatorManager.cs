using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_AnimatorManager : MonoBehaviour
{
    public Animator animator;
    int horizontal;
    int vertical;

    public float speed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        #region 
        ////Animation Snapping
        float snappedHorizontal;

        if (horizontalMovement > 0 && horizontalMovement < 0.55f) { snappedHorizontal = 0.5f; }
        else if (horizontalMovement > 0.55f) { snappedHorizontal = 1f; }
        else if (horizontalMovement < 0f && horizontalMovement > -0.55f) { snappedHorizontal = -0.5f; }
        else if (horizontalMovement < -0.55f) { snappedHorizontal = -1f; }
        else { snappedHorizontal = 0f; }
        #endregion


        animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);

        if (isSprinting) { animator.SetFloat(vertical, 2, 0.1f, Time.deltaTime); animator.speed = speed; }
        else { animator.SetFloat(vertical, verticalMovement, 0.1f, Time.deltaTime); animator.speed = 1f; }
    }

    [PunRPC]
    public void PlayTargetAnimation(string targetAnimation, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnimation, 0.2f);
    }
}
