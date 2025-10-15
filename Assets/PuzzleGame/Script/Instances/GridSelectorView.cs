#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridVisualizer))]
public class gridVisualizerView : Editor
{
    SerializedProperty gridSizeProp;
    SerializedProperty cellSpacingProp;
    SerializedProperty selectedCellsProp;
    SerializedProperty cellPrefabProp;
    SerializedProperty enabledCellsProp;
    SerializedProperty enumColorIDProp;
    SerializedProperty enumColorIDCellsProp;
    GridVisualizer gridVisualizer;

    const float cellSize = 20f;
    const float margin = 2f;

    // ← Declare the guard here
    private bool guiRefreshScheduled = false;

    void OnEnable()
    {
        gridVisualizer = (GridVisualizer)target;
        gridSizeProp = serializedObject.FindProperty("gridSize");
        cellSpacingProp = serializedObject.FindProperty("cellSpacing");
        cellPrefabProp = serializedObject.FindProperty("cellPrefab");
        selectedCellsProp = serializedObject.FindProperty("selectedCells");
        enabledCellsProp = serializedObject.FindProperty("enabledCells");
        enumColorIDProp = serializedObject.FindProperty("currentID");
        enumColorIDCellsProp = serializedObject.FindProperty("idColorCells");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(cellPrefabProp);
        EditorGUILayout.PropertyField(gridSizeProp);
        EditorGUILayout.PropertyField(cellSpacingProp);
        EditorGUILayout.PropertyField(enumColorIDProp);
        //EditorGUILayout.PropertyField(enumColorIDCellsProp);
        //EditorGUILayout.PropertyField(selectedCellsProp);
        //EditorGUILayout.PropertyField(enabledCellsProp);

        DrawToggleGrid();
        serializedObject.ApplyModifiedProperties();

        /* Bake Prefaba
        GUILayout.Space(10);
        if (GUILayout.Button("Bake Prefab…"))
        {
            // ensure the grid is up to date
            gridVisualizer.RefreshGrid();

            // ask for save path
            string path = EditorUtility.SaveFilePanelInProject(
                "Save Grid Prefab",
                gridVisualizer.name + ".prefab",
                "prefab",
                "Choose location to save your grid prefab."
            );
            if (!string.IsNullOrEmpty(path))
            {
                // Instantiate a temporary scene copy
                var temp = Instantiate(gridVisualizer.gameObject);
                temp.name = gridVisualizer.gameObject.name;

                // Save that copy as the new prefab asset
                PrefabUtility.SaveAsPrefabAsset(temp, path);

                // Clean up the temporary copy
                DestroyImmediate(temp);

                // Refresh the AssetDatabase as before
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog(
                    "Grid Prefab Saved",
                    "Prefab saved to:\n" + path,
                    "OK"
                );
            }
        }*/
    }

    private void DrawToggleGrid()
    {
        Vector2Int size = gridSizeProp.vector2IntValue;
        int cols = Mathf.Max(1, size.x);
        int rows = Mathf.Max(1, size.y);

        // Ensure list size matches
        int needed = cols * rows;
        if (selectedCellsProp.arraySize != needed)
            selectedCellsProp.arraySize = needed;
        if (enumColorIDCellsProp.arraySize != needed)
            enumColorIDCellsProp.arraySize = needed;

        float totalW = cols * cellSize + (cols + 1) * margin;
        float totalH = rows * cellSize + (rows + 1) * margin;
        Rect gridRect = GUILayoutUtility.GetRect(totalW, totalH);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int idx = (rows - 1 - y) * cols + x;
                var idsProp = enumColorIDCellsProp.GetArrayElementAtIndex(idx).FindPropertyRelative("ids");
                bool isSel = selectedCellsProp.GetArrayElementAtIndex(idx).boolValue;
                float xPos = gridRect.xMin + margin + x * (cellSize + margin);
                float yPos = gridRect.yMin + margin + y * (cellSize + margin);
                Rect cellRect = new Rect(xPos, yPos, cellSize, cellSize);

                Color bg = new Color(0f, 0f, 0f, 0.5f);
                if (idsProp.arraySize > 0)
                {
                    int firstId = idsProp.GetArrayElementAtIndex(0).intValue;
                    bg = gridVisualizer.GetColor((EnumID)firstId);
                }

                EditorGUI.DrawRect(cellRect, bg);

                if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
                {
                    int colorId = enumColorIDProp.enumValueIndex;
                    idsProp.ClearArray();
                    if (colorId == (int)EnumID.NONE)
                    {
                        selectedCellsProp.GetArrayElementAtIndex(idx).boolValue = false;
                    }
                    else
                    {
                        selectedCellsProp.GetArrayElementAtIndex(idx).boolValue = true;
                    }
                    idsProp.InsertArrayElementAtIndex(0);
                    idsProp.GetArrayElementAtIndex(0).intValue = colorId;
                    serializedObject.ApplyModifiedProperties();
                    ScheduleRefresh();
                    Event.current.Use();
                }
            }
        }
    }

    private void ScheduleRefresh()
    {
        if (!guiRefreshScheduled)
        {
            guiRefreshScheduled = true;
            EditorApplication.delayCall += () =>
            {
                guiRefreshScheduled = false;
                // Only refresh if the component still exists
                if (gridVisualizer)
                {
                    gridVisualizer.RefreshGrid();
                    EditorUtility.SetDirty(gridVisualizer);
                }
            };
        }
    }
}
#endif

