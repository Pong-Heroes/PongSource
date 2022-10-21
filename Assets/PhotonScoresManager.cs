using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;


public class PhotonScoresManager : MonoBehaviourPunCallbacks
{
    private static PhotonScoresManager instance;
    [SerializeField] private int playerScoresA, playerScoresB;
    [SerializeField] private Text TextPlayerA, TextPlayerB;

    private void Awake()
    {
        instance = this;
    }

    public static int GetPlayerScoresA => instance.playerScoresA;
    public static int GetPlayerScoresB => instance.playerScoresB;

    public static void UpdatePlayerScoresA(int increment)
    {
        instance.playerScoresA += increment;
        instance.TextPlayerA.text = instance.playerScoresA.ToString();
    }

    public static void UpdatePlayerScoresB(int increment)
    {
        instance.playerScoresB += increment;
        instance.TextPlayerB.text = instance.playerScoresB.ToString();
    }
}
