using UnityEngine;
using Unity.Cinemachine;
using System;
using UnityEditor.UI;

public class PlayerCarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] carPreFabs;

    [Header("Camera")]
    [SerializeField]
    CinemachineCamera cinemachineCamera;

    [Header("Menu")]
    [SerializeField]
    bool isMainMenu = false;

    //Instantiated car
    GameObject instantiatedPlayerCar = null;

    //Which Car is Selected
    int carIndex = 0;

    //Selected car from menu
    static GameObject selectedCarPrefab = null;

    Quaternion carRotation = Quaternion.identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (isMainMenu)
        {
            instantiatedPlayerCar = Instantiate(carPreFabs[carIndex].GetComponent<CarHandler>().CarMeshRenderer.gameObject);
            selectedCarPrefab = carPreFabs[carIndex];
        }
        else
        {
            if (selectedCarPrefab != null)
            {
                instantiatedPlayerCar = Instantiate(selectedCarPrefab);

            }
            else { instantiatedPlayerCar = Instantiate(carPreFabs[0]); }
        }

        if (cinemachineCamera != null)
            cinemachineCamera.Follow = instantiatedPlayerCar.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainMenu)
        {
            instantiatedPlayerCar.transform.position = new Vector3(1, 0, 0);
            instantiatedPlayerCar.transform.Rotate(new Vector3(0, 0, 20) * Time.deltaTime);

            carRotation = instantiatedPlayerCar.transform.rotation;
        }
    }

    void changeCar()
    {
        Destroy(instantiatedPlayerCar);

        instantiatedPlayerCar = Instantiate(carPreFabs[carIndex].GetComponent<CarHandler>().CarMeshRenderer.gameObject);

        selectedCarPrefab = carPreFabs[carIndex];

        instantiatedPlayerCar.transform.rotation = carRotation;
    }

    public void OnNextCarClicked()
    {
        carIndex++;

        if (carIndex > carPreFabs.Length - 1)
            carIndex = 0;

        changeCar();
    }

    public void OnPreviousCarClicked()
    {
        carIndex--;

        if (carIndex < 0)
            carIndex = carPreFabs.Length - 1;

        changeCar();
    }
}
