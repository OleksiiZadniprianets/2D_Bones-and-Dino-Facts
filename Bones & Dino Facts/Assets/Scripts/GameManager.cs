using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public TMP_Text factText;
    public TMP_Text scoreText;

    public DinosaurData[] dinosaurs;
    public Image resultImage;
    public Image backgroundImage;
    public AudioSource musicSource;

    public int difficulty = 0; 
    public float resultDelay = 2.5f;

    public Color normalColor = new Color(0.24f, 0.36f, 0.58f, 1f);
    public Color correctColor = new Color(0.2f, 0.6f, 0.3f, 1f);
    public Color wrongColor = new Color(0.7f, 0.2f, 0.2f, 1f);

    private int currentIndex = 0;
    private int score = 0;
    private bool roundLocked = false;
    private List<int> questionOrder = new List<int>();
    private int totalScore = 0;

    void Start()
    {
        GenerateRandomOrder();
        currentIndex = 0;
        score = 0;
        roundLocked = false;

        if (resultImage != null)
        {
            resultImage.gameObject.SetActive(false);
            resultImage.rectTransform.localScale = Vector3.one;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.Play();
        }
        ShowFact();
        UpdateScore();
    }

    void ShowFact()
    {
        if (questionOrder.Count == 0) return;
        if (currentIndex < 0 || currentIndex >= questionOrder.Count) return;

        DinosaurData dino = dinosaurs[questionOrder[currentIndex]];

        if (difficulty == 0)
            factText.text = dino.factEasy;
        else if (difficulty == 1)
            factText.text = dino.factMedium;
        else
            factText.text = dino.factHard;
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score + " / " + dinosaurs.Length;
    }

    public void CheckAnswer(string droppedName)
    {
        if (roundLocked) return;
        if (questionOrder.Count == 0) return;
        if (currentIndex < 0 || currentIndex >= questionOrder.Count) return;

        roundLocked = true;

        DinosaurData correct = dinosaurs[questionOrder[currentIndex]];
        bool isCorrect = droppedName == correct.name;

        if (isCorrect)
        {
            score++;
            totalScore++;
        }

        ShowResult(correct, isCorrect);

        currentIndex++;

        Invoke(nameof(NextStep), resultDelay);
    }

    void ShowResult(DinosaurData correct, bool isCorrect)
    {
        if (isCorrect)
        {
            factText.text = "Correct!\n" + correct.name;
            if (backgroundImage != null) backgroundImage.color = correctColor;
        }
        else
        {
            factText.text = "Wrong!\nCorrect: " + correct.name;
            if (backgroundImage != null) backgroundImage.color = wrongColor;
        }

        if (resultImage != null)
        {
            resultImage.sprite = correct.real;
            resultImage.gameObject.SetActive(true);
            resultImage.rectTransform.localScale = new Vector3(1.2f, 1.2f, 1f);
        }

        UpdateScore();
    }

    void NextStep()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }

        if (resultImage != null)
        {
            resultImage.gameObject.SetActive(false);
            resultImage.rectTransform.localScale = Vector3.one;
        }

        roundLocked = false;

        if (currentIndex >= questionOrder.Count)
        {
            NextDifficulty();
            return;
        }

        ShowFact();
    }

    void NextDifficulty()
    {
        difficulty++;

        if (difficulty > 2)
        {
            int maxScore = dinosaurs.Length * 3;

            factText.text = "Game Finished!\nFinal Score: " + totalScore + "/" + maxScore;

            if (resultImage != null)
            {
                resultImage.gameObject.SetActive(false);
            }

            if (musicSource != null)
            {
                musicSource.Stop();
            }

            return;
        }

        factText.text = "Round Finished!\nNext Difficulty: " + GetDifficultyName();

        Invoke(nameof(StartNextRound), 2f);
    }

    void StartNextRound()
    {
        GenerateRandomOrder();
        currentIndex = 0;
        score = 0;
        roundLocked = false;

        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }

        if (resultImage != null)
        {
            resultImage.gameObject.SetActive(false);
            resultImage.rectTransform.localScale = Vector3.one;
        }

        ShowFact();
        UpdateScore();
    }

    string GetDifficultyName()
    {
        if (difficulty == 0) return "Easy";
        if (difficulty == 1) return "Medium";
        return "Hard";
    }

    void GenerateRandomOrder()
    {
        questionOrder.Clear();

        for (int i = 0; i < dinosaurs.Length; i++)
        {
            questionOrder.Add(i);
        }

        for (int i = 0; i < questionOrder.Count; i++)
        {
            int rand = Random.Range(i, questionOrder.Count);

            int temp = questionOrder[i];
            questionOrder[i] = questionOrder[rand];
            questionOrder[rand] = temp;
        }
    }
}