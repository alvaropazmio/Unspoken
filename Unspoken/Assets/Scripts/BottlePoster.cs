using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

    //To Do:
    //Picks a random Question
    //Displays question to the UI
    //On button pressed checks if input text is empty
    //If input text is not empty, posts a bottle passing the question and Answer

public class BottlePoster : MonoBehaviour
{
    [SerializeField]
    private GameObject gameManager;
    private QuestionsManager questionsManager;
    private BottleManager bottleManager;

    private string currentQuestion;
    private string currentAnswer;

    [SerializeField]
    private GameObject questionGameObject;
    private TMP_Text questionText;

    [SerializeField]
    private GameObject answerGameObject;
    private TMP_Text answerText;


    private void Awake()
    {
        questionsManager = gameManager.GetComponent<QuestionsManager>();
        bottleManager = gameManager.GetComponent<BottleManager>();

        questionText = questionGameObject.GetComponent<TMP_Text>();
        answerText = answerGameObject.GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {

        if (questionsManager != null)
        {
            currentQuestion = questionsManager.RandomQuestion();
            questionText.text = currentQuestion;
        }
        else
        {
            Debug.Log("Questions manager missing");
        }
    }

    public void NewPost()
    {
        currentAnswer = answerText.text;

        bottleManager.CreateNewBottle(currentQuestion, currentAnswer);
    }

    private void OnDisable()
    {
        currentQuestion = null;
        currentAnswer = null;
    }
}
