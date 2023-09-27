using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
    public Conversation conversation;

    public void TriggerConversation()
    {
        ConversationManager.Instance.StartConversation(conversation);
    }
}
