using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    transform.Translate(Time.deltaTime * speed, 0, 0);
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Time.fixedDeltaTime * speed, 0, 0);
    }
}
