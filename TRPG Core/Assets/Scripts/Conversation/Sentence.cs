using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sentence
{
    public int speakerNumber;
    [TextArea(3, 10)]
    public string lines;
}
