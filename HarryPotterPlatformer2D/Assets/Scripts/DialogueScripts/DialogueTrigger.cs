using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private int _heroLayerNum;
    private System.Action _myAction;
    private CharacterDialogue _curentDialogue;

    public void SetDialogue(CharacterDialogue dialogue)
    {
        _curentDialogue = dialogue;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_curentDialogue != null && collision.gameObject.layer == _heroLayerNum)
        {
            if (!collision.gameObject.GetComponent<PlayerMovement>().Talking)
            {
                _dialogueManager.StartDialogue(_curentDialogue);
                _myAction?.Invoke();
            }
        }
    }
    public void SetAction(System.Action act)
    {
        _myAction = act;
    }
}
