using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable] public class FillLine
{
    public List<FillHoleController> groupFillHoles;
    public List<Vector3> groupPos;

    public FillLine(List<FillHoleController> hole, List<Vector3> groupPos)
    {
        groupFillHoles = hole;
        this.groupPos = groupPos;
    }
}

public class MapController : MonoBehaviour
{
    [Header("Map Setting Properties")]
    public float CameraDistance = 10f;
    public float RewardCoin = 40f;
    public float Timer = 120f;

    [Header("Map Set Up Properties")]
    public Transform _people;
    public Transform _grid;
    public Transform _hole;
    public Transform BlackHole;
    public GridVisualizer GridCanvasController;

    // If want to debug or test, comment the [HideInInspector]
    [HideInInspector] public List<Transform> groupPeople;
    [HideInInspector] public Dictionary<EnumID,List<MeshCollider>> dictPeople = new Dictionary<EnumID, List<MeshCollider>>();
    [HideInInspector] public Dictionary<HoleController, List<EnumID>> dictHole = new Dictionary<HoleController, List<EnumID>>();
    //[HideInInspector] public List<SpotController> groupSpots;
    //[HideInInspector] public List<FillLine> groupFillLines;
    //[HideInInspector] public List<IAssignID> People;

    private List<GameObject> suckedObjects = new List<GameObject>();
    public int fillCount = 0;
    public int totalCount = 0;

#if UNITY_EDITOR
    [Header("Map Data")]
    public List<MapObjectData> peopleList = new();
    public List<MapObjectData> holeList = new();
    public List<MapObjectData> gridList = new();
#endif

    public void INIT()
    {
        suckedObjects.Clear();
        foreach (Transform hole in _hole.transform)
        {
            var holeScript = hole.GetComponentInChildren<HoleController>();
            dictHole[holeScript] = new List<EnumID>(holeScript.ListID);
        }
        foreach (Transform floor in _people.transform)
        {
            totalCount++;
            foreach (Transform group in floor) 
            {
                groupPeople.Add(group);
            }
        }
        foreach (Transform person in groupPeople)
        {
            var idObject = person.GetComponentInChildren<IAssignID>();
            var colliderObject = person.GetComponentInChildren<MeshCollider>();
            foreach (var id in idObject.GetIDs())
            {
                var key = (EnumID)id;
                if (!dictPeople.TryGetValue(key, out List<MeshCollider> list))
                {
                    list = new List<MeshCollider>();
                    dictPeople[key] = list;
                }

                list.Add(colliderObject);
                break;
            }
        }
        foreach (var hole in dictHole)
        {
            var holeScript = hole.Key;
            foreach (var id in hole.Value)
            {
                foreach (var people in dictPeople)
                {
                    if (holeScript.GetIDs().Contains((int)people.Key))
                    {
                        foreach (var single in people.Value)
                        {
                            holeScript.totalCubes++;
                        }
                    }
                    else
                    {
                        foreach (var single in people.Value)
                        {
                            holeScript.IgnoreCollision(single);
                        }
                    }
                }
                #region FOR SPECIAL CASES IDK HOW THIS HAPPEN
                if (GameManager.Instance.CurrentLevel == 4)
                {
                    if (id == EnumID.YELLOW) 
                    {
                        holeScript.totalCubes--;
                        totalCount--;
                    }
                }
                else if (GameManager.Instance.CurrentLevel == 8)
                {
                    if (id == EnumID.BLACK)
                    {
                        holeScript.totalCubes--;
                        totalCount--;
                    }
                }
                #endregion

            }
        }

        GridCanvasController.INIT();
        GridPathfinder3D.Instance.InitializeGraph(_grid);
        /*foreach (Transform line in _fillhole)
        { 
            var linehole = new List<FillHoleController>();
            var linepos = new List<Vector3>();
            foreach (Transform fillhole in line)
            {
                var hole = fillhole.GetComponent<FillHoleController>();
                linehole.Add(hole);
                linepos.Add(fillhole.position);
            }
            FillLine fillLine = new FillLine(linehole, linepos);
            groupFillLines.Add(fillLine);
        }

        int count = 0;
        foreach (Transform group in RectSpotLayout.transform)
        {
            count++;
            group.gameObject.SetActive(true);
            groupSpots.Add(group.GetComponentInChildren<SpotController>());
            if (count == AvailableSpots) { break; }
        }*/
    }

