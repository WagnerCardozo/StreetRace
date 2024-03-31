using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float maxSpeed = 20.0f; // Adjust for desired speed
    public float turnSpeed = 5.0f; // Adjust for desired turning sensitivity
    public float driftForce = 1.0f; // Adjust for desired drift effect

    private Rigidbody carRigidbody;

    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Input Handling
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Movement
        carRigidbody.velocity = transform.forward * verticalInput * maxSpeed;

        // Steering
        float turnAngle = horizontalInput * turnSpeed;
        transform.Rotate(0f, turnAngle, 0f);

        // Drift (Optional)
        if (Input.GetKey(KeyCode.Space) && Mathf.Abs(horizontalInput) > 0.1f) // Adjust key for drift and threshold
        {
            carRigidbody.AddForce(transform.right * horizontalInput * driftForce);
        }
    }
}
