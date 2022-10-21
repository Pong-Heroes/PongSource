using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class MapSelection : MonoBehaviour
{
    public Sprite[] mapSprites;
    public Image mapImageContainer;
    public Button previousBtn;
    public Button nextBtn;
    public GameObject[] levelBgs;
    [SerializeField]
    LauncherSceneUIManager launcherSceneUIManager;
    int index = 0;

    private void Start()
    {
        SetScreen();
    }

    void SetScreen()
    {

        if (index == mapSprites.Length - 1)
            nextBtn.interactable = false;
        else
            nextBtn.interactable = true;

        if (index == 0)
            previousBtn.interactable = false;
        else
            previousBtn.interactable = true;

        mapImageContainer.sprite = mapSprites[index];

    }


    public void OnClickContinue()
    {
        launcherSceneUIManager.mapPanel.SetActive(false);
        launcherSceneUIManager.shopPanel.SetActive(true);
        foreach (GameObject bg in levelBgs)
        {
            bg.SetActive(false);
        }
        levelBgs[index].SetActive(true);

        Constants.LevelIndex = (byte)index;
    }

    public void NextButton()
    {
        if (index < mapSprites.Length)
        {
            index += 1;
        }

        SetScreen();

    }

    public void PreviousButton()
    {
        if (index > 0)
        {
            index -= 1;
        }



        SetScreen();

    }
}
