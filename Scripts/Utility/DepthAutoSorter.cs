using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthAutoSorter : MonoBehaviour
{

	[SerializeField]
	private bool UpdateSorting = false;
	private void Start()
	{
		foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
		{
			rend.sortingOrder = -(int)(transform.position.y * 10);
		}
	}

	private void Update()
	{
		if (UpdateSorting)
		{
			foreach (SpriteRenderer rend in GetComponentsInChildren<SpriteRenderer>())
			{
				rend.sortingOrder = -(int)(transform.position.y * 10);
			}
		}
	}
}
