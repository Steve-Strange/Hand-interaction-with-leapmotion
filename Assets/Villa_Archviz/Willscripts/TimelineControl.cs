using UnityEngine;
using System.Collections;

public class TimelineControl : MonoBehaviour {

	public GameObject timeUI = null;
	private uSkyManager skyer;
	private float increment = 0.02f;
	// Use this for initialization

	void Awake(){
		skyer = (uSkyManager)GameObject.Find ("skymanager").GetComponent ("uSkyManager");
	}
	void Start () {
		 
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.N)) {
			skyer.Timeline += increment;
			timeUI.GetComponent<CanvasGroup> ().alpha = 1;
		} 
		else {
			timeUI.GetComponent<CanvasGroup>().alpha = 0;
		}


		if (Input.GetKey(KeyCode.M)) 
		{
			skyer.Timeline -= increment;
			timeUI.GetComponent<CanvasGroup>().alpha = 1;
		}


   }
}

