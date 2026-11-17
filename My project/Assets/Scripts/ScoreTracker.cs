using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public Transform jugador;
    public int puntaje;

    public AudioEffects audioEffects;
    public HudUI hudUI;
    public ObstacleSpawner obstacleSpawner;
    public PlayerManager playerController;

    private int siguienteBonus = 1000;
    private bool scoringEnabled;

    public void SetScoringEnabled(bool value)
    {
        scoringEnabled = value;
    }

    public void ResetScore()
    {
        puntaje = 0;
        siguienteBonus = 1000;
        if (hudUI != null)
        {
            hudUI.SetScore(puntaje);
        }
    }

    public void AddPuntos(int cantidad)
    {
        puntaje += cantidad;

        while (puntaje >= siguienteBonus)
        {
            puntaje += 50;
            siguienteBonus += 1000;
            if (audioEffects != null)
            {
                audioEffects.PlayBonus();
            }
        }

        if (hudUI != null)
        {
            hudUI.SetScore(puntaje);
        }
    }

    void Update()
    {
        if (!scoringEnabled || obstacleSpawner == null || jugador == null)
        {
            return;
        }

        for (int i = 0; i < obstacleSpawner.obstaculos.Count; i++)
        {
            EvaluarSaltoObstaculo(i);
        }
    }

    void EvaluarSaltoObstaculo(int index)
    {
        if (obstacleSpawner.FuePuntuado(index))
        {
            return;
        }

        GameObject obstaculo = obstacleSpawner.obstaculos[index];
        if (obstaculo.transform.position.x >= jugador.position.x)
        {
            return;
        }

        bool enSuelo = playerController == null || playerController.EnSuelo;
        bool saltoValido = !enSuelo &&
            jugador.position.y >= obstaculo.transform.position.y + 0.8f;

        if (!saltoValido)
        {
            if (obstaculo.transform.position.x < jugador.position.x - 1.5f)
            {
                obstacleSpawner.MarcarPuntuado(index);
            }

            return;
        }

        obstacleSpawner.MarcarPuntuado(index);

        ObstacleData data = obstaculo.GetComponent<ObstacleData>();
        int puntos = data != null ? data.puntos : 50;
        AddPuntos(puntos);

        if (audioEffects != null)
        {
            audioEffects.PlayClear(data);
        }
    }
}
