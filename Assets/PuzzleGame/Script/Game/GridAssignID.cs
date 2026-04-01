using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAssignID : MonoBehaviour, IAssignID
{
    public List<EnumID> ID;
    public bool Taken = false;
    public List<EnumID> GetIDs()
    {
        return ID;
    }

    public void SetIDs(List<EnumID> ids)
    {
        ID = new List<EnumID>(ids);
    }
}
