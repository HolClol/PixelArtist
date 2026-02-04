using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class HoleController : IDAssign, IDraggable, IAssignID
{
    public GameObject main;
    public GameObject prefab;
    public List<EnumID> ListID = new List<EnumID>();    
    [HideInInspector] public List<IDAssign> people = new List<IDAssign>();

    [SerializeField] private Collider[] holecollider;
    private int cubeSucked = 0;
    [HideInInspector] public int totalCubes = 0;
    private bool draggable = true;
    private Rigidbody rigidBody; 

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public List<int> GetIDs()
    {
        var list = new List<int>();
        foreach (EnumID id in ListID)
        {
            list.Add((int)id);
        }
        return list;
    }

    public void OnClick()
    {
        /*int destID = (int)ID;
        int count = 0;
        Vector3 dest = transform.position;

        people = new List<IDAssign>(GameManager.Instance.currentMap.ShufflePeopleTable(this));
        foreach (var ppl in people)
        {
            Vector3 start = ppl.transform.position + Vector3.up * 0.5f;

            var path = GridPathfinder3D.Instance.FindPath(start, dest, destID);

            if (path.Count > 0)
            {
                path.Add(dest);
                ppl.GetComponent<PeopleController>().FollowPath(path);
                count++;
                if (count >= GameManager.Instance.MaxHoleSearch)
                {
                    break;
                }
            }
            else
            {
                // Incase it got blocked
            }
        }*/
    }

    public void OnRelease(Node3D node)
    {
        main.transform.position = new Vector3(node.pos.x, main.transform.position.y, node.pos.z);
    }

    public void SetIDs(List<int> ids)
    {
        throw new System.NotImplementedException();
    }

    public void IgnoreCollision(Collider other)
    {
        foreach (var collider in holecollider)
        {
            Physics.IgnoreCollision(collider, other);
        }
    }

    public bool GetDraggable()
    {
        return draggable;
    }

    public void Drag(Vector3 targetpos)
    {
        rigidBody.MovePosition(targetpos);
    }

    private bool CheckID(List<int> pplid)
    {
        foreach (var holeid in ListID)
        {
            foreach (var id in pplid)
            {
                if ((int)holeid == id)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<IAssignID>() != null)
        {
            var people = collider.GetComponent<PeopleController>();
            if (!CheckID(people.GetIDs())) { return; }

            cubeSucked++;
            people.gameObject.transform.SetParent(transform, true);
            people.SuckedIntoHole(transform);
            
            /*Debug.Log("sucked " + cubeSucked);
            Debug.Log("totalCubes: " + totalCubes);*/

            if (cubeSucked < totalCubes) return;
            draggable = false;
            
            var parent = FunctionManager.Instance.GetRootParent(gameObject, 1);
            if (InputManager.Instance.currentlyDragging == parent)
            {
                InputManager.Instance.HoleDestroyed();
                
            }
            FunctionManager.Instance.DelayFunction(0.25f, () =>
            {
                main.transform.DOScale(0f, 0.25f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => 
                    {
                        var confetti = Pooling.Spawn("ConfettiBlast", prefab, "");
                        confetti.transform.position = main.transform.position;
                        parent.SetActive(false);
                    });
            });
        }
    } 
}
