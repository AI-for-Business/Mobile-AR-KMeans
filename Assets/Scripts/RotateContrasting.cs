using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RotateContrasting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //float yRot = Camera.main.transform.eulerAngles.y;
        //float yRot = (this.transform.position - Camera.main.transform.position).y;
        //this.transform.rotation = Quaternion.Euler(0, yRot, 0);
        //transform.LookAt(Camera.main.transform);
        transform.rotation = Quaternion.Euler(0, Quaternion.LookRotation(transform.position - Camera.main.transform.position).y - 22, 0);
    }
}
