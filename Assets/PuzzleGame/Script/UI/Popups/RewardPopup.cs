using DG.Tweening;
using ITCGameStudio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : UIPanel
{
    public Button rewardNormalBtn;
    public Button rewardAdsBtn;
    public TextMeshProUGUI rewardNormalTxt;
    public TextMeshProUGUI rewardAdsTxt;

    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        rewardNormalBtn.onClick.AddListener(() => 
        {
            AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_INTERACT, transform);
            GameManager.Instance.ReceiveReward(CurrencyTypeEnum.Coin, RewardTypeEnum.WIN, 1f);
            GameManager.Instance.NextLevel();
            Close();
        });
        rewardAdsBtn.onClick.AddListener(() =>
        {
            AdsManager.Instance.ShowReward(() =>
            {
                AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_INTERACT, transform);
                GameManager.Instance.ReceiveReward(CurrencyTypeEnum.Coin, RewardTypeEnum.WIN, 2f);
                GameManager.Instance.NextLevel();
                Close();
            });
            
        });
    }

    public void SetAmount(int value)
    {
        rewardNormalTxt.text = value.ToString();   
        rewardAdsTxt.text = (value * 2f).ToString();
    }

    public override void Open()
    {
        base.Open();
        UIManager.Instance.OpenShower(true);
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, true);
        UIManager.Instance.SetDisplayCam(true);
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
    }

    public override void Close()
    {
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        UIManager.Instance.OpenShower(false);
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, false);
        UIManager.Instance.SetDisplayCam(false);

        canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f).OnComplete(() =>
        {
            base.Close();
        });
    }
}
