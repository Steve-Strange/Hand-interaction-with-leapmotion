using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
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

    void ResetAll()
    {
        for (int i = 0; i <= 15; i++)
        {
            GameObject.Find("Obj/Balls/Ball" + i).transform.position = GameObject.Find("Obj/Balls/ResetPoint" + i + " (1)").transform.position;
        }
    }
}
