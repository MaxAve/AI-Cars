using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public bool ignoreOtherCars;
    public bool controllable;
    public float acceleration;
    public float maxSpeed;
    public float turnSpeed;
    public int place;
    public GameObject lastPassedCheckpoint = null;

    SpriteRenderer renderer;
    Rigidbody2D body;
    Collider2D collider;
    Color deadColor = new Vector4(1f, 1f, 1f, 0.3f);
    public bool alive = true;
    public float speed = 0f;

    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (alive)
        {
            if (controllable)
            {
                if (Input.GetKey(KeyCode.UpArrow) && speed < maxSpeed)
                {
                    speed += acceleration * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    speed -= acceleration * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.RightArrow) && speed > -maxSpeed)
                {
                    transform.Rotate(-Vector3.forward * Time.deltaTime * turnSpeed);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(Vector3.forward * Time.deltaTime * turnSpeed);
                }
            }
            float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            body.velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));
        }
        else
        {
            if(renderer.color.a > 0.3f)
            {
                renderer.color = deadColor;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            alive = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("car"))
        {
            Physics2D.IgnoreCollision(collider, collision.collider);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("checkpoint"))
        {
            if (other.gameObject.GetComponent<Checkpoint>().sequenceNumber == (place + 1))
            {
                place++;
                lastPassedCheckpoint = other.gameObject;
            }
        }
        else if (other.CompareTag("finishline"))
        {
            place = 0;
        }
    }
}
