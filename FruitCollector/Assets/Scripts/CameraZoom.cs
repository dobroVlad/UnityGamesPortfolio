using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField][Range(40f, 60f)] private float _finalZoomAngle;
    [SerializeField]
    [Range(0f, 2f)] private float _zommingTime;
    [SerializeField] private Camera _camera;

    private void Awake()
    {
        EventAggregator.Subscribe<GameFinishEvent>(LevelCompleted);
    }
    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameFinishEvent>(LevelCompleted);
    }
    private void LevelCompleted(object sender, GameFinishEvent e)
    {
        StartCoroutine(FinalZoom());
    }
    IEnumerator FinalZoom()
    {
        Handheld.Vibrate();
        var time = 0f;
        while (time < _zommingTime)
        {
            _camera.fieldOfView= Mathf.Lerp(_camera.fieldOfView, _finalZoomAngle, time/_zommingTime);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
