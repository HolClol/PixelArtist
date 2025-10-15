using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UIController Controller;
    public GameObject DisplayCam;

    private Camera camComponent;

    private void Awake()
    {
        Instance = this;
        camComponent = DisplayCam.GetComponent<Camera>();
    }

    public void INIT()
    {
        Controller.INIT();
        UpdateUI(CurrencyTypeEnum.Coin);
        UpdateUI(CurrencyTypeEnum.Life);
    }

    // Preferbally this should be in UI Controller instead of here
    public void UpdateGameTimer(float timer)
    {
        Controller.GameplayPanel.UpdateTimer(timer);
    }

    public void UpdateLifeTimer(float timer)
    {
        Controller.PopupPanel.RefillPopup.UpdateTimer(timer);
    }

    public void UpdateLevel(int level)
    {
        Controller.GameplayPanel.UpdateLevel(level + 1);
        Controller.PopupPanel.RetryPopup.UpdateLevel(level + 1);
    }

    public void UpdateUI(CurrencyTypeEnum type)
    {
        switch (type)
        {
            case CurrencyTypeEnum.Coin:
                var value = CurrencyManager.Instance.GetCurrency(type);
                Controller.GameplayPanel.UpdateCoin(value);
                break;
            case CurrencyTypeEnum.Life:
                Controller.PopupPanel.RefillPopup.UpdateLife();
                break;
        }
    }

    public void OpenWin()
    {
        Controller.PopupPanel.Win();
    }

    public void OpenLose()
    {
        Controller.PopupPanel.Timeout();
    }

    public void OpenReward()
    {
        Controller.PopupPanel.Reward();
    }

    public void OpenSetting()
    {
        Controller.PopupPanel.Setting();
    }

    public void OpenRetry()
    {
        Controller.PopupPanel.Retry();
    }

    public void OpenRefill(bool command)
    {
        Controller.PopupPanel.Refill(command);
    }

    public void OpenConfetti()
    {
        Controller.FireConfetti();
    }

    public void OpenShower(bool value)
    {
        Controller.SetShower(value);
    }

    public void SetDisplayCam(bool value)
    {  
        
        int camdist = (int)(GameManager.Instance.currentMap.CameraDistance * 2f);
        int targetdist = camdist;
        if (value)
        {
            DisplayCam.SetActive(value);
            targetdist = camdist * 4;
            camComponent.fieldOfView = targetdist;
            camComponent.DOFieldOfView(camdist * 0.85f, 0.25f);
        }
        else
        {
            camComponent.fieldOfView = camdist;
            camComponent.DOFieldOfView(targetdist * 4, 0.25f)
                .OnComplete(() => 
                {
                    DisplayCam.SetActive(false);
                });
        }
    }
}
