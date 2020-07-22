using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerFlameWallController : MonoBehaviour
{
	private int direction = 1;

	[SerializeField]
	float waveSpeed = 1f;

	#region Properties
	public int WaveDirection
	{
		get
		{
			return direction;
		}

		set
		{
			direction = value;
		}
	}
	#endregion

	private void Start()
	{
		transform.position += new Vector3(0, Random.Range(-3f, 3f));
	}

	private void Update()
	{
		transform.position += new Vector3(direction * waveSpeed, 0) * Time.deltaTime;
	}
}
