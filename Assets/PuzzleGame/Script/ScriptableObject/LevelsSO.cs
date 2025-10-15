using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Levels List", menuName = "Scriptable Object/Levels List", order = 0)]
public class LevelsSO : ScriptableObject
{
    public List<GameObject> Levels;
}
