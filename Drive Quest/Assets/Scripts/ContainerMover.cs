using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ContainerMover : MonoBehaviour
{
    public float animationDuration = 2.0f; // Adjust the animation duration as needed
    public Vector3 startPosition;
    public Vector3 endPosition;
    private Quaternion startRotation;
    public Quaternion endRotation; // Declare endRotation as a Quaternion
    Animator anim;
    public bool m;
    public Car car;
    GameObject placeSpot;

    void Start()
    {


        anim = GetComponent<Animator>();
    }

    public void Place()
    {
        GameManager.Instance.canplay = false;
        closeDoor();
        placeSpot = GameManager.Instance.placecontainerSpot(car, this);
        startPosition = transform.position;
        startRotation = transform.rotation;
        endPosition = placeSpot.transform.position;

        // Start the animation
        StartCoroutine(MoveContainer());
        GameManager.Instance.anycarmoving = false;
        GameManager.Instance.checkShip();
        GameManager.Instance.canplay = true;
    }

    // Check if the container is parked in the container
    public void closeDoor()
    {
        if (car.carColor == Colorv.Yellow)
        {
            anim.Play("Door colsing yello");
        }
        else if (car.carColor == Colorv.Green)
        {
            anim.Play("Doorclosing");
        }


    }

    IEnumerator MoveContainer()
    {
        float elapsedTime = 0;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            t = t * t * (3f - 2f * t); // Smoothstep function for a smooth animation

            // Calculate the curved motion
            float height = Mathf.Sin(t * Mathf.PI) * 5f; // Adjust the height of the curve
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, t);
            currentPosition.y += height; // Add the curved motion to the y-axis

            Quaternion currentRotation = Quaternion.Lerp(startRotation, endRotation, t);
            transform.position = currentPosition;
            transform.rotation = currentRotation;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Ensure the container reaches the exact end position and rotation
        transform.position = endPosition;
        transform.rotation = endRotation;
        transform.SetParent(placeSpot.transform);
        placeSpot = null;
    }
}