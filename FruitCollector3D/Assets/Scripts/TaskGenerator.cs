using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskGenerator : MonoBehaviour
{
    [SerializeField][Range(1, 5)] private int _MinNumber;
    [SerializeField][Range(3, 10)] private int _MaxNumber;
    [SerializeField] private string _taskHeaderText;
    [SerializeField] private string _finalMessage;
    [SerializeField] TextMeshProUGUI _taskText;
    [SerializeField] private CollectTextBehaviour _collectionText;
    private FoodType _targetType;
    private int _targetNum;
    private int _curentNum;
    void Awake()
    {
        EventAggregator.Subscribe<GameStartEvent>(NewTask);
        EventAggregator.Subscribe<PackedFoodEvent>(UpdateTask);
    }

    private void OnDestroy()
    {
        EventAggregator.Unsubscribe<GameStartEvent> (NewTask);
        EventAggregator.Unsubscribe<PackedFoodEvent>(UpdateTask); ;
    }
    private void NewTask(object s, GameStartEvent start)
    {
        _taskText.gameObject.SetActive(true);
        _targetNum = Random.Range(Mathf.Min(_MinNumber,_MaxNumber), Mathf.Max(_MinNumber, _MaxNumber) + 1);
        _curentNum = _targetNum;
        var values = System.Enum.GetValues(typeof(FoodType));
        int random = Random.Range(0, values.Length);
        _targetType = (FoodType)values.GetValue(random);
        UpdateTaskText();
        EventAggregator.Post(this, new NewTaskEvent(_targetType));
    }
    private void UpdateTask(object s, PackedFoodEvent food)
    {
        _curentNum--;
        var collectionText = ObjectPoolManager.GetObject(_collectionText.gameObject);
        collectionText.SetActive(true);
        collectionText.transform.SetParent(transform);
        collectionText.GetComponent<CollectTextBehaviour>()
            .Activate(transform.position);
        if(_curentNum > 0 )
        {
         UpdateTaskText();            
        }
        else
        {
            _taskText.text = _finalMessage;
            AudioManager.Instance.PlaySfx(SfxType.LevelCompleted);
            EventAggregator.Post(this, new GameFinishEvent());
        }
    }
    private void UpdateTaskText()
    {
        var text = _taskHeaderText + $"\n{_curentNum} {_targetType}"+ (_curentNum>1?"s":"");
        _taskText.text = text ;
    }

}
