using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlatformSwitcher : MonoBehaviour
{
    [SerializeField] private PlatformEffector2D _platformEffector;
    [SerializeField] private Collider2D _myCollider;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovement movement;
        if (collision.gameObject.TryGetComponent(out movement) && ((movement.OnPlatform!=null&& movement.OnPlatform != gameObject) || movement.MyState == 2))
        {
            _myCollider.isTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _myCollider.isTrigger = false;
    }
}
