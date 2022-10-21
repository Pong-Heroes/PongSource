using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using Hash = ExitGames.Client.Photon;

public class ShopMenu : PersistentSingleton<ShopMenu>
{
    [SerializeField] private Text StatusText;

    [SerializeField] private Button NextButton;
    [SerializeField] private Button PrevButton;
    [SerializeField] private Button SelectButton;

    [SerializeField] private Animator Preview;
    [SerializeField] private Image CharacterPrefab;
    [SerializeField] private Transform CharacterContainer;
    [SerializeField] private HorizontalScrollSnap Pagination;

    private CharactersDatabase Database => Databases.CharactersDatabase;

    private int Index;

    private bool Initialised = false;
    private CharactersDatabase.Character GetCharacter => Database.Characters[Index];

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
    }

    private void Init()
    {
        Index = Database.GetSelectedCharacterIndex;
        Pagination.OnSelectionPageChangedEvent.AddListener(OnSelectionPageChangedEvent);

        var properties = new Hash.Hashtable();
        properties[Constants.CHARACTER_KEY] = Index;
        PhotonNetwork.SetPlayerCustomProperties(properties);

        for (int i = 0; i < Database.Characters.Count; i++)
        {
            var character = Instantiate(CharacterPrefab, CharacterContainer);
            character.sprite = Database.Characters[i].GetIcon;
            character.gameObject.SetActive(true);
        }
    }

    private void OnSelectionPageChangedEvent(int value)
    {
        Index = value;
        UpdatePreview();
    }

    private void OnEnable()
    {
        if (!Initialised)
        {
            Init();
        }

        Index = Database.GetSelectedCharacterIndex;
        StartCoroutine(Refresh());
    }

    private IEnumerator Refresh()
    {
        yield return new WaitForSeconds(0.1f);

        UpdatePreview();
        Pagination.ChangePage(Index);
    }

    private IEnumerator Start()
    {
        NextButton.onClick.AddListener(OnNext);
        PrevButton.onClick.AddListener(OnPrev);
        SelectButton.onClick.AddListener(OnSelect);

        yield return new WaitForSeconds(0.1f);
        
        Initialised = true;
        Index = Database.GetSelectedCharacterIndex;
        StartCoroutine(Refresh());
    }

    private void OnSelect()
    {
        var character = Database.Characters[Index];
        Database.Select(character.Key);

        var properties = new Hash.Hashtable();
        properties[Constants.CHARACTER_KEY] = Index;
        PhotonNetwork.SetPlayerCustomProperties(properties);

        UpdatePreview();
        if (AudioManager.Audio)
            AudioManager.Audio.PlayClickSound();
    }

    private void OnNext()
    {
        // if (Index + 1 >= Database.Characters.Count)
        // {
        //     Index = 0;
        // }
        // else
        //     Index++;

        // UpdatePreview();
        if (AudioManager.Audio)
            AudioManager.Audio.PlayClickSound();
    }

    private void OnPrev()
    {
        // if (Index - 1 >= 0)
        // {
        //     Index--;
        // }
        // else
        //     Index = Database.Characters.Count - 1;

        // UpdatePreview();
        if (AudioManager.Audio)
            AudioManager.Audio.PlayClickSound();
    }

    private void UpdatePreview()
    {
        StatusText.text = GetCharacter.IsSelected ? "EQUIPPED" : "";
        StatusText.gameObject.SetActive(GetCharacter.IsSelected);
        SelectButton.gameObject.SetActive(!GetCharacter.IsSelected);
        Preview.runtimeAnimatorController = GetCharacter.AnimatorController;
    }
}
