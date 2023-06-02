using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField] private string _hitAnimationParameter;
    [SerializeField] Rigidbody2D _myBody;
    [SerializeField] Animator _myAnimator;
    [SerializeField] Collider2D _myCollider;
    [SerializeField] CharacterDamage _damage;
    public float Scale { get; set; }
    public int Damage { get; set; }

    public void Fire(float force, Vector2 flyDirection)
    {
        StartCoroutine(SetScale());
        _damage.SetDamage(Damage);
        _myBody.AddForce(flyDirection.normalized * force, ForceMode2D.Impulse);
        transform.Rotate(0, 0, Vector2.SignedAngle(Vector2.left, flyDirection.normalized));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != gameObject.layer)
        {
            gameObject.layer = 0;
            _myCollider.isTrigger = true;
            _myBody.velocity = Vector2.zero;
            _myAnimator.SetBool(_hitAnimationParameter, true);
        }
    }
    private IEnumerator SetScale()
    {
        var time = 0f;
        var duration = 0.4f;
        Vector3 myScale = new Vector3(Scale, Scale, 1);
        while (time <= duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, myScale, time / duration);
            yield return null;
        }
    }
}
