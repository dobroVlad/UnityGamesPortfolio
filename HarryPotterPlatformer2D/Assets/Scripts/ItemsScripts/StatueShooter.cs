using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueShooter : MonoBehaviour
{
    [SerializeField] List<bool> _shotsQueue;
    [SerializeField] private float _timeOfShot;
    [SerializeField] private int _bulletDamage;
    [SerializeField] private Vector2 _shotDirection;
    [SerializeField] private float _shotForce;
    [SerializeField] [Range(0.8f, 5f)] private float _bulletSize;
    [SerializeField] private GameObject _bulletPrefab;
    private float _curentTime;
    private int _curentQueuePosition;
    private GameObject _myBullet;
    private BulletMovement _myBulletsLogic;
    void Start()
    {
        _curentTime = 0f;
        _curentQueuePosition = 0;

    }
    void Update()
    {
        if (_curentTime <= 0f)
        {
            if (_shotsQueue[_curentQueuePosition])
            {
                _myBullet = Instantiate(_bulletPrefab);
                _myBulletsLogic = _myBullet.GetComponent<BulletMovement>();
                _myBullet.transform.position = transform.position;
                _myBulletsLogic.Damage = _bulletDamage;
                _myBulletsLogic.Scale = _bulletSize;
                _myBulletsLogic.Fire(_shotForce, _shotDirection);
            }
            _curentQueuePosition += 1;
            if (_curentQueuePosition > _shotsQueue.Count - 1)
            {
                _curentQueuePosition = 0;
            }
            _curentTime = _timeOfShot;
        }
        _curentTime -= Time.deltaTime;
    }
}
