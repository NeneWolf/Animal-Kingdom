using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multi_AnimatorManager : MonoBehaviour
{
    Animator animator;

    int horizontal;
    int vertical;
    public float speed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    [PunRPC]
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

        if(targetAnimation == "Wolf_Jump_Forward")
        {
            StartCoroutine(ResetJump());
        }
        else if(targetAnimation == "Wolf_Sprint_Jump")
        {
            StartCoroutine(ResetJump2());
        }
    }

    [PunRPC]
    public void PlayTargetAnimationOthers(string targetAnimation, bool value)
    {
        animator.SetBool(targetAnimation, value);
    }

    IEnumerator ResetJump()
    {
        animator.SetLayerWeight(1, 1);
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(1, 0);
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("PlayTargetAnimationOthers", RpcTarget.Others, "Wolf_Jump_Forward", true);
        animator.SetBool("isInteracting", false);
    }

    IEnumerator ResetJump2()
    {
        animator.SetLayerWeight(1, 1);
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(1, 0);
        PhotonView photonView = GetComponent<PhotonView>();
        photonView.RPC("PlayTargetAnimationOthers", RpcTarget.Others, "Wolf_Sprint_Jump", true);
        animator.SetBool("isJumping", false);
        animator.SetBool("isInteracting", false);
    }
}
