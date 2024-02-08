using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGameScreenBehaviour : MonoBehaviour
{
    [SerializeField] private Button _tryAgainButton;
    private List<TextMeshProUGUI> _texts;
    private Dictionary<Image,float> _images;

    private void Awake()
    {
        _texts = new List<TextMeshProUGUI>();
        _images = new Dictionary<Image, float>();
        _texts.AddRange(GetComponentsInChildren<TextMeshProUGUI>());
        var images = GetComponentsInChildren<Image>();
        for (int i = 0; i < _images.Count; i++)
        {
            _images.Add(images[i], images[i].color.a);
        }
        var scene = SceneManager.GetActiveScene();
        SetButtonAction(() => SceneManager.LoadScene(scene.name));
        EventAggregator.Subscribe<GameOverEvent>(ShowScreen);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameOverEvent>(ShowScreen);
    }
    public void SetButtonAction(System.Action action)
    {
        _tryAgainButton.onClick.RemoveAllListeners();
        _tryAgainButton.onClick.AddListener(()=>action.Invoke());
    }
    public void ShowScreen(object sender, GameOverEvent end)
    {
        gameObject.SetActive(true);
        StartCoroutine(ShowTryAgain());
    }
    private IEnumerator ShowTryAgain()
    {
        var timer = 0f;
        var duration = 0.3f;
       
        while (timer < duration)
        {
            float alpha = Mathf.Lerp(0.01f, 1f, timer / duration);
            foreach (var text in _texts)
            {
                text.alpha = alpha;
            }
            foreach(var image in _images)
            {
                var tempColor = image.Key.color;
                tempColor.a = alpha* image.Value;
                image.Key.color = tempColor;
            }
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
