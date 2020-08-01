using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NippleBone : MonoBehaviour
{

    public float distance;
    Vector2 origin;
    Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.parent.localPosition;
        offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = (origin - (Vector2)transform.parent.localPosition)*distance+offset;
    }
}
