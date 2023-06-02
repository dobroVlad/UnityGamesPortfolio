using System.Collections;
using System.Linq;
using System.Net.WebSockets;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jump;
    [SerializeField][Range(0.5f, 3f)] private float _climbingSpeed;
    [SerializeField][Range(0.64f, 1f)] private float _crawlingSpeed;
    [SerializeField] private float _footRayLength;
    [SerializeField] private float _headRayLength;
    [SerializeField] private float _ladderRayLength;
    [SerializeField] private int _groundLayerNum;
    [SerializeField] private int _platformLayerNum;
    [SerializeField] private int _ladderLayerNum;
    [SerializeField] private int _enemyLayerNum;
    //Components of object
    private Rigidbody2D _myBody;
    private BoxCollider2D _myCollider;
    private Animator _myAnimator;
    private SpriteRenderer _mySprite;
    private CharacterHealth _myHealth;
    //State of object
    private HeroState _myState;
    private GameObject _onPlatform;
    public int MyState => (int)_myState;
    public GameObject OnPlatform => _onPlatform;
    private bool _talking;
    public bool Talking
    {  
        get { return _talking; }
        set
        {
            if (value) {
                ResetMove(); }
            _talking = value;
        }
    }
    //To change state of object
    private bool _goingLeft;
    private bool _goingRight;
    private bool _goingUp;
    private bool _goingDown;
    private bool _goingJump;
    private bool _crawling;
    private bool _inCloseSpace;
    private bool _nearLadder;
    private bool _ignoreLadder;
    private float _ignoreLadderTime = 0.6f;
    private bool _testGround;
    //Local parameters
    private Vector2 _directionLR;
    private Vector2 _directionUD;
    private enum HeroState
    {
        Grouned,
        Falling,
        Climbing
    }

    private void Awake()
    {
        _myBody = GetComponent<Rigidbody2D>();
        _myCollider = GetComponent<BoxCollider2D>();
        _myAnimator = GetComponent<Animator>();
        _mySprite = GetComponent<SpriteRenderer>();
        _myHealth = GetComponent<CharacterHealth>();
        _goingJump = false;
        _crawling = false;
        _ignoreLadder = false;
        _testGround = false;
        Talking = false;
    }
    private void FixedUpdate()
    {
        _directionLR = Vector2.zero;
        _directionUD = Vector2.zero;
        if (_crawling) { CheckForCeiling(); }
        if (!_ignoreLadder) { CheckForLadder(); }
        if (_myState != HeroState.Climbing) { CheckForGround(); }
        if (_crawling && _myState != HeroState.Grouned)
        {
            SwitchCrawling();
        }
        if (_goingLeft) { _directionLR += Vector2.left; }
        if (_goingRight) { _directionLR += Vector2.right; }
        if (_goingUp) { _directionUD += Vector2.up; }
        if (_goingDown) { _directionUD += Vector2.down; }
        if (_goingJump)
        {
            Jump(new Vector2(_directionLR.x * 0.1f, 1), true);
            _goingJump = false;
        }
        Movement(_directionLR, _directionUD);
        UpdateAnimationParams();
    }
    void Update()
    {
        if (_nearLadder && _myState == HeroState.Falling)
        {
            _myState = HeroState.Climbing;
            _myBody.gravityScale = 0;
            _myBody.velocity = Vector2.zero;
        }
        if (!Talking)
        {
            InputLeftRight();
            InputUpDown();

            if (Input.GetKeyDown(KeyCode.Space) && _myState != HeroState.Falling)
            {
                _goingJump = true;
                StartCoroutine(JumpFromLadder());
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (_myState == HeroState.Grouned && !_inCloseSpace) { SwitchCrawling(); };
            }
        }
    }
    void InputLeftRight()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _goingLeft = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _goingRight = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            _goingLeft = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _goingRight = false;
        }
    }
    void InputUpDown()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _goingUp = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            _goingDown = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            _goingUp = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _goingDown = false;
        }
    }
    public void ResetMove()
    {
        _goingLeft = false;
        _goingDown = false;
        _goingRight = false;
        _goingJump = false;
        _goingUp = false;
    }
    private void Movement(Vector2 directionLR, Vector2 directionUD)
    {
        if (_myState == HeroState.Grouned)
        {
            _myBody.AddForce(_crawling? directionLR * _speed* _crawlingSpeed : directionLR * _speed, ForceMode2D.Force);
        }
        else if (_myState == HeroState.Falling)
        {
            _myBody.AddForce(directionLR * _speed * 0.6f, ForceMode2D.Force);
        }
        else if (_myState == HeroState.Climbing)
        {
            if (_nearLadder)
            {
                _myBody.velocity = directionUD * _climbingSpeed;
            }
            else
            {
                StartCoroutine(JumpFromLadder());
                Jump(new Vector2(0, 0.5f),false);
            }
        }
        if (_myBody.velocity.x > 0.01f)
        {
            _mySprite.flipX = false;
        }
        else if (_myBody.velocity.x < -0.01f)
        {
            _mySprite.flipX = true;
        }
    }
    private void Jump(Vector2 direction, bool jumpAnimation)
    {
        if (!_inCloseSpace)
        {
            _myAnimator.SetBool(AnimationParameters.MainHero.boolJump.ToString(), jumpAnimation);
            _myBody.AddForce(direction * _jump, ForceMode2D.Impulse);
        }
    }
    private void CheckForGround()
    {
        var hits = Physics2D.BoxCastAll(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.min.y), new Vector2(_myCollider.bounds.size.x, 0.01f), 0f, Vector2.down, _footRayLength);
        if(hits.Count(p => p.collider.gameObject.layer == _groundLayerNum || p.collider.gameObject.layer == _platformLayerNum) != 0)
        {
            _myState = HeroState.Grouned;
        }
        else if (_myState == HeroState.Grouned &&!_testGround)
        {
            StartCoroutine(SwitchGrounded());
        }
        _onPlatform = _myState != HeroState.Grouned ? null : _onPlatform;
        if (hits.Count(p => p.collider.gameObject.layer == _enemyLayerNum && p.normal==Vector2.up)!=0)
        {
            _myBody.velocity = Vector2.zero;
            Jump(new Vector2(_myBody.velocity.x>0? 0.05f:-0.05f, 0.5f), true);
        }
    }
    private void CheckForLadder()
    {
        var hits = Physics2D.RaycastAll(_myCollider.bounds.center, Vector2.up, _ladderRayLength);
        _nearLadder = hits.Count(p => p.collider.gameObject.layer == _ladderLayerNum) != 0 ? true : false;
    }
    private void CheckForCeiling()
    {
        var hits = Physics2D.BoxCastAll(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.max.y), new Vector2(_myCollider.bounds.size.x, 0.01f), 0f, Vector2.up, _headRayLength);
        _inCloseSpace = hits.Count(p => p.collider.gameObject.layer == _groundLayerNum) != 0;
    }
    private IEnumerator SwitchGrounded()
    {
        _testGround = true;
        var timeCounter = 0f;
        var duration = _crawling? 0.12f: 0.06f;
        while (timeCounter < duration)
        {
            timeCounter += Time.deltaTime;
            yield return null;
        };
        var hits = Physics2D.BoxCastAll(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.min.y), new Vector2(_myCollider.bounds.size.x, 0.01f), 0f, Vector2.down, _footRayLength);
        _myState = hits.Count(p => p.collider.gameObject.layer == _groundLayerNum
                               || p.collider.gameObject.layer == _platformLayerNum) != 0 ? HeroState.Grouned : HeroState.Falling;
        _testGround = false;
    }
    private void SwitchCrawling()
    {
        Vector2 vector2 = !_crawling ? new Vector2(1f, 0.7f) : new Vector2(0.49f, 1.35f);
        Vector2 offset2 = !_crawling ? new Vector2(0f, -0.05f) : new Vector2(0f, 0f);
        _myCollider.size = vector2;
        _myCollider.offset = offset2;
        _crawling = !_crawling;
        _inCloseSpace = _crawling;
        _myHealth.UpdateColliderSize();
    }
    private IEnumerator JumpFromLadder()
    {
        if (_myState == HeroState.Climbing)
        {
            _myBody.gravityScale = 4;
            _ignoreLadder = true;
            _nearLadder = false;
            _myState = HeroState.Falling;
            var timeCounter = 0f;
            while (timeCounter < _ignoreLadderTime)
            {
                timeCounter += Time.deltaTime;
                yield return null;
            }
            _ignoreLadder = false;
        }
    }
    public void TakePunch(Vector3 vector)
    {
        if (!_crawling)
        {
            StartCoroutine(JumpFromLadder());
            Vector2 vector2 = new Vector2((transform.position - vector).normalized.x * 0.05f, 0.35f);
            _myBody.AddForce(vector2 * _jump, ForceMode2D.Impulse);
        }
    }
    private void SwitchJumpAnimation() //Using by animation in Unity to end itself.
    {
        _myAnimator.SetBool(AnimationParameters.MainHero.boolJump.ToString(), !_myAnimator.GetBool(AnimationParameters.MainHero.boolJump.ToString()));
    }
    private void UpdateAnimationParams()
    {
        _myAnimator.SetBool(AnimationParameters.MainHero.boolRun.ToString(), _myState == HeroState.Grouned && _directionLR.x != 0);
        _myAnimator.SetBool(AnimationParameters.MainHero.boolFall.ToString(), _myState == HeroState.Falling);
        _myAnimator.SetBool(AnimationParameters.MainHero.boolClimb.ToString(), _myState == HeroState.Climbing);
        _myAnimator.SetBool(AnimationParameters.MainHero.boolCrawl.ToString(), _crawling);
        if (_myState == HeroState.Climbing)
        {
            _myAnimator.SetFloat(AnimationParameters.MainHero.floatPlaySpeed.ToString(), _myBody.velocity.y > 0.01f ? 1 : (_myBody.velocity.y < -0.01f ? -1 : 0));
        }
        else if (_crawling)
        {
            _myAnimator.SetFloat(AnimationParameters.MainHero.floatPlaySpeed.ToString(), Mathf.Abs(_myBody.velocity.x) > 0.01f ? 1 : 0);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(_onPlatform==null&& collision.gameObject.layer == _platformLayerNum && _myState!=HeroState.Climbing)
        {
            _onPlatform = collision.gameObject;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_onPlatform == collision.gameObject)
        {
            _onPlatform = null; ;
        }
    }
}
