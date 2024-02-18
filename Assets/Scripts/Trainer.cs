using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    public GameObject childContainer; // GameObject under which to instantiate cars
    public GameObject childPrefab;
    public int childrenPerGeneration;
    public float mutationRange;

    GameObject bestInCurrentGen;

    int generation = 0;
    bool allChildrenDead;

    void Start()
    {
        for (int i = 0; i < childrenPerGeneration; i++)
        {
            GameObject child = Instantiate(childPrefab);
            child.transform.SetParent(childContainer.transform);
        }
    }

    void NewGeneration()
    {
        NeuralNetwork.Network bestNNParameters = null;

        // Get the most successful one
        float bestScore = 0f;
        for (int i = 0; i < childContainer.transform.childCount; i++)
        {
            float successOfCurrentCar = childContainer.transform.GetChild(i).GetComponent<NeuralNetwork>().CalculateSuccess();
            if (successOfCurrentCar > bestScore)
            {
                //bestInCurrentGen = childContainer.transform.GetChild(i).gameObject;
                bestNNParameters = new NeuralNetwork.Network(childContainer.transform.GetChild(i).gameObject.GetComponent<NeuralNetwork>().network);
                bestScore = successOfCurrentCar;
            }
        }

        // Kill them all
        for (int i = 0; i < childContainer.transform.childCount; i++)
        {
            Destroy(childContainer.transform.GetChild(i).gameObject);
        }

        // Clone the most successful one & mutate all except one
        // (We leave one unmutated just in case we get unlucky and everyone gets a debuff, that way we can at least try again)
        for (int i = 0; i < childrenPerGeneration; i++)
        {
            GameObject child = Instantiate(childPrefab);
            child.transform.SetParent(childContainer.transform);
            child.GetComponent<NeuralNetwork>().network = new NeuralNetwork.Network(bestNNParameters);
            if (i > 0)
            {
                //Debug.Log(child.GetComponent<NeuralNetwork>().network == null);
                child.GetComponent<NeuralNetwork>().network.Mutate(mutationRange);
            }
        }

        generation++;
    }

    void Update()
    {
        allChildrenDead = true;
        for (int i = 0; i < childContainer.transform.childCount; i++)
        {
            if (childContainer.transform.GetChild(i).GetComponent<CarController>().alive)
            {
                allChildrenDead = false;
                break;
            }
        }
        // Spawn the next generation if all cars are dead or the user presses the 'R' key
        if (allChildrenDead || Input.GetKeyDown(KeyCode.R))
        {
            NewGeneration();
        }
    }
}
