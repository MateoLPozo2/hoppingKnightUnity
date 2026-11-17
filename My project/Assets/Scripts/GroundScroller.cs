using System.Collections.Generic;
using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float velocidad = 2f;
    public GameObject col;
    public List<GameObject> cols = new List<GameObject>();

    private bool running;

    public void Init()
    {
        if (col == null || cols.Count > 0)
        {
            return;
        }

        for (int i = 0; i < 21; i++)
        {
            cols.Add(Instantiate(col, new Vector2(-10 + i, -3), Quaternion.identity));
        }
    }

    public void SetRunning(bool value)
    {
        running = value;
    }

    void Update()
    {
        if (!running)
        {
            return;
        }

        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i].transform.position.x <= -10)
            {
                cols[i].transform.position = new Vector3(10, -3, 0);
            }

            cols[i].transform.position = cols[i].transform.position +
                new Vector3(-1, 0, 0) * Time.deltaTime * velocidad;
        }
    }
}
