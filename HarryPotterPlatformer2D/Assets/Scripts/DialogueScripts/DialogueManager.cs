using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using static CharacterDialogue;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject _dialogueBoxContent;
    [SerializeField] GameObject _sentencePrefab;
    [SerializeField] GameObject _responsePrefab;
    [SerializeField] Animator _dialogueBoxAnimator;
    [SerializeField] string _showDialogueAnimationParameterName;
    [SerializeField] GameObject _hero;
    private CharacterDialogue _curentDialogue;
    private PlayerMovement _moves;
    private bool _active;
    void Start()
    {
        _active = false;
        _moves = _hero.GetComponent<PlayerMovement>();
    }
    public void StartDialogue(CharacterDialogue dialogue)
    {
        if (!_active)
        {
            _active = true;
            _curentDialogue = dialogue;
            _moves.Talking = true;
            StartCoroutine(WaitASec());
        }
    }
    private IEnumerator WaitASec()
    {
        var timer = 0f;
        var duration = 0.8f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _dialogueBoxAnimator.SetBool(_showDialogueAnimationParameterName, true);
        StartCoroutine(DisplayNextSentence(_curentDialogue.TalkingCharSentences[0]));
    }
    private IEnumerator DisplayNextSentence(Sentences charSentence)
    {
        foreach (Transform child in _dialogueBoxContent.transform)
        {
            Destroy(child.gameObject);
        }
        var sentence = Instantiate(_sentencePrefab,_dialogueBoxContent.transform);
        var textField = sentence.GetComponentInChildren<TextMeshProUGUI>();
        textField.text = $"    <b>{_curentDialogue.TalkingCharacterName}</b> : \r\n    ";
        foreach (char letter in charSentence.Sentence)
        {
            textField.text += letter;
            yield return null;
        }
        if (charSentence.ResponsesIndexes.Length > 0)
        {
            textField.text += $"\r\n    <b>{_curentDialogue.OurCharacterName}</b> :";
            var counter = 1;
            for (int i = 0; i < charSentence.ResponsesIndexes.Length; i++)
            {
                var response = _curentDialogue.OurCharSentences[(int)charSentence.ResponsesIndexes[i]];
                if (response.Accessible)
                {
                    var button = Instantiate(_responsePrefab, _dialogueBoxContent.transform);
                    var buttonBrain = button.GetComponentInChildren<DialogueResponseButton>();
                    buttonBrain.SetResponseText($"    {counter}) {response.Sentence}");
                    buttonBrain.SetResponseButtonClickCallback(() => OnDialogueResponseButtonClickHandler(response));
                    counter++;
                }
            }
        }
        else
        {
            var button = Instantiate(_responsePrefab, _dialogueBoxContent.transform);
            var buttonBrain = button.GetComponentInChildren<DialogueResponseButton>();
            buttonBrain.SetResponseText($"    [End of dialogue]");
            buttonBrain.SetResponseButtonClickCallback(() => OnDialogueResponseButtonClickHandler(null));
        }
    }
    private void OnDialogueResponseButtonClickHandler(PlayerSentences response)
    {
        if (_active&&response == null) 
        {
            _active = false;
            _moves.Talking = false;
            _dialogueBoxAnimator.SetBool(_showDialogueAnimationParameterName, false);
            _curentDialogue.EndOfDialogue();
            _curentDialogue = null;
        }
        else if(_active)
        {
            response.Consequences();
            StartCoroutine(DisplayNextSentence(_curentDialogue.TalkingCharSentences[(int)response.ResponseIndex]));
        }
    }
}
