using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Woodcutting_Controller : MonoBehaviour {
	float curVal = 0;
	Slider sliderRef;

	[SerializeField] RectTransform zoneSegment;

	[SerializeField] float lowerRange = 0, upperRange = 1, width = .1f;
	[SerializeField] float gameRate = 1.0f;
	
	// Use this for initialization
	void Start () {
		sliderRef = GetComponent<Slider>();
		lowerRange = Random.Range(0.0f, 1.0f - width);
		upperRange = lowerRange + width;
		zoneSegment.sizeDelta = new Vector2(zoneSegment.transform.parent.GetComponent<RectTransform>().rect.size.x * width, zoneSegment.transform.parent.GetComponent<RectTransform>().rect.size.y);
		zoneSegment.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, zoneSegment.transform.parent.GetComponent<RectTransform>().rect.size.x * lowerRange, zoneSegment.sizeDelta.x);
	}
	
	// Update is called once per frame
	void Update () {
		curVal += Time.deltaTime * gameRate;
		//sliderRef.value = .5f * (Mathf.Sin(curVal) + 1);
		if (Input.GetKeyDown(KeyCode.Space)){
			if (sliderRef.value >= lowerRange && sliderRef.value <= upperRange){
				print("Nailed it!");
				//Do Stuff & Things.
			}
		}
	}
}
