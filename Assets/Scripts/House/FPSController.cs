using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1;
    [SerializeField] private float _rotationSpeed = 1;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        ApplyMovement(vertical);

        ApplyRotation(horizontal);
    }

    private void ApplyRotation(float value)
    {
        float rotation = value * _rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
    }

    private void ApplyMovement(float vertical)
    {
        transform.Translate(Vector3.forward * Time.deltaTime * vertical * _moveSpeed);
    }
}
