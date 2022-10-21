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
using Photon.Pun;

[System.Serializable]
public enum PlayerSelected
{
    PLAYERA=0,
    PLAYERB=1
}
public enum PaddleOwner
{
    PLAYER,
    AI
}

public class BasePaddle : MonoBehaviourPunCallbacks
{
    public float speed;
    [HideInInspector]
    public Vector2 scale;
    int cnt = 0;
    #region Private Vars
    protected Ball _ball;
    [SerializeField]protected Rigidbody2D _rigidBody;
    [SerializeField] protected float MaxBound = 2.36f;
    [SerializeField] protected Animator Animator;
    #endregion
    [HideInInspector] public bool RubySwordActivated = false;
    [HideInInspector] public static bool RubySwordGlobalActivattion=false;
    [HideInInspector] public static bool StoreVelocity = false;
    public PlayerSelected playerSelected;

    protected virtual void Start()
    {
        _ball = Managers.Match.ball;
    }

    public override void OnEnable()
    {
        if (!Animator)
            Animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {

    }

    public void PowerUpEnlarge()
    {
        StopCoroutine(ApplyEnlarge());
        StartCoroutine(ApplyEnlarge());
    }
    public IEnumerator ApplyEnlarge()
    {
        this.transform.localScale = Managers.PowUps.enlargeData.EnlargeScale;
        this.GetComponent<BoxCollider2D>().size = Managers.PowUps.enlargeData.EnlargeColliderSize;

        yield return new WaitForSeconds(Managers.PowUps.enlargeData.EnlargeImpactDuration);

        this.transform.localScale = Managers.PowUps.enlargeData.NormalScale;
        this.GetComponent<BoxCollider2D>().size = Managers.PowUps.enlargeData.NoramlColliderSize;
    }

    public void PowerUpShrink()
    {
        StopCoroutine(ApplyShrink());
        StartCoroutine(ApplyShrink());
    }
    public IEnumerator ApplyShrink()
    {
        this.transform.localScale = Managers.PowUps.shrinkData.ShrinkScale;
        this.GetComponent<BoxCollider2D>().size = Managers.PowUps.shrinkData.ShrinkColliderSize;

        yield return new WaitForSeconds(Managers.PowUps.shrinkData.ShrinkImpactDuration);

        this.transform.localScale = Managers.PowUps.shrinkData.NormalScale;
        this.GetComponent<BoxCollider2D>().size = Managers.PowUps.shrinkData.NoramlColliderSize;
    }

    public void PowerUpRubySword()
    {
        StopCoroutine(ApplyRubySword());
        StartCoroutine(ApplyRubySword());
    }


    public IEnumerator ApplyRubySword()
    {
        if(!BasePaddle.StoreVelocity)
            BasePaddle.StoreVelocity = true;

        BasePaddle.RubySwordGlobalActivattion = true;
        RubySwordActivated = true;
        yield return new WaitForSeconds(Managers.PowUps.rubySwordData.SwordImpactDuration);
        RubySwordActivated = false;
        BasePaddle.RubySwordGlobalActivattion = false;
        BasePaddle.StoreVelocity = false;
    }

    public void PowerUpMysteryBox(BasePaddle _pod)
    {
        StopCoroutine(ApplyMysteryBox(_pod));
        StartCoroutine(ApplyMysteryBox(_pod));
    }

    public IEnumerator ApplyMysteryBox(BasePaddle _pod)
    {
        int _random = Random.Range(0, Managers.PowUps.mysteryBoxData.PowerUpRandom.Count);
        GameObject clone = (GameObject)Instantiate(Managers.PowUps.mysteryBoxData.PowerUpRandom[_random], Vector3.zero, Quaternion.identity);
        clone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        clone.GetComponent<Powerup>().TriggerPowerup(_pod);
        yield return new WaitForSeconds(Managers.PowUps.mysteryBoxData.BoxImpactDuration);
    }

    public void PowerUpExtraLife(BasePaddle pad)
    {
        StopCoroutine(ApplyExtraLife(pad));
        StartCoroutine(ApplyExtraLife(pad));
    }

    public IEnumerator ApplyExtraLife(BasePaddle pad)
    {
        if(pad.playerSelected==PlayerSelected.PLAYERA)
            Managers.Score.UpdateScore(true, Managers.PowUps.extraData.LifeIncrement);
        else if (pad.playerSelected == PlayerSelected.PLAYERB)
            Managers.Score.UpdateScore(false, Managers.PowUps.extraData.LifeIncrement);

        yield return null;
    }

    public void PowerUpShield(BasePaddle pad)
    {
        StopCoroutine(ApplyShield(pad));
        StartCoroutine(ApplyShield(pad));
    }

    public IEnumerator ApplyShield(BasePaddle pad)
    {
        if (pad.playerSelected == PlayerSelected.PLAYERA)
            Managers.PowUps.shieldData.PlayerAShield.SetActive(true);
        else if (pad.playerSelected == PlayerSelected.PLAYERB)
            Managers.PowUps.shieldData.PlayerBShield.SetActive(true);

        yield return new WaitForSeconds(Managers.PowUps.shieldData.ShieldImpactDuration);

        if (pad.playerSelected == PlayerSelected.PLAYERA)
            Managers.PowUps.shieldData.PlayerAShield.SetActive(false);
        else if (pad.playerSelected == PlayerSelected.PLAYERB)
            Managers.PowUps.shieldData.PlayerBShield.SetActive(false);
    }

}
