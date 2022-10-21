using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class _DoEmissiveColor : _tween
{
    public Renderer _target;
    public Color _color;
    public string _matProperty = "_EmissionColor";
    public float _intensity;
    // Start is called before the first frame update
    public override void Start()
    {
        _myTween = _target.material.DOColor(_color * _intensity, _matProperty, _tweenParameters._duration)
        .SetLoops(_tweenParameters._loopCycles, _tweenParameters._loopType)
        .SetEase(_tweenParameters._easeType);
        base.Start();
    }
}
