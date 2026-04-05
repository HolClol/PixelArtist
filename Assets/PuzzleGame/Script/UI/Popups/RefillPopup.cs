using DG.Tweening;
using ITCGameStudio;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RefillPopup : UIPanel
{
    public Button coinRefillBtn;
    public Button adRefillBtn;
    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI lifeCountTxt;
    public TextMeshProUGUI costTxt;

    [HideInInspector] public bool isThruRetry = false;

    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        coinRefillBtn.onClick.AddListener(() =>
        {
            if (CurrencyManager.Instance.GetCurrency(CurrencyTypeEnum.Coin) < GameManager.Instance.lifeRefillCost)
            {
                // Not enough money
                return;
            }
            GameManager.Instance.lives = 5;
            AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_INTERACT, transform);
            CurrencyManager.Instance.SubCurrency(CurrencyTypeEnum.Coin, GameManager.Instance.lifeRefillCost);
            Close();
        });
        adRefillBtn.onClick.AddListener(() =>
        {
            AdsManager.Instance.ShowReward(() => 
            {
                GameManager.Instance.lives++;
                if (isThruRetry)
                {
                    AudioManager.Instance.PlayAudio(AudioTypeEnum.SFX_INTERACT, transform);
                    UIManager.Instance.OpenRetry();
                }
                Close();
            });
            
        });
    }

    public void UpdateTimer(float timer)
    {
        if (GameManager.Instance.lives >= GameManager.Instance.maxLives)
        {
            timerTxt.text = "FULL";
        }
        else
        {
            var text = FunctionManager.Instance.FormatSecondToStringTime(timer);
            timerTxt.text = text;
        }
           
    }

    public void UpdateLife()
    {
        lifeCountTxt.text = GameManager.Instance.lives.ToString();
    }


    public override void Open()
    {
        base.Open();
        costTxt.text = GameManager.Instance.lifeRefillCost.ToString();
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, true);
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
