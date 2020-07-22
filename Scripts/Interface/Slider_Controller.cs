using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_Controller : MonoBehaviour {
	Slider sliderRef;
	VerticalLayoutGroup group;
	float startOff, endOff;

	private void Start()
	{
		sliderRef = transform.parent.GetComponentInChildren<Slider>();
		group = GetComponent<VerticalLayoutGroup>();
		startOff = group.padding.top;
		endOff = 50 * transform.childCount;
	}

	public void UpdateUIElement()
	{
		endOff = 50 * (transform.childCount - 2);
		RectOffset newOff = new RectOffset(group.padding.left, group.padding.right, -(int) Mathf.Lerp(startOff, endOff, sliderRef.value), 0);
		group.padding = newOff;
	}
}
