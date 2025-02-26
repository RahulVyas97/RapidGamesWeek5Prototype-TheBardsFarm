using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBoxHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            GameManager.Instance.IncreaseScore();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            GameManager.Instance.DecreaseScore();
        }
    }
}

