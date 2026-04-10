using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler: MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    //Multipliers
    float accelerationMultiplier = 3;
    float brakesMultiplier = 15;
    float steerMultiplier = 5;

    //Input
    Vector2 input = Vector2.zero;
    
    private void FixedUpdate()
    {
        if (input.y > 0)
            Accelerate();
        else
            rb.linearDamping = 0.2f;

        //Brake
        if (input.y < 0)
            Brake();

        Steer();
    }

    void Accelerate()
    {
        rb.linearDamping = 0;

        rb.AddForce(rb.transform.forward * accelerationMultiplier * input.y);
    }

    void Brake()
    {
        //Don't Brake unless moving forward
        if (rb.linearVelocity.z <= 0)
            return;

        rb.AddForce(rb.transform.forward * brakesMultiplier * input.y);
    }

    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            rb.AddForce(rb.transform.right * steerMultiplier * input.x);
        }
    }

    public void setInput(Vector2 inputVector)
    {
        inputVector.Normalize();

        input = inputVector;
    }
}
