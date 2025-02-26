using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Playermovment _playerMovement;
    private void Awake()
    {
        _playerMovement = GetComponent<Playermovment>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < _playerMovement.animalList.Count; i++)
            {
                _playerMovement.animalList[i].ToggleAttractingState();
            }
        }
    }
}
