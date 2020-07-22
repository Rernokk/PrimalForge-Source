using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawBones : MonoBehaviour {

	List<Transform> bones = new List<Transform>();

	void Start()
	{
		if (bones == null)
		{
			bones = new List<Transform>();
		}
	}

	void AddBones(Transform t)
	{
		bones.Add(t);
		for (int i = 0; i < t.childCount; i++)
		{
			AddBones(t.GetChild(i));
		}
	}

	void Update () {
		bones.Clear();
		AddBones(transform);
		foreach(Transform t in bones)
		{
			if (t.parent != null)
			{
				Debug.DrawLine(t.position, t.parent.position, Color.red);
			}
		}
	}
}
