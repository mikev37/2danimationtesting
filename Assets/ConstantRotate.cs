using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    public Quaternion rotate;
    float time;
    private void Start()
    {
        time = rotate.w;
    }
    // Update is called once per frame
    void Update()
    {
        rotate.w = 1 / Time.deltaTime * time;
        this.transform.localRotation = rotate * transform.localRotation;
    }
}
