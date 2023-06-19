using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    GameObject whiteBall;
    GameObject resetPoint;
    // Start is called before the first frame update
    void Start()
    {
        whiteBall = GameObject.Find("Obj/Balls/Ball_00");
        resetPoint = GameObject.Find("resetPoint");
    }

    // Update is called once per frame
    void Update()
    {
        if (whiteBall.transform.position.y < 0.1)
        {
            whiteBall.transform.position = resetPoint.transform.position;
            whiteBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            whiteBall.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        }
    }
}
