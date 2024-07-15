using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] UIController uIController;
    public int defaultScore;
    
    [SerializeField] TMP_Text scoreUI;
    [SerializeField] GameObject scoreUIClone;
    TMP_Text scoreUICloneText;

    [SerializeField] TMP_Text comboUI;
    [SerializeField] GameObject comboUIClone;
    TMP_Text comboUICloneText;

    [SerializeField] AudioSource comboSound;
    int combo = 1;
    int score;
    int lastValueAdded;
    GameObject lastProp;

    void Start()
    {
        scoreUICloneText = scoreUIClone.GetComponentInChildren<TMP_Text>();
        comboUICloneText = comboUIClone.GetComponentInChildren<TMP_Text>();
    }

    public void AddToScore(int value, bool comboing)
    {
        if(comboing)
        {
            comboSound.Play();
            comboSound.pitch = comboSound.pitch + 0.1f; 
            combo++;
            
            // Combo effect
            comboUIClone.SetActive(true);
            comboUICloneText.text = combo.ToString() + "x";
            
        } else {
            comboSound.pitch = 1f;
            combo = 1;
        }
        lastValueAdded = value + combo - 1;
        score += lastValueAdded;
        scoreUI.text = score.ToString();
        comboUI.text = combo.ToString() + "x";

        // Score effect
        scoreUIClone.SetActive(true);
        scoreUICloneText.text = scoreUI.text;
    }

    public GameObject GetLastProp()
    {
        return lastProp;
    }

    public void AddLastProp(GameObject newProp)
    {
        lastProp = newProp;
        uIController.SpawnTicker(lastProp.transform.position, lastValueAdded);
    }

    public int GetScore()
    {
        return score;
    }
}
