using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopupPanel : UIPanel
{
    public CanvasGroup bgCanvas;
    public WinPopup WinPopup;
    public LosePopup LosePopup;
    public RewardPopup RewardPopup;
    public SettingPopup SettingPopup;
    public RetryPopup RetryPopup;
    public RefillPopup RefillPopup;

    private List<UIPanel> PopupPanels = new List<UIPanel>();
    private Queue<UIPanel> Queue = new Queue<UIPanel>();

    private void Awake()
    {
        PopupPanels.Add(WinPopup);
        PopupPanels.Add(LosePopup);
        PopupPanels.Add(RewardPopup);
        PopupPanels.Add(SettingPopup);
        PopupPanels.Add(RetryPopup);
        PopupPanels.Add(RefillPopup);
    }
    private void Start()
    {
        /*foreach (var panel in PopupPanels)
        {
            panel.gameObject.SetActive(false);
        }*/
    }

    public void ModifyQueue(UIPanel popup, bool command)
    {
        if (command)
        {
            Queue.Enqueue(popup);
            if (Queue.Count == 1) 
            {
                ToggleBG(command);
            }
        }
        else 
        { 
            Queue.Dequeue();
            if (Queue.Count == 0)
            {
                ToggleBG(command);
            }
        }
    }

    private void ToggleBG(bool command)
    {
        if (command)
        {
            bgCanvas.alpha = 0f;
            bgCanvas.DOFade(0.95f, 0.25f);
            bgCanvas.blocksRaycasts = true;
        }
        else
        {
            bgCanvas.DOFade(0f, 0.25f);
            bgCanvas.blocksRaycasts = false;
        }
    }

    public void Win()
    {
        WinPopup.Open();
    }

    public void Timeout()
    {
        LosePopup.Open();
    }

    public void Lose()
    {
    }

    public void Reward()
    {
        RewardPopup.SetAmount((int)GameManager.Instance.currentMap.RewardCoin);
        RewardPopup.Open();
    }

    public void Setting()
    {
        SettingPopup.Open();
    }

    public void Retry()
    {
        RetryPopup.Open();
    }

    public void Refill(bool command)
    {
        RefillPopup.isThruRetry = command;
        RefillPopup.Open();
    }
}
