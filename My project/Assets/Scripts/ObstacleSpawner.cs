using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public float velocidad = 5f;
    public GameObject piedra1;
    public GameObject piedra2;
    public float minGapRocas = 5f;
    public float maxGapRocas = 8f;
    public List<GameObject> obstaculos = new List<GameObject>();

    private bool running;
    private readonly List<bool> obstaculoPuntuado = new List<bool>();

    public IReadOnlyList<GameObject> Obstaculos => obstaculos;

    public void Init()
    {
        if (obstaculos.Count > 0)
        {
            return;
        }

        float gapInicial = Random.Range(minGapRocas, maxGapRocas);

        GameObject grande = Instantiate(piedra1, new Vector2(14f, -2f), Quaternion.identity);
        AsegurarObstacleData(grande, 950);
        obstaculos.Add(grande);
        obstaculoPuntuado.Add(false);

        GameObject pequena = Instantiate(piedra2, new Vector2(14f + gapInicial, -2f), Quaternion.identity);
        AsegurarObstacleData(pequena, 50);
        obstaculos.Add(pequena);
        obstaculoPuntuado.Add(false);
    }

    public void SetRunning(bool value)
    {
        running = value;
    }

    public bool FuePuntuado(int index)
    {
        return index >= 0 && index < obstaculoPuntuado.Count && obstaculoPuntuado[index];
    }

    public void MarcarPuntuado(int index)
    {
        if (index >= 0 && index < obstaculoPuntuado.Count)
        {
            obstaculoPuntuado[index] = true;
        }
    }

    void Update()
    {
        if (!running)
        {
            return;
        }

        for (int i = 0; i < obstaculos.Count; i++)
        {
            if (obstaculos[i].transform.position.x <= -10)
            {
                ReposicionarObstaculo(i);
            }

            obstaculos[i].transform.position = obstaculos[i].transform.position +
                new Vector3(-1, 0, 0) * Time.deltaTime * velocidad;
        }
    }

    void ReposicionarObstaculo(int index)
    {
        float maxX = float.MinValue;
        for (int i = 0; i < obstaculos.Count; i++)
        {
            if (i == index)
            {
                continue;
            }

            float x = obstaculos[i].transform.position.x;
            if (x > maxX)
            {
                maxX = x;
            }
        }

        if (maxX < 11f)
        {
            maxX = 11f;
        }

        float nuevoX = maxX + Random.Range(minGapRocas, maxGapRocas);
        obstaculos[index].transform.position = new Vector3(nuevoX, -2f, 0f);
        obstaculoPuntuado[index] = false;
    }

    void AsegurarObstacleData(GameObject obstaculo, int puntos)
    {
        if (obstaculo == null)
        {
            return;
        }

        obstaculo.tag = "Obstaculo";
        ObstacleData data = obstaculo.GetComponent<ObstacleData>();
        if (data == null)
        {
            data = obstaculo.AddComponent<ObstacleData>();
        }

        data.puntos = puntos;
    }
}
