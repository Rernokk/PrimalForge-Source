using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective_Pane_Controller : MonoBehaviour
{
	public void PushUpdatePhase()
	{
		Objective_Interface.instance.UpdateObjectiveInterface();
		Destroy(gameObject);
	}
}
