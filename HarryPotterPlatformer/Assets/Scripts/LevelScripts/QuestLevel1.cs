using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AnimationParameters;
using static CharacterDialogue;

public class QuestLevel1 : MonoBehaviour
{
    [SerializeField] GameObject _questGiverPosition;
    [SerializeField] GameObject _questActionPosition;
    [SerializeField] private List<CharacterDialogue> _questDialogues;
    [SerializeField] private QuestCharactersAnimations _ghost;
    [SerializeField] private QuestCharactersAnimations _elf;
    [SerializeField] private GameObject _levelEnd;
    [SerializeField] private TextMeshProUGUI _karmaPoints;
    [SerializeField] private TextMeshProUGUI _swordHave;
    [SerializeField] private TextMeshProUGUI _swordCurse;
    [SerializeField] private Button _restart;
    [SerializeField] private PlayerMovement _hero;
    private int _karma;
    private Sword _swordState;
    private DialogueTrigger _ghostTrigger;
    private DialogueTrigger _elfTrigger;
    private QuestCharactersAnimations _ghostAnimation;
    private QuestCharactersAnimations _elfAmimation;
    private enum Sword
    {
        Unknown,
        Stolen,
        Returned,
        Uncursed
    };
    private enum CharactersEmotions
    {
        HappyElf,
        SadElf,
        HappyGhost,
        SadGrost
    }
    private enum Characters
    {
        Elf,
        Ghost
    }
    private int _stage;
    void Start()
    {
        _restart.onClick.AddListener(()=>Restart());
        _karma = 0;
        _stage = 0;
        _swordState = Sword.Unknown;
        _questDialogues[2].OurCharSentences[0].Accessible = false;
        _questDialogues[2].OurCharSentences[1].Accessible = false;
        _questDialogues[2].OurCharSentences[2].Accessible = false;
        _questDialogues[0].OurCharSentences[0].SetConsequences(() => UpdateQuest(0, 0, CharactersEmotions.HappyGhost));
        _questDialogues[0].OurCharSentences[3].SetConsequences(() => UpdateQuest(0, 0, CharactersEmotions.HappyGhost));
        _questDialogues[1].OurCharSentences[0].SetConsequences(() => UpdateQuest(1,0));
        _questDialogues[1].OurCharSentences[1].SetConsequences(() => UpdateQuest(0,1, CharactersEmotions.SadElf, _questDialogues[2].OurCharSentences[1], _questDialogues[2].OurCharSentences[3]));
        _questDialogues[1].OurCharSentences[4].SetConsequences(() => UpdateQuest(0,1, CharactersEmotions.SadElf, _questDialogues[2].OurCharSentences[0], _questDialogues[2].OurCharSentences[3]));
        _questDialogues[1].OurCharSentences[5].SetConsequences(() => UpdateQuest(1,1, CharactersEmotions.HappyElf, _questDialogues[2].OurCharSentences[1], _questDialogues[2].OurCharSentences[2], _questDialogues[2].OurCharSentences[3]));
        _questDialogues[2].OurCharSentences[0].SetConsequences(() => UpdateQuest(1, 2, CharactersEmotions.HappyGhost));
        _questDialogues[2].OurCharSentences[1].SetConsequences(() => UpdateQuest(0, 1, CharactersEmotions.SadGrost));
        _questDialogues[2].OurCharSentences[2].SetConsequences(() => UpdateQuest(1, 3));
        _questDialogues[2].OurCharSentences[3].SetConsequences(() => UpdateQuest(0, 0));
        _questDialogues[0].SetTheEndOfDialogue(()=>UpdateQuest(2));
        _questDialogues[1].SetTheEndOfDialogue(() => UpdateQuest(3));
        _questDialogues[2].SetTheEndOfDialogue(() => UpdateQuest(4));
        _ghostTrigger = _questGiverPosition.GetComponent<DialogueTrigger>();
        _elfTrigger = _questActionPosition.GetComponent<DialogueTrigger>();
        _elfTrigger.SetDialogue(_questDialogues[1]);
        _ghostTrigger.SetAction(() => CreateCharacter(Characters.Ghost));
        _elfTrigger.SetAction(() => CreateCharacter(Characters.Elf));
        NextStage(1);
    }
    void CreateCharacter(Characters character)
    {
        if (character == Characters.Ghost && _ghostAnimation == null)
        {
            _ghostAnimation = Instantiate(_ghost, _questGiverPosition.transform);        
        }
        else if (character == Characters.Elf && _elfAmimation == null)
        {
            _elfAmimation = Instantiate(_elf, _questActionPosition.transform);
        }
    }
    private void NextStage(int num)
    {
        if (num == 3)
        {
            _elfAmimation.UseDisappearAnimation();
           _questActionPosition.GetComponent<BoxCollider2D>().enabled= false;
        }
        if (num == 4)
        {
            _ghostAnimation.UseDisappearAnimation();
            _questGiverPosition.GetComponent<BoxCollider2D>().enabled = false;
            EndLevel();
        } 
        _stage= num;
        _ghostTrigger.SetDialogue(_questDialogues[_stage == 1 ? 0 : 2]);
    }
    private void UpdateQuest(int num)
    {
        if (!(_stage == 2 && _swordState == Sword.Unknown)) { NextStage(num); }
    }
    private void UpdateQuest(int carmaChange, int swordState, params PlayerSentences[] updateAccess)
    {
        _karma += carmaChange;
        _swordState = (Sword)swordState;
        foreach(var sentence in updateAccess)
        {
            sentence.Accessible = !sentence.Accessible;
        }
    }
    private void UpdateQuest(int carmaChange, int swordState, CharactersEmotions emo, params PlayerSentences[] updateAccess)
    {
        _karma += carmaChange;
        _swordState = (Sword)swordState;
        foreach (var sentence in updateAccess)
        {
            sentence.Accessible = !sentence.Accessible;
        }
        switch (emo)
        {
            case CharactersEmotions.HappyElf:
                _elfAmimation.UseHappyAnimation();
                return;
            case CharactersEmotions.SadElf:
                _elfAmimation.UseSadAnimation();
                return;
            case CharactersEmotions.HappyGhost:
                _ghostAnimation.UseHappyAnimation();
                return;
            case CharactersEmotions.SadGrost:
                _ghostAnimation.UseSadAnimation();
                return;
            default: return;
        }
    }
    private void EndLevel()
    {
        _hero.Talking = true;
        _karmaPoints.text = $"{_karma}/3";
        _swordHave.text = (int)_swordState == 2 ? "Sir Harold Vancee" : "House-elf Domenic";
        _swordCurse.text = (int)_swordState != 2 && _karma > 0 ? "Removed" : "Horrible";
        _levelEnd.SetActive(true);
    }
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
