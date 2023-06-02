using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterDialogue;

[System.Serializable]
public class CharacterDialogue
{
    public string TalkingCharacterName;
    public List<Sentences> TalkingCharSentences;
    public string OurCharacterName;
    public List<PlayerSentences> OurCharSentences;
    private System.Action _endOfDialogue;

    [System.Serializable]
    public class Sentences
    {
        public string Sentence;
        public uint[] ResponsesIndexes;
    }
    [System.Serializable]
    public class PlayerSentences
    {
        private System.Action _consequences;
        public string Sentence;
        public uint ResponseIndex;
        private bool _accesible = true;
        public bool Accessible { get { return _accesible; } set { _accesible = value; } }
        public void SetConsequences(System.Action consequences)
        {
            _consequences = consequences;
        }
        public void Consequences()
        {
            _consequences?.Invoke();
        }
    }
    public void SetTheEndOfDialogue(System.Action endOfDialogue)
    {
        _endOfDialogue = endOfDialogue;
    }
    public void EndOfDialogue()
    {
        _endOfDialogue?.Invoke();
    }
}