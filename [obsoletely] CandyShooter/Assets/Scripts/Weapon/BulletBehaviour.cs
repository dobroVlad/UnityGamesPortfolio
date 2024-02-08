using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;


public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private float _timeToDie = 4f;
    [SerializeField] private Transform _myModel;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private LayerMask _interactionLayers;
    private AttackInfo _info;
    private bool _used;
    private float _speed = 4f;
    void Start()
    {
        _used = false;
        _myModel.rotation = Random.rotation;
    }
    void FixedUpdate()
    {
        transform.position += _speed * transform.forward * Time.fixedDeltaTime;
        _myModel.Rotate(transform.forward, _rotationSpeed);
    }
    public void SetStartData(Vector3 position, float speed, AttackInfo info)
    {
        transform.position = position;
        _speed = speed;
        _info = info;
    }
    public void Activate()
    {
        _used = false;
        _myModel.rotation = Random.rotation;
        StartCoroutine(DelayedDestroy());
    }
    private void DestroyInner()
    {
        PoolManager.PutObject(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((1<<(other.gameObject.layer)& _interactionLayers.value)>0)
        {
            if(other.gameObject.TryGetComponent<CharacterBodyPart>(out var bodyPart)&&!_used)
            {
                _used = true;
                bodyPart.TakeDamage(_info);
            }
            DestroyInner();
        }
    }
    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(_timeToDie);
        DestroyInner();
    }
}
