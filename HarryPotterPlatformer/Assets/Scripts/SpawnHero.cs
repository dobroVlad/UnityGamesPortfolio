using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHero : MonoBehaviour
{
    [SerializeField] GameObject _startPortal;
    [SerializeField] GameObject _spawnAnimationPrefab;
    [SerializeField] private Rigidbody2D _myBody;
    [SerializeField] private BoxCollider2D _myCollider;
    [SerializeField] private SpriteRenderer _mySprite;
    [SerializeField] private CharacterHealth _myHealth;
    [SerializeField] private PlayerMovement _myMoves;
    private Vector3 _checkPointPosition;
    private int _checkPointHP;
    private void Awake()
    {
        _checkPointHP = _myHealth.HealthPoints;
        _myBody.bodyType = RigidbodyType2D.Kinematic;
        _myCollider.enabled = false;
        _mySprite.enabled = false;
        _myMoves.enabled = false;
        _checkPointPosition = _startPortal.transform.position;
        StartCoroutine(Spawn());
    }
    public void UpdateCheckPoint(Vector3 position)
    {
        _checkPointPosition = position;
        _checkPointHP = _myHealth.HealthPoints;
    }
    public IEnumerator Spawn()
    {
        transform.position = _checkPointPosition;
        _myHealth.HealthPoints = _checkPointHP;
        var timer = 0f;
        var duration = 0.25f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        var spawnAnimation = Instantiate(_spawnAnimationPrefab);
        spawnAnimation.transform.position = transform.position;
        timer = 0f;
        duration = 0.75f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _myBody.bodyType = RigidbodyType2D.Dynamic;
        _myCollider.enabled = true;
        _mySprite.enabled = true;
        _myMoves.enabled = true;
        _myMoves.ResetMove();
        _myHealth.ShowHealthBar();
    }
}
