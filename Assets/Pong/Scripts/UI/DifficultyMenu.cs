using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DifficultyMenu : PersistentSingleton<MonoBehaviour>
{
    [SerializeField] private Button Easy, Medium, Hard, BackButton;
    public override void Awake()
    {
        base.Awake();

        Easy.onClick.AddListener(OnEasyPressed);
        Medium.onClick.AddListener(OnMediumPressed);
        Hard.onClick.AddListener(OnHardPressed);
        BackButton.onClick.AddListener(OnBackButtonPressed);
    }

    private void OnEasyPressed()
    {
        StartGame();
        GA_AnalyticsManager.Instance.StoredProgression.Difficulty = "Beginner";
        Events.DoChangeIntelligence(1);
    }

    private void OnMediumPressed()
    {
        StartGame();
        GA_AnalyticsManager.Instance.StoredProgression.Difficulty = "Intermediate";
        Events.DoChangeIntelligence(2);
    }

    private void OnHardPressed()
    {
        StartGame();
        GA_AnalyticsManager.Instance.StoredProgression.Difficulty = "Expert";
        Events.DoChangeIntelligence(3);
    }

    private void OnBackButtonPressed()
    {
        AudioManager.Audio.PlayClickSound();
        Managers.UI.ActivateUI(Menus.LEVELS);
    }

    private void StartGame()
    {
        Events.DoFireGameStart(true);
        AudioManager.Audio.PlayClickSound();
        Managers.Match.ResetSavedGame();
        Managers.Game.SetState(typeof(KickOffState));
        Managers.UI.ActivateUI(Menus.INGAME);
        Managers.Score.playerScore = Managers.Score.aiScore = 0;
        Managers.UI.inGameUI.UpdateScore();

        int levelIndex = PlayerPrefs.GetInt("Level");

        switch (levelIndex)
        {
            case 0:
                AudioManager.Audio.PlayCyberpunkMusic();
                break;
            case 1:
                AudioManager.Audio.PlaySpaceMusic();
                break;
            case 2:
                AudioManager.Audio.PlayForestMusic();
                break;
        }
    }
}
