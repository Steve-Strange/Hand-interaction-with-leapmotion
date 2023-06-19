using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leftPalmHandler : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject cue;
    GameObject leftHandIndex_a;
    private string leftHand = "Service Provider (Desktop)/GhostHands/Generic Hand_Left/baseMeshHand_Left_GRP/Elbow/Wrist/index_meta/index_a";
    private string rightHand = "Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/middle_meta/middle_a";
    private string rightHand_hand_thumb_middle = "Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/index_meta";
    private string rightHand_hand_thumb_end = "Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/index_meta/index_a/index_b/index_c/index_end";
    float speed = 1.0f;
    private Vector3 startPosition;
    public void setPos()
    {
        if(leftHandIndex_a.transform.position.x > 3 && leftHandIndex_a.transform.position.x < 6.5 && leftHandIndex_a.transform.position.z > 3.5 && leftHandIndex_a.transform.position.z < 8.5)
        {
            startPosition = GameObject.Find(rightHand).GetComponent<Transform>().position;
            cue.transform.position = (GameObject.Find(leftHand).GetComponent<Transform>().position + 10 * GameObject.Find(rightHand).GetComponent<Transform>().position) / 11;
            Vector3 targetRotation = GameObject.Find(rightHand_hand_thumb_end).GetComponent<Transform>().position
                - GameObject.Find(rightHand_hand_thumb_middle).GetComponent<Transform>().position;
            Quaternion rotation = Quaternion.LookRotation(targetRotation, Vector3.forward);
            cue.transform.rotation = rotation;
            // cue.transform.rotation = Quaternion.FromToRotation(cue.transform.up, targetPosition);
        }
    }


    public void OnPos()
    {
        if (leftHandIndex_a.transform.position.x > 3 && leftHandIndex_a.transform.position.x < 6.5 && leftHandIndex_a.transform.position.z > 3.5 && leftHandIndex_a.transform.position.z < 8.5)
        {
            cue.transform.position = startPosition + (GameObject.Find(rightHand).GetComponent<Transform>().position - startPosition) * 3;
            GameObject.Find("Obj/Pool").GetComponent<Rigidbody>().velocity = 5 * GameObject.Find(rightHand).GetComponent<Rigidbody>().velocity;
            // cue.transform.position = startPosition +
            //    new Vector3((GameObject.Find(rightHand).GetComponent<Transform>().position.x - startPosition.x) * 1.5f, (GameObject.Find(rightHand).GetComponent<Transform>().position.y - startPosition.y) * 1.5f, (GameObject.Find(rightHand).GetComponent<Transform>().position.z - startPosition.z) * 3);
            //      Vector3 targetPosition = GameObject.Find(leftHand).GetComponent<Transform>().position - GameObject.Find(rightHand).GetComponent<Transform>().position;
            //      cue.transform.rotation = Quaternion.FromToRotation(cue.transform.up, targetPosition);
            cue.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        }
        }

    void Start()
    {
        cue = GameObject.Find("Obj/Pool");
    }

    // Update is called once per frame
    void Update()
    {
        leftHandIndex_a = GameObject.Find(leftHand);
    }
}
