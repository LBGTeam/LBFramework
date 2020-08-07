using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            print(Mathf.SmoothStep(1000,2000,i/10f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
