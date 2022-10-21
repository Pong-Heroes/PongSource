using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ConsoleMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text Text;
    [SerializeField] private GameObject Parent;
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button ResendButton;
    [SerializeField] private Button ForgotButton;
    private Action Callback;

    private void OnEnable() {
        Events.OnReportMessage += OnReportMessage;
    }

    private void OnDisable() {
        Events.OnReportMessage -= OnReportMessage;
    }

    // Start is called before the first frame update
    void Start()
    {
        CloseButton.onClick.AddListener(OnCloseButtonClicked);
        ResendButton.onClick.AddListener(OnResendButtonClicked);
        ForgotButton.onClick.AddListener(OnForgotButtonClicked);
    }

    private void OnReportMessage(messageInfo info)
    {
        Parent.SetActive(true);
        Text.text = info.message;
        Callback = info.Callback;

        this.ForgotButton.gameObject.SetActive(info.Forget);
        this.ResendButton.gameObject.SetActive(info.Resend);
    }

    private void OnForgotButtonClicked()
    {
        AudioManager.Audio.PlayClickSound();
        Events.DoFireForgetPassword();
    }

    private void OnCloseButtonClicked()
    {
        AudioManager.Audio.PlayClickSound();
        Parent.SetActive(false);
        Callback?.Invoke();
    }

    private void OnResendButtonClicked()
    {
        AudioManager.Audio.PlayClickSound();
        this.ResendButton.interactable = false;
        apiRequestHandler.Instance.sendVerificationAgain();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
