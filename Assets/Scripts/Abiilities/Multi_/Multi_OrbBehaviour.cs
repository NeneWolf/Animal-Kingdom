using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Multi_OrbBehaviour : MonoBehaviour, IPunObservable
{
    public Transform centerTransform;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject self; // player
    [SerializeField] Multi_PlayerManager multi_PlayerManager;

    //public Transform parentTransform;
    public float orbitSpeed = 30f;  // The speed of the orbit in degrees per second.
    public Vector3 orbitAxis = Vector3.up;  // The axis around which the object orbits.
    public float radius = 5f;     // The radius of the circular path.
    public float speed = 30f;     // The speed of movement in degrees per second.

    private float angle = 0f;
    public float initialAngle;

    private Vector3 offset;

    [Header("Effect")]
    [SerializeField] private GameObject powerUpVFX;

    PhotonView photonView;

    private void Awake()
    {
        photonView = self.GetComponent<PhotonView>();
        multi_PlayerManager = self.GetComponent<Multi_PlayerManager>();
    }

    private void Start()
    {
        #region
        if (centerTransform == null)
        {   enabled = false; // Disable the script to prevent errors.
            return;
        }
        #endregion
    }

    private void Update()
    {
        Movement();

        if (transform.GetComponent<SphereCollider>().enabled == true)
        {
            powerUpVFX.SetActive(true);
        }
        else
        {
            powerUpVFX.SetActive(false);
        }
    }

    void Movement()
    {
        Rotate();

        // Update the angle based on the speed.
        angle += speed * Time.deltaTime;

        // Ensure the angle stays within the 360-degree range.
        if (angle >= 360f)
        {
            angle -= 360f;
        }

        // Calculate the new position of the object on the circular path.
        float x = centerTransform.position.x + radius * Mathf.Cos((initialAngle + angle) * Mathf.Deg2Rad);
        float z = centerTransform.position.z + radius * Mathf.Sin((initialAngle + angle) * Mathf.Deg2Rad);

        // Set the object's new position.
        transform.position = new Vector3(x, transform.position.y, z);
    }

    void Rotate()
    {
        // Rotate the object around the center object.
        transform.RotateAround(centerTransform.position, orbitAxis, orbitSpeed * Time.deltaTime);

        // Maintain the initial offset.
        transform.position = centerTransform.position + offset;
    }

    [PunRPC]
    public void FireBullet(Quaternion quaternion)
    {
        GameObject proj = Instantiate(projectile,transform.position, quaternion);
        proj.GetComponent<Multi_OrbMovement>().SpawnBullet(centerTransform, self, photonView.Owner);
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(powerUpVFX.activeInHierarchy);
        }
        else
        {
            powerUpVFX.SetActive((bool)stream.ReceiveNext());
        }
    }
}