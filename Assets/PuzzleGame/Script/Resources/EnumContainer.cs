using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumContainer { }

public enum GameStateEnum
{
    NONE,
    WIN,
    LOSE,
    TIMEOUT,
    PAUSE,
    PLAYING
}

public enum RewardTypeEnum
{
    WIN,
    DAILY,
    LUCKYWHEEL,
}

public enum EnumID : int
{
    NONE = 0,
    BLOCK = 1,
    BLUE = 2,
    RED = 3,
    GREEN = 4,
    YELLOW = 5,
    CYAN = 6,
    PINK = 7,
    ORANGE = 8,
    WHITE = 9,
    BLACK = 10,
    BROWN = 11,
    DARKBROWN = 12,
}

public enum AudioTypeEnum
{
    SFX_BG,
    SFX_CUBE_POP,
    SFX_INTERACT,
    SFX_WIN,
    SFX_TICK_TOCK_WARNING,
    SFX_HOLE_COMPLETE,
}

