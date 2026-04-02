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
        float enddist = GameManager.Instance.currentMap.CameraDistance;
        float startdist = enddist*2f;
        if (value) // Open camera
        {
            DisplayCam.SetActive(value);
            camComponent.orthographicSize = startdist;
            camComponent.DOOrthoSize(enddist, 0.2f);
        }
        else // Close camera (Reverse the order)
        {
            camComponent.orthographicSize = enddist;
            camComponent.DOFieldOfView(startdist, 0.2f)
                .OnComplete(() => 
                {
                    DisplayCam.SetActive(false);
                });
        }
    }
}
