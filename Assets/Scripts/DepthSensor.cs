using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DepthSensor : MonoBehaviour
{
    public int raysTotal;
    public float rayDistance;
    public float FOVMultiplier;
    public LayerMask hitLayers;
    public LayerMask ignoreLayer;
    public GameObject raycastIndicationDebugPrefab;
    public bool debug;

    public float[] distanceSensorData;

    GameObject[] raycastIndicationDebugSprites;

    void Start()
    {
        distanceSensorData = new float[raysTotal];
        raycastIndicationDebugSprites = new GameObject[raysTotal];
        if (debug)
        {
            for(int i = 0; i < raysTotal; i++)
            {
                raycastIndicationDebugSprites[i] = Instantiate(raycastIndicationDebugPrefab);
            }
        }
    }

    void Update()
    {
        for(int i = 0; i < raysTotal; i++)
        {
            Vector2 hit = RayCast(transform.right + transform.up * ((float)(i - (raysTotal - 1) / 2) / (float)raysTotal * FOVMultiplier));
            distanceSensorData[i] = Vector2.Distance(transform.position, hit);
            if (debug)
            {
                raycastIndicationDebugSprites[i].transform.position = hit;
            }
        }
    }

    Vector2 RayCast(Vector2 direction)
    {
        RaycastHit2D[] hits = new RaycastHit2D[1];
        int hitCount = Physics2D.RaycastNonAlloc(transform.position, direction, hits, rayDistance, hitLayers);

        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].collider != null && hits[i].collider.gameObject.layer != ignoreLayer)
            {
                return hits[i].point;
            }
        }
        return direction * rayDistance;
    }
}
