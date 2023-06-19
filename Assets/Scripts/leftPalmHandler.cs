using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leftPalmHandler : MonoBehaviour
{


    private string leftHand = "Service Provider (Desktop)/GhostHands/Generic Hand_Left/baseMeshHand_Left_GRP/Elbow/Wrist/index_meta/index_a";
    private string rightHand = "Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/middle_meta/middle_a";
    private string rightHand_hand_thumb_middle = "Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/index_meta";
    private string rightHand_hand_thumb_end = "Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/index_meta/index_a/index_b/index_c/index_end";
    // Start is called before the first frame update
    GameObject cue;
    GameObject whiteBall;
    GameObject leftHandIndex_a;
    GameObject rightHand_thumb;
    
    float speed = 1.0f;
    private Vector3 startPosition;
    public void setPos()
    {
  
        if(leftHandIndex_a.transform.position.x > 3 && leftHandIndex_a.transform.position.x < 6.5 && leftHandIndex_a.transform.position.z > 3.5 && leftHandIndex_a.transform.position.z < 8.5)
        {
            Vector3 alignHand = rightHand_thumb.transform.position;
            alignHand.y = whiteBall.transform.position.y;
            Vector3 direction = whiteBall.transform.position - alignHand;
            cue.transform.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0), direction);
            cue.transform.position = whiteBall.transform.position - direction / direction.magnitude;
        }
    }


    public void OnPos()
    {
        if (leftHandIndex_a.transform.position.x > 3 && leftHandIndex_a.transform.position.x < 6.5 && leftHandIndex_a.transform.position.z > 3.5 && leftHandIndex_a.transform.position.z < 8.5)
        {
            Vector3 alignHand = rightHand_thumb.transform.position;
            alignHand.y = whiteBall.transform.position.y;
            Vector3 direction = whiteBall.transform.position - alignHand;
            cue.transform.rotation = Quaternion.LookRotation(new Vector3(0, -1, 0), direction);
            cue.transform.position = whiteBall.transform.position - direction / direction.magnitude;
        }
     }

    void Start()
    {
        cue = GameObject.Find("Obj/Pool");
        whiteBall = GameObject.Find("Obj/Balls/Ball_00");
        rightHand_thumb = GameObject.Find("Service Provider (Desktop)/GhostHands/Generic Hand_Right/baseMeshHand_Right_GRP/Elbow/Wrist/middle_meta/middle_a");
    }

    // Update is called once per frame
    void Update()
    {
        leftHandIndex_a = GameObject.Find(leftHand);
    }
}
