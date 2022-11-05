
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
