using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class TweenSettingsObject
{
    public float AxisOffset = 0.0f;
    public float TweenSpeed = 0.0f;
    public iTween.EaseType EaseType;
    public iTween.LoopType LoopType;
}
public class HoverAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool AutomateHover = false;
    public TweenSettingsObject TweenSetting;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(AutomateHover)
        Invoke("TweenHover", 1f);
    }

    public void TweenHover()
    {
       if(AnimationsHandler.Instance)
            AnimationsHandler.Instance.HoverYAxis(this.gameObject, TweenSetting.AxisOffset, TweenSetting.TweenSpeed, TweenSetting.EaseType, TweenSetting.LoopType);
    }


    //this function will be called whenver user points on this game object
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AutomateHover)
            return;
        if(AudioManager.Audio)
            AudioManager.Audio.PlayHoverSound();

            AnimationsHandler.Instance.scaleUpButton(gameObject);
    }

    //this function will be called when user perform pointerexit event
    public void OnPointerExit(PointerEventData eventData)
    {
        if (AutomateHover)
            return;

            AnimationsHandler.Instance.scaleDownButton(gameObject);
    }
}
