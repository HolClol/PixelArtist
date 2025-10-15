using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FunctionManager : MonoBehaviour
{
    public static FunctionManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetRootParent(GameObject obj, int count)
    {
        Transform t = obj.transform;
        for (int i = 0; i < count; i++)
            t = t.parent;
        return t.gameObject;
    }

    public void DelayFunction(float delay, Action action)
    {
        StartCoroutine(CallDelayFunction(delay, action));
    }

    private IEnumerator CallDelayFunction(float timer, Action action)
    {
        yield return new WaitForSeconds(timer);
        action();
    }

    public string FormatCurrency(float amount)
    {
        if (amount >= 1000000)
        {
            return (amount / 1000000f).ToString("0.0") + "M";
        }
        else if (amount >= 10000)
        {
            return (amount / 1000f).ToString("0.0") + "K";
        }
        else
        {
            return amount.ToString("0");
        }
    }
    public string FormatCurrency(float amount, int type)
    {
        if (amount >= 1000000)
        {
            return (amount / 1000000f).ToString("1") + "M";
        }
        else if (amount >= 10000)
        {
            return (amount / 1000f).ToString("1") + "K";
        }
        else
        {
            return amount.ToString("0");
        }
    }
    public string FormatSecondToStringTime(float seconds)
    {
        string value;
        TimeSpan time;
        if (seconds >= 3600f)
        {
            time = TimeSpan.FromSeconds(seconds);
            value = time.ToString(@"h\:mm\:ss");
        }
        else
        {
            time = TimeSpan.FromSeconds(seconds);
            value = time.ToString(@"mm\:ss");
        }
        return value;
    }
}
