using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public float crystallizeTime = 2f;
    public float fractureTime = 1f;
    public Material triCrystallizeMaterial;
    public Material quadCrystallizeMaterial;
    public Material pentaCrystallizeMaterial;
    public Material hexaCrystallizeMaterial;
    public Material fractureMaterial;

    [SerializeField] private Vector2 axialCoordinates;

    private Vector3 _crystallizePos;
    private Vector3 _fracturePos;
    private Renderer _renderer;
    private GridManager _gridManager;
    private Dictionary<Ship.ShipType, Material> _crystallizeMaterials;

    void Awake()
    {
        axialCoordinates = GridManager.GetAxialCoordinates(transform.position);

        _renderer = GetComponent<Renderer>();

        _renderer.material = fractureMaterial;
        _renderer.material.DisableKeyword("_EMISSION");
        _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

        _gridManager = gameObject.GetComponentInParent<GridManager>();

        _crystallizeMaterials = new Dictionary<Ship.ShipType, Material>
        {
            {Ship.ShipType.Tri, triCrystallizeMaterial},
            {Ship.ShipType.Quad, quadCrystallizeMaterial},
            {Ship.ShipType.Penta, pentaCrystallizeMaterial},
            {Ship.ShipType.Hexa, hexaCrystallizeMaterial}
        };
    }
    
    void Start()
    {
        _fracturePos = transform.position;
        _crystallizePos = new Vector3(transform.position.x, 0.25f, transform.position.z);
    }

    private IEnumerator posEnumerator;
    private IEnumerator matEnumerator;

    public void Crystallize(Ship.ShipType shipType)
    {
        _renderer.material.EnableKeyword("_EMISSION");
        _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        StartCoroutine(_Change(crystallizeTime, _crystallizePos, _crystallizeMaterials[shipType], true));
    }

    public void Fracture()
    {
        StartCoroutine(_Change(fractureTime, _fracturePos, fractureMaterial, false));
    }

    private IEnumerator _Change(float time, Vector3 pos, Material mat, bool isCrystallize)
    {
        if(transform.position == pos)
        {
            yield break;
        }

        if(posEnumerator != null) StopCoroutine(posEnumerator);
        if(matEnumerator != null) StopCoroutine(matEnumerator);

        posEnumerator = SmoothLerpPos(time, transform.position, pos);
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
            _gridManager.AddCrystal(axialCoordinates);
        }
        else
        {
            _renderer.material.DisableKeyword("_EMISSION");
            _renderer.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;

            _gridManager.RemoveCrystal(axialCoordinates);
        }
    }


    private IEnumerator SmoothLerpPos(float time, Vector3 initialPos, Vector3 finalPos)
    {
        float elapsedTime = 0;
         
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(initialPos, finalPos, (elapsedTime / time));
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
