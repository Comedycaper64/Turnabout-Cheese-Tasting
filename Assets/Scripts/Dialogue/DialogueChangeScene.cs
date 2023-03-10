using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueChangeScene", fileName = "NewDialogueChangeScene")]
public class DialogueChangeScene : ConversationNode
{
    public AudioClip musicTrack;
    public Sprite sceneBackground;
    public bool fadeToBlack;
    public Color sceneFontColour;
    public int changeToScene;
}
