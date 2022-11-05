using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum PowerupType
{
    POWERUP_SHIELD,
    POWERUP_MYSTERYBOX,
    POWERUP_RUBYSWORD,
    POWERUP_EXTRALIFE,
    PADDLE_ENLARGE,
    PADDLE_SHRINK,
    PADDLE_GHOST
}

[RequireComponent(typeof(SpriteRenderer))]
public class Powerup : MonoBehaviour
{

    public PowerupType type;
    public float PowerupLifeDuration;

    private SpriteRenderer _sprite;
    bool CanCheckActive = true;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        CanCheckActive = true;
    }

    void Start()
    {
        FadePowerup(false);
    }

    public void TriggerPowerup(BasePaddle pad)
    {
        PowerUp(pad);
        FadePowerup(true);
    }


    public void FadePowerup(bool collected)
    {
        if (collected)
        {
            DisablePowerup();
        }
        else
        {
            DOTween.ToAlpha(() => _sprite.color, x => _sprite.color = x, 0, PowerupLifeDuration / 7).SetLoops(7)
        .OnComplete(() =>
        {
            DisablePowerup();
        }
        );
        }
    }

    private void FixedUpdate()
    {
        CheckForGameActive();
    }

    public void CheckForGameActive()
    {
        if(!Managers.Game.isGameActive && CanCheckActive)
        {
            CanCheckActive = false;
            DisablePowerup();
        }
    }

    public void PowerUp(BasePaddle pad)
    {
        switch (type)
        {
            case PowerupType.POWERUP_SHIELD:
                pad.PowerUpShield(pad);
                break;
            case PowerupType.POWERUP_MYSTERYBOX:
                pad.PowerUpMysteryBox(pad);
                break;
            case PowerupType.POWERUP_RUBYSWORD:
                pad.PowerUpRubySword();
                break;
            case PowerupType.POWERUP_EXTRALIFE:
                pad.PowerUpExtraLife(pad);
                break;
            case PowerupType.PADDLE_ENLARGE:
                pad.PowerUpEnlarge();
                break;
            case PowerupType.PADDLE_SHRINK:
                pad.PowerUpShrink();
                break;
            case PowerupType.PADDLE_GHOST:
                PaddleGhost(pad);
                break;
        }
    }

    public void PaddleSpeedUp(BasePaddle pad)
    {
        if (pad.speed < 15)
            pad.speed += Managers.PowUps.speedUpValue;
    }

    public void PaddleSpeedDown(BasePaddle pad)
    {
        if (pad.speed > 5)
            pad.speed -= Managers.PowUps.speedDownValue;
    }

    public void PaddleEnlarge(BasePaddle pad)
    {
        if (pad.transform.localScale.y < 1.7f)
            pad.transform.localScale += new Vector3(0, Managers.PowUps.enlargeValue, 0);
    }

    public void PaddleShrink(BasePaddle pad)
    {
        if (pad.transform.localScale.y > 0.4f)
            pad.transform.localScale -= new Vector3(0, Managers.PowUps.shrinkValue, 0);
    }

    public IEnumerator PaddleGhost(BasePaddle pad)
    {
        print("IMPLEMENT YOUR GHOSTING USE YOUR IMAGINATION xd");
        yield break;
    }

    IEnumerator DisableEffect()
    {
        _sprite.enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(Managers.PowUps.powerupImpactDuration);
        DisablePowerup();
    }

    public void DisablePowerup()
    {
        Managers.PowUps.spawnedPowerupList.Remove(this);
        Destroy(this.gameObject);
    }
}
