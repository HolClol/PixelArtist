using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using System.Reflection;
#endif


public class ResetLevel : MonoBehaviour
{
    public void ResetScene()
    {
        GameManager.Instance.MapLoad();
        ClearLog();
    }

    public void ClearLog()
    {
#if UNITY_EDITOR
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var clear = type.GetMethod("Clear");
        clear.Invoke(null, new object[] { });
#endif
    }
}
