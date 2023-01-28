using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float playerRotateSpeed;
    [SerializeField] private InputReader playerInput;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private CharacterController controller;

    private void Update() 
    {
        if (!DialogueManager.Instance.inConversation && !Journal.Instance.journalOpen)
        {
            Move();
            RotateCamera();
        }
    }

    private void Move()
    {
        controller.Move(CalculateMovement() * playerMoveSpeed * Time.deltaTime); 
    }

    private void RotateCamera()
    {
        cameraTarget.transform.eulerAngles += new Vector3(0, playerInput.RotateValue * playerRotateSpeed * Time.deltaTime, 0);
    }

    private Vector3 CalculateMovement()
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0f;
        right.Normalize();

        return forward * playerInput.MovementValue.y + right * playerInput.MovementValue.x;
    }
}
