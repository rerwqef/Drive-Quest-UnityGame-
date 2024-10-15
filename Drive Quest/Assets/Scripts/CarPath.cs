using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CarPath : MonoBehaviour
{
    public Transform[] waypoints;
    public GameObject car;
    public float rotationSpeed = 5.0f;
    public Color lineColor = Color.red;
    public float lineWidth = 2.0f;
    private int currentWaypointIndex;
    private Quaternion targetRotation;
    private LineRenderer lineRenderer;
    private Car carScript;
    public float offsetValue;
    public ContainerMover containerMover;

    void Start()
    {
        carScript = car.GetComponent<Car>();
        InitializeCarPosition();
        CreateLineRenderer();
    }

    void InitializeCarPosition()
    {
        car.transform.position = waypoints[0].position;
        carScript.startRotation = car.transform.rotation;
        currentWaypointIndex = 0;
    }

    void CreateLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = waypoints.Length;

        Vector3[] positions = waypoints.Select(p => p.position).ToArray();
        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] += new Vector3(0, offsetValue, 0);
        }

        lineRenderer.SetPositions(positions);
    }

    public Vector3 GetNextPosition(float moveSpeed)
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            Vector3 direction = waypoints[currentWaypointIndex].position - car.transform.position;
            car.transform.position = Vector3.MoveTowards(car.transform.position, waypoints[currentWaypointIndex].position, moveSpeed);
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            car.transform.rotation = Quaternion.Lerp(car.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(car.transform.position, waypoints[currentWaypointIndex].position) < 0.01f)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    containerMover.Place();
                    car.SetActive(false);
                }
            }
        }

        return car.transform.position;
    }

    public void ResetCarToStartPosition()
    {
        // Reset the car to its starting position
        car.transform.position = waypoints[0].position;
        car.transform.rotation = carScript.startRotation;

        // Reset the car's Rigidbody velocity and angular velocity
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    void OnDrawGizmos()
    {
        // Check if waypoints is not null
        if (waypoints != null)
        {
            // Draw the line in Edit mode
            Gizmos.color = lineColor; // Set the line color

            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}