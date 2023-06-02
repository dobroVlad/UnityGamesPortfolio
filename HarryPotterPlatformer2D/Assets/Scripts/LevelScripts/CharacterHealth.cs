using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField][Range(1, 1000)] private int _healthPoints;
    [SerializeField] private bool _invulnerability;
    [SerializeField][Range(1, 10)] private float _invulnerabilityDuration;
    [SerializeField][Range(1, 100)] private int _topHitsResist;
    [SerializeField][Range(1, 100)] private int _leftHitsResist;
    [SerializeField][Range(1, 100)] private int _rightHitsResist;
    [SerializeField][Range(1, 100)] private int _downHitsResist;
    [SerializeField][Range(0.05f, 0.4f)] private float _rayCastLength;
    [SerializeField] private int _damageLayerNum;
    [SerializeField] [Range(1, 10)] private int _timesToBlink;
    [SerializeField] GameObject _dieAnimationPrefab;
    [SerializeField] private float _diePrefabScale;
    [SerializeField] SpriteRenderer _myRenderer;
    [SerializeField] Material _damageBlinkMaterial;
    [SerializeField] Material _defaultMaterial;
    [SerializeField] Collider2D _myCollider;
    [SerializeField] Rigidbody2D _myBody;
    [SerializeField] MonoBehaviour _myMoves;
    [SerializeField] Slider _mySlider;
    [SerializeField] TextMeshProUGUI _myHP;
    private Vector3 _boxCastSize;
    public int HealthPoints { get { return _healthPoints; } set {_healthPoints = value; } }
    public bool Invulnerability => _invulnerability;
    private void Awake()
    {
        UpdateColliderSize();
        _myHP.text = _healthPoints.ToString();
        _mySlider.maxValue = _healthPoints;
        _mySlider.value = _healthPoints;
    }
    void FixedUpdate()
    {
        CheckDamage();
        _myHP.text = _healthPoints.ToString();
        _mySlider.value = _healthPoints > _mySlider.maxValue ? _mySlider.maxValue : _healthPoints;

    }
    void CheckDamage()
    {
        var hits = Physics2D.BoxCastAll(_myCollider.bounds.center, _boxCastSize, 0f, Vector2.zero);
        foreach (var hit in hits.Where(p => p.collider.gameObject.layer == _damageLayerNum))
        {
            if (!_invulnerability)
            {
                CharacterDamage damage;
                hit.collider.gameObject.TryGetComponent(out damage);
                if (hit.normal == Vector2.left)
                { TakeADamage(hit.normal, (100 - _rightHitsResist) * damage.Left / 100); }
                else if (hit.normal == Vector2.right)
                { TakeADamage(hit.normal, (100 - _leftHitsResist) * damage.Right / 100); }
                else if (hit.normal == Vector2.down)
                { TakeADamage(hit.normal, (100 - _topHitsResist) * damage.Down / 100); }
                else if (hit.normal == Vector2.up)
                { TakeADamage(hit.normal, (100 - _downHitsResist) * damage.Top / 100); }
            }
        }
    }
    public void TakeADamage(Vector3 attackPosition, int damage)
    {
        if (damage > 0)
        {
            _invulnerability = true;
            _healthPoints -= damage;
            StartCoroutine(BlinkOnDamage());
            if (_myMoves is PlayerMovement)
            {
                ((PlayerMovement)_myMoves).TakePunch(attackPosition);
            }
            if (_healthPoints <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }
    public void ShowHealthBar()
    {
        _mySlider.enabled = true;
        _myHP.enabled = true;
    }
    private IEnumerator BlinkOnDamage()
    {
        var timeCounter = 0f;
        var switchTime = 0f;
        bool blink= true;
        while (timeCounter< _invulnerabilityDuration)
        {
            if (switchTime <= 0f)
            {
                _myRenderer.material = blink ? _damageBlinkMaterial : _defaultMaterial;
                blink = !blink;
                switchTime = _invulnerabilityDuration / (2 * _timesToBlink);
            }
            switchTime -= Time.deltaTime;
            timeCounter += Time.deltaTime;
            yield return null;
        }
        _myRenderer.material = _defaultMaterial;
        if (_healthPoints > 0) { _invulnerability = false; }
    }
    private IEnumerator Die()
    {
        var timeCounter = 0f;
        _myCollider.enabled = false;
        _myBody.velocity = Vector2.zero;
        Vector3 pos = transform.position;
        _mySlider.enabled = false;
        _myHP.enabled = false;
        if (!(_myMoves is PlayerMovement))
        {
            float randomDirection = Random.Range(-1, 1);
            while (timeCounter < 0.6f)
            {
                float y = pos.y + 2f - Mathf.Pow(2 - 5f * timeCounter, 2f) / 2;
                transform.position = new Vector2(pos.x + 5 * timeCounter * randomDirection, y);
                timeCounter += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            _myMoves.enabled = false;
            _myCollider.enabled = false;
            _myBody.bodyType = RigidbodyType2D.Kinematic;
            GetComponent<Animator>().SetBool("boolFall",true);
            Camera.main.GetComponent<CameraMovement>().enabled = false;
            Camera.main.transform.parent = null;
            while (timeCounter < 1f)
            {
                transform.Rotate(0,0,30f);
                float y = pos.y + 6f* timeCounter;
                transform.position = new Vector2(pos.x, y);
                timeCounter += Time.deltaTime;
                yield return null;
            }
        }
        var dieAnimation = Instantiate(_dieAnimationPrefab);
        dieAnimation.transform.position = transform.position;
        dieAnimation.transform.localScale *= _diePrefabScale;
        _myRenderer.enabled = false;
        if (_myMoves is PlayerMovement)
        {
            timeCounter = 0f;
            while (timeCounter < 0.8f)
            {
                timeCounter += Time.deltaTime;
                yield return null;
            }
            _invulnerability = false;
            StartCoroutine(GetComponent<SpawnHero>().Spawn());
            Camera.main.transform.parent = transform;
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            Camera.main.GetComponent<CameraMovement>().enabled = true;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Camera.main.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            timeCounter = 0f;
            while (timeCounter < 0.1f)
            {
                timeCounter += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
    public void UpdateColliderSize()
    {
        _boxCastSize = new Vector3(_myCollider.bounds.size.x + _rayCastLength, _myCollider.bounds.size.y + _rayCastLength, 0);
    }
}
