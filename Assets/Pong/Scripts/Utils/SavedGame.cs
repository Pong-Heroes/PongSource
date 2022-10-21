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
#if UNITY_EDITOR
using UnityEditor;
#endif
public class SavedGame: ScriptableObject
{
	public Vector2 ballVelocity;
    public Vector2 ballPosition;
	public Vector2 playerPosition;
	public Vector2 aiPosition;
    public Vector2 playerScale;
    public Vector2 aiScale;
    public float playerSpeed;
    public float aiSpeed;

	public int aiScore;
	public int playerScore;

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/SavedGameObject")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<SavedGame> ();
	}
	#endif

}