    public List<PeopleController> ShufflePeopleTable(HoleController hole)
    {
        /*int holeID = (int)hole.ID;
        Vector3 holePos = hole.transform.position;

        var orderedContainer = new List<Transform>();
        orderedContainer = groupPeople
            .OrderBy(c => (c.transform.position - holePos).sqrMagnitude)                         
            .ToList();

        var result = new List<PeopleController>();
        foreach (Transform group in orderedContainer)
        {
            foreach (Transform person in group)
            {
                PeopleController pplid = person.GetComponentInChildren<PeopleController>();
                if ((int)pplid.ID == holeID && !pplid.InHole && !pplid.Moving)
                    result.Add(pplid);
            }   
        }   */

        return null;
    }
    

    public void ObjectSucked(PeopleController obj)
    {
        var list = GridCanvasController.enabledCells;
        int max = list.Count - 1;
        bool placed = false;
        for (int i = max; i >= 0; i--)
        {
            foreach (var id in obj.GetIDs())
            {
                if (list[i].GetIDs().Contains(id) && !list[i].Taken) 
                {
                    obj.gameObject.SetActive(true);
                    obj.MovementFill(BlackHole.position, list[i].transform);
                    list[i].Taken = true;
                    placed = true;
                    break;
                } 
            }
            if (placed) 
            {
                //IncreaseCount();
                break; 
            }
        }
        IncreaseCount();
    }

    private void IncreaseCount()
    {
        fillCount++;
        if (fillCount >= totalCount /*GridCanvasController.enabledCells.Count*/)
        {
            GameManager.Instance.GameWin();
        }
    }
    
    private bool CheckLineAvailable(PeopleController p)
    {
        bool value = false;
        /*FillHoleController foundhole = null;
        foreach (var line in groupFillLines)
        {
            // Always get the first fill hole in the line
            // Fix this so if there are two of the same holes colors in the same line, it will fill those two also
            if (line.groupFillHoles[0].ID == p.ID && line.groupFillHoles[0].Open)
            {
                line.groupFillHoles[0].AddTempo();
                foundhole = line.groupFillHoles[0];
                value = true;
                break;
            }
        }
        
        if (foundhole != null)
        {
            var ufo = GameManager.Instance.GetUFO((int)p.ID, GameManager.Instance.MaxHoleIntake - foundhole.Amount);
            if (ufo.hole == null)
                ufo.hole = foundhole;
            if (!ufo.Running)
                ufo.StartAnimation(foundhole.transform.position + Vector3.up * 4f);
            ufo.AddPeople(p);
        }*/

        return value;
    }

    private bool CheckSpotAvailable(PeopleController p)
    {
        bool value = false;
        /*SpotController foundspot = null;
        foreach (var spot in groupSpots)
        {
            if ((spot.ID == p.ID || spot.ID == EnumID.NONE) && spot.Open)
            {
                spot.AddTempo(p);
                foundspot = spot;
                value = true;
                break;
            }
        }

        if (foundspot != null)
        {
            var ufo = GameManager.Instance.GetUFO((int)p.ID, GameManager.Instance.MaxHoleSearch - foundspot.Amount);
            if (ufo.spot != foundspot)
                ufo.spot = foundspot;
            if (!ufo.Running)
                ufo.StartAnimation(foundspot.transform.position + Vector3.up * 8f);
            ufo.AddPeople(p);

        }*/
        return value;
    }

    private void NothingAvailable(PeopleController p)
    {
        /*UFOController ufo = GameManager.Instance.GetUFO((int)p.ID, GameManager.Instance.MaxHoleSearch);
        if (!ufo.Running)
            ufo.StartAnimation(new Vector3(2.75f, 6f, -5.75f));
        ufo.AddPeople(p);*/
    }
}
