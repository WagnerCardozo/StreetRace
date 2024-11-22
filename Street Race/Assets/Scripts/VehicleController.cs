using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 15f;
    public float maxSpeed = 60f;
    public float reverseSpeed = 20f;
    public float brakingForce = 50f;
    public float deceleration = 10f;

    [Header("Steering Settings")]
    public float turnSpeed = 5f;
    public float steeringSensitivity = 2f;

    [Header("Stabilization")]
    public float rotationStabilization = 5f; // Suaviza a rotação do modelo
    public float angularDragInAir = 2f;      // Diminui tremores no ar

    [Header("Collision Recovery")]
    public float collisionBounceForce = 10f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.5f;

    private Rigidbody rb;
    private float inputVertical;
    private float inputHorizontal;
    private bool isGrounded;
    private float currentSpeed = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void Update()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
        HandleSteering();

        if (!isGrounded)
        {
            ApplyAirStabilization();
        }
        else
        {
            StabilizeRotation();
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void HandleMovement()
    {
        if (inputVertical != 0)
        {
            float targetSpeed = inputVertical > 0 ? maxSpeed : -reverseSpeed;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed * inputVertical, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.fixedDeltaTime);
        }

        Vector3 velocity = transform.forward * currentSpeed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    private void HandleSteering()
    {
        if (currentSpeed != 0)
        {
            float turnAmount = inputHorizontal * turnSpeed * Time.fixedDeltaTime;
            float steeringFactor = Mathf.Lerp(steeringSensitivity, 0.5f, Mathf.Abs(currentSpeed) / maxSpeed);
            transform.Rotate(0, turnAmount * steeringFactor, 0);
        }
    }

    private void StabilizeRotation()
    {
        // Aplica suavização à rotação para evitar tremores
        Vector3 stabilizedAngularVelocity = rb.angularVelocity;
        stabilizedAngularVelocity.x = Mathf.Lerp(stabilizedAngularVelocity.x, 0, rotationStabilization * Time.fixedDeltaTime);
        stabilizedAngularVelocity.z = Mathf.Lerp(stabilizedAngularVelocity.z, 0, rotationStabilization * Time.fixedDeltaTime);
        rb.angularVelocity = stabilizedAngularVelocity;
    }

    private void ApplyAirStabilization()
    {
        // Suaviza a rotação no ar
        rb.angularDrag = angularDragInAir;

        if (inputHorizontal != 0)
        {
            Vector3 torque = new Vector3(0, inputHorizontal, 0) * turnSpeed * 0.5f;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionNormal = collision.contacts[0].normal;

        // Aplica força para evitar travamento na parede
        if (Vector3.Dot(collisionNormal, Vector3.up) < 0.1f)
        {
            rb.AddForce(collisionNormal * collisionBounceForce, ForceMode.Impulse);
        }
    }
}
