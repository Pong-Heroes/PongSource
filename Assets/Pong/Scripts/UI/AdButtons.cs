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

public class AdButtons : MonoBehaviour {

	public void OnClickRewarded()
	{
        print("Show Rewarded Ads");
		Managers.Adv.ShowRewardedAd ();
	}

	public void OnClickDefault()
	{
        print("Show Default Ads");
        Managers.Adv.ShowDefaultAd ();
	}

}
