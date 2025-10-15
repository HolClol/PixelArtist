using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[Serializable]
public class IntListWrapper
{
    public List<int> ids;
}

[ExecuteAlways]
public class GridVisualizer : MonoBehaviour
{
    public Vector2Int gridSize = new Vector2Int(3, 2);
    public Vector2 cellSpacing = Vector2.zero;
    public GameObject cellPrefab;
    public EnumID currentID;

    public List<GridAssignID> enabledCells = new List<GridAssignID>();
    public List<bool> selectedCells = new List<bool>();
    public List<IntListWrapper> idColorCells = new List<IntListWrapper>();
   
#if UNITY_EDITOR
    private bool refreshScheduled = false;
#endif

    public void INIT()
    {
        // Rebuild immediately at play time
        if (Application.isPlaying)
        {
            ResizeCellList();
            RefreshGrid();
        }
    }

    public virtual void RefreshGrid()
    {
        ResizeCellList();
        // Find or create the container
        Transform container = transform.Find("__GridContainer");
        if (container == null)
        {
            var go = new GameObject("__GridContainer");
            go.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            go.transform.SetParent(transform, false);
            container = go.transform;
        }

        // Clear old cells
        for (int i = container.childCount - 1; i >= 0; i--)
            DestroyImmediate(container.GetChild(i).gameObject);

        enabledCells.Clear();
        int cols = Mathf.Max(1, gridSize.x);
        int rows = Mathf.Max(1, gridSize.y);

        for (int y = 0; y < rows; y++)
        {
            for (int x = cols - 1; x >= 0; x--)
            {
                int idx = (rows - 1 - y) * cols + x;
                GameObject cell;

                if (cellPrefab != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        // in the editor but not running playmode
                        cell = PrefabUtility.InstantiatePrefab(cellPrefab) as GameObject;
                        cell.GetComponent<MeshRenderer>().enabled = true;
                    }
                    else
                    {
                        // in playmode in the editor
                        cell = Instantiate(cellPrefab);
                    }
#else
// in a player build (or on device) always use Instantiate
cell = Instantiate(cellPrefab);
#endif
                }
                else
                {
                    cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                }

                cell.name = $"Cell_{x}_{y}";
                cell.transform.SetParent(container, false);
                cell.layer = 5;

                // position on XY plane with user-set spacing
                float posX = x * (1f + cellSpacing.x);
                float posY = y * (1f + cellSpacing.y);
                cell.transform.localPosition = new Vector3(posX, posY, 0f);

                bool isSelected = selectedCells[idx];
                cell.SetActive(isSelected);

                if (isSelected)
                {
                    SetBlock(cell, idx);
                }

            }
        }
           
    }

    public virtual void SetBlock(GameObject cell, int idx)
    {
        var id = cell.GetComponent<IAssignID>();
        var rend = cell.GetComponent<Renderer>();
        var grid = cell.GetComponent<GridAssignID>();
        var enumid = EnumID.NONE;

        if (grid != null)
        {
            enabledCells.Add(grid);
        }
        if (id != null)
        {
            if (idColorCells[idx].ids.Count <= 0) return;
            id.SetIDs(new List<int>(idColorCells[idx].ids));
            enumid = (EnumID)idColorCells[idx].ids[0];
        }
        if (rend != null)
        {
            Color selectedColor = GetColor(enumid);
            var mat = new Material(rend.sharedMaterial) { color = selectedColor };
            rend.sharedMaterial = mat;
        }

    }

    public Color GetColor(EnumID id)
    {
        Color selectedColor = Color.white;
        switch (id)
        {
            case EnumID.NONE:
                selectedColor = new Color(0f, 0f, 0f, 0.5f);
                break;
            case EnumID.RED:
                selectedColor = new Color(255f / 255f, 53 / 255f, 59 / 255f, 1);
                break;
            case EnumID.BLUE:
                selectedColor = new Color(44 / 255f, 0, 214 / 255f, 1);
                break;
            case EnumID.GREEN:
                selectedColor = new Color(0, 135 / 255f, 60 / 255f, 1f);
                break;
            case EnumID.YELLOW:
                selectedColor = new Color(254 / 255f, 255 / 255f, 66 / 255f, 1);
                break;
            case EnumID.ORANGE:
                selectedColor = new Color(250 / 255f, 105 / 255f, 45 / 255f, 1);
                break;
            case EnumID.CYAN:
                selectedColor = new Color(37 / 255f, 177 / 255f, 233 / 255f, 1);
                break;
            case EnumID.PINK:
                selectedColor = new Color(237 / 255f, 138 / 255f, 136 / 255f, 1);
                break;
            case EnumID.WHITE:
                selectedColor = Color.white;
                break;
            case EnumID.BLACK:
                selectedColor = Color.black;
                break;
            case EnumID.BROWN:
                selectedColor = new Color(202 / 255f, 89 / 255f, 39 / 255f, 1);
                break;
            case EnumID.DARKBROWN:
                selectedColor = new Color(103 / 255f, 57 / 255f, 31 / 255f, 1);
                break;

        }
        return selectedColor;
    }

    private void ResizeCellList()
    {
        int total = Mathf.Max(1, gridSize.x) * Mathf.Max(1, gridSize.y);

        if (selectedCells == null)
        {
            selectedCells = new List<bool>(new bool[total]);
        }       
        else if (selectedCells.Count != total)
        {
            var old = selectedCells.ToArray();
            selectedCells = new List<bool>(new bool[total]);
            for (int i = 0; i < Mathf.Min(old.Length, total); i++)
            {
                selectedCells[i] = old[i];
            }
        }

        if (idColorCells == null)
        {
            idColorCells = new List<IntListWrapper>(new IntListWrapper[total]);
        }
        else if (idColorCells.Count != total)
        {
            var old = idColorCells.ToArray();
            idColorCells = new List<IntListWrapper>(new IntListWrapper[total]);
            for (int i = 0; i < Mathf.Min(old.Length, total); i++)
            {
                idColorCells[i] = old[i];
            }
        }
    }

    private void OnValidate()
    {
        ResizeCellList();

#if UNITY_EDITOR
        // skip auto‐rebuild if this is the prefab asset being edited
        if (!Application.isPlaying && PrefabUtility.IsPartOfPrefabAsset(gameObject))
            return;

        if (!refreshScheduled)
        {
            refreshScheduled = true;
            EditorApplication.delayCall += () =>
            {
                refreshScheduled = false;
                if (this) RefreshGrid();
            };
        }
#else
    RefreshGrid();
#endif
    }
}