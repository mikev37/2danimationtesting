using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool blink = false;
    public float speed = 5;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            blink = true;
        }
        if (blink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(1, -1), speed * Time.deltaTime);
            if(transform.localScale.y <= -.9f)
            {
                blink = false;
            }
        }
        else
        {
            transform.localScale = Vector2.Lerp(transform.localScale, Vector2.one, speed * Time.deltaTime);
        }
    }
}
