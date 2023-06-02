using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] private float _appearanceTime;
    [SerializeField] private BoxCollider _myGroundCollider;
    private System.Action _generateNextPlatform;
    public Vector3 Size => _myGroundCollider.bounds.size;

    public void Activate(System.Action generateNextPlatform)
    {
        _generateNextPlatform = generateNextPlatform;
        StartCoroutine(Appearance());
    }
    public void TakeApart()
    {
        var count = transform.childCount;
        for (int i = 0; i<count; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child.layer == (int)CustomLayers.WallCube || child.layer == (int)CustomLayers.PickUpCube)
            {
                PoolManager.PutObject(child);
                count = transform.childCount;
                i--;
            }
        }
        PoolManager.PutObject(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (int)CustomLayers.Player)
        {
            _generateNextPlatform?.Invoke();
        }
    }
    private IEnumerator Appearance()
    {
        var targetPosition = Vector3.zero + Vector3.forward * transform.position.z;
        var timer = 0f;
        while (timer < _appearanceTime)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, timer / _appearanceTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

}

