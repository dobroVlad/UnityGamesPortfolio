using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperSphere : MonoBehaviour
{
    [SerializeField] private SuperSphereSpecialization _specialization;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _liftingSpeed;
    [SerializeField] private float _liftAmplitude;
    [SerializeField] private float _disappearTime;
    [SerializeField] private ParticleSystem _pickUpLights;
    private Vector3 _defaultPos;
    private Vector3 _position;
    private float _curentLevitation;
    private void Start()
    {
        _curentLevitation = 0f;
        _defaultPos = transform.position;
        _specialization.SetDisappearCoroutine(() => StartCoroutine(Disappear()));
        _specialization.InitializeSphereMethod();
    }
    private void Update()
    {
        transform.Rotate(0f, _rotationSpeed, 0f);
        _position.Set(_defaultPos.x, GetCurentLevitationHeight(), _defaultPos.z);
        transform.position = _position;
    }
    private float GetCurentLevitationHeight()
    {
        var y = Mathf.Sin(_curentLevitation) * _liftAmplitude + _defaultPos.y;
        _curentLevitation += _liftingSpeed * Time.deltaTime;
        if (_curentLevitation > 2 * Mathf.PI) { _curentLevitation = 0f; }
        return y;
    }
    private void OnTriggerEnter(Collider other)
    {
        _specialization.UseSphereMethod(other);
    }
    private IEnumerator Disappear()
    {
        var timer = 0f;
        var scale = transform.localScale;
        _pickUpLights?.Play();
        while (timer< _disappearTime)
        {
            transform.localScale = Vector3.Lerp(scale, Vector3.zero, timer / _disappearTime);
            timer += Time.deltaTime;
            yield return null;
        }
        PoolManager.PutObject(gameObject);
    }
}
public abstract class SuperSphereSpecialization : MonoBehaviour
{
    protected bool _used;
    private void Start()
    {
        _used = false;
    }
    protected System.Action _disappearCoroutine;
    public void SetDisappearCoroutine(System.Action method)
    {
        _disappearCoroutine = method;
    }
    public abstract void InitializeSphereMethod();
    public abstract void UseSphereMethod(Collider other);
}
