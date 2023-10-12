using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class OrbBehaviour : MonoBehaviour
{
    Rigidbody rigidbody;

    public Transform centerObject;  // The object to orbit around.
    public float orbitSpeed = 30f;  // The speed of the orbit in degrees per second.
    public Vector3 orbitAxis = Vector3.up;  // The axis around which the object orbits.
    public float radius = 5f;     // The radius of the circular path.
    public float speed = 30f;     // The speed of movement in degrees per second.

    bool isFired = false;

    private float angle = 0f;
    public float initialAngle;

    private Vector3 offset;

    public float fireSpeed = 10f;

    private void Awake()
    {
        
    }
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        #region
        if (centerObject == null)
        {   enabled = false; // Disable the script to prevent errors.
            return;
        }
        #endregion

        // Calculate an initial random angle.
        //initialAngle = Random.Range(0f, 360f);

        // Calculate the initial position based on the random angle.
        float x = centerObject.position.x + radius * Mathf.Cos(initialAngle * Mathf.Deg2Rad);
        float z = centerObject.position.z + radius * Mathf.Sin(initialAngle * Mathf.Deg2Rad);

        // Set the object's initial position.
        transform.position = new Vector3(x, transform.position.y, z);
    }

    private void Update()
    {
        if (!isFired) {

            Rotate();

            // Update the angle based on the speed.
            angle += speed * Time.deltaTime;

            // Ensure the angle stays within the 360-degree range.
            if (angle >= 360f)
            {
                angle -= 360f;
            }

            // Calculate the new position of the object on the circular path.
            float x = centerObject.position.x + radius * Mathf.Cos((initialAngle + angle) * Mathf.Deg2Rad);
            float z = centerObject.position.z + radius * Mathf.Sin((initialAngle + angle) * Mathf.Deg2Rad);

            // Set the object's new position.
            transform.position = new Vector3(x, transform.position.y, z);
        }
    }

    void Rotate()
    {
        // Rotate the object around the center object.
        transform.RotateAround(centerObject.position, orbitAxis, orbitSpeed * Time.deltaTime);

        // Maintain the initial offset.
        transform.position = centerObject.position + offset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 7 && isFired) { Destroy(gameObject);}
    }

    public void BulletMovement()
    {
        isFired = true;
        transform.forward = centerObject.transform.forward;
        rigidbody.velocity = transform.forward * fireSpeed;
    }

    public void BulletMovementToTarget(Transform Target)
    {

    }

}