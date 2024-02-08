using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _frontWheelBody;
    [SerializeField]
    private Rigidbody2D _backWheelBody;
    [SerializeField]
    private Rigidbody2D _vehicleBody;
    [SerializeField]
    private float _wheelTorque = 200f;
    [SerializeField]
    private float _carTorque = 300f;
    private static bool _pedalGas { get;  set; }
    private static bool _pedalBreak { get; set; }
    private void Awake()
    {
        _pedalBreak = false;
        _pedalGas = false;
    }
    private void FixedUpdate()
    {
        float input = _pedalBreak ? -1 : _pedalGas ? 1 : 0;
            _frontWheelBody.AddTorque(-input * _wheelTorque * Time.fixedDeltaTime);
            _backWheelBody.AddTorque(-input *_wheelTorque * Time.fixedDeltaTime);
            _vehicleBody.AddTorque(input * _carTorque * Time.fixedDeltaTime);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetBreak(true);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            SetBreak(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetGas(true);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            SetGas(false);
        }
    }
    public static void SetGas(bool gas)
    {
        _pedalGas = gas;
    }
    public static void SetBreak(bool stop)
    {
        _pedalBreak = stop;
    }
}
