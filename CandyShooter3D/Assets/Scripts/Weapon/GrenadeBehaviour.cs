using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrenadeBehaviour : MonoBehaviour
{
    [SerializeField] GrenadeType _type;
    [SerializeField] private LayerMask _interactionLayers;
    [SerializeField] ParticleSystem _mySparks;
    [SerializeField] ParticleSystem _myExplosion;
    [SerializeField] MeshRenderer _mySkin;
    [SerializeField] Rigidbody _myBody;
    [SerializeField] Collider _myCollider;
    [SerializeField] float _rorationAngle;
    [SerializeField] float _throwForce;
    private AttackInfo _info;
    private GrenadeData _data;
    private bool _rotateInAir;
    private Vector3 _rorateVector;
    void Start()
    {
        Restart();
        _data = WeaponManager.Instance.GetGrenadeData(_type);
    }
    void Update()
    {
        if (_rotateInAir)
        {
            transform.Rotate(_rorateVector, _rorationAngle);
        }
    }
    public void Restart()
    {
        _myBody.useGravity = false;
        _myCollider.enabled = false;
        _myBody.isKinematic = true;
        _rotateInAir = false;
        _mySkin.enabled = true;
        _mySparks.Stop();
        _rorateVector = Vector3.zero;
    }
    public void Activate(int playerId)
    {
        _mySparks?.Play();
        _info = _data.GiveAttackInfo(playerId);
        StartCoroutine(ExplosionCounter());
    }
    public void Throw(Vector3 target)
    {
        transform.parent = null;
        _myBody.useGravity = true;
        _myCollider.enabled = true;
        _myBody.isKinematic = false;
        _rotateInAir = true;
        var direction = (target - transform.position).normalized;
        direction = new Vector3(direction.x, direction.y + 0.5f, direction.z);
        _myBody.AddForce(direction*_throwForce, ForceMode.Impulse);
        var r = Random.rotation;
        _rorateVector = new Vector3(r.x, r.y, r.z);
    }
    private void Explosion()
    {
        var hits = Physics.OverlapSphere(transform.position, _data.DamageRange,_interactionLayers);
        foreach (var hit in hits)
        {
            if (hit.gameObject.TryGetComponent<CharacterBodyPart>(out var part))
            {
                var updatedAttackInfo = _info;
                var koef = Mathf.Lerp(0.4f, 2f, Mathf.Clamp(1 - (hit.transform.position - transform.position).magnitude / _data.DamageRange, 0f, 1f));
                updatedAttackInfo.MultiplyDamage(koef);
                part.TakeDamage(updatedAttackInfo);
            }
        }
    }
    private IEnumerator ExplosionCounter()
    {
        var timer = 0f;
        while (timer < _data.ExplosionDelay)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Explosion();
        _myExplosion?.Play();
        _mySkin.enabled = false;
        _mySparks.Stop();
        while (_myExplosion.isPlaying)
        {
            yield return null;
        }
        Restart();
        PoolManager.PutObject(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        _rotateInAir = false;
    }
}
