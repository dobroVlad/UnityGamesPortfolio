using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HideStarterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _myText; 
    private void Awake()
    {
        EventAggregator.Subscribe<GameStartEvent>(StartGame);
    }

    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameStartEvent>(StartGame);
    }
    private void StartGame(object sender, GameStartEvent start)
    {
        StartCoroutine(HideText());
    }
    IEnumerator HideText()
    {
        var timer = 0f;
        var duration = 0.3f;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(1, 0.01f, timer / duration);
            _myText.alpha = alpha;
            timer += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
