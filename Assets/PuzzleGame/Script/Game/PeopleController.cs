using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PeopleController : IDAssign, IAssignID
{
    public List<EnumID> ListID = new List<EnumID>();
    public GameObject FakeGrid, RealGrid;
    private Vector3 scaleOrigin;
    private Rigidbody rb;
    private MeshCollider colliderbox;
    private GameObject realOwner;
    private HoleController _holeScript;

    private LayerMask mask = (1 << 0) | (1 << 8);

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        realOwner = transform.parent.gameObject;
        var boxes = transform.GetComponents<Collider>();
        colliderbox = boxes[1] as MeshCollider; // Because fuck it 
        scaleOrigin = transform.localScale;
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

    public void SuckedIntoHole(Transform holepos, HoleController holescript)
    {
        colliderbox.excludeLayers = LayerMask.GetMask("People", "Grid", "Default");
        _holeScript = holescript;
        StartCoroutine(MovementSucked(holepos));
    }

    private IEnumerator MovementSucked(Transform holepos)
    {
        _holeScript.SetStandbyCube(1);
        Vector3 offset = new Vector3(0f, -1.5f, 0f);
        float stopDist = 0.5f;     
        float baseSpeed = 5f;
        float multiplySpeed = 1f;
        float startY = transform.position.y;
        float delayForce = 0.05f;
        rb.AddForce(new Vector3(0f, 25f, 0f), ForceMode.Impulse);
        yield return new WaitForSeconds(delayForce);
        while (true)
        {
            Vector3 yPos = new Vector3(holepos.position.x, startY, holepos.position.z);
            Vector3 linearPos = Vector3.Lerp(transform.position, yPos, 0.3f);
            Vector3 targetPos = linearPos + offset;
            float dist = Vector3.Distance(transform.position, targetPos);
            if (dist <= stopDist)
                break;

            float speed = (baseSpeed + multiplySpeed + dist * 10f) * GameManager.Instance.gameSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed);
            multiplySpeed += (multiplySpeed * 10f + dist * 10f) * Time.deltaTime;
            
            yield return null;
        }
        gameObject.SetActive(false);
        GameManager.Instance.currentMap.ObjectSucked(this);
        _holeScript.SetStandbyCube(-1);
        /*FunctionManager.Instance.DelayFunction(0.5f, () => 
        {
            
        });*/
        
    }

    public void MovementFill(Vector3 start, Transform end)
    {
        transform.SetParent(realOwner.transform, true);
        FakeGrid.SetActive(false);
        RealGrid.SetActive(true);
        rb.isKinematic = true;
        rb.useGravity = false;
        colliderbox.isTrigger = true;
        transform.position = start + new Vector3(Random.Range(-2.5f, 2.5f), 0, 0);
        var t = DOTween.Sequence()
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                //gameObject.SetActive(false);
            }); ;
        t.Join(transform.DOJump(end.position, 1f, 1, 1f * GameManager.Instance.gameSpeed));
        t.Join(transform.DORotateQuaternion(end.rotation, 1f * GameManager.Instance.gameSpeed));
        t.Join(transform.DOScale(scaleOrigin, 1f * GameManager.Instance.gameSpeed));
    }

    public void SetIDs(List<int> ids)
    {
        throw new System.NotImplementedException();
    }
}

