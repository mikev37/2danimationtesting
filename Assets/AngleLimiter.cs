using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleLimiter : MonoBehaviour
{

    public Transform other;
    public bool lower;
    public bool upper;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Mathf.DeltaAngle(transform.rotation.z, other.transform.rotation.z));
        if(upper && transform.rotation.z > other.transform.rotation.z)
        {
            transform.right = other.transform.right;
        }
        if (lower && transform.rotation.z < other.transform.rotation.z)
        {
            transform.right = other.transform.right;
        }
    }
}
