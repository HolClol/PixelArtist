using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPopup : UIPanel
{
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void Open()
    {
        base.Open();
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, true);
        canvasGroup.alpha = 0f;
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        canvasGroup.DOFade(1.0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(Vector3.one, 0.25f)
            .OnComplete(() => 
            {
                UIManager.Instance.OpenConfetti();
            })
            .SetEase(Ease.OutQuad);
        FunctionManager.Instance.DelayFunction(1.5f, () => 
        {
            Close();
        });
    }

    public override void Close()
    {
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;

        UIManager.Instance.OpenReward();
        UIManager.Instance.Controller.PopupPanel.ModifyQueue(this, false);
        canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.25f).OnComplete(() => 
        {
            base.Close();
        });
    }
}
