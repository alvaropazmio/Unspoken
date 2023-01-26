using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionsManager : MonoBehaviour
{
    [SerializeField]
    TextAsset file;

    public List<string> questions = new List<string>();

    private void Start()
    {
        if (file != null)
        {
            ReadFile();
        }
    }


    private void ReadFile()
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var Lines = file.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < Lines.Length; i++)
        {
            questions.Add(Lines[i]);
        }
    }

    public string RandomQuestion()
    {
        int randomIndex = Random.Range(0, questions.Count);
        return questions[randomIndex];
    }
}
