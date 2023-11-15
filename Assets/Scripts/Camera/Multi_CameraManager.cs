using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Multi_CameraManager : MonoBehaviour
{
    Multi_InputManager inputManager;

    public Transform targetTransform; //Object to follow
    public Transform cameraPivot; //Object to rotate

    private float defaultPosition; //Default position of the camera
    public Transform cameraTransform; // Transform of the actual camera object in the scene

    private Vector3 cameraFollowVelocity = Vector3.zero;
    private Vector3 cameraVectorPosition;
    public float followSpeed = 0.1f;

    public float lookAngle; //Rotation on Y axis up/down
    public float pivotAngle; //Rotation on X axis left/right

    public float cameraLookSpeed = 15f;
    public float cameraPivotSpeed = 15f;
    public float camLookSmoothTime = 1f;

    public float minPivotAngle = -25f;
    public float maxPivotAngle = 35f;

    public float cameraCollisionRadius = 0.2f;
    public float cameraCollisionOffSet = 0.2f; // How much a camera can jump off a objects colliding with
    public float minCollisionOffSet = 0.2f;
    public LayerMask collisionLayers;

    [SerializeField] GameObject player;
    [SerializeField] Camera camera;
    PhotonView photonView;

    public void WakeCamera()
    {
        targetTransform = player.transform;
        inputManager = player.GetComponent<Multi_InputManager>();
        photonView = player.GetComponent<PhotonView>();
        cameraTransform = camera.transform;
        defaultPosition = cameraTransform.localPosition.z;

        // Lock mouse cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void HandleAllCameraMovement()
    {
        FollowPlayer();
        RotationCamera();
        HandleCameraCollisions();
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, followSpeed);

        transform.position = targetPosition;
    }

    private void RotationCamera()
    {
        Vector3 rotation;

        lookAngle = Mathf.Lerp(lookAngle, lookAngle + (inputManager.cameraInputX * cameraLookSpeed), camLookSmoothTime * Time.deltaTime);
        pivotAngle = Mathf.Lerp(pivotAngle, pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed), camLookSmoothTime * Time.deltaTime);
        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;

        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivot.transform.position,cameraCollisionRadius,direction,out hit, Mathf.Abs(targetPosition),collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition -= (distance - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minCollisionOffSet)
            targetPosition = targetPosition - minCollisionOffSet;

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, Time.deltaTime * 15f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
}
