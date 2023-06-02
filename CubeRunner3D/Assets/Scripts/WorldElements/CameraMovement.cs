using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] GameObject _targetObject;
    [SerializeField][Range(0f, 0.5f)] float _shakeAmplitude; 
    private Vector3 _offset;
    private bool _shaking;
    private void Awake()
    {
        EventAggregator.Subscribe<HitWallEvent>(ShakeWave);
    }
    private void Start()
    {
        _shaking = false;
        _offset = _targetObject.transform.position - transform.position;
    }
    private void FixedUpdate()
    {
        var direction = _targetObject.transform.position - transform.position - _offset;
        transform.position += Vector3.forward * direction.z;
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<HitWallEvent>(ShakeWave);
    }
    public void ShakeWave(object sender, HitWallEvent hit)
    {
        if (!_shaking)
        {
            StartCoroutine(Shake());
        }
    }
    private IEnumerator Shake()
    {
        _shaking = true;
        Handheld.Vibrate();
        int count = 10;
        while (count > 0)
        {
            var koef = count == 10 ? -_shakeAmplitude / 2: count == 1 ? _shakeAmplitude / 2 : count % 2 == 0 ? -_shakeAmplitude : _shakeAmplitude;
            transform.position += transform.right * koef;
            count--;
            yield return null;
        }
        _shaking = false;
    }
}
