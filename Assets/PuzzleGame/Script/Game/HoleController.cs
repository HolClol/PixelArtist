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
    [HideInInspector] public int totalCubes = 0;
    
    [SerializeField] private Collider[] holecollider;
    private bool draggable = true;
    private Rigidbody rigidBody;
    private int cubeSucked = 0;
    private int _inAnimationCubes = 0;

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

    public void SetStandbyCube(int value)
    {
        _inAnimationCubes += value;
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

    private IEnumerator HoleDestroy()
    {
        var parent = FunctionManager.Instance.GetRootParent(gameObject, 1);
        if (InputManager.Instance.currentlyDragging == parent)
        {
            InputManager.Instance.HoleDestroyed();
        }

        while (_inAnimationCubes > 0)
        {
            yield return new WaitForSeconds(0.1f);
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponent<IAssignID>() != null)
        {
            var cube = collider.GetComponent<PeopleController>();
            if (!CheckID(cube.GetIDs())) { return; }

            cubeSucked++;
            cube.gameObject.transform.SetParent(transform, true);
            cube.SuckedIntoHole(transform, this);
            
            /*Debug.Log("sucked " + cubeSucked);
            Debug.Log("totalCubes: " + totalCubes);*/

            if (cubeSucked < totalCubes) return;
            draggable = false;
            StartCoroutine(HoleDestroy());
            
        }
    } 
}
