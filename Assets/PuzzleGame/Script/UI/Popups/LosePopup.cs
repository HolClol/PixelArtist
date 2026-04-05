using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ITCGameStudio;

public class LosePopup : UIPanel
{
    public Button adresBtn;
    public Button buyresBtn;
    public Button closeBtn;
    public TextMeshProUGUI coinTxt;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        adresBtn.onClick.AddListener(() =>
        {
            AdsManager.Instance.ShowReward(() => 
            {
                AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_INTERACT, transform);
                GameManager.Instance.GameRevive(true);
                Close();
            });
            
        });
        buyresBtn.onClick.AddListener(() =>
        {
            if (CurrencyManager.Instance.GetCurrency(CurrencyTypeEnum.Coin) < GameManager.Instance.reviveCost)
            {
                // Not enough money
                return;
            }
            AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_INTERACT, transform);
            CurrencyManager.Instance.SubCurrency(CurrencyTypeEnum.Coin, GameManager.Instance.reviveCost);
            GameManager.Instance.GameRevive(false);
            Close();
        });
        closeBtn.onClick.AddListener(() =>
        {
            // Return home
        });
    }

    public override void Open()
    {
        base.Open();
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, true);
        coinTxt.text = GameManager.Instance.reviveCost.ToString();
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutQuad);
    }

    public override void Close()
    {
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;

        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, false);
        canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f).OnComplete(() =>
        {
            base.Close();
        });
    }
}
