using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private Transform _worldContainer;
    [SerializeField] [Range(1,10)] private int _startPlatfrmsNumber;
    [SerializeField] private float _firstPickupCubeDistance;
    [SerializeField] private float _lastPickupCubeDistance;
    [SerializeField] private float _pickUpsOffset;
    [SerializeField] private PlatformBehaviour _platformBlockPrefab;
    [SerializeField] private PickUpCubeBehaviour _pickUpCubePrefab;
    [SerializeField] private GameObject _wallCubePrefab;
    [SerializeField] private List<PlatformConfiguration> _wallVariants;
    private Queue<PlatformBehaviour> _createdPlatforms = new Queue<PlatformBehaviour>();
    private PlatformBehaviour _lastCreatedPlatform;
    private Vector3 _platformOffset;
    void Start()
    {
        for(int i = 0; i < _startPlatfrmsNumber; i++)
        {
         CreatePlatform();
        }
        _platformOffset = Vector3.down * 10;
    }
    private void CreatePlatform()
    {
        if (_createdPlatforms.Count > _startPlatfrmsNumber)
        {
            var firstPlatform = _createdPlatforms.Dequeue();
            firstPlatform.TakeApart();
        }
        var platform = PoolManager.GetObject(_platformBlockPrefab.gameObject);
        platform.transform.position = (_lastCreatedPlatform?.transform.position ?? Vector3.zero) + _platformOffset + Vector3.forward * (_lastCreatedPlatform?.Size.z?? 0f);
        platform.transform.parent = _worldContainer;
        platform.SetActive(true);
        var platformBehaviour = platform.GetComponent<PlatformBehaviour>();
        var  variant = _wallVariants[Random.Range(0, _wallVariants.Count)]; 
        for (int i = 0; i < variant.PickUpsCubs; i++)
        {
            var pickCube = PoolManager.GetObject(_pickUpCubePrefab.gameObject);
            pickCube.transform.position = platform.transform.position + 
                Vector3.forward * Mathf.Lerp(_firstPickupCubeDistance, _lastPickupCubeDistance, (i + 1f) / variant.PickUpsCubs) +
                Vector3.up * _pickUpCubePrefab.BoxCenter.y + Vector3.right * _pickUpsOffset * Random.Range(-1, 2);
            pickCube.transform.parent = platform.transform;
            pickCube.SetActive(true);
        }
        var wallStartPostion = platform.transform.position +
            Vector3.forward * platformBehaviour.Size.z - 
            Vector3.right * (PlatformConfiguration._maxWallCubesRow - 1)/2;
        for(int i = 0; i < variant.WallRows.Count; i++)
        {
            var cubesInRow = variant.WallRows[i];
            var offset = Random.Range(0, PlatformConfiguration._maxWallCubesRow - cubesInRow);
            for(int k=0;k< cubesInRow; k++)
            {
                var wallCube = PoolManager.GetObject(_wallCubePrefab);
                wallCube.transform.position = wallStartPostion + Vector3.up * i + Vector3.right * (k + offset);
                wallCube.transform.parent = platform.transform;
                wallCube.SetActive(true);
            }
        }
        _createdPlatforms.Enqueue(platformBehaviour);
        _lastCreatedPlatform = platformBehaviour;
        platformBehaviour.Activate(()=> CreatePlatform());
    }
}
[System.Serializable]
public class PlatformConfiguration
{
    [SerializeField] [Range(0, _maxWallCubesRow)] private List<int> _wallRowsCubesNumber;
    [SerializeField] private int _pickUpCubesNumber;
    public const int _maxWallCubesRow = 5;
    public List<int> WallRows => _wallRowsCubesNumber;
    public int PickUpsCubs => _pickUpCubesNumber;
}
