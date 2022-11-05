
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CameraShake))]
public class CameraManager : MonoBehaviour {

	public Camera main;

	[HideInInspector]
	public CameraShake shaker;

	void Awake()
	{
		shaker = main.gameObject.GetComponent<CameraShake> ();
	}

}
