using UnityEngine;

public enum CustomLayers
{
    WallCube = 6,
    PickUpCube = 7,
    Player =8,
    Ground=9,
}
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private Rigidbody _myRigidBody;
    [SerializeField] private ParticleSystem _warpEffect;
    private bool _active;
    private float _horizontal;

    private void Awake()
    {
        EventAggregator.Subscribe<GameStartEvent>(SetMovementActive);
        EventAggregator.Subscribe<GameOverEvent>(SetMovementActive);
        EventAggregator.Subscribe<JoystickInputEvent>(SetDirection);
    }
    void Start()
    {
        _active = false;
        _horizontal = 0f;
    }
    void FixedUpdate()
    {
        if (_active)
        {
            if (!_warpEffect.isPlaying) { _warpEffect.Play(); }
            var direction = _myRigidBody.position + _runSpeed * transform.forward *Time.fixedDeltaTime + _turnSpeed * transform.right * _horizontal;
            var clampX = Mathf.Clamp(direction.x, -2f, 2f);
            _myRigidBody.MovePosition(new Vector3(clampX,0f, direction.z));
        }
        else if (_warpEffect.isPlaying)
        { 
            _warpEffect.Stop(); 
        }
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameStartEvent>(SetMovementActive);
        EventAggregator.Unsubscribe<GameOverEvent>(SetMovementActive);
        EventAggregator.Unsubscribe<JoystickInputEvent>(SetDirection);
    }
    private void SetDirection(object sender, JoystickInputEvent input)
    {
        _horizontal = input.HorizontalAxis;
    }
    private void SetMovementActive(object sender, object myEvent)
    {
         _active = myEvent is GameStartEvent;
    }
}
