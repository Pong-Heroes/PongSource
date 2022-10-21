using UnityEngine;
using System.Collections;

public class GamePlayState : _StatesBase
{


    #region implemented abstract members of _StatesBase
    public override void OnActivate()
    {
        GA_AnalyticsManager.Instance.PushProgressionEvent(true, false, false);
        Debug.Log("<color=green>Gameplay State</color> OnActive");
    }
    public override void OnDeactivate()
    {
        // Debug.Log ("<color=red>Gameplay State</color> OnDeactivate");
    }

    public override void OnUpdate()
    {
        // Debug.Log ("<color=yellow>Gameplay State</color> OnUpdate");
        Managers.Match.ball.ParticleRotation();

    }
    #endregion

}
