using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private Button _myButton;
    private void Awake()
    {
        EventAggregator.Subscribe<GameFinishEvent>(ShowScreen);
        var scene = SceneManager.GetActiveScene();
        _myButton.onClick.RemoveAllListeners();
        _myButton.onClick.AddListener(() => AudioManager.Instance.PlaySfx(SfxType.Click));
        _myButton.onClick.AddListener(() => SceneManager.LoadScene(scene.name));
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameFinishEvent>(ShowScreen);
    }
    public void ShowScreen(object sender, GameFinishEvent end)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowNextLevel());
    }
    private IEnumerator ShowNextLevel()
    {
        var timer = 0f;
        var duration = 1f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        var rectTransform = GetComponent<RectTransform>();
        while (timer < duration)
        {
            rectTransform.offsetMax = new Vector2 (0, Mathf.Lerp(rectTransform.offsetMax.y, 0, timer/duration));
            rectTransform.offsetMin = new Vector2(0, Mathf.Lerp(rectTransform.offsetMin.y, 0, timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
