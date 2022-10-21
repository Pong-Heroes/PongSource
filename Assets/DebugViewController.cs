using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugViewController : MonoBehaviour
{
    [SerializeField] private GameObject ViewObject;
    [SerializeField] private Slider IntelligenceSlider;

    [Space(10)]
    [SerializeField] private Slider ImpactSlider;
    [SerializeField] private TMP_Text ImpactCurrentText;

    [Space(10)]
    [SerializeField] private Slider BallSpeedSlider;
    [SerializeField] private TMP_Text BallSpeedCurrentText;


    // Start is called before the first frame update
    void Start()
    {
        OnBallSpeedValueChanged(BallSpeedSlider.value);
        OnImpactMultiplierValueChanged(ImpactSlider.value);
        OnIntelligenceValueChanged(IntelligenceSlider.value);

        BallSpeedSlider.onValueChanged.AddListener(OnBallSpeedValueChanged);
        ImpactSlider.onValueChanged.AddListener(OnImpactMultiplierValueChanged);
        IntelligenceSlider.onValueChanged.AddListener(OnIntelligenceValueChanged);

        Events.DoChangeBallSpeed(BallSpeedSlider.value);
        Events.DoChangeImpactMultiplier(ImpactSlider.value);
        Events.DoChangeIntelligence(Mathf.RoundToInt(IntelligenceSlider.value));
    }

    public void ActivateView() => ViewObject.SetActive(!ViewObject.activeSelf);

    private void OnBallSpeedValueChanged(float value)
    {
        BallSpeedCurrentText.text = value.ToString("0.00");
        Events.DoChangeBallSpeed(value);
    }

    private void OnImpactMultiplierValueChanged(float value)
    {
        ImpactCurrentText.text = value.ToString("0.00");
        Events.DoChangeImpactMultiplier(value);
    }
    private void OnIntelligenceValueChanged(float value) => Events.DoChangeIntelligence(Mathf.RoundToInt(value));
}
