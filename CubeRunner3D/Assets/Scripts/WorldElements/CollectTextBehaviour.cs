using System.Collections;
using TMPro;
using UnityEngine;

public class CollectTextBehaviour : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField]
    [Range(0, 1f)] private float _startAlfa;
    [SerializeField][Range(0f, 1f)] private float _startScale;
    [SerializeField] private float _flyTime;
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _disappearTime;
    private Vector3 _startLocalScale => new Vector3(_startScale, _startScale, _startScale);

    private void Update()
    {
        transform.LookAt(2*transform.position - Camera.main.transform.position);
    }
    public void Activate(Vector3 position)
    {
        transform.position = position;
        _text.alpha = _startAlfa;
        _text.rectTransform.localScale = _startLocalScale;
        StartCoroutine(Fly());
    }
    private IEnumerator Fly()
    {
        var timer = 0f;
        var endScale = new Vector3(1f, 1f, 1f);
        while (timer < _flyTime)
        {
            _text.rectTransform.localScale = Vector3.Lerp(_text.rectTransform.localScale, endScale, timer/ _disappearTime);
            _text.alpha = Mathf.Lerp(_startAlfa, 1f, timer / _disappearTime);
            transform.position +=Vector3.up*_flySpeed* Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Disappear());
    }
    private IEnumerator Disappear()
    {
        var timer = 0f;
        while (timer < _disappearTime)
        {
            transform.position += Vector3.up * _flySpeed*0.05f * Time.deltaTime;
            _text.alpha = Mathf.Lerp(1f, 0, timer / _disappearTime);
            timer += Time.deltaTime;
            yield return null;
        }
        PoolManager.PutObject(gameObject);
    }
}
