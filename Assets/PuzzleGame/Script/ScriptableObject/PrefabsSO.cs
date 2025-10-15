using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prefabs List", menuName = "Scriptable Object/Prefabs List", order = 0)]
public class PrefabsSO : ScriptableObject
{
    public List<GameObject> Prefabs;
}
