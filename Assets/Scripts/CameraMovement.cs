using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject carsContainerGameObject;

    public GameObject followTarget;
    int fastestCarProgression = 0;

    // public float speed;

    // Rigidbody2D body;

    // void Start()
    // {
    //     body = GetComponent<Rigidbody2D>();
    // }

    void Update()
    {
        for(int i = 0; i < carsContainerGameObject.transform.childCount; i++)
        {
            if (carsContainerGameObject.transform.GetChild(i).gameObject.GetComponent<CarController>().place > fastestCarProgression)
            {
                fastestCarProgression = carsContainerGameObject.transform.GetChild(i).gameObject.GetComponent<CarController>().place;
                followTarget = carsContainerGameObject.transform.GetChild(i).gameObject;
            }
        }

        transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, -10);

        // body.velocity = new Vector2(0, 0);
        // if (Input.GetKey(KeyCode.W))
        // {
        //     body.velocity = new Vector2(body.velocity.x, speed);
        // }
        // if (Input.GetKey(KeyCode.A))
        // {
        //     body.velocity = new Vector2(-speed, body.velocity.y);
        // }
        // if (Input.GetKey(KeyCode.S))
        // {
        //     body.velocity = new Vector2(body.velocity.x, -speed);
        // }
        // if (Input.GetKey(KeyCode.D))
        // {
        //     body.velocity = new Vector2(speed, body.velocity.y);
        // }
    }
}
