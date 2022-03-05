using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GridManager : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _territoryPrefab;
    [SerializeField] private NetworkPrefabRef _fractalBasePrefab;
    public static Transform fractalBaseTransform;
    public static Dictionary<Vector2Int, NetworkObject> gridCells = new Dictionary<Vector2Int, NetworkObject>();
    public static HashSet<Vector2> crystal = new HashSet<Vector2>();
    public static HashSet<Vector2> protectedCrystal = new HashSet<Vector2>();
    public static List<Vector2> shadowOutline;
    public static int highestCrystalSize = 0;
    
    public static GridManager currentGridManager;

    public int gridSize;

    private static Vector3 center = Vector3.zero;
    private static float hexSize = 5f;
    private static float width = 2 * hexSize;
    private static float height = Mathf.Sqrt(3) * hexSize;

    private Vector2[] GetNeighbors(Vector2 axialCoordinates)
    {
        return new Vector2[]
        {
            axialCoordinates + new Vector2(0, 1),
            axialCoordinates + new Vector2(-1, 1),
            axialCoordinates + new Vector2(-1, 0),
            axialCoordinates + new Vector2(1, 0),
            axialCoordinates + new Vector2(1, -1),
            axialCoordinates + new Vector2(0, -1)
        };
    }

    private void ComputeShadowOutline()
    {
        HashSet<Vector2> shadowOutlineSet = new HashSet<Vector2>();

        foreach(Vector2 cell in crystal)
        {
            foreach(Vector2 neighbor in GetNeighbors(cell))
            {
                if(!crystal.Contains(neighbor))
                {
                    shadowOutlineSet.Add(neighbor);
                }
            }
        }
        
        shadowOutline = new List<Vector2>(shadowOutlineSet);
    }

    // Using axial coordinates
    public static Vector3 GetCellCenter(Vector2 axialCoordinates)
    {
        float q = axialCoordinates.x;
        float r = axialCoordinates.y;
        
        return center + new Vector3(q * (0.75f * width), 0, (q * height / 2f) + (r * height));
    }

    public static Vector2 GetAxialCoordinates(Vector3 cellCenter)
    {
        Vector3 adjustedCenter = cellCenter - center;

        float q = adjustedCenter.x / (0.75f * width);
        float r = (adjustedCenter.z - (q * height / 2f)) / height;

        return new Vector2(q, r);
    }

    public override void Spawned()
    {
        currentGridManager = this;

        if (!Object.HasStateAuthority) return;

        int halfGridSize = gridSize / 2;

        for (int q = -halfGridSize; q < halfGridSize; q++)
        {
            for (int r = -halfGridSize; r < halfGridSize; r++)
            {
                Vector2 axialCoordinates = new Vector2(q, r);

                if (q == 0 && r == 0)
                {
                    NetworkObject tower = Runner.Spawn(_fractalBasePrefab, Vector3.zero, Quaternion.identity);
                    fractalBaseTransform = tower.transform;
                    fractalBaseTransform.parent = transform;
                    gridCells.Add(Vector2Int.RoundToInt(axialCoordinates), tower);

                    crystal.Add(axialCoordinates);
                    highestCrystalSize = crystal.Count;

                    ComputeShadowOutline();
                }
                else
                {
                    NetworkObject territoryCell = Runner.Spawn(_territoryPrefab, GetCellCenter(axialCoordinates), Quaternion.identity);
                    territoryCell.transform.parent = transform;
                    gridCells.Add(Vector2Int.RoundToInt(axialCoordinates), territoryCell);
                }
            }
        }
    }

    public NetworkObject PlaceTower(Vector2 axialCoordinates, NetworkPrefabRef towerPrefab)
    {   
        NetworkObject tower = Runner.Spawn(towerPrefab, GetCellCenter(axialCoordinates) + new Vector3(0, 0.5f, 0), Quaternion.identity);
        tower.transform.parent = transform;
        tower.gameObject.GetComponent<Tower>().axialCoordinates = axialCoordinates;

        protectedCrystal.Add(axialCoordinates);

        return tower;
    }

    public static void OnTowerDestroyed(Vector2 axialCoordinates)
    {
        // FIXME Convert data structures to us Vector2Int
        protectedCrystal.Remove(axialCoordinates);
    }

    public void AddCrystal(Vector2 axialCoordinates)
    {
        crystal.Add(axialCoordinates);

        if (crystal.Count > highestCrystalSize)
        {
            highestCrystalSize = crystal.Count;
        }

        ComputeShadowOutline();
    }

    public void RemoveCrystal(Vector2 axialCoordinates)
    {
        crystal.Remove(axialCoordinates);
        ComputeShadowOutline();
    }
        
    public override void FixedUpdateNetwork() {}

    void OnGUI ()
    {
        GUI.Label (new Rect (Screen.width - 100,0,100,50), "Crystal Size: " + crystal.Count);
    }
}
