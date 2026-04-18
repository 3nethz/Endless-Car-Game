using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    [SerializeField]
    Transform gameModel;

    [SerializeField]
    MeshRenderer carMeshRenderer;

    [SerializeField]
    ExplodeHandler explodeHandler;

    //Max Values
    float maxSteerVelocity = 2;
    float maxForwardVelocity = 30;

    //Multipliers
    float accelerationMultiplier = 3;
    float brakesMultiplier = 15;
    float steerMultiplier = 5;

    //Exploded state
    bool isexploded = false;

    //Input
    Vector2 input = Vector2.zero;

    int _EmissionColor = Shader.PropertyToID("_EmissionColor");
    Color emisiveColor = Color.white;
    float emissiveColorMultiplier = 0f;

    void Start()
    {

    }

    void Update()
    {
        if (isexploded)
            return;
        //rotate car model when turning
        gameModel.transform.rotation = Quaternion.Euler(0, rb.linearVelocity.x * 5, 0);

        if (carMeshRenderer != null)
        {
            float desiredCarEmissiveColorMultiplier = 0f;

            if (input.y < 0)
                desiredCarEmissiveColorMultiplier = 4.0f;

            emissiveColorMultiplier = Mathf.Lerp(emissiveColorMultiplier, desiredCarEmissiveColorMultiplier, Time.deltaTime * 4);
            carMeshRenderer.material.SetColor(_EmissionColor, emisiveColor * emissiveColorMultiplier);
        }
    }

    private void FixedUpdate()
    {
        if (isexploded)
        {
            //Apply drag
            rb.linearDamping = rb.linearVelocity.z * 0.5f;
            rb.linearDamping = Mathf.Clamp(rb.linearDamping, 1.5f, 10);

            //Move towards the center after car is exploded
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), Time.deltaTime * 0.5f));

            return;
        }

        if (input.y > 0)
            Accelerate();
        else
            rb.linearDamping = 0.2f;

        //Brake
        if (input.y < 0)
            Brake();

        Steer();

        //Force the car not to go backwards
        if (rb.linearVelocity.z <= 0)
            rb.linearVelocity = Vector3.zero;
    }

    void Accelerate()
    {
        rb.linearDamping = 0.2f;

        //Stay within the speed limit
        if (rb.linearVelocity.z >= maxForwardVelocity)
            return;
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
            //Move the car sideways
            float speedBasedSteerLimit = rb.linearVelocity.z / 5.0f;
            speedBasedSteerLimit = Mathf.Clamp01(speedBasedSteerLimit);

            rb.AddForce(rb.transform.right * steerMultiplier * input.x * speedBasedSteerLimit);

            //Normalize the x velocity
            float normalizedX = rb.linearVelocity.x / maxSteerVelocity;

            //Ensure it does not get bigger than 1 in magnitude
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);

            //Make sure we stay within the turn speed limit
            rb.linearVelocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.linearVelocity.z);
            // Debug.Log(rb.linearVelocity.x);
        }
        else
        {
            //Auto center car
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0, 0, rb.linearVelocity.z), Time.fixedDeltaTime * 3);
            // Vector3 pos = rb.position;
            // pos.x = Mathf.Lerp(pos.x, 0, Time.fixedDeltaTime);
            // rb.MovePosition(pos);
            // Debug.Log("X velocity: " + rb.linearVelocity.x);
        }
    }

    public void setInput(Vector2 inputVector)
    {
        inputVector.Normalize();

        input = inputVector;
    }
    private Coroutine slowDownCoroutine;
    IEnumerator SlowDownTimeCO()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.5f);

        while (Time.timeScale <= 1.0f)
        {
            Time.timeScale += Time.deltaTime;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    //Events
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit {collision.collider.name}*");
        Vector3 velocity = rb.linearVelocity;
        explodeHandler.Explode(velocity * 45);

        isexploded = true;

        StartCoroutine(SlowDownTimeCO());
    }
}
