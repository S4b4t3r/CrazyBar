using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBtn : MonoBehaviour
{
    [SerializeField] UIController uIController;
    void Update()
    {
        if (Input.GetButtonDown("P1_B1"))
        {
            uIController.RetryBtn();
        }
    }
}
