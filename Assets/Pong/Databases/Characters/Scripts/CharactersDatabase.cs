using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharactersDatabase", menuName = "Pong/Databases/Characters Database")]
public class CharactersDatabase : ScriptableObject
{
    public enum State
    {
        Default, Locked, Unlocked, Selected
    }

    [System.Serializable]
    public class Character
    {
        [SerializeField] private string Name;
        [SerializeField] private RuntimeAnimatorController CharacterAnimator;

        [Space(10)]
        [SerializeField] private Sprite Icon;
        [SerializeField] private Sprite Silhouette;

        [Space(10)]
        [SerializeField] private State UnlockStatus;
        [SerializeField] private int Price;
        [SerializeField] private Vector2 ImageSize;

        public State GetState => UnlockStatus;
        public RuntimeAnimatorController AnimatorController => CharacterAnimator;

        public int GetPrice => Price;
        public Vector2 GetImageSize => ImageSize;
        public Sprite GetIcon => Icon;
        public Sprite GetSilhouette => Silhouette;

        public string StatusKey => $"Character.Status ? {Key}";
        public string SelectedKey => $"Character.Selected ? {Key}";

        public string Key { get; set; }
        public bool IsDefault => PlayerPrefs.GetInt(StatusKey, (int)GetState) == (int)State.Default;
        public bool IsSelected => PlayerPrefs.GetInt(StatusKey, (int)GetState) == (int)State.Selected || IsDefault;
        public bool IsUnlocked => IsDefault || IsSelected || PlayerPrefs.GetInt(StatusKey, (int)GetState) == (int)State.Unlocked;

        private State SetState(State state)
        {
            PlayerPrefs.SetInt(StatusKey, (int)state);
            PlayerPrefs.Save();

            return UnlockStatus;
        }

        public void Select() => SetState(State.Selected);
        public void Unlock() => SetState(State.Unlocked);
    }

    public List<Character> Characters;

    const string BasePrefix = "Character_";

    public Character GetSelectedCharacter => Characters.Find(x => x.IsSelected);
    public int GetSelectedCharacterIndex => Characters.FindIndex(x => x.IsSelected);
    public Character GetCharacterOfIndex(int index) => Characters[index];

    public void Init()
    {
        for (var i = 0; i < Characters.Count; i++)
        {
            var character = Characters[i];
            character.Key = BasePrefix + i;
        }

        Debug.Log(" ---> CharactersDatabase.Init()");
    }

    public void Select(string key)
    {
        try
        {
            var character = Characters.Find(x => x.Key == key);
            if (character != null)
            {
                GetSelectedCharacter.Unlock();
                character.Select();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void Unlock(string key)
    {
        var character = Characters.Find(x => x.Key == key);
        if (character != null)
        {
            character.Unlock();
        }
    }
}
