using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingPopup : UIPanel
{
    [Serializable] public class SettingToggle
    {
        public Button toggleBtn;
        public RectTransform toggleRect;
        public Image toggleImg;
    }

    public Sprite toggleSpriteOn;
    public Sprite toggleSpriteOff;
    public Button resumeBtn;
    public Button closeBtn;
    public List<SettingToggle> settingClasses;

    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        resumeBtn.onClick.AddListener(() => 
        {
            Close();
        });
        closeBtn.onClick.AddListener(() =>
        {
            // Something home
        });
        // Music
        settingClasses[0].toggleBtn.onClick.AddListener(() => 
        {
            PlayAnimationToggle(settingClasses[0], 0);
        });
        // Sound
        settingClasses[1].toggleBtn.onClick.AddListener(() =>
        {
            PlayAnimationToggle(settingClasses[1], 1);
        });
        // Vibrate
        settingClasses[2].toggleBtn.onClick.AddListener(() =>
        {
            PlayAnimationToggle(settingClasses[2], 2);
        });
    }

    private void PlayAnimationToggle(SettingToggle option, int index)
    {
        bool value = true;
        switch (index)
        {
            case 0:
                value = !SettingData.Instance.music;
                SettingData.Instance.music = value;
                break;
            case 1:
                value = !SettingData.Instance.sound;
                SettingData.Instance.sound = value;
                break;
            case 2:
                value = !SettingData.Instance.vibration;
                SettingData.Instance.vibration = value;
                break;
        }

        if (value)
        {
            option.toggleImg.sprite = toggleSpriteOn;
            option.toggleRect.DOLocalMoveX(55, 0.15f).SetEase(Ease.OutQuad);
        }   
        else
        {
            option.toggleImg.sprite = toggleSpriteOff;
            option.toggleRect.DOLocalMoveX(-55, 0.15f).SetEase(Ease.OutQuad);
        }
        SettingData.Instance.Save();
    }

    public void StartUp()
    {
        for (int i = 0; i < settingClasses.Count; i++)
        {
            var pos = settingClasses[i].toggleRect.localPosition;
            var value = true;
            switch (i)
            {
                case 0:
                    value = SettingData.Instance.music;
                    break;
                case 1:
                    value = SettingData.Instance.sound;
                    break;
                case 2:
                    value = SettingData.Instance.vibration;
                    break;
            }
            if (value)
            {
                settingClasses[i].toggleImg.sprite = toggleSpriteOn;
                settingClasses[i].toggleRect.localPosition = new Vector3(55, pos.y, pos.z);
            }
            else
            {
                settingClasses[i].toggleImg.sprite = toggleSpriteOff;
                settingClasses[i].toggleRect.localPosition = new Vector3(-55, pos.y, pos.z);
            }
                
        }
        ;
    }

    public override void Open()
    {
        base.Open();
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, true);
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        GameManager.Instance.gameState = GameStateEnum.PAUSE;

        canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
    }

    public override void Close()
    {
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        GameManager.Instance.gameState = GameManager.Instance.PreviousGameState;

        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, false);
        canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f).OnComplete(() =>
        {
            base.Close();
        });
    }
}
