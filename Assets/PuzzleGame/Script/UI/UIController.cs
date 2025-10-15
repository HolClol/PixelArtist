using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public UIGameplayPanel GameplayPanel;
    public UIPopupPanel PopupPanel;
    public List<GameObject> Confettis;
    public GameObject ConfettiShower;

    private List<UIPanel> Panels = new List<UIPanel>();
    private void Awake()
    {
        Panels.Add(GameplayPanel);
        Panels.Add(PopupPanel);
    }

    public void INIT()
    {
        PopupPanel.SettingPopup.StartUp();
    }

    public void FireConfetti()
    {
        for (int i = 0; i < Confettis.Count; i++)
        {
            var delay = i * 0.1f;
            var index = i;
            FunctionManager.Instance.DelayFunction(delay, () =>
            {
                Confettis[index].SetActive(true);
            });
        }
    }

    public void SetShower(bool value)
    {
        ConfettiShower.SetActive(value);
    }
}