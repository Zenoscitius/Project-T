using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class ConversationManager : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI conversationText;
    public GameObject dialogueBox;
    public Image[] characterSprites;

    public static ConversationManager Instance { get; private set; }
    private Queue<Sentence> sentences;
    private Conversation conversation;


    private void Awake()
    {
        Instance = this;
    }
    

    void Start()
    {
        sentences = new Queue<Sentence>();
    }

    public void StartConversation (Conversation inputConversation)
    {
        conversation = inputConversation;
        if (conversation == null) throw new ArgumentException("Conversation cannot be null");
        dialogueBox.SetActive(true);
        for (int i = 0; i < characterSprites.Length; i++)
        {
            characterSprites[i].sprite = conversation.speakers[i].gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        
        sentences.Clear();

        foreach (Sentence sentence in conversation.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndConversation();
            return;
        }

        Sentence sentence = sentences.Dequeue();
        conversationText.text = sentence.lines;

        //Highlight only the speaking character
        for (int i = 0; i < characterSprites.Length; i++)
        {
            float alpha;
            if (i == (sentence.speakerNumber - 1))
            {
                alpha = 1.0f;
            }
            else
            {
                alpha = 0.25f;
            }

            Color tmp = characterSprites[i].color;
            tmp.a = alpha;
            characterSprites[i].color = tmp;
        }


        BaseUnit speaker = conversation.speakers[sentence.speakerNumber - 1];
        nameText.text = speaker.name;
    }

    void EndConversation()
    {
        dialogueBox.SetActive(false);
        conversation = null;
    }


}
