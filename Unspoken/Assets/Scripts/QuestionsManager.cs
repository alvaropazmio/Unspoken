/////////////////////////////////////////////////////////////////////
//Purpose: Manager which reads and Lists questions from a text file//
//temporarilly houses the MockUp answers as well                   //
//Developer: Alvaro Pazmi�o                                        //
/////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionsManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset QuestionsFile;
    [SerializeField]
    private TextAsset MU_AnswersFile;

    public List<string> questions = new List<string>();
    public List<string> mu_Answers = new List<string>();

    private Dictionary<string, List<string>> fileToList = new Dictionary<string, List<string>>();

    private void Awake()
    {
        if (QuestionsFile != null)
        {
            fileToList.Add(QuestionsFile.name, questions);
            ReadFile(QuestionsFile);
        }
        else return;

        if (MU_AnswersFile != null)
        {
            fileToList.Add(MU_AnswersFile.name, mu_Answers);
            ReadFile(MU_AnswersFile);
        }
        else return;
    }


    private void ReadFile(TextAsset file)
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var Lines = file.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < Lines.Length; i++)
        {
            fileToList[file.name].Add(Lines[i]);
        }
    }

    public string RandomQuestion(bool mockup)
    {

        if (questions.Count != 0)
        {
            int randomIndex = Random.Range(0, questions.Count);
            if (mockup)
            {
                return questions[randomIndex] + " + " + mu_Answers[randomIndex];
            }
            return questions[randomIndex];
        }
        else return null;
    }
}
