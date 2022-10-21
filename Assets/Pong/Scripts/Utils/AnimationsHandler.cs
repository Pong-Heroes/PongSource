using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
public class AnimationsHandler : MonoBehaviour
{
    #region DataMembers

    #region private
    private List<Button> mainMenuButtons = new List<Button>(); //this list will contain all the buttons on the main menu screen
    private List<Button> mainMenuGameModeButtons = new List<Button>(); //this list will contain only the game mode buttons on the main menu screen
    private List<Vector3> defaultMainMenuButtonsPos = new List<Vector3>(); //this list will save the default position of buttons on the main menu screen
    private float mainMenuPulsateScale = 0.10f; //this variable the value to which the game mode buttons will scale up
    private float pulsateAnimationTime = 5f; // this variable will store the time for the pulsate animation
    private int mainMenuButtonsPositionX = -750; //this variable will store that value of Y where the buttons will be positioned before the animation starts
    private float mainMenuAnimationTime = 0.25f; //this variable will store the time of animation for each button on main menu screen
    private float scaleAnimationValue = 1.10f; //this variable will store the scale to which the buttons will be scaled when the mouse is hovered over them
    private float popupAnimationTime = 0.20f; //this variable will store the time for which each popup animation cycle will run
    private float popupAnimationCycles = 2; //the number of cycle that the popup animation will complete
    private bool onGameModeButton = false; // this varaible will store if the user has any hovered on any game mode button
    private Button utilityButton = null; //this variable will be used to store the reference of button component of the entity that is getting hovered on
    #endregion

    #region public
    public static AnimationsHandler Instance;
    [HideInInspector]
    public bool mainMenuAnimating = false; //this variable will be used to tell if the main menu buttons entrance animation is running

    #endregion

    #endregion

    #region StartFunctionality

    void Start()
    {
        Instance = this;
    }
    #endregion

    #region MainMenuAnimations
    //this function will be called when user performs mouse over event on any of the game mode buttons
    //@param {button},
    //@return {}, no return
    public void onGameModePointerEnter(GameObject g)
    {
        onGameModeButton = true;
        stopGameModeButtonsAnimations();
        scaleUpButton(g);
    }
    //this function will be called when userperforms mouse enter event on any of the game mode buttons
    //@param {button}
    //@return {} no return
    public void onGameModePointerExit(GameObject g)
    {
        StartCoroutine(gameModePointerExit(g));
    }

    IEnumerator gameModePointerExit(GameObject g)
    {
        onGameModeButton = false;
        scaleDownButton(g);
        yield return new WaitForSeconds(0.25f);
    }
    //this function will be used to stop animation on gameModeButtons
    //@param {} no param
    //@return {} no return
    private void stopGameModeButtonsAnimations()
    {
        foreach (var button in mainMenuGameModeButtons)
        {
            iTween.Stop(button.gameObject);
            button.gameObject.transform.localScale = new Vector3(1, 1, 1);
        }
            
    }
    //this function will be called when user performs mouse over event on any of the buttons
    //@param {gameObject}, gameObject that was hovered
    //@return {}, no return
    public void scaleUpButton(GameObject g)
    {
        utilityButton = g.GetComponent<Button>();
        if(utilityButton != null && !utilityButton.interactable)
        {
                return;
        }
        iTween.ScaleTo(g, iTween.Hash(
            "x", scaleAnimationValue,
            "y", scaleAnimationValue,
            "time", 0.25f
        ));

    }
    //this function will be called when user performs mouse exit event on any of the buttons
    //@param {gameObject}, gameObject that was hovered
    //@return {}, no return
    public void scaleDownButton(GameObject g)
    {
        iTween.ScaleTo(g, iTween.Hash(
            "x", 1f,
            "y", 1f,
            "time", 0.25f
        ));
    }

    //this function will be used to animate gameobject in y axis (hovering in Y Axis)
    //@param {} no param
    //@return {} no return
    public void HoverYAxis(GameObject g,float yAxis,float speed, iTween.EaseType Type, iTween.LoopType loopType)
    {
        iTween.MoveBy(g, iTween.Hash(
            "y", yAxis,
            "time", speed,
            "easetype", Type,
            "looptype", loopType
            ));
    }

