using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movable_UI_Element : MonoBehaviour
{
	[SerializeField]
	Transform rootElement;

	bool hasControl = false;
	Vector3 offset = Vector3.zero;

	public void ToggleControl()
	{
		hasControl = !hasControl;
		offset = rootElement.transform.position - Input.mousePosition;
		if (!hasControl){
			SaveElementPosition();
		}
	}

	void Update()
	{
		if (hasControl){
			rootElement.transform.position = Input.mousePosition + offset;
		}
	}

	public void SaveElementPosition(){
		GameObject.FindGameObjectWithTag("PrimaryInterface").GetComponent<Interface_Controller>().SaveElementPositions();
	}
}
