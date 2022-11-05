
using UnityEngine;
using System.Collections;

public enum InputMethod
{
	KeyboardInput,
	DragInput,
	TouchLRInput
}

public class PlayerInputManager : MonoBehaviour {

	public bool isActive;
	public InputMethod inputType;
}
