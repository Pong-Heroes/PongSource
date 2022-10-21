using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class _DoRotate : _tween
{
    public Vector3 _target;

    // Start is called before the first frame update
    public override void Start()
    {
        _myTween = transform.DOLocalRotate(_target, _tweenParameters._duration).SetLoops(_tweenParameters._loopCycles).SetEase(_tweenParameters._easeType);
        base.Start();
    }
}
