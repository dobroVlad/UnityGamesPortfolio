using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCharactersAnimations : MonoBehaviour
{
    [SerializeField] private Character _myRole;
    [SerializeField] private Animator _myAnimator;
    private enum Character
    {
        Elf,
        Ghost
    }
    public void UseHappyAnimation()
    {
        if (_myRole == Character.Elf)
        {
            _myAnimator.SetTrigger(AnimationParameters.ElfCharacter.triggerHappy.ToString());
        }
        else if (_myRole == Character.Ghost)
        {
            _myAnimator.SetTrigger(AnimationParameters.GhostCharacter.triggerHappy.ToString());
        }
    }
    public void UseSadAnimation()
    {
        if (_myRole == Character.Elf)
        {
            _myAnimator.SetTrigger(AnimationParameters.ElfCharacter.triggerSad.ToString());
        }
        else if (_myRole == Character.Ghost)
        {
            _myAnimator.SetTrigger(AnimationParameters.GhostCharacter.triggerSad.ToString());
        }
    }
    public void UseDisappearAnimation()
    {
        if (_myRole == Character.Elf) 
        {
            _myAnimator.SetTrigger(AnimationParameters.ElfCharacter.triggerDisappear.ToString());
        }
        else if (_myRole == Character.Ghost)
        {
            _myAnimator.SetTrigger(AnimationParameters.GhostCharacter.triggerDisappear.ToString());
        }
    }
}
