//  /*********************************************************************************
//   *********************************************************************************
//   *********************************************************************************
//   * Produced by Skard Games										                  *
//   * Facebook: https://goo.gl/5YSrKw											      *
//   * Contact me: https://goo.gl/y5awt4								              *											
//   * Developed by Cavit Baturalp Gürdin: https://tr.linkedin.com/in/baturalpgurdin *
//   *********************************************************************************
//   *********************************************************************************
//   *********************************************************************************/

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
