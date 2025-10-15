#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public enum MapCreatorType
{
    Grid,
    People,
    Hole
}
[System.Serializable]
public class MapObjectData
{
    public Vector3 position;
    public GameObject game_object;
}
[ExecuteInEditMode]
public class MapCreatorManager : MonoBehaviour
{
    public MapCreatorType type;
    public EnumID idColor;
    public int mapID;


    public AssetReference map;
    private MapController mapController;


    public GameObject objectGrid;
    private Transform parentGrid;
    private Dictionary<Vector3, GameObject> gridPos = new();

    public GameObject objectPeople_blue;
    public GameObject objectPeople_green;
    public GameObject objectPeople_red;
    public GameObject objectPeople_yellow;
    public GameObject objectPeople_cyan;
    public GameObject objectPeople_pink;
    public GameObject objectPeople_orange;
    public GameObject objectPeople_white;
    public GameObject objectPeople_black;
    public GameObject objectPeople_brown;
    public GameObject objectPeople_darkbrown;
    private Transform parentPeople;
    private Dictionary<Vector3, GameObject> peoplePos = new();

    public GameObject objectHole;
    private Transform parentHole;
    private Dictionary<Vector3, GameObject> holePos = new();
    private void Start()
    {
        gridPos = new Dictionary<Vector3, GameObject>();
    }
    void Update()
    {
        // Kiểm tra khi người dùng click chuột trái
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            switch (type)
            {
                case MapCreatorType.Grid:
                    Vector3 localPosition = parentGrid.InverseTransformPoint(mousePosition);
                    localPosition.x = Mathf.Round(localPosition.x / 2f) * 2f;
                    localPosition.z = Mathf.Round(localPosition.z / 2f) * 2f;
                    localPosition.y = 0;
                    if (!gridPos.ContainsKey(localPosition))
                    {
                        GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(objectGrid);
                        spawnedObject.transform.SetParent(parentGrid, false);
                        spawnedObject.transform.localPosition = localPosition;
                        gridPos[localPosition] = spawnedObject;
                    }
                    break;
                case MapCreatorType.People:
                    Vector3 localPositionPeople = parentPeople.InverseTransformPoint(mousePosition);
                    localPositionPeople.x = Mathf.Round(localPositionPeople.x)+0.2f;
                    localPositionPeople.z = Mathf.Round(localPositionPeople.z)+0.15f;
                    localPositionPeople.y = 0;

                    localPositionPeople = Result_PeoplePos_Insert(localPositionPeople);

                    if (!peoplePos.ContainsKey(localPositionPeople))
                    {
                        switch (idColor)
                        {
                            case EnumID.BLUE:
                                GameObject spawnedObject1 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_blue);
                                spawnedObject1.transform.SetParent(parentPeople, false);
                                spawnedObject1.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject1;
                                break;
                            case EnumID.RED:
                                GameObject spawnedObject2 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_red);
                                spawnedObject2.transform.SetParent(parentPeople, false);
                                spawnedObject2.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject2;
                                break;
                            case EnumID.GREEN:
                                GameObject spawnedObject3 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_green);
                                spawnedObject3.transform.SetParent(parentPeople, false);
                                spawnedObject3.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject3;
                                break;
                            case EnumID.YELLOW:
                                GameObject spawnedObject4 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_yellow);
                                spawnedObject4.transform.SetParent(parentPeople, false);
                                spawnedObject4.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject4;
                                break;
                            case EnumID.CYAN:
                                GameObject spawnedObject5 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_cyan);
                                spawnedObject5.transform.SetParent(parentPeople, false);
                                spawnedObject5.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject5;
                                break;
                            case EnumID.PINK:
                                GameObject spawnedObject6 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_pink);
                                spawnedObject6.transform.SetParent(parentPeople, false);
                                spawnedObject6.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject6;
                                break;
                            case EnumID.ORANGE:
                                GameObject spawnedObject7 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_orange);
                                spawnedObject7.transform.SetParent(parentPeople, false);
                                spawnedObject7.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject7;
                                break;
                            case EnumID.WHITE:
                                GameObject spawnedObject8 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_white);
                                spawnedObject8.transform.SetParent(parentPeople, false);
                                spawnedObject8.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject8;
                                break;
                            case EnumID.BLACK:
                                GameObject spawnedObject9 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_black);
                                spawnedObject9.transform.SetParent(parentPeople, false);
                                spawnedObject9.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject9;
                                break;
                            case EnumID.BROWN:
                                GameObject spawnedObject10 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_brown);
                                spawnedObject10.transform.SetParent(parentPeople, false);
                                spawnedObject10.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject10;
                                break;
                            case EnumID.DARKBROWN:
                                GameObject spawnedObject11 = (GameObject)PrefabUtility.InstantiatePrefab(objectPeople_darkbrown);
                                spawnedObject11.transform.SetParent(parentPeople, false);
                                spawnedObject11.transform.localPosition = localPositionPeople;
                                peoplePos[localPositionPeople] = spawnedObject11;
                                break;
                        }
                    }
                    break;
                case MapCreatorType.Hole:
                    Vector3 localPositionHole = parentHole.InverseTransformPoint(mousePosition);
                    localPositionHole.x = Mathf.Round(localPositionHole.x / 3f) * 3f;
                    localPositionHole.z = Mathf.Round(localPositionHole.z / 3f) * 3f;
                    localPositionHole.y = 0;
                    if (!holePos.ContainsKey(localPositionHole))
                    {
                        GameObject spawnedObject5 = (GameObject)PrefabUtility.InstantiatePrefab(objectHole);
                        spawnedObject5.transform.SetParent(parentHole, false);
                        spawnedObject5.transform.localPosition = localPositionHole;
                        holePos[localPositionHole] = spawnedObject5;
                    }
                    break;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            switch (type)
            {
                case MapCreatorType.Grid:
                    Vector3 localPosition = parentGrid.InverseTransformPoint(mousePosition);
                    localPosition.x = Mathf.Round(localPosition.x / 2f) * 2f;
                    localPosition.z = Mathf.Round(localPosition.z / 2f) * 2f;
                    localPosition.y = 0;
                    if (gridPos.ContainsKey(localPosition))
                    {
                        DestroyImmediate(gridPos[localPosition]);
                        gridPos.Remove(localPosition);
                    }
                    break;
                case MapCreatorType.People:
                    Vector3 localPositionPeople = parentPeople.InverseTransformPoint(mousePosition);
                    localPositionPeople.x = Mathf.Round(localPositionPeople.x) + 0.2f;
                    localPositionPeople.z = Mathf.Round(localPositionPeople.z) + 0.15f;
                    localPositionPeople.y = 0;

                    localPositionPeople = Result_PeoplePos_Remove(localPositionPeople);

                    if (peoplePos.ContainsKey(localPositionPeople))
                    {
                        DestroyImmediate(peoplePos[localPositionPeople]);
                        peoplePos.Remove(localPositionPeople);
                    }
                    break;
                case MapCreatorType.Hole:
                    Vector3 localPositionHole = parentHole.InverseTransformPoint(mousePosition);
                    localPositionHole.x = Mathf.Round(localPositionHole.x / 3f) * 3f;
                    localPositionHole.z = Mathf.Round(localPositionHole.z / 3f) * 3f;
                    localPositionHole.y = 0;
                    if (holePos.ContainsKey(localPositionHole))
                    {
                        DestroyImmediate(holePos[localPositionHole]);
                        holePos.Remove(localPositionHole);
                    }
                    break;
            }
        }
    }
    private Vector3 Result_PeoplePos_Insert(Vector3 pos)
    {
        Vector3 temp = pos;
        for (int i = 0; i < 100; i++)
        {
            temp.y = i * 0.7f;
            if (!peoplePos.ContainsKey(temp))
            {
                return temp;
            }
        }
        return temp;
    }
    private Vector3 Result_PeoplePos_Remove(Vector3 pos)
    {
        Vector3 temp = pos;
        for (int i = 100; i >=0; i--)
        {
            temp.y = i * 0.7f;
            if (peoplePos.ContainsKey(temp))
            {
                return temp;
            }
        }
        return temp;
    }
    [Button]
    public void LoadNewMap()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(map);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject loadedObject = Instantiate(op.Result) as GameObject;
                if (loadedObject != null)
                {
                    loadedObject.transform.position = Vector3.zero;
                    mapController = loadedObject.GetComponent<MapController>();
                    parentGrid = mapController._grid;
                    parentHole = mapController._hole;
                    parentPeople = mapController._people;   
                }
            }
        };
    }
    [Button]
    public void LoadMap_ID()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>($"Assets/PuzzleGame/MapCreator/Map {mapID}.prefab");
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject loadedObject = Instantiate(op.Result) as GameObject;
                if (loadedObject != null)
                {
                    loadedObject.transform.position = Vector3.zero;
                    mapController = loadedObject.GetComponent<MapController>();
                    parentGrid = mapController._grid;
                    parentHole = mapController._hole;
                    parentPeople = mapController._people;

                    peoplePos.Clear();
                    foreach (var data in mapController.peopleList)
                    {
                        peoplePos[data.position] = data.game_object;
                    }
                    gridPos.Clear();
                    foreach (var data in mapController.gridList)
                    {
                        gridPos[data.position] = data.game_object;
                    }
                    holePos.Clear();
                    foreach (var data in mapController.holeList)
                    {
                        holePos[data.position] = data.game_object;
                    }
                }
            }
        };
    }
    [Button]
    public void SaveMap()
    {
        mapController.peopleList.Clear();
        foreach (var pos in peoplePos)
        {
            mapController.peopleList.Add(new MapObjectData { position = pos.Key, game_object = pos.Value });
        }
        mapController.gridList.Clear();
        foreach (var pos in gridPos)
        {
            mapController.gridList.Add(new MapObjectData { position = pos.Key, game_object = pos.Value });
        }
        mapController.holeList.Clear();
        foreach (var pos in holePos)
        {
            mapController.holeList.Add(new MapObjectData { position = pos.Key, game_object = pos.Value });
        }
        string prefabPath = "Assets/PuzzleGame/MapCreator/Map " + mapID + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(mapController.gameObject, prefabPath);
    }
}
#endif