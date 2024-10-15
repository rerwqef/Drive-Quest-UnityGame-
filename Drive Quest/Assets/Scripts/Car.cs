using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Car : MonoBehaviour
{
    public bool canMove = false;
    public bool isHit = false;
    public CarPath path;
    public Quaternion startRotation;
    public Colorv carColor = Colorv.None;
    private Rigidbody rb;
    public float impactForce = 5f; // Adjustable impact force
    public float moveSpeed = 5.0f;
    public ContainerMover containerMover;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startRotation = transform.rotation;
    }

    void Update()
    {

        if (canMove)
        {
            MoveAlongPath();
containerMover.GetComponent<BoxCollider>().enabled=false;
        }else{
            containerMover.GetComponent<BoxCollider>().enabled=true;
        }
    }

    void MoveAlongPath()
    {
        if (path != null)
        {
            rb.MovePosition(path.GetNextPosition(moveSpeed * Time.deltaTime));
            ApplySmoothDamping();
        }
    }

    void ApplySmoothDamping()
    {
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f);
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, 0.1f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the other object is a car
        if (collision.collider.GetComponent<Car>())
        {
            Car otherCar = collision.collider.GetComponent<Car>();
            // Apply an impulse force to the affected car
            if (!canMove) rb.AddForce(collision.contacts[0].normal * impactForce, ForceMode.Impulse);

            // Stop the moving car
            canMove = false;

            // Reset both cars to their starting positions after 2 seconds
            Invoke("ResetCarPosition", 1f);
            otherCar.Invoke("ResetCarPosition", 1f);
            GameManager.Instance.anycarmoving = false;
        }
        else if (collision.collider.GetComponent<ContainerMover>())
        {
            canMove = false;

            // Reset both cars to their starting positions after 2 seconds
            Invoke("ResetCarPosition", 1f);

            GameManager.Instance.anycarmoving = false;
        }
    }

    void ResetCarPosition()
    {
        path.ResetCarToStartPosition();
        canMove = false;
    }
}