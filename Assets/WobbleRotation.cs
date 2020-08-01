using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleRotation : MonoBehaviour
{
    Rigidbody2D r;
    // Start is called before the first frame update
    void Start()
    {
        r = GetComponentInParent<Rigidbody2D>();
        trueRight = transform.right;
    }
    Vector2 trueRight;
    static Vector2 wind;
    public Transform target;
    public float amount;
    // Update is called once per frame
    void Update()
    {
        //trueRight = transform.parent.right;
        Vector2 jass = Vector2.MoveTowards(trueRight, target.position, amount);
        transform.right = Vector2.Lerp(transform.right, jass,.01f);
    }
}
