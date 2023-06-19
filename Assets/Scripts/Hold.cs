using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hold : MonoBehaviour
{
    private GameObject whiteBall;
    private GameObject poseDetector;
    private Vector3 forceDirection;
    private float force;

    void OnTriggerEnter(Collider other)     //�Ӵ�ʱ�������������
    {
        Debug.Log(Time.time + ":����ô������Ķ����ǣ�" + other.gameObject.name);
        if(other.gameObject == whiteBall)
        {
            forceDirection = poseDetector.GetComponent<leftPalmHandler>().direction.normalized;
            force = poseDetector.GetComponent<leftPalmHandler>().distance;
            whiteBall.GetComponent<Rigidbody>().AddForce(forceDirection * force);
        }
    }
    void OnTriggerStay(Collider other)    //ÿ֡����һ��OnTriggerStay()����
    {
        Debug.Log(Time.time + "���ڴ������Ķ����ǣ�" + other.gameObject.name);
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log(Time.time + "�뿪�������Ķ����ǣ�" + other.gameObject.name);

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