    //this function will be used to start the animations of main menu buttons
    //@param {} no param
    //@return {} no return
    private void startMainMenuButtonAnimation(List<Button> buttons, List<Vector3> positions, int _index)
    {
        if (_index >= buttons.Count)
            return;

        if (buttons[_index] != null)
        {
            iTween.MoveTo(buttons[_index].gameObject, iTween.Hash(
                "x", positions[_index].x,
                "time", mainMenuAnimationTime,
                "easetype", iTween.EaseType.easeInOutQuad,
                "oncomplete", "mainMenuAnimationHandler",
                "oncompleteparams", iTween.Hash("value", _index + 1),
                "oncompletetarget", gameObject
                ));
        }
    }

    //this function will be used to handle the main menu animations
    //@param {_index}
    //@return {} no return
    public void mainMenuAnimationHandler(object _index)
    {
        Hashtable hstbl = (Hashtable)_index;

        if ((int)hstbl["value"] >= mainMenuButtons.Count)
        {
            mainMenuAnimating = false;
            //animateGameModeButtons();
            return;
        }
        startMainMenuButtonAnimation(mainMenuButtons, defaultMainMenuButtonsPos, (int)hstbl["value"]);
    }


    //this function will start gitter animation on a game mode button
    //@param {button}, button on which to start the gitter animation
    //@return {} no return
    private void startGameModeAnimation(Button _button)
    {
        iTween.PunchScale(_button.gameObject, iTween.Hash(
            "amount", new Vector3(mainMenuPulsateScale, mainMenuPulsateScale, 0f),
            "time", pulsateAnimationTime,
            "looptype", iTween.LoopType.loop
            ));
    }


    //this function will return a list of position vector of list of gameobjects supplied
    //@param {List<GameObject>}
    //@return {List<Vector3>}
    private List<Vector3> getPosition(List<Button> buttonsList)
    {
        //if buttons list is empty return null
        if (buttonsList.Count <= 0)
            return null;

        List<Vector3> positionsList = new List<Vector3>();
        //iterate each button in the main menu buttons list
        foreach (var button in buttonsList)
        {
            if(button!=null)
                positionsList.Add(button.GetComponent<Transform>().position);
        }
        //return list of positions of all the main menu buttons
        return positionsList;
    }

    //this function will set the y-position of a list of gameobject to the provided value
    //@param {list<GameObject>,y-axis}
    //@return {} no return
    private void setPositionX(List<Button> buttons, List<Vector3> defaultPositions, float _xPosition)
    {
        if (buttons.Count <= 0)
            return;

        for (int i = 0; i < buttons.Count; i++)
        {
            if(buttons[i]!=null)
                buttons[i].transform.position = new Vector3(_xPosition, defaultPositions[i].y, defaultPositions[i].z);
        }

    }
    #endregion

    #region Popup Animations
    //this function will be used to animate popups in this game
    //@param {g}, gameobject to animate
    //@return {} no return
    public void runPopupAnimation(GameObject g)
    {
        animateRotation(g,0,-15f);
    }

    //this function will be used to rotate right the passed gameobject to the desired rotation
    //@param {gameobject, count}
    //@return {} no return
    private void animateRotation(GameObject g, int _count, float rotationAngle)
    {
        if(_count <= popupAnimationCycles)
        {
            iTween.RotateTo(g, iTween.Hash(
                "z", (_count < popupAnimationCycles) ? rotationAngle : 0,
                "time", popupAnimationTime,
                "easetype", iTween.EaseType.easeOutQuint,
                "oncomplete", "rotationAnimationHelper",
                "oncompleteparams", iTween.Hash("gameobject", g, "count", _count, "angle", rotationAngle),
                "oncompletetarget", gameObject
            ));
        }
    }

    //this function will be used help in the rotation animation
    //@param {object}
    //@return {} no return
    private void rotationAnimationHelper(object _data)
    {
        Hashtable data = (Hashtable)_data;
        animateRotation((GameObject)data["gameobject"], (int)data["count"] + 1, -(float)data["angle"]);
    }
    #endregion
}
