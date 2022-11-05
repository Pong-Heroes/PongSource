using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : PersistentSingleton<MainMenu>
{
    public GameObject menuButtons;
    public GameObject settingsMenu;
    public GameObject credits;
    public GameObject showAdsRewarded;
    public GameObject showAdsDefault;
    public GameObject continueButton;
    public GameObject shopMenu;
    public GameObject ModeSelection;
    public Text pongLogoText;
    public Text XpText;
    public UIManager UIRef;

    void OnEnable()
    {
        pongLogoText.enabled = true;
        menuButtons.SetActive(true);
        UpdateXPText();
        UIRef.DisableObjects();
        UIRef.DisableObsImages();
    }

    public void UpdateXPText()
    {
        if (FirebaseManager.Instance)
        {
            if (FirebaseManager.Instance.PlayerData != null)
                XpText.text = "XP : " + FirebaseManager.Instance.PlayerData.TotalScore.ToString();
            else
                XpText.text = "XP : 0";
        }
    }

    void OnDisable()
    {
        pongLogoText.enabled = false;
        menuButtons.SetActive(false);
    }

    public void Continue()
    {
        AudioManager.Audio.PlayClickSound();
        Managers.Match.RetrieveSavedMatch();
        Managers.Game.SetState(typeof(KickOffState));
        Managers.UI.ActivateUI(Menus.INGAME);
    }

    public void NewGame()
    {
        Constants.Mode = GameMode.CLASSIC;
        AudioManager.Audio.PlayClickSound();
        Managers.Match.Reset();
        // Managers.Match.ResetSavedGame ();
        // Managers.Game.SetState(typeof(KickOffState));
        Managers.UI.ActivateUI(Menus.LEVELS);
        GA_AnalyticsManager.Instance.StoredProgression.Mode = "Practice";
        ModeSelection.SetActive(false);
    }

    public void FreeStyle_NewGame()
    {
        Constants.Mode = GameMode.FREESTYLE;
        AudioManager.Audio.PlayClickSound();
        Managers.Match.Reset();
        // Managers.Match.ResetSavedGame ();
        // Managers.Game.SetState(typeof(KickOffState));
        Managers.UI.ActivateUI(Menus.LEVELS);
        GA_AnalyticsManager.Instance.StoredProgression.Mode = "FreeStyle";
        ModeSelection.SetActive(false);
    }

    public void OnModeSelection()
    {
        AudioManager.Audio.PlayClickSound();
        ModeSelection.SetActive(true);
    }

    public void Tournament()
    {
        if (!Constants.PlayingAsGuest)
        {
            if (Constants.TournamentActive)
            {
                Constants.Mode = GameMode.TOURNAMENT;
                AudioManager.Audio.PlayClickSound();
                Managers.Match.Reset();
                Managers.UI.ActivateUI(Menus.LEVELS);
                GA_AnalyticsManager.Instance.StoredProgression.Mode = "Tournament";
            }
            else
            {
                Events.DoReportMessage(new messageInfo("Tournament is not active"));
            }
        }else
        {
            Events.DoReportMessage(new messageInfo("You cannot play tournament as a guest, please register or login."));
        }
    }

    public void Multiplayer()
    {
        AudioManager.Audio.PlayClickSound();
        SceneManager.LoadScene("PhotonLauncher");
    }

    public void Settings()
    {
        AudioManager.Audio.PlayClickSound();
        DisableMenuButtons();
        settingsMenu.SetActive(true);
    }

    public void Shop()
    {
        AudioManager.Audio.PlayClickSound();
        DisableMenuButtons();
        shopMenu.SetActive(true);
    }

    public void Credits()
    {
        AudioManager.Audio.PlayClickSound();
        DisableMenuButtons();
        credits.SetActive(true);
    }

    public void DisableMenuButtons()
    {
        menuButtons.SetActive(false);
    }

    public void Logout()
    {
        Constants.PlayingAsGuest = false;
        AudioManager.Audio.PlayClickSound();
        Managers.UI.ActivateUI(Menus.LOGIN);
        //AudioManager.Audio.PlayLoginMusic();
    }

}

