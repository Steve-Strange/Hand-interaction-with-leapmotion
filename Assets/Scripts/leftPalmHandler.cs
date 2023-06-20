using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class leftPalmHandler : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject person;
    GameObject cue;
    GameObject leftHandIndex_a;
    GameObject PlayerCamera;
    GameObject whiteBall;
    GameObject rightHand_thumb;
    private string leftHand = "Service Provider (Desktop)/GhostHands/Generic Hand_Left/baseMeshHand_Left_GRP/Elbow/Wrist/index_meta/index_a";
    Vector3 tempCameraPosition;
    Vector3 startWhiteBallPosition;
    bool flag = false;
    Vector3 startHand;
    Vector3 curHand;
    public float distance;
    public Vector3 direction;
    public void setPos()
    {
        startHand = rightHand_thumb.transform.position;
        if (!flag)
        {
            flag = true;
            tempCameraPosition = PlayerCamera.transform.position;
        }
        if (leftHandIndex_a.transform.position.x > 2.6 && leftHandIndex_a.transform.position.x < 7 && leftHandIndex_a.transform.position.z > 3.8 && leftHandIndex_a.transform.position.z < 9)
        {
            whiteBall.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
            whiteBall.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
            person.GetComponent<FirstPersonController>().enabled = false;
            
            startWhiteBallPosition = whiteBall.transform.position;

            Vector3 alignHand = rightHand_thumb.transform.position;
            alignHand.y = startWhiteBallPosition.y;
            direction = alignHand;
            cue.transform.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0), direction);
            cue.transform.position = startWhiteBallPosition - direction / (direction.magnitude * 2);

        }
    }


    public void OnPos()
    {
        if (leftHandIndex_a.transform.position.x > 2.6 && leftHandIndex_a.transform.position.x < 7 && leftHandIndex_a.transform.position.z > 3.8 && leftHandIndex_a.transform.position.z < 9)
        {
            curHand = rightHand_thumb.transform.position;
            Vector3 alignHand = rightHand_thumb.transform.position;
            alignHand.y = startWhiteBallPosition.y;
            direction = startWhiteBallPosition - alignHand;
            distance = Vector3.Dot((curHand - startHand), direction) / direction.magnitude;

            cue.transform.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0), direction);
            cue.transform.position = startWhiteBallPosition - direction / (direction.magnitude * 2) + direction / (direction.magnitude * 2) * distance * 4;
            cue.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;

            PlayerCamera.transform.position = startWhiteBallPosition - direction / (direction.magnitude * 2) * 2.5f + new Vector3(0, 0.3f, 0);
            PlayerCamera.transform.LookAt(whiteBall.transform);

        }
    }

    public void OffPos()
    {
        if (flag)
        {
            flag = false;
            PlayerCamera.transform.position = tempCameraPosition;
        }
        person.GetComponent<FirstPersonController>().enabled = true;
        PlayerCamera.transform.position = tempCameraPosition;
    }

    void Start()
    {
        person = GameObject.Find("FirstPersonController");
        cue = GameObject.Find("Obj/Pool");
        leftHandIndex_a = GameObject.Find(leftHand);
        PlayerCamera = GameObject.Find("FirstPersonController/Joint/PlayerCamera");
        whiteBall = GameObject.Find("Obj/Balls/Ball0");
        rightHand_thumb = GameObject.Find("Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/middle_meta/middle_a");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
