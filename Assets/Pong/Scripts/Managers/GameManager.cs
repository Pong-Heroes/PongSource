using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public static partial class Events
{
    public static Action<bool> OnGameStart = null;
    public static void DoFireGameStart(bool value) => OnGameStart?.Invoke(value);
}

public class GameManager : MonoBehaviour
{
    public string debugState;
    public bool isGameActive;

    void Awake()
    {
        isGameActive = false;
    }

    private _StatesBase currentState;
    public _StatesBase State
    {
        get { return currentState; }
    }
    public void SetState(System.Type newStateType)
    {
        if (currentState != null)
        {
            currentState.OnDeactivate();
        }

        currentState = GetComponentInChildren(newStateType) as _StatesBase;
        if (currentState != null)
        {
            currentState.OnActivate();
        }
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    void Start()
    {
        SetState(typeof(MenuState));
    }
}