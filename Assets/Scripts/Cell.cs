using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Cell : NetworkBehaviour
{
    public float crystallizeTime = 2f;
    public float fractureTime = 1f;
    public Material triCrystallizeMaterial;
    public Material quadCrystallizeMaterial;
    public Material pentaCrystallizeMaterial;
    public Material hexaCrystallizeMaterial;
    public Material fractureMaterial;
    public Transform cellTransform;
    
    [Networked(OnChanged = nameof(Crystallize), OnChangedTargets = OnChangedTargets.All)]
    public Ship crystallizedBy { get; set; }

    [SerializeField] private Vector2 axialCoordinates;

    private Vector3 _crystallizePos;
    private Vector3 _fracturePos;
    private Renderer _renderer;
    private GridManager _gridManager;
    private Dictionary<Ship.ShipType, Material> _crystallizeMaterials;

    private void Awake()
    {
        _renderer = cellTransform.gameObject.GetComponent<Renderer>();

        _renderer.material = fractureMaterial;
        _renderer.material.DisableKeyword("_EMISSION");
        _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

        _crystallizeMaterials = new Dictionary<Ship.ShipType, Material>
        {
            {Ship.ShipType.Tri, triCrystallizeMaterial},
            {Ship.ShipType.Quad, quadCrystallizeMaterial},
            {Ship.ShipType.Penta, pentaCrystallizeMaterial},
            {Ship.ShipType.Hexa, hexaCrystallizeMaterial}
        };
    }

    private GridManager GetGridManager()
    {
        if (_gridManager == null)
        {
            _gridManager = gameObject.GetComponentInParent<GridManager>();
        }

        return _gridManager;
    }

    public Vector2 GetAxialCoordinates()
    {
        return axialCoordinates;
    }

    public bool IsProtected()
    {
        return GridManager.protectedCrystal.Contains(axialCoordinates);
    }

    public Ship.ShipType GetFaction()
    {
        if (crystallizedBy == null)
        {
            return Ship.ShipType.None;
        }

        return crystallizedBy.shipType;
    }

    public override void Spawned()
    {
        axialCoordinates = GridManager.GetAxialCoordinates(transform.position);

        _fracturePos = cellTransform.position;
        _crystallizePos = new Vector3(cellTransform.position.x, 0.25f, cellTransform.position.z);

        _Crystallize();
    }

    private IEnumerator posEnumerator;
    private IEnumerator matEnumerator;

    public static void Crystallize(Changed<Cell> changed)
    {
        changed.Behaviour._Crystallize();
    }

    private void _Crystallize()
    {
        if (crystallizedBy != null)
        {
            _renderer.material.EnableKeyword("_EMISSION");
            _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            StartCoroutine(_Change(crystallizeTime, _crystallizePos, _crystallizeMaterials[crystallizedBy.shipType], true));            
        }
        else
        {
            StartCoroutine(_Change(fractureTime, _fracturePos, fractureMaterial, false));

        }
    }


    private IEnumerator _Change(float time, Vector3 pos, Material mat, bool isCrystallize)
    {
        if(cellTransform.position == pos)
        {
            yield break;
        }

        if(posEnumerator != null) StopCoroutine(posEnumerator);
        if(matEnumerator != null) StopCoroutine(matEnumerator);

        posEnumerator = SmoothLerpPos(time, cellTransform.position, pos);
        matEnumerator = SmoothLerpMaterial(time, _renderer.material, mat);

        if (isCrystallize)
        {
            _renderer.material.EnableKeyword("_EMISSION");
            _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }

        Coroutine posCoroutine = StartCoroutine(posEnumerator);
        Coroutine matCoroutine = StartCoroutine(matEnumerator);

        yield return posCoroutine;
        yield return matCoroutine;

        if (isCrystallize)
        {
            GetGridManager().AddCrystal(axialCoordinates);
        }
        else
        {
            _renderer.material.DisableKeyword("_EMISSION");
            _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            GetGridManager().RemoveCrystal(axialCoordinates);
        }
    }


    private IEnumerator SmoothLerpPos(float time, Vector3 initialPos, Vector3 finalPos)
    {
        float elapsedTime = 0;
         
        while (elapsedTime < time)
        {
            cellTransform.position = Vector3.Lerp(initialPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SmoothLerpMaterial(float time, Material fromMat, Material toMat)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            _renderer.material.Lerp(fromMat, toMat, elapsedTime / time);
            _renderer.material.SetColor("_EmissionColor", Color.Lerp(
                fromMat.GetColor("_EmissionColor"),
                toMat.GetColor("_EmissionColor"),
                elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
