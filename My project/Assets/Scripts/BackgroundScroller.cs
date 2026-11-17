using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float velocidad = 2f;
    public Renderer fondo;

    private bool running;

    public void SetRunning(bool value)
    {
        running = value;
    }

    void Update()
    {
        if (!running || fondo == null)
        {
            return;
        }

        fondo.material.mainTextureOffset = fondo.material.mainTextureOffset +
            new Vector2(0.02f, 0) * Time.deltaTime;
    }
}
