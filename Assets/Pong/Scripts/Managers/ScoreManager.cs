using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{

    public int playerScore;
    public int aiScore;
    //public int scoreLimit;

    public int TotalHits;
    public int TotalWins;
    public int TotalScore => TotalHits + TotalWins;

    public PaddleOwner Winner
    {
        get
        {
            var playerWon = playerScore > aiScore;

            if (playerWon)
            {
                var data = FirebaseManager.Instance.PlayerData;

                if (Constants.Mode == GameMode.TOURNAMENT)
                {
                    data.TotalWins += 1;
                    TotalWins += data.TotalWins;
                    TotalHits += data.TotalHits;

                    data.TotalHits = TotalHits;
                    data.TotalWins = TotalWins;

                    if (data.TotalScore < TotalScore)
                    {
                        data.TotalScore = TotalScore;
                    }

                    apiRequestHandler.Instance.updatePlayerData();
                }
            }

            TotalWins = 0;
            TotalHits = 0;
            return playerWon ? PaddleOwner.PLAYER : PaddleOwner.AI;
        }
    }

    public void OnScore(PaddleOwner scorer)
    {
        BasePaddle.RubySwordGlobalActivattion = false;
        if (scorer == PaddleOwner.PLAYER)
        {
            playerScore++;
            AudioManager.Audio.PlayPointWinSound();
        }
        else if (scorer == PaddleOwner.AI)
        {
            aiScore++;
            AudioManager.Audio.PlayPointLoseSound();
        }

        Managers.UI.inGameUI.UpdateScore();

        if (playerScore == Constants.MaxScore || aiScore == Constants.MaxScore)
            Managers.Game.SetState(typeof(GameOverState));
        else
            Managers.Game.SetState(typeof(GoalState));
    }

    public void UpdateScore(bool isPlayerA, int _score)
    {
        if (isPlayerA)
            aiScore = aiScore - _score <= 0 ? 0 : aiScore - _score;
        else
            playerScore = playerScore - _score <= 0 ? 0 : playerScore - _score;
       
        Managers.UI.inGameUI.UpdateScore();
    }
}
