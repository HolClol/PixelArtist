using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceContainer { }

public interface IAssignID
{
    public List<EnumID> GetIDs();
    public void SetIDs(List<EnumID> ids);
}



