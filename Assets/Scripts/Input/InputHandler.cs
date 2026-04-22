using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    CarHandler carHandler;

    private void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        input.x = Keyboard.current.rightArrowKey.isPressed ? 1f :
                  Keyboard.current.leftArrowKey.isPressed ? -1f : 0f;

        input.y = Keyboard.current.upArrowKey.isPressed ? 1f :
                  Keyboard.current.downArrowKey.isPressed ? -1f : 0f;

        carHandler.setInput(input);

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

}