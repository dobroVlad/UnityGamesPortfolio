using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody _myRigidBody;
    [SerializeField] private Collider _myCollider;
    [SerializeField] private Animator _myAnimator;
    private List<Collider> _ragdollColliders;
    private bool _alive;
    private void Awake()
    {
        _ragdollColliders = new List<Collider>();
        foreach (Transform child in transform)
        {
            _ragdollColliders.AddRange(child.GetComponentsInChildren<Collider>());
        }
       SetActiveRagdoll(false);
    }
    void Update()
    {
        if (_alive)
        {
            var hits = Physics.BoxCastAll(_myCollider.bounds.center, _myCollider.bounds.extents, Vector3.forward, Quaternion.Euler(0f, 0f, 0f), 0.15f);
            if (hits.Count(p => p.collider.gameObject.layer == (int)CustomLayers.Ground || p.collider.gameObject.layer == (int)CustomLayers.WallCube) > 0)
            {
                SetActiveRagdoll(true);
                EventAggregator.Post(this, new GameOverEvent());
                EventAggregator.Post(this, new HitWallEvent());
            }
        }
    }
    private void SetActiveRagdoll( bool active)
    {
        _alive = !active;
        foreach (var collirer in _ragdollColliders)
        {
            collirer.attachedRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            collirer.isTrigger = !active;
        }
       _myRigidBody.useGravity = !active;
       _myCollider.enabled = !active;
        if (!active)
        {
            _myAnimator.enabled = true; 
        }
        else
        {
            StartCoroutine(FallRagdoll());
        }
        foreach (var collirer in _ragdollColliders)
        {
            collirer.attachedRigidbody.constraints = RigidbodyConstraints.None;
        }
    }
    public void Jump(Vector3 position)
    {
        if (transform.position.y < position.y)
        {
            var up = position.y - transform.position.y -0.1f;
            _myRigidBody.MovePosition(transform.position+ Vector3.up * up);
            _myAnimator.SetTrigger(CharacterAnimatorParams.Jump.ToString());
        }
    }
    private IEnumerator FallRagdoll()
    {
        var timer = 0f;
        while (timer < 0.05f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _myAnimator.enabled = false;
    }
}
public enum CharacterAnimatorParams
{
    Jump
}
