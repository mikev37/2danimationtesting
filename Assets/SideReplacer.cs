using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

public class SideReplacer : MonoBehaviour
{

    SpriteResolver sr;
    public Transform relative;
    public Transform bone;
    public float threshold;
    public bool flip;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteResolver>();
    }

    // Update is called once per frame
    void Update()
    {
        if((!flip && bone.transform.position.x - relative.position.x > threshold )|| (flip && bone.transform.position.x - relative.position.x < -1 * threshold))
        {
            sr.SetCategoryAndLabel(sr.GetCategory(), "Side");
        }
        else
        {
            sr.SetCategoryAndLabel(sr.GetCategory(), "Front");
        }
    }
}
