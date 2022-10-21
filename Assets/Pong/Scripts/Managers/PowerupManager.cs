using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ShieldData
{
	public GameObject PlayerAShield;
	public GameObject PlayerBShield;
	public float ShieldImpactDuration;
}


[System.Serializable]
public class ShrinkData
{
	public Vector3 NormalScale;
	public Vector3 ShrinkScale;
	public Vector2 NoramlColliderSize;
	public Vector2 ShrinkColliderSize;
	public float ShrinkImpactDuration;
}

[System.Serializable]
public class EnlargeData
{
	public Vector3 NormalScale;
	public Vector3 EnlargeScale;
	public Vector2 NoramlColliderSize;
	public Vector2 EnlargeColliderSize;
	public float EnlargeImpactDuration;
}

[System.Serializable]
public class MysteryBoxData
{
	public List<GameObject> PowerUpRandom;
	public float BoxImpactDuration;
}

[System.Serializable]
public class RubySwordData
{
	public float SpeedIncrease;
	public float SwordImpactDuration;
}

[System.Serializable]
public class ExtraData
{
	public int LifeIncrement;
	public float SwordImpactDuration;
}
public class PowerupManager : MonoBehaviour {

	public EnlargeData enlargeData;
	public ShrinkData shrinkData;
	public MysteryBoxData mysteryBoxData;
	public RubySwordData rubySwordData;
	public ExtraData extraData;
	public ShieldData shieldData;
	public float speedUpValue;
	public float speedDownValue;
	public float enlargeValue;
	public float shrinkValue;
	public float ghostingValue;

	public List<GameObject> powerupPrefabs;

	public bool canSpawnPowerup;
	public float newPowerupInterval;
	public float powerupImpactDuration;

	public List<Powerup> spawnedPowerupList;

	public IEnumerator SpawnPowerup()
	{
		while (canSpawnPowerup)
		{
			Vector2 randomPosition = Random.insideUnitCircle * 2.4f;
			int randomType = Random.Range (0, powerupPrefabs.Count);
			GameObject clone = (GameObject)Instantiate(powerupPrefabs[randomType], randomPosition, transform.rotation);
			spawnedPowerupList.Add (clone.GetComponent<Powerup>());
			yield return new WaitForSeconds (newPowerupInterval);
		}
		yield return null;
	}

	public void DestroyPowerups()
	{
		foreach (Powerup p in spawnedPowerupList.ToList())
		{
			p.DisablePowerup ();
		}
	}
}
