using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationParameters 
{
    public enum MainHero
    {
        boolRun,
        boolJump,
        boolFall,
        boolClimb,
        floatPlaySpeed,
        boolCrawl
    }
    public enum BookMonster
    {
        boolTurn,
        floatSpeed
    }
    public enum GhostCharacter
    {
        triggerDisappear,
        triggerHappy,
        triggerSad
    }
    public enum ElfCharacter
    {
        triggerDisappear,
        triggerHappy,
        triggerSad
    }
    public enum SpawnPortal
    {
        triggerUsed
    }
    public enum ItemsActions
    {
        triggerUsedHealing
    }
}
