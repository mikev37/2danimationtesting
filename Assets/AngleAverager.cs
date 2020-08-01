using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleAverager : MonoBehaviour
{

    public Transform[] others;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 sum = Vector2.zero ;
        foreach(Transform t in others)
        {
            sum += (Vector2)t.right;
        }
        sum /= others.Length;

        transform.right = Vector2.Lerp(transform.right, sum, Time.deltaTime);
    }
}
