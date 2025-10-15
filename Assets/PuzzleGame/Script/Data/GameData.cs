using System;

public class GameData : DataBase<GameData>
{
    public int currentLevel = 0;
    public float currentTimerLife = 0f;
    public bool unlimitedLives = false;
    public long dateTimeOfPause;
    protected override SaveKey SaveKey => SaveKey.GameData;
}
