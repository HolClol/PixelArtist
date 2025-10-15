using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayPanel : UIPanel
{
    public TextMeshProUGUI timerTxt;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI coinTxt;
    public Button pauseBtn;
    public Button restartBtn;

    private int currentCoins;

    private CanvasGroup canvasGroup;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        pauseBtn.onClick.AddListener(() => 
        {
            UIManager.Instance.OpenSetting();
        });
        restartBtn.onClick.AddListener(() => 
        {
            if (GameManager.Instance.gameState == GameStateEnum.PLAYING)
                UIManager.Instance.OpenRetry();
            else
                GameManager.Instance.GameRetry();
        });
    }

    public void UpdateTimer(float timer)
    {
        timerTxt.text = FunctionManager.Instance.FormatSecondToStringTime(timer);
    }
    public void UpdateLevel(int level)
    {
        levelTxt.text = "LEVEL " + level;
    }

    public void UpdateCoin(int value)
    {
        float tempvalue = currentCoins;
        DOTween.To(() => tempvalue, x => tempvalue = x, value, 0.1f).SetEase(Ease.OutBack).OnUpdate(() =>
        {
            coinTxt.text = FunctionManager.Instance.FormatCurrency(tempvalue);
        }).OnComplete(() =>
        {
            currentCoins = value;
        });
    }

    public override void Open()
    {
        base.Open();
        currentCoins = CurrencyManager.Instance.GetCurrency(CurrencyTypeEnum.Coin);
    }
}
