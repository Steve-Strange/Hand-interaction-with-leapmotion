using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWhiteBall : MonoBehaviour
{
    private GameObject whiteBall;
    private GameObject onPalm;
    private Vector3 forceDirection;
    private float force;

    void OnTriggerEnter(Collider other)     //�Ӵ�ʱ�������������
    {
        Debug.Log(Time.time + ":����ô������Ķ����ǣ�" + other.gameObject.name);

        if(other.gameObject == whiteBall)
        {
            forceDirection = onPalm.GetComponent<leftPalmHandler>().direction.normalized;
            force = onPalm.GetComponent<leftPalmHandler>().distance * 3000;
            whiteBall.GetComponent<Rigidbody>().AddForce(forceDirection * (force > 0 ? force : 0));
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
        whiteBall = GameObject.Find("Obj/Balls/Ball0");
        onPalm = GameObject.Find("onPalm");
    }

    // Update is called once per frame
    void Update()
    {

    }
}