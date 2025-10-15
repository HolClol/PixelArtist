using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class HoleVisual : MonoBehaviour
{
    public List<EnumID> ListID = new List<EnumID>();

    public HoleController controller;
    public MeshRenderer hole_meshrender;

    public Material mat_black;
    public Material mat_blue;
    public Material mat_brown;
    public Material mat_cyan;
    public Material mat_darkbrown;
    public Material mat_green;
    public Material mat_orange;
    public Material mat_pink;
    public Material mat_red;
    public Material mat_white;
    public Material mat_yellow;

    [Button]
    public void Validate()
    {
        controller.ListID = ListID;
        if (ListID.Count == 1)
        {
            Material[] newListMat = new Material[2];
            for (int i = 0; i < 2; i++)
            {
                switch (ListID[0])
                {
                    case EnumID.BLACK:
                        newListMat[i] = mat_black;
                        break;
                    case EnumID.BLUE:
                        newListMat[i] = mat_blue;
                        break;
                    case EnumID.BROWN:
                        newListMat[i] = mat_brown;
                        break;
                    case EnumID.CYAN:
                        newListMat[i] = mat_cyan;
                        break;
                    case EnumID.DARKBROWN:
                        newListMat[i] = mat_darkbrown;
                        break;
                    case EnumID.GREEN:
                        newListMat[i] = mat_green;
                        break;
                    case EnumID.ORANGE:
                        newListMat[i] = mat_orange;
                        break;
                    case EnumID.PINK:
                        newListMat[i] = mat_pink;
                        break;
                    case EnumID.RED:
                        newListMat[i] = mat_red;
                        break;
                    case EnumID.WHITE:
                        newListMat[i] = mat_white;
                        break;
                    case EnumID.YELLOW:
                        newListMat[i] = mat_yellow;
                        break;
                }
            }
            hole_meshrender.materials = newListMat;
        }
        if (ListID.Count == 2)
        {
            Material[] newListMat = new Material[ListID.Count];
            for (int i = 0; i < ListID.Count; i++)
            {
                switch (ListID[i])
                {
                    case EnumID.BLACK:
                        newListMat[i] = mat_black;
                        break;
                    case EnumID.BLUE:
                        newListMat[i] = mat_blue;
                        break;
                    case EnumID.BROWN:
                        newListMat[i] = mat_brown;
                        break;
                    case EnumID.CYAN:
                        newListMat[i] = mat_cyan;
                        break;
                    case EnumID.DARKBROWN:
                        newListMat[i] = mat_darkbrown;
                        break;
                    case EnumID.GREEN:
                        newListMat[i] = mat_green;
                        break;
                    case EnumID.ORANGE:
                        newListMat[i] = mat_orange;
                        break;
                    case EnumID.PINK:
                        newListMat[i] = mat_pink;
                        break;
                    case EnumID.RED:
                        newListMat[i] = mat_red;
                        break;
                    case EnumID.WHITE:
                        newListMat[i] = mat_white;
                        break;
                    case EnumID.YELLOW:
                        newListMat[i] = mat_yellow;
                        break;
                }
            }
            hole_meshrender.materials = newListMat;
        }
    }
}
