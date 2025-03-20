using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DogsBreedsView : MonoBehaviour
{
    public event Action<bool> IChangedState;
    public event Action<string> OnBreedSelected; 
    [SerializeField]  ActionBT breedButtonPrefab;
    [SerializeField]  PopUp popup;
    List<ActionBT> createdButtons = new();//Здесь сделал некий objectPool чтобы постоянно не удалять и не создавать кнопки.
    ActionBT activeBT;
    RectTransform popupRect=>popup.GetComponent<RectTransform>();
    RectTransform btReq;
    
    void  Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            CreateBT();
        }
        
    }
   // обновление кнопок
    public void Refresh(BreedsArgs a)
    {
        for (int i = 0; i < a.breeds.Length; i++)
        {
            var bt =createdButtons[i];
            bt.gameObject.SetActive(true);
            bt.text.text = a.breeds[i].Item2;
            string breedId = a.breeds[i].Item1; 
            bt.button.onClick.AddListener(() => 
            {
                OnBreedSelected?.Invoke(breedId); 
                if(activeBT!=null) activeBT.ShowIndicator(false);
                activeBT=bt;
                activeBT.ShowIndicator(true);
                btReq=activeBT.GetComponent<RectTransform>();
            });
        }
    }
    //здесь реализовано открытие попапа. Поп ап позиционируется на месте кнопки. В во внутренний метод поп апа передается информация для отображения
    public void ShowPopup(BreedInfoArgs a)
    {
        if(activeBT!=null) activeBT.ShowIndicator(false);
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            popupRect.parent as RectTransform, 
            RectTransformUtility.WorldToScreenPoint(null, activeBT.transform.position), 
            null, 
            out localPos
        );
        float popupHeight = popupRect.rect.height;
        popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x, localPos.y - popupHeight * (1 - popupRect.pivot.y)+btReq.rect.height/2);
        popup.ShowPopup(a.name,a.description);
    }
    
    public void SetEnable(bool flag)
    {
        popup.HidePopup();
        gameObject.SetActive(flag);
        IChangedState?.Invoke(flag);
    }
     void CreateBT()
    {
        ActionBT newButton = Instantiate(breedButtonPrefab, transform);
        newButton.gameObject.SetActive(false);
        createdButtons.Add(newButton);
    }
}