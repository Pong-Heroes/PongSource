using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetainPosition : MonoBehaviour
{
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(offset.x, Screen.height - offset.y, offset.z);
        transform.position = Camera.main.ScreenToWorldPoint(pos);
    }
}
