using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum Colorv
{
    None,
    Orange,
    Blue,
    Green,
    Yellow
}

[System.Serializable]
public class ship
{
    public GameObject shipPrefab;
    public int numparkingspot = 3;
    public int currentplacespot = 0;
    public Colorv shipcolor = Colorv.None;
    public GameObject[] shipplacespots;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public ship[] ships;
    public int currentindex = 0;
    public ship currentship;
    public GameObject[] placespots;
    public List<ContainerMover> containersINLocalspot;
    public int currentplacespot;
    public bool anycarmoving = false;
    public bool canplay=true;
    public GameObject RestartPannel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentship = ships[currentindex];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !anycarmoving&&canplay)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Car"))
                {
                    hit.transform.gameObject.GetComponent<Car>().canMove = true;
                    anycarmoving = true;
                }
            }
        }

    }

    public GameObject placecontainerSpot(Car car, ContainerMover containerMover)
    {


        if (car.carColor == currentship.shipcolor)
        {
            if (currentship.currentplacespot < currentship.numparkingspot)
            {
                currentship.currentplacespot++;

                return currentship.shipplacespots[currentship.currentplacespot - 1];
            }
        }
        else if (currentplacespot < placespots.Length)
        {
            currentplacespot++;
            containersINLocalspot.Add(containerMover);
            return placespots[currentplacespot - 1];
        }
        return null;
    }
    public void checkShip()
    {
        if (currentship.currentplacespot == 3)
        {
            Moveship();
            currentindex++;
   if (currentindex < ships.Length ){

    currentship = ships[currentindex];
   currentship.shipPrefab.GetComponent<Animator>().enabled = true;
   }


        }
    }
    void Moveship()
    {
        if (currentindex < ships.Length - 1)
        {
            StartCoroutine(MoveShips(currentship.shipPrefab, ships[currentindex + 1].shipPrefab));
        }
        else
        {
            StartCoroutine(MoveShips(currentship.shipPrefab));
          
            RestartPannel.SetActive(true);
        }

    }

    IEnumerator MoveShips(GameObject currentShip, GameObject nextShip = null)
    {
        // Add a 2-second delay at the start
        yield return new WaitForSeconds(2f);
        currentShip.GetComponent<Animator>().enabled = false;
       

        // Move the current ship forward
        float elapsedTime = 0;
        Vector3 startPosition = currentShip.transform.position;
        Vector3 endPosition = startPosition + Vector3.forward * -20f;
        while (elapsedTime < 2f)
        {
            currentShip.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / 2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currentShip.transform.position = endPosition;

        // Move the next ship to the current ship's position
        if (nextShip != null)
        {
             nextShip.GetComponent<Animator>().enabled = false;
            elapsedTime = 0;
            endPosition = startPosition;
            startPosition = nextShip.transform.position;

            while (elapsedTime < 2f)
            {
                nextShip.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / 2f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            nextShip.transform.position = endPosition;

            // Wait until the next ship reaches the correct spot
            yield return new WaitUntil(() => Vector3.Distance(nextShip.transform.position, endPosition) < 0.01f);

            MovecontainersToship();
        }

    }

    void MovecontainersToship()
    {
        foreach (ContainerMover container in containersINLocalspot)
        {
            container.Place();
        }
    }
    public void Restart(){
        SceneManager.LoadScene(0);
    }
}