using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class _DoAnchorMove : _tween
{
    public RectTransform _target;

    // Start is called before the first frame update
    public override void Start()
    {
        _myTween = this.GetComponent<RectTransform>().DOAnchorPos(_target.anchoredPosition, _tweenParameters._duration);
        base.Start();
    }
}
