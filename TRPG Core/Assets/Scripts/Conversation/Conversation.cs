using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Conversation
{
    public string name;
    public int conversationID;
    public BaseUnit[] speakers;
    public Sentence[] sentences;

}
