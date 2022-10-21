using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelsMenu : PersistentSingleton<LevelsMenu>
{
    [SerializeField] private Text StatusText;

    [SerializeField] private Button NextButton;
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button SelectButton;
    [SerializeField] private GameObject[] Levels;
    [SerializeField] private Sprite[] Previews;

    [SerializeField] private Transform ButtonsContainer;
    [SerializeField] private LevelButton LevelButtonPrefab;
    private List<LevelButton> LevelButtons = new List<LevelButton>();

    [SerializeField] private Button BackButton;

    public void StartTournament()
    {
        Constants.tournamentMode = TournamentMode.FREESTYLE;
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

        GA_AnalyticsManager.Instance.StoredProgression.Difficulty = "Intermediate";
        Events.DoChangeIntelligence(2);
    }

    private void OnBackButtonPressed()
    {
        AudioManager.Audio.PlayClickSound();
        Managers.UI.ActivateUI(Menus.MAIN);
    }

    private int Index;

    private void OnSelect()
    {
        PlayerPrefs.SetInt("Level", Index);
        PlayerPrefs.Save();

        AudioManager.Audio.PlayClickSound();
        GA_AnalyticsManager.Instance.StoredProgression.MapUsed = $"Level_{Index}";

        if (Constants.Mode == GameMode.TOURNAMENT)
            StartTournament();
        else
            Managers.UI.ActivateUI(Menus.DIFFICULTY);
    }

    private void OnNext()
    {
        Levels[Index].SetActive(false);
        if (Index + 1 >= Levels.Length)
        {
            Index = 0;
        }
        else
            Index++;

        UpdateView();
        AudioManager.Audio.PlayClickSound();
        //Debug.Log("OnNext()");
    }

    private void OnPrev()
    {
        Levels[Index].SetActive(false);
        if (Index - 1 >= 0)
        {
            Index--;
        }
        else
            Index = Levels.Length - 1;

        UpdateView();
        AudioManager.Audio.PlayClickSound();
        //Debug.Log("OnPrev()");
    }

    private void UpdateView()
    {
        for (int q = 0; q < Levels.Length; q++)
        {
            if(q==Index)
                Levels[q].SetActive(true);
            else
                Levels[q].SetActive(false);
        }
    }

    private void OnEnable()
    {
        NextButton.onClick.AddListener(OnNext);
        PrevButton.onClick.AddListener(OnPrev);
        SelectButton.onClick.AddListener(OnSelect);
        BackButton.onClick.AddListener(OnBackButtonPressed);

        if (Constants.Mode == GameMode.TOURNAMENT)
        {
            Index = Constants.SelectedMapTournament;
            PlayerPrefs.SetInt("Level", Index);
        }
        else
            Index = PlayerPrefs.GetInt("Level", Index);

        UpdateView();
        GenerateButtons();
    }

    private void OnDisable()
    {
        NextButton.onClick.RemoveListener(OnNext);
        PrevButton.onClick.RemoveListener(OnPrev);
        SelectButton.onClick.RemoveListener(OnSelect);
        BackButton.onClick.RemoveListener(OnBackButtonPressed);
    }

    private void GenerateButtons()
    {
        for (int i = 0; i < LevelButtons.Count; i++)
        {
            Destroy(LevelButtons[i].gameObject);
        }

        LevelButtons.Clear();
        for (int i = 0; i < Levels.Length; i++)
        {
            var LevelButton = Instantiate<LevelButton>(LevelButtonPrefab, ButtonsContainer);
            LevelButtons.Add(LevelButton);

            if (Constants.Mode == GameMode.TOURNAMENT)
            {
                if(i==Constants.SelectedMapTournament)
                {
                    LevelButton.Init(Previews[i], i, Index == i, (ind) =>
                    {
                        AudioManager.Audio.PlayClickSound();
                        LevelButtons[Index].UpdateView(ind);
                        Levels[Index].SetActive(false);
                        LevelButton.UpdateView(ind);
                        Index = ind;
                        UpdateView();
                    }, true);
                }else
                {
                    LevelButton.Init(Previews[i], i, Index == i, (ind) =>
                    {
                        AudioManager.Audio.PlayClickSound();
                        LevelButtons[Index].UpdateView(ind);
                        Levels[Index].SetActive(false);
                        LevelButton.UpdateView(ind);
                        Index = ind;
                        UpdateView();
                    }, false);
                }
            }
            else
            {
                LevelButton.Init(Previews[i], i, Index == i, (ind) =>
                {
                    AudioManager.Audio.PlayClickSound();
                    LevelButtons[Index].UpdateView(ind);
                    Levels[Index].SetActive(false);
                    LevelButton.UpdateView(ind);
                    Index = ind;
                    UpdateView();
                }, true);
            }
        }
    }

}
