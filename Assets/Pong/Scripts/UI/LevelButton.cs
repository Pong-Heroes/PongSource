using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject Highlight;
    [SerializeField] private Image PreviewImage;
    private int Index;
    private Action<int> Callback;

    public void Init(Sprite preview, int index = -1, bool isSelected = false, Action<int> callback = null,bool _isenable=true)
    {
        Index = index;
        Callback = callback;
        PreviewImage.sprite = preview;
        Highlight.SetActive(isSelected);
        this.gameObject.SetActive(_isenable);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Callback?.Invoke(Index);
    }

    public void UpdateView(int index)
    {
        Highlight.SetActive(Index == index);
    }
}