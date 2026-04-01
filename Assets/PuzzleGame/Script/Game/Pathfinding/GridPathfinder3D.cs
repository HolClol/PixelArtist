using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Node3D
{
    public Vector3 pos;
    public List<Node3D> neighbors = new List<Node3D>();
    public float g, h;
    public Node3D parent;
    public GameObject go;
    public float F => g + h;
}

public class GridPathfinder3D : MonoBehaviour
{
    public static GridPathfinder3D Instance;

    public List<Node3D> Node3Ds;
    private Dictionary<Vector3, Node3D> lookup;
    public float cellSize = 1f;

#if UNITY_EDITOR
    [Header("Debug Gizmos")]
    [Tooltip("Draw all nodes & edges")]
    public bool drawGraphGizmos = true;
#endif

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    // Building Graph (This should be called after a grid has been changed
    public void InitializeGraph(Transform visualCellParent)
    {
        // Grab every actual cell center
        var cellTransforms = visualCellParent.GetComponentsInChildren<Transform>(true).Where(t => t.GetComponent<Grid>() != null);

        Node3Ds = new List<Node3D>();
        lookup = new Dictionary<Vector3, Node3D>();

        // Make a Node3D for each
        foreach (var t in cellTransforms)
        {
            var n = new Node3D { pos = t.position };
            Node3Ds.Add(n);
            n.go = t.gameObject;
            lookup[t.position] = n;
            //t.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

        // Link only true orthogonal neighbors
        Vector3[] dirs = {
          Vector3.right * cellSize,
          Vector3.left  * cellSize,
          Vector3.forward * cellSize,
          Vector3.back    * cellSize
        };

        foreach (var n in Node3Ds)
        {
            // A small leeway for easier neighbours searching
            float eps = cellSize * 0.05f;  

            foreach (var d in dirs)
            {
                Vector3 want = n.pos + d;

                // Try exact first
                if (lookup.TryGetValue(want, out var nbr))
                {
                    n.neighbors.Add(nbr);
                    continue;
                }

                // Find the closest node within eps
                Node3D best = null;
                float bestD = eps * eps;
                foreach (var candidate in Node3Ds)
                {
                    float sq = (candidate.pos - want).sqrMagnitude;
                    if (sq < bestD)
                    {
                        bestD = sq;
                        best = candidate;
                    }
                }
                if (best != null)
                    n.neighbors.Add(best);
            }
        }
    }

    public List<Vector3> FindPath(Vector3 startWorld, Vector3 endWorld, Dictionary<Node3D, bool> overrideWalkMap = null)
    {
        Node3D start = NearestNode3D(startWorld);
        Node3D goal = NearestNode3D(endWorld);

        if (overrideWalkMap != null)
        {
            if (overrideWalkMap.ContainsKey(start)) overrideWalkMap[start] = true;
            if (overrideWalkMap.ContainsKey(goal)) overrideWalkMap[goal] = true;
        }

        foreach (var n in Node3Ds)
        {
            n.g = float.MaxValue;
            n.h = 0;
            n.parent = null;
        }

        var openSet = new List<Node3D> { start };
        var closedSet = new HashSet<Node3D>();

        start.g = 0;
        Node3D bestSoFar = start;
        float bestHeuristic = start.h;

        while (openSet.Count > 0)
        {
            Node3D current = openSet.OrderBy(n => n.F).First();
            openSet.Remove(current);
            closedSet.Add(current);

            if (current.h < bestHeuristic)
            {
                bestHeuristic = current.h;
                bestSoFar = current;
            }
            if (current == goal)
                return RetracePath(start, goal);

            foreach (var nbr in current.neighbors)
            {
                bool walkable = overrideWalkMap != null? overrideWalkMap[nbr] : true; 

                if (!walkable || closedSet.Contains(nbr))
                    continue;

                float tentativeG = current.g + Vector3.Distance(current.pos, nbr.pos);
                if (tentativeG < nbr.g)
                {
                    nbr.g = tentativeG;
                    nbr.h = Vector3.Distance(nbr.pos, goal.pos);
                    nbr.parent = current;

                    if (!openSet.Contains(nbr))
                        openSet.Add(nbr);
                }
            }
        }

        return RetracePath(start, bestSoFar);
    }

    private List<Vector3> RetracePath(Node3D start, Node3D end)
    {
        var path = new List<Vector3>();
        for (var n = end; n != null; n = n.parent)
            path.Add(n.pos);
        path.Reverse();
        return path;
    }

    public Node3D NearestNode3D(Vector3 w)
    { 
        Node3D best = null;
        float bestD = float.MaxValue;
        foreach (var n in Node3Ds)
        {
            float d = (n.pos - w).sqrMagnitude;
            if (d < bestD)
            {
                bestD = d;
                best = n;
            }
        }
        return best;
        
    }

    public Node3D NearestNode3D(Vector3 w, Dictionary<Node3D, bool> nodes)
    {
        Node3D best = null;
        float bestD = float.MaxValue;
        foreach (var n in Node3Ds)
        {
            float d = (n.pos - w).sqrMagnitude;
            if (d < bestD && nodes.ContainsKey(n) && nodes[n])
            {
                bestD = d;
                best = n;
            }
        }
        return best;
    }

    public Dictionary<Node3D, bool> BuildInflatedWalkability(List<EnumID> requiredIDs)
    {
        var raw = BuildDynamicWalkability(requiredIDs);
        var corridor = new Dictionary<Node3D, bool>();

        float cs = cellSize;
        float tol = 0.01f;
        float half = cs * 0.5f + tol;

        foreach (var n in Node3Ds)
        {
            bool ok = raw[n];
            if (ok)
            {
                for (int dx = -1; dx <= 1 && ok; dx++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        Vector3 sample = n.pos + new Vector3(dx * cs, 0, dz * cs);
                        var nbr = NearestNode3D(sample);
                        if ((nbr.pos - sample).sqrMagnitude > half * half || !raw[nbr])
                        {
                            ok = false;
                            break;
                        }
                    }
                }
            }
            corridor[n] = ok;
        }

        return corridor;
    }


