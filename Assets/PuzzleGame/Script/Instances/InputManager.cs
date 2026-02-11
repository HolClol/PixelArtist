using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDraggable
{ 
    public void OnClick();
    public void OnRelease(Node3D node);
    public bool GetDraggable();
    public void Drag(Vector3 targetpos);
}

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public GameObject currentlyDragging;
    public float MoveSpeed = 3f;

    private Vector3 dragOffset;
    private Plane dragPlane;
    private Dictionary<Node3D, bool> dynamicWalkInflate = new Dictionary<Node3D, bool>();
    private List<MeshRenderer> lastNodes = new List<MeshRenderer>();
    private Node3D dragStartNode, lastNode;
    private List<Vector3> pathNode = new List<Vector3>();

    private Node3D debugNode;
    private Coroutine movingCoroutine;
    private LayerMask[] holeMask;
    private List<int> holeID;
    private IDraggable _dragInterface;

#if UNITY_EDITOR
    [Header("Debug Gizmos")]
    [Tooltip("Draw all nodes & edges")]
    public bool drawGraphGizmos = true;
#endif

    private void Awake()
    {
        Instance = this;
    }

    private bool GameCanInput()
    {
        if (GameManager.Instance.gameState == GameStateEnum.PAUSE || GameManager.Instance.gameState == GameStateEnum.TIMEOUT || GameManager.Instance.gameState == GameStateEnum.WIN || GameManager.Instance.gameState == GameStateEnum.LOSE) return false;
        return true;
    }

    private void Update()
    {
        if (GameManager.Instance.inTermination || !GameCanInput()) return;
        if (Input.GetMouseButtonDown(0))
        {
            if (currentlyDragging != null) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitlist = Physics.RaycastAll(ray, Mathf.Infinity);
            //Debug.DrawRay(ray.origin, ray.direction * 200f, Color.red);
            foreach (var hit in hitlist)
            {
                if (hit.collider.GetComponent<IDraggable>() != null)
                {
                    _dragInterface = hit.collider.GetComponent<IDraggable>();
                    if (!_dragInterface.GetDraggable()) return;
                    currentlyDragging = FunctionManager.Instance.GetRootParent(hit.collider.gameObject, 1);
                    hit.collider.GetComponent<IDraggable>().OnClick();

                    float y = currentlyDragging.transform.position.y;
                    dragPlane = new Plane(Vector3.up, new Vector3(0, y, 0));

                    if (dragPlane.Raycast(ray, out float enter))
                    {
                        Vector3 planeHit = ray.GetPoint(enter);
                        dragOffset = currentlyDragging.transform.position - planeHit;
                    }

                    holeID = currentlyDragging.GetComponentInChildren<IAssignID>().GetIDs();
                    var inflatedWalk = GridPathfinder3D.Instance.BuildInflatedWalkability(holeID);

                    dragStartNode = GridPathfinder3D.Instance.NearestNode3D(currentlyDragging.transform.position);
                    lastNode = dragStartNode;
                    lastNodes = GetHoleFootprint3x3(lastNode);
                    dynamicWalkInflate = GridPathfinder3D.Instance.PruneToReachable(dragStartNode, inflatedWalk);

                    if (GameManager.Instance.gameState != GameStateEnum.PLAYING) { GameManager.Instance.gameState = GameStateEnum.PLAYING; }
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentlyDragging == null) { return; }
            if (movingCoroutine != null) { StopCoroutine(movingCoroutine); }
            var nodeSnap = GridPathfinder3D.Instance.NearestNode3D(currentlyDragging.transform.position, dynamicWalkInflate);
            _dragInterface.OnRelease(nodeSnap);
            dragStartNode = null;
            currentlyDragging = null;
            _dragInterface = null;
            holeID.Clear();
            lastNodes.Clear();
        }

        if (currentlyDragging != null) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!dragPlane.Raycast(ray, out float enter)) return;
            var planeHit = ray.GetPoint(enter);
            var desiredRaw = planeHit + dragOffset;

            float y = currentlyDragging.transform.position.y;
            Node3D startNode = GridPathfinder3D.Instance.NearestNode3D(currentlyDragging.transform.position, dynamicWalkInflate);
            Node3D targetNode = GridPathfinder3D.Instance.NearestNode3D(desiredRaw, dynamicWalkInflate);
            Node3D cursorNode = GridPathfinder3D.Instance.NearestNode3D(desiredRaw);
            debugNode = targetNode;

            /*Vector3 startPosition = currentlyDragging.transform.position;
            Vector3 endPosition = desiredRaw;

            Vector3 direction = (endPosition - startPosition).normalized;
            float distance = Vector3.Distance(startPosition, endPosition);*/

            if (FootprintFree(targetNode) && PathExists3x3(targetNode))
            {

                if (!dynamicWalkInflate.TryGetValue(cursorNode, out bool walkable)) return;

                Vector3 endPos = new Vector3(desiredRaw.x, y, desiredRaw.z);
                if (!walkable)
                {
                    float xdiff = Mathf.Abs(cursorNode.pos.x - targetNode.pos.x);
                    float zdiff = Mathf.Abs(cursorNode.pos.z - targetNode.pos.z);
                    if (xdiff > zdiff && zdiff == 0) // Lock X
                    {
                        endPos = new Vector3(targetNode.pos.x, y, desiredRaw.z);
                    }
                    else if (zdiff > xdiff && xdiff == 0) // Lock Z
                    {
                        endPos = new Vector3(desiredRaw.x, y, targetNode.pos.z);
                    }
                    else // Corner
                    {
                        endPos = new Vector3(targetNode.pos.x, y, targetNode.pos.z);
                    }                 
                }

                var pathNode = GridPathfinder3D.Instance.FindPath(currentlyDragging.transform.position, endPos);
                if (pathNode.Count >= 6) return;
                //_dragInterface.Drag(Vector3.Lerp(currentlyDragging.transform.position, endPos,Time.deltaTime * MoveSpeed * GameManager.Instance.gameSpeed));
                currentlyDragging.transform.position = Vector3.Lerp(currentlyDragging.transform.position, endPos,Time.deltaTime * MoveSpeed * GameManager.Instance.gameSpeed);
            }
        }
    }

    public void HoleDestroyed()
    {
        var nodeSnap = GridPathfinder3D.Instance.NearestNode3D(currentlyDragging.transform.position, dynamicWalkInflate);
        currentlyDragging.GetComponentInChildren<IDraggable>().OnRelease(nodeSnap);
        dragStartNode = null;
        currentlyDragging = null;
        holeID.Clear();
        lastNodes.Clear();
    }

    /*private IEnumerator ApplyMovement(Vector3 mousepos)
    {
        float y = currentlyDragging.transform.position.y;
        foreach (var node in pathNode)
        {
            if (currentlyDragging == null) continue;
            Vector3 target = new Vector3(node.x, y, node.z);
            if (Vector3.Distance(currentlyDragging.transform.position, target) > 0.2f)
            {
                currentlyDragging.transform.position = Vector3.Lerp(currentlyDragging.transform.position, target, GameManager.Instance.gameSpeed);
            }
            yield return null;
        }
    }*/

    private List<MeshRenderer> GetHoleFootprint3x3(Node3D center)
    {
        foreach (var node in lastNodes)
        {
            //node.isTrigger = false;
            //node.enabled = true;
        }
        
        lastNodes.Clear();
        float cs = GridPathfinder3D.Instance.cellSize;
        var footprint = new List<MeshRenderer>(9);
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                Vector3 samplePos = center.pos + new Vector3(dx * cs, 0, dz * cs);
                Node3D n = GridPathfinder3D.Instance.NearestNode3D(samplePos);
                var box = n.go.transform.GetComponent<MeshRenderer>();
                footprint.Add(box);
                //box.enabled = false;
                //box.isTrigger = true;
            }
        }

        return footprint;
    }

    private bool FootprintFree(Node3D center)
    {
        float cs = GridPathfinder3D.Instance.cellSize;
        float half = cs * 0.5f + 0.001f;

        Vector3 sampleWorld = center.pos;
        Node3D sampleNode = GridPathfinder3D.Instance.NearestNode3D(sampleWorld);
        if ((sampleNode.pos - sampleWorld).sqrMagnitude > half * half)
            return false;

        if (!dynamicWalkInflate[sampleNode])
            return false;

        return true;
    }

    private bool PathExists3x3(Node3D dest)
    {
        var path = GridPathfinder3D.Instance.FindPath(dragStartNode.pos,dest.pos,overrideWalkMap: dynamicWalkInflate);
        return path?.Count > 0;
    }



    #region ==========// DEBUG \\===========
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!drawGraphGizmos || GridPathfinder3D.Instance == null || GameManager.Instance.currentMap == null || currentlyDragging == null || pathNode == null)
            return;

        // Draw each node: green = passable, red = blocked
        float s = GridPathfinder3D.Instance.cellSize * 0.8f;
        Gizmos.matrix = Matrix4x4.identity;
        foreach (var node in GridPathfinder3D.Instance.Node3Ds)
        {
            Vector3 c = node.pos;
            Gizmos.color = dynamicWalkInflate[node] ? new Color(0, 1, 0, 0.4f)
                                         : new Color(1, 0, 0, 0.4f);
            Gizmos.DrawCube(c, Vector3.one * s);
        }
        Gizmos.color = new Color(1, 1, 0, 0.5f);
        Gizmos.DrawCube(debugNode.pos, Vector3.one * s);

        Gizmos.color = new Color(1, 1, 0, 1f);
        for (int i = 1; i < pathNode.Count; i++)
            Gizmos.DrawLine(pathNode[i - 1] + new Vector3(0f,1f,0f), pathNode[i] + new Vector3(0f, 1f, 0f));
    }
#endif
    #endregion
}
