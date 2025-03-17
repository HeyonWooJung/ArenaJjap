using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RyzeQScript : MonoBehaviour
{
    public Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(startPos, transform.position) <= 10)
        {
            transform.Translate(Vector3.forward);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
