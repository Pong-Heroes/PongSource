//  /*********************************************************************************
//   *********************************************************************************
//   *********************************************************************************
//   * Produced by Skard Games										                  *
//   * Facebook: https://goo.gl/5YSrKw											      *
//   * Contact me: https://goo.gl/y5awt4								              *											
//   * Developed by Cavit Baturalp Gürdin: https://tr.linkedin.com/in/baturalpgurdin *
//   *********************************************************************************
//   *********************************************************************************
//   *********************************************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Menus
{
    MAIN,
    INGAME,
    GAMEOVER,
    DIFFICULTY,
    LEVELS,
    LOGIN,
    REGISTER,
    COMMUNITY,
}

public class UIManager : MonoBehaviour
{

    public MainMenu mainMenu;
    public InGameUI inGameUI;
    public DifficultyMenu DifficultyMenu;
    public LevelsMenu LevelsMenu;
    public LoginMenu LoginMenu;
    public RegisterMenu RegisterMenu;
    public CommunityMenu CommunityMenu;
    public GameObject LevelObstacles;

    public GameObject[] ObjectToDisable;
    public Image[] ObstacleImages;

    public void ActivateUI(Menus menutype)
    {
        inGameUI.gameObject.SetActive(menutype.Equals(Menus.INGAME));
        mainMenu.gameObject.SetActive(menutype.Equals(Menus.MAIN));
        DifficultyMenu.gameObject.SetActive(menutype.Equals(Menus.DIFFICULTY));
        LevelsMenu.gameObject.SetActive(menutype.Equals(Menus.LEVELS));
        LoginMenu.gameObject.SetActive(menutype.Equals(Menus.LOGIN));
        RegisterMenu.gameObject.SetActive(menutype.Equals(Menus.REGISTER));
        CommunityMenu.gameObject.SetActive(menutype.Equals(Menus.COMMUNITY));
    }

    public void DisableObjects()
    {
        for (int i = 0; i < ObjectToDisable.Length; i++)
        {
            ObjectToDisable[i].SetActive(false);
        }
    }

    public void DisableObsImages()
    {
        for (int i = 0; i < ObstacleImages.Length; i++)
        {
            ObstacleImages[i].enabled = false;
        }
    }
}
