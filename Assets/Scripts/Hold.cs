using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    private GameObject whiteBall;
    private GameObject poseDetector;
    private Vector3 forceDirection;
    private float force;

    void OnTriggerEnter(Collider other)     //接触时触发，无需调用
    {
        Debug.Log(Time.time + ":进入该触发器的对象是：" + other.gameObject.name);
        if(other.gameObject == whiteBall)
        {
            forceDirection = poseDetector.GetComponent<leftPalmHandler>().direction.normalized;
            force = poseDetector.GetComponent<leftPalmHandler>().distance;
            whiteBall.GetComponent<Rigidbody>().AddForce(forceDirection * force);
        }
    }
    void OnTriggerStay(Collider other)    //每帧调用一次OnTriggerStay()函数
    {
        Debug.Log(Time.time + "留在触发器的对象是：" + other.gameObject.name);
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log(Time.time + "离开触发器的对象是：" + other.gameObject.name);

    }

    // Start is called before the first frame update
    void Start()
    {
        whiteBall = GameObject.Find("Obj/Pool");
        poseDetector = GameObject.Find("Pose Detector");
    }

    // Update is called once per frame
    void Update()
    {

    }
}