using Assets.Scripts;
using UnityEngine;

public class CharacterMoods : MonoBehaviour
{
    public CharacterName Name;

    public Sprite Idle;
    public Sprite Idle_2;
    public Sprite BitHappy;
    public Sprite Happy;
    public Sprite Uncomfortable;
    public Sprite Surprised;
    public Sprite Sad;
    public Sprite Upset;

    //Idle, Idle_2, BitHappy, Happy, Uncomfortable, Surprised, Sad, Upset

    public Sprite GetMoodSprite(CharacterMood mood)
    {
        switch (mood)
        {
            case CharacterMood.Idle:
                return Idle;
            case CharacterMood.Idle_2:
                return Idle_2 ?? Idle;
            case CharacterMood.BitHappy:
                return BitHappy ?? Idle;
            case CharacterMood.Happy:
                return Happy ?? Idle;
            case CharacterMood.Uncomfortable:
                return Uncomfortable ?? Idle;
            case CharacterMood.Surprised:
                return Surprised ?? Idle;
            case CharacterMood.Sad:
                return Sad ?? Idle;
            case CharacterMood.Upset:
                return Upset ?? Idle;
            default:
                Debug.Log($"Didn't find Sprite for character: {Name}, mood: {mood}");
                return Idle;
        }
    }
}
