using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurrencyTypeEnum
{
    Coin,
    Life,
}

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;
    private float liveRecoverInterval = 1800f;
    private float currentRecoverTime = 0f;
    private bool notMaxLife = false;

    private DateTime lastRecoveryTime;
    private Coroutine _coroutineLifeTimer, _coroutineUnlimitedTimer;
    private void Awake()
    {
        Instance = this;
        currentRecoverTime = liveRecoverInterval;
    }
    public void INIT()
    {
        // First initiation check
        currentRecoverTime = GameData.Instance.currentTimerLife;
        DateTime lastPlayed = DateTime.FromBinary(GameData.Instance.dateTimeOfPause);
        var timeOffline = (float)(DateTime.Now - lastPlayed).TotalSeconds;
        int maxLife = GameManager.Instance.maxLives;

        // If unlimited lives is true, decrease the timer based on the time offline
        if (GameData.Instance.unlimitedLives && GameData.Instance.currentTimerLife > 0f)
        {
            GameData.Instance.currentTimerLife -= timeOffline;
            if (GameData.Instance.currentTimerLife < 0f)
                GameData.Instance.currentTimerLife = 0f;
        }

        // If time offline is within the maximum and minimum, add lives according to it
        if (timeOffline <= liveRecoverInterval * maxLife && timeOffline > liveRecoverInterval)
        {
            int livecount = (int)(timeOffline / liveRecoverInterval);
            timeOffline -= liveRecoverInterval * livecount;
            for (int i = 0; i < livecount; i++)
            {
                AddCurrency(CurrencyTypeEnum.Life, 1f);
            }
        }
        // If time offline is higher than maximum time interval, set max live
        else if (timeOffline > liveRecoverInterval * maxLife)
        {
            SetCurrency(CurrencyTypeEnum.Life, maxLife);
            timeOffline = 0f;
        }
        CheckMaxLifeCount();
        SetUnlimitedLives(GameData.Instance.unlimitedLives, 0);
        currentRecoverTime -= timeOffline;
    }

    public int GetCurrency(CurrencyTypeEnum type)
    {
        switch (type) 
        { 
            case CurrencyTypeEnum.Coin:
                return CurrencyData.Instance.Coins;
            case CurrencyTypeEnum.Life:
                return CurrencyData.Instance.Lives;
            default:
                return 0;
        }
    }

    public void AddCurrency(CurrencyTypeEnum type, float value)
    {
        switch (type)
        {
            case CurrencyTypeEnum.Coin:
                CurrencyData.Instance.Coins += (int)value;
                break;
            case CurrencyTypeEnum.Life:
                if (CurrencyData.Instance.Lives >= 5) return;
                CurrencyData.Instance.Lives += (int)value;
                currentRecoverTime = liveRecoverInterval; // Reset timer if a life is obtained through other means
                CheckMaxLifeCount();
                break;
        }
        CurrencyData.Instance.Save();
    }

    public void SetCurrency(CurrencyTypeEnum type, float value)
    {
        switch (type)
        {
            case CurrencyTypeEnum.Coin:
                CurrencyData.Instance.Coins = (int)value;
                break;
            case CurrencyTypeEnum.Life:
                CurrencyData.Instance.Lives = (int)value;
                if (CurrencyData.Instance.Lives >= 5) CurrencyData.Instance.Lives = 5;
                CheckMaxLifeCount();
                break;
        }
        CurrencyData.Instance.Save();
    }

    public void SubCurrency(CurrencyTypeEnum type, float value)
    {
        switch (type)
        {
            case CurrencyTypeEnum.Coin:
                CurrencyData.Instance.Coins -= (int)value;
                if (CurrencyData.Instance.Coins < 0)
                    CurrencyData.Instance.Coins = 0;
                break;
            case CurrencyTypeEnum.Life:
                if (GameData.Instance.unlimitedLives) { return; }
                CurrencyData.Instance.Lives -= (int)value;
                CheckMaxLifeCount();
                break;
        }
        CurrencyData.Instance.Save();
    }

    public float GetcurrentTimerLife()
    {
        return currentRecoverTime;
    }

    public void SetUnlimitedLives(bool value, float timer)
    {
        GameData.Instance.unlimitedLives = value;
        GameData.Instance.currentTimerLife += timer;
        GameData.Instance.Save();

        if (value && _coroutineUnlimitedTimer == null)
        {
            _coroutineUnlimitedTimer = StartCoroutine(RunUnlimitedLifeCountdown());
            SetCurrency(CurrencyTypeEnum.Life, 5);
        }

    }

    private IEnumerator RunUnlimitedLifeCountdown()
    {
        while (GameData.Instance.unlimitedLives && GameData.Instance.currentTimerLife > 0)
        {
            yield return new WaitForSecondsRealtime(1f);
            GameData.Instance.currentTimerLife -= 1f;
        }
        GameData.Instance.currentTimerLife = 0f;
        SetUnlimitedLives(false, 0f);
        _coroutineUnlimitedTimer = null;
    }

    // Run live recover timer until it is maxed
    private IEnumerator RunLifeCountdown()
    {
        while (notMaxLife)
        {
            currentRecoverTime -= 1f;
            UIManager.Instance.UpdateLifeTimer(currentRecoverTime);
            if (currentRecoverTime <= 0f)
            {
                currentRecoverTime += liveRecoverInterval;
                AddCurrency(CurrencyTypeEnum.Life, 1f);
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void CheckMaxLifeCount()
    {
        // Deactivate Countdown Timer
        if (CurrencyData.Instance.Lives >= GameManager.Instance.maxLives && notMaxLife)
        {
            notMaxLife = false;
            CurrencyData.Instance.Lives = 5;
            if (_coroutineLifeTimer != null)
            {
                StopCoroutine(_coroutineLifeTimer);
                _coroutineLifeTimer = null;
            }

        }
        // Activate Countdown Timer
        else if (CurrencyData.Instance.Lives < GameManager.Instance.maxLives && !notMaxLife)
        {
            if (CurrencyData.Instance.Lives < 0)
                CurrencyData.Instance.Lives = 0;
            notMaxLife = true;
            currentRecoverTime = liveRecoverInterval;
            _coroutineLifeTimer = StartCoroutine(RunLifeCountdown());
        }
    }

    private void OnApplicationQuit()
    {
        GameData.Instance.currentTimerLife = currentRecoverTime;
        GameData.Instance.dateTimeOfPause = DateTime.Now.ToBinary();
        GameData.Instance.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {

            if (CurrencyData.Instance.Lives < GameManager.Instance.maxLives)
            {
                GameData.Instance.currentTimerLife = currentRecoverTime;
                lastRecoveryTime = DateTime.Now;
            }
            GameData.Instance.dateTimeOfPause = DateTime.Now.ToBinary();
            GameData.Instance.Save();
        }
        else
        {
            if (CurrencyData.Instance.Lives < GameManager.Instance.maxLives)
                currentRecoverTime -= (float)(DateTime.Now - lastRecoveryTime).TotalSeconds;
        }
    }
}