    public Dictionary<Node3D, bool> BuildDynamicWalkability(List<EnumID> requiredID)
    {
        var map = Node3Ds.ToDictionary(n => n, n => true);

        foreach (var idObj in Object.FindObjectsOfType<IDAssign>())
        {
            var ids = idObj.GetComponent<IAssignID>().GetIDs();
            foreach (var id in requiredID)
            {
                if (ids.Contains(id)) continue;

                var col = FunctionManager.Instance.GetRootParent(idObj.gameObject, 1).GetComponentInChildren<Collider>();
                if (col != null)
                {
                    foreach (var n in Node3Ds)
                    {
                        Vector3 closest = col.ClosestPoint(n.pos);
                        if ((closest - n.pos).sqrMagnitude < 0.001f)
                        {
                            map[n] = false;
                        }
                    }
                }
            }
            
        }

        return map;
    }

    public Dictionary<Node3D, bool> PruneToReachable(Node3D start, Dictionary<Node3D, bool> walkMap)
    {
        var visited = new HashSet<Node3D>();
        var q = new Queue<Node3D>();

        if (walkMap.TryGetValue(start, out bool ok) && ok)
        {
            visited.Add(start);
            q.Enqueue(start);
        }

        while (q.Count > 0)
        {
            var n = q.Dequeue();
            foreach (var nbr in n.neighbors)
            {
                if (!visited.Contains(nbr)
                 && walkMap.TryGetValue(nbr, out bool w) && w)
                {
                    visited.Add(nbr);
                    q.Enqueue(nbr);
                }
            }
        }

        foreach (var kv in walkMap.ToList())
        {
            if (!visited.Contains(kv.Key))
                walkMap[kv.Key] = false;
        }

        return walkMap;
    }

    #region =========// DEBUG \\==========
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!drawGraphGizmos || lookup == null) return;

        Gizmos.color = Color.green;
        float s = cellSize * 0.5f;
        foreach (var node in lookup.Values)
        {
            Gizmos.DrawWireCube(node.pos, Vector3.one * s);
            foreach (var nbr in node.neighbors)
            {
                Gizmos.DrawLine(node.pos, nbr.pos);
            }
        }
    }
#endif
    #endregion
}
