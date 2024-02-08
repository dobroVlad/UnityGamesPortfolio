using System.Linq;
using UnityEngine;

public class PickUpCubeBehaviour : MonoBehaviour
{
    [SerializeField] private BoxCollider _myCollider;
    private System.Action<PickUpCubeBehaviour> _addCubeCallback;
    private bool _active;
    private Vector3 _boxCastSize;
    public bool Active => _active;
    public Vector3 BoxSize => _myCollider.bounds.size;
    public Vector3 BoxCenter => _myCollider.bounds.center;

    private void Awake()
    {
        _active = false;
        _boxCastSize = new Vector3(_myCollider.bounds.extents.x*0.95f, _myCollider.bounds.extents.y*0.5f, 0.1f);
    }
    private void Update()
    {
        if (_active)
        {
            var hits = Physics.BoxCastAll(_myCollider.bounds.center+transform.forward* _myCollider.bounds.extents.z, _boxCastSize, transform.forward, transform.rotation,0.1f);
            if (hits.Count(p => p.collider.gameObject.layer == (int)CustomLayers.WallCube) > 0)
            {
                transform.parent = hits.First(p => p.collider.gameObject.layer == (int)CustomLayers.WallCube).collider.transform.parent;
                _active = false;
                _addCubeCallback = null;
                EventAggregator.Post(this, new HitWallEvent());
            }
            else
            {
                foreach (var pickUpCube in hits.Where(p => p.collider.gameObject.layer == (int)CustomLayers.PickUpCube))
                {
                    if (pickUpCube.collider.gameObject.TryGetComponent<PickUpCubeBehaviour>(out var behaviour) && !behaviour.Active)
                    {
                        _addCubeCallback?.Invoke(behaviour);
                    }
                }
            }
        }
    }
    public void Activate(System.Action<PickUpCubeBehaviour> addCubeCallback)
    {
        _active = true;
        _addCubeCallback = addCubeCallback;
    }
}
