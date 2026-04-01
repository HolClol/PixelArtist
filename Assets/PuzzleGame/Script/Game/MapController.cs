using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

#region JUNK STUFF
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
#endregion

public class BlockInfo
{
    public MeshCollider Collider;
    public PeopleController Controller;

    public BlockInfo(MeshCollider collider, PeopleController controller)
    {
        Collider = collider;
        Controller = controller;
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
    [HideInInspector] public Dictionary<EnumID,List<BlockInfo>> dictPeople = new Dictionary<EnumID, List<BlockInfo>>();
    [HideInInspector] public Dictionary<HoleController, List<EnumID>> dictHole = new Dictionary<HoleController, List<EnumID>>();
    //[HideInInspector] public List<SpotController> groupSpots;
    //[HideInInspector] public List<FillLine> groupFillLines;
    //[HideInInspector] public List<IAssignID> People;

    private List<GameObject> suckedObjects = new List<GameObject>();
    private int fillCount = 0;
    private int totalCount = 0;

#if UNITY_EDITOR
    [Header("Map Data")]
    public List<MapObjectData> peopleList = new();
    public List<MapObjectData> holeList = new();
    public List<MapObjectData> gridList = new();
#endif

    public void INIT()
    {
        suckedObjects.Clear();
        // Get all holes in the map
        foreach (Transform hole in _hole.transform)
        {
            var holeScript = hole.GetComponentInChildren<HoleController>();
            dictHole[holeScript] = new List<EnumID>(holeScript.ListID);
        }
        // Get all the blocks unsorted in the map
        foreach (Transform floor in _people.transform)
        {
            totalCount++;
            foreach (Transform group in floor) 
            {
                groupPeople.Add(group);
            }
        }
        // Wtf is this
        foreach (Transform person in groupPeople)
        {
            MeshCollider colliderObject = person.GetComponentInChildren<MeshCollider>();
            PeopleController controllerObject = person.GetComponentInChildren<PeopleController>();
            foreach (var id in controllerObject.GetIDs())
            {
                var key = id;
                // Create new key to fill the ID
                if (!dictPeople.TryGetValue(key, out List<BlockInfo> list))
                {
                    list = new List<BlockInfo>();
                    dictPeople[key] = list;
                }
                BlockInfo info = new BlockInfo(colliderObject, controllerObject);
                list.Add(info);
                break;
            }
        }
        // Sort holes according to blocks with matching ID
        foreach (var hole in dictHole)
        {
            var holeScript = hole.Key;
            foreach (var id in hole.Value)
            {
                foreach (var people in dictPeople)
                {
                    if (holeScript.GetIDs().Contains(people.Key))
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
                            holeScript.IgnoreCollision(single.Collider);
                        }
                    }
                }
                #region FOR SPECIAL CASES IDK HOW THIS HAPPEN
                /*if (GameManager.Instance.CurrentLevel == 4)
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
                }*/
                #endregion

            }
        }

        GridCanvasController.INIT();
        GridPathfinder3D.Instance.InitializeGraph(_grid);
        #region JUNK STUFF
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
        #endregion 
    }
    #region JUNK STUFF
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
    #endregion
    

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

    public void CallHighlightBlock(List<EnumID> ids, bool value)
    {
        List<PeopleController> foundIDs = new List<PeopleController>();
        foreach (var neededid in ids)
        {
            foreach (var id in dictPeople)
            {
                if (id.Key == neededid)
                {
                    foreach (var block in id.Value)
                    {
                        foundIDs.Add(block.Controller);
                    }
                    
                }
            }
        }
        foreach (var block in foundIDs)
        {
            if (value)
                block.Highlight();
            else 
                block.UnHighlight();
        }
    }
    

    private void IncreaseCount()
    {
        fillCount++;
        if (fillCount >= totalCount /*GridCanvasController.enabledCells.Count*/)
        {
            GameManager.Instance.GameWin();
        }
    }

    #region JUNK STUFF
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
    #endregion
}
