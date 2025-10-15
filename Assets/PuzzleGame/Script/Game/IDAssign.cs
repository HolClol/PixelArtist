
using System.Collections.Generic;
using UnityEngine;
public enum EnumID
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
public class IDAssign : MonoBehaviour
{
    
}

public interface IAssignID
{
    public List<int> GetIDs();
    public void SetIDs(List<int> ids);
}
