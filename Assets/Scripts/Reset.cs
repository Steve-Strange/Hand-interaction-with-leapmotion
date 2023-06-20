using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{

    public void ResetAll()
    {
        for (int i = 0; i <= 15; i++)
        {
            GameObject thisBall = GameObject.Find("Obj/Balls/Ball" + i);
            thisBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            thisBall.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            thisBall.transform.position = GameObject.Find("Obj/Balls/Ball" + i + " (1)").transform.position;

        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i <= 15; i++)
        {
            if (GameObject.Find("Obj/Balls/Ball" + i).transform.position.y < 0.1)
            {
                GameObject.Find("Obj/Balls/Ball" + i).transform.position = GameObject.Find("Obj/Balls/ResetPoint" + i).transform.position;
                GameObject.Find("Obj/Balls/Ball" + i).GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                GameObject.Find("Obj/Balls/Ball" + i).GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            }
        }
    }
}
