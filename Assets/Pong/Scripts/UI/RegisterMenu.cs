using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static partial class Events
{
    public static Action OnRegisterationSuccess = null;
    public static void DoFireRegsiterationSuccess() => OnRegisterationSuccess?.Invoke();
    public static Action<string> OnRegisterationFailed = null;
    public static void DoFireRegsiterationFailed(string reason) => OnRegisterationFailed?.Invoke(reason);

    public static Action OnVerificationSent = null;
    public static void DoFireVerificationSent() => OnVerificationSent?.Invoke();
}

public class RegisterMenu : MonoBehaviour
{
    [SerializeField] private Button CloseButton;
    [SerializeField] private Button SubmitButton;
    [SerializeField] private Button ResendButton;

    [SerializeField] private TMP_InputField EmailField, PasswordField, ConfirmPasswordField, UsernameField, WalletField;
    [SerializeField] private TMP_Text ResendTimerText;
    [SerializeField] private TMP_Text StatusText;
    private Coroutine animateRoutine;

    private const float MaxResendTimer = 120;
    private float ResendTimer;

   
    private void OnEnable()
    {
        EmailField.text = PasswordField.text = ConfirmPasswordField.text = UsernameField.text = "";
        WalletField.text = Constants.GetShortWalletAddress(Constants.WalletAddress);
        MakeInteractable(true);

        ResendTimer = 120;
        ResendTimerText.text = "Resend";
        SubmitButton.gameObject.SetActive(true);
        ResendButton.gameObject.SetActive(false);
    }
    private void MakeInteractable(bool value)
    {
        EmailField.interactable =
        PasswordField.interactable =
        ConfirmPasswordField.interactable =
        UsernameField.interactable =
        SubmitButton.interactable = value;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.CloseButton.onClick.AddListener(OnCloseButtonClicked);
        this.SubmitButton.onClick.AddListener(OnSubmitButtonClicked);
        this.ResendButton.onClick.AddListener(OnResendButtonClicked);

        Events.OnRegisterationFailed += OnFailure;
        Events.OnRegisterationSuccess += OnRegisterationSuccess;
        Events.OnVerificationSent += OnVerificationSent;
    }

    private void OnVerificationSent()
    {
        ResendTimer = MaxResendTimer;
        ResendButton.interactable = false;
        SubmitButton.gameObject.SetActive(false);
        ResendButton.gameObject.SetActive(true);
        StopCoroutine(animateRoutine);
        this.StatusText.text = $"";

        EmailField.interactable = PasswordField.interactable = ConfirmPasswordField.interactable = UsernameField.interactable = false;
    }

    private void OnFailure(string error)
    {
        MakeInteractable(true);
        this.StatusText.text = $"";
        StopCoroutine(animateRoutine);
    }
    private void OnRegisterationSuccess()
    {
        MakeInteractable(true);
        this.StatusText.text = $"";
        StopCoroutine(animateRoutine);
        Managers.UI.ActivateUI(Menus.MAIN);
        //AudioManager.Audio.PlayLobbyMusic();
    }

    private void OnCloseButtonClicked()
    {
        Managers.UI.ActivateUI(Menus.LOGIN);
    }

    private void OnSubmitButtonClicked()
    {
        MakeInteractable(false);
        animateRoutine = StartCoroutine(AnimateStatus());
        apiRequestHandler.Instance.signUpWithEmail(EmailField.text, PasswordField.text, UsernameField.text);
    }
    IEnumerator AnimateStatus()
    {
        var delay = 0.5f;
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait.";
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait..";
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait...";
        yield return new WaitForSeconds(delay);
        this.StatusText.text = $"please wait";

        animateRoutine = StartCoroutine(AnimateStatus());
    }
    private void OnResendButtonClicked()
    {
        apiRequestHandler.Instance.sendVerificationAgain();
    }

    // Update is called once per frame
    void Update()
    {
        if (ResendButton.gameObject.activeSelf && !ResendButton.interactable)
        {
            ResendTimer = Mathf.Clamp(ResendTimer - Time.deltaTime, 0, MaxResendTimer);

            float minutes = Mathf.Floor(ResendTimer / 60);
            float seconds = Mathf.RoundToInt(ResendTimer % 60);
            ResendTimerText.text = $"Resend In ({ResendTimer.ToString($"{minutes}:{seconds}")})";

            if (ResendTimer <= 0)
            {
                ResendTimerText.text = $"Resend";
                ResendButton.interactable = true;
            }
        }
    }
}
