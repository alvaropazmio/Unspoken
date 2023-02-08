/////////////////////////////////////////////////////////////////////
//Purpose: Manager in charge of handling questions and answers that//
//will be charged in every bottle                                  //
//Developer: Alvaro Pazmiño                                        //
/////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BottlePoster : MonoBehaviour
{
    private QuestionsManager questionsManager;
    private BottleManager bottleManager;

    [SerializeField]
    private GameObject gameManager;
    [SerializeField]
    private GameObject questionGameObject;
    [SerializeField]
    private GameObject cancelMessage;

    [SerializeField]
    private TMP_InputField answerInput;
    private TMP_Text questionText;

    private string currentQuestion;
    private string currentAnswer;

    private void Awake()
    {
        questionsManager = gameManager.GetComponent<QuestionsManager>();
        bottleManager = gameManager.GetComponent<BottleManager>();

        questionText = questionGameObject.GetComponent<TMP_Text>();
    }

    public void NewPost()
    {
        currentQuestion = questionsManager.RandomQuestion(false);
        questionText.text = currentQuestion;

        bottleManager.CreateNewBottle(currentQuestion);
    }

    public void PostAnswer()
    {
        if (answerInput.text == "")
        {
            CancelPost();
        }
        else
        {
            currentAnswer = answerInput.text.ToString();
            bottleManager.PostBottle(currentAnswer);
            answerInput.text = "";
        }
    }

    public void CancelPost()
    {
        currentQuestion = null;
        currentAnswer = null;
        bottleManager.DestroyBottle();
        answerInput.text = "";
        cancelMessage.SetActive(true);
    }


    private void OnDisable()
    {
        currentQuestion = null;
        currentAnswer = null;
    }
}
