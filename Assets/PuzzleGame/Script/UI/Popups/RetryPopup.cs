using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RetryPopup : UIPanel
{
    public Button retryBtn;
    public Button homeBtn;
    public Button closeBtn;
    public TextMeshProUGUI levelTxt;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        retryBtn.onClick.AddListener(() =>
        {
            // Send to game manager to check life count
            if (GameManager.Instance.lives > 0)
            {
                GameManager.Instance.GameRetry();
            }
            else
            {
                UIManager.Instance.OpenRefill(true);
            }
            Close();
        });
        closeBtn.onClick.AddListener(() =>
        {
            Close();
        });
    }

    public void UpdateLevel(int level)
    {
        levelTxt.text = "LEVEL " + level;
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
