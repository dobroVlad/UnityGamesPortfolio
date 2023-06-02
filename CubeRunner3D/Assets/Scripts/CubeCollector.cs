using UnityEngine;

public class CubeCollector : MonoBehaviour
{
    [SerializeField] [Range(6,20)] private int _cubesNumberLimit;
    [SerializeField] private Transform _cubeHolder;
    [SerializeField] private CharacterBehaviour _character;
    [SerializeField] private ParticleSystem _addCubeParticles;
    [SerializeField] private CollectTextBehaviour _addCubeTextPrefab;
    void Start()
    {
        var childCubs = _cubeHolder.GetComponentsInChildren<PickUpCubeBehaviour>();
        foreach (var cube in childCubs)
        {
            cube.Activate((pickUp) => AddCube(pickUp));
        }
    }
    private void AddCube(PickUpCubeBehaviour cube)
    {
        if (_cubeHolder.childCount <= _cubesNumberLimit)
        {
            var highestObject = GetHighestCube();
            cube.transform.parent = _cubeHolder;
            cube.transform.position = highestObject.center + Vector3.up * (cube.BoxSize.y / 2 + highestObject.size.y / 2);
            _character.Jump(cube.transform.position);
            PlayAddCubeEffect(cube.transform.position);
            cube.Activate((pickUp) => AddCube(pickUp));
        }
        else
        {
            PoolManager.PutObject(cube.gameObject);
        }
    }
    private void PlayAddCubeEffect(Vector3 position)
    {
        var text = PoolManager.GetObject(_addCubeTextPrefab.gameObject).GetComponent<CollectTextBehaviour>();
        text.gameObject.SetActive(true);
        text.Activate(position);
        _addCubeParticles.transform.position = position;
        _addCubeParticles.Play();
    }
    private Bounds GetHighestCube()
    {
        var height = 0f;
        Bounds bounds = new Bounds();
        for (int i=0;i< _cubeHolder.childCount; i++)
        {
            var child = _cubeHolder.GetChild(i);
            if(child.TryGetComponent<Collider>(out var childCollider)&& childCollider.bounds.center.y>height)
            {
                height = childCollider.bounds.center.y;
                bounds = childCollider.bounds;
            }
        }
        return bounds;
    }
}
