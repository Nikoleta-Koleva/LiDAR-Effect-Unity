using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

[RequireComponent(typeof(LineRenderer))]
public class Scanner : MonoBehaviour
{
    private InputAction _fire;
    private InputAction _changeRadius;
    private List<Vector3> _positionsList = new();
    private List<VisualEffect> _vfxList = new();
    private VisualEffect _currentVFX;
    private Texture2D _texture;
    private Color[] _positions;
    private bool _createNewVFX;
    private int _particleAmount; 
    private LineRenderer _lineRenderer;

    private const string TEXTURE_NAME = "PositionsTexture";
    private const string RESOLUTION_PARAMETER_NAME = "Resolution";

    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private VisualEffect _vfxPrefab;
    [SerializeField] private GameObject _vfxContainer;
    [SerializeField] private Transform _castPoint;
    [SerializeField] private float _radius = 10f;
    [SerializeField] private float _maxRadius = 10f;
    [SerializeField] private float _minRadius = 1f;
    [SerializeField] private int _pointsPerScan = 100;
    [SerializeField] private float _range = 10f;

    [SerializeField] private int resolution = 100;

    //Start is called before the first frame update
    private void Start()
    {
        // Get InputAction from PlayerInput
        _fire = playerInput.actions["Fire"];
        _changeRadius = playerInput.actions["Scroll"];
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _createNewVFX = true;
        CreateNewVisualEffect();
        ApplyPositions();
    }

    //Update is called once per frame
    private void FixedUpdate()
    {
        Scan();
    }

    //Raycasts are done based on the received 
    private void Scan()
    {
        // only call if button is pressed
        if (_fire.IsPressed())
        {
            for (int i = 0; i < _pointsPerScan; i++)
            {
                // generate random point
                Vector3 randomPoint = Random.insideUnitSphere * _radius;
                randomPoint += _castPoint.position;

                // calculate direction to random point
                Vector3 dir = (randomPoint - transform.position).normalized;

                // cast ray
                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _range, _layerMask))
                {
                    // only add point if the particle count limit is not reached
                    if (_positionsList.Count < resolution * resolution)
                    {
                        _positionsList.Add(hit.point);
                        _lineRenderer.enabled = true;
                        _lineRenderer.SetPositions(new[]
                        {
                            transform.position,
                            hit.point
                        });
                        _particleAmount++;
                    }
                    // create new VFX if the particle count limit is reached
                    else
                    {
                        _createNewVFX = true;
                        CreateNewVisualEffect();
                        break;
                    }
                } 
            } 
            // for loop
            ApplyPositions();
        } 
        // when stop pressing line renderer disables
        else
        {
            _lineRenderer.enabled = false;
        }
    }

    private void CreateNewVisualEffect()
    {
        // make sure it only gets called once
        if (!_createNewVFX) return;

        // add old VFX to list
        _vfxList.Add(_currentVFX);

        // create new VFX
        _currentVFX = Instantiate(_vfxPrefab, transform.position, Quaternion.identity, _vfxContainer.transform);
        _currentVFX.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution);

        // create texture
        _texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);

        // create color array for positions
        _positions = new Color[resolution * resolution];

        // clear list
        _positionsList.Clear();

        _particleAmount = 0;

        _createNewVFX = false;
    }

    private void ApplyPositions()
    {
        // create array from list
        Vector3[] pos = _positionsList.ToArray();

        // cache position for offset
        Vector3 vfxPos = _currentVFX.transform.position;

        // cache transform position
        Vector3 transformPos = transform.position;

        // cache some more stuff for faster access
        int loopLength = _texture.width * _texture.height;
        int posListLen = pos.Length;

        for (int i = 0; i < loopLength; i++)
        {
            Color data;

            if (i < posListLen - 1)
            {
                data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
            }
            else
            {
                data = new Color(0, 0, 0, 0);
            }
            _positions[i] = data;
        }

        // apply to texture
        _texture.SetPixels(_positions);
        _texture.Apply();

        // apply to VFX
        _currentVFX.SetTexture(TEXTURE_NAME, _texture);
        _currentVFX.Reinit();
    }
}