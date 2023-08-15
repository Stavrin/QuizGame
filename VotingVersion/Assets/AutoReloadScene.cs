using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoReloadScene : MonoBehaviour
{
    private float timer = 0f;
    private float timeoutDuration = 70f; // 70 seconds

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            // Reset the timer if there's any user input
            timer = 0f;
        }
        else
        {
            // Increment the timer if no input
            timer += Time.deltaTime;

            if (timer >= timeoutDuration)
            {
                // Reload the scene if timeoutDuration is reached
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
