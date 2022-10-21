using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class leaderboardCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text UsernameText;
    [SerializeField] private Text ScoresText;
    [SerializeField] private Text WalletAddressText;
    [SerializeField] private GameObject[] Ranks;

    private int rank;
    private string username;
    private string wallet;
    private int score;
    private int avatarId;
    private Action<int, string> callback;

    public void Init(int rank, string username, string wallet, int score, int avatarId, Action<int, string> callback = null)
    {
        this.rank = rank;
        this.username = username;
        this.wallet = wallet;
        this.score = score;
        this.avatarId = avatarId;
        this.callback = callback;

        if(rank<Ranks.Length)
            Ranks[rank].SetActive(true);

        UsernameText.text = $"{rank + 1}. {username}";
        WalletAddressText.text =Constants.GetShortWalletAddress(wallet);
        ScoresText.text = score.ToString();
        this.gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        callback?.Invoke(this.avatarId, this.username);
    }
}
