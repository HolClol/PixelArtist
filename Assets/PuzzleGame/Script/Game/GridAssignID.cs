using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAssignID : MonoBehaviour, IAssignID
{
    public List<int> ID;
    public bool Taken = false;
    public List<int> GetIDs()
    {
        return ID;
    }

    public void SetIDs(List<int> ids)
    {
        ID = new List<int>(ids);
    }
}
