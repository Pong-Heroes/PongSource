using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static partial class Events
{
    public static event Action<UserData[]> OnGetUserData = null;
    public static void DoFireGetUserData(UserData[] data) => OnGetUserData?.Invoke(data);
}

public class LeaderboardMenu : PersistentSingleton<LeaderboardMenu>
{
    [SerializeField] private Text UsernameText;
    [SerializeField] private int AvatarID;
    [SerializeField] private Animator AvatarPreview;

    [SerializeField] private leaderboardCard CardPrefab;
    [SerializeField] private Transform EntriesContainer;
    private HashSet<leaderboardCard> Entries = new HashSet<leaderboardCard>();
    [SerializeField] private Text StatusText;
    private Coroutine animateRoutine;

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

    private void OnEnable()
    {
        foreach (var entry in Entries)
        {
            Destroy(entry.gameObject);
        }

        Entries.Clear();

        AvatarID = -1;
        StatusText.text = "";
        UsernameText.text = "";
        AvatarPreview.gameObject.SetActive(false);
        animateRoutine = StartCoroutine(AnimateStatus());
        apiRequestHandler.Instance.getLeaderboard();
    }

    // Start is called before the first frame update
    void Start()
    {
        Events.OnGetUserData += OnGetUserData;
    }

    private void OnGetUserData(UserData[] data)
    {
        StatusText.text = "";
        StopCoroutine(animateRoutine);
        for (int i = 0; i < data.Length; i++)
        {
            var entry = data[i];
            var card = Instantiate<leaderboardCard>(CardPrefab, EntriesContainer);
            card.Init(i, entry.UserName, entry.WalletAddress, entry.TotalScore, entry.AvatarID, OnCardClick);
            Entries.Add(card);
        }
    }

    private void OnCardClick(int avatarId, string username)
    {
        AvatarPreview.gameObject.SetActive(true);

        AvatarID = avatarId;
        UsernameText.text = username;
        var avatar = Databases.CharactersDatabase.Characters[avatarId];
        AvatarPreview.runtimeAnimatorController = avatar.AnimatorController;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
