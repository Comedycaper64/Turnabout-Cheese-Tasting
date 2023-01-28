using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueRoute", fileName = "NewDialogueRoute")]
public class DialogueRoute : ScriptableObject
{
   public Conversation routeConversation;
   public string dialogueOption;

   public InventoryObject itemKey;

   /*

   public DialogueRoute(string dialogueOption, Conversation routeConversation)
   {
        this.dialogueOption = dialogueOption;
        this.routeConversation = routeConversation;
   }

   public Conversation GetRouteConversation()
   {
        return routeConversation;
   }

   public string GetDialogueOption()
   {
        return dialogueOption;
   }
   */
}
