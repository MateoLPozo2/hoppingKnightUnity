using UnityEngine;

public class AudioEffects : MonoBehaviour
{
    public AudioClip sonidoPiedraGrande;
    public AudioClip sonidoPiedraPequena;
    public AudioClip sonidoBonus;
    public AudioClip sonidoSalto;
    public AudioClip sonidoGameOver;
    public AudioClip sonidoAmbiente;

    private AudioSource audioSource;
    private AudioSource audioAmbiente;
    private bool ambienteIniciado;

    void Awake()
    {
        CargarSonidos();
        ConfigurarAudio();
    }

    public void PlayJump()
    {
        ReproducirClip(sonidoSalto);
    }

    public void PlayGameOver()
    {
        ReproducirClip(sonidoGameOver);
    }

    public void PlayBonus()
    {
        ReproducirClip(sonidoBonus);
    }

    public void PlayClear(ObstacleData data)
    {
        if (data != null && data.clearSfx != null)
        {
            ReproducirClip(data.clearSfx);
            return;
        }

        int puntos = data != null ? data.puntos : 50;
        ReproducirClip(puntos >= 100 ? sonidoPiedraGrande : sonidoPiedraPequena);
    }

    public void StartAmbience()
    {
        if (ambienteIniciado || audioAmbiente == null || sonidoAmbiente == null)
        {
            return;
        }

        audioAmbiente.clip = sonidoAmbiente;
        audioAmbiente.loop = true;
        audioAmbiente.volume = 0.35f;
        audioAmbiente.Play();
        ambienteIniciado = true;
    }

    public void StopAmbience()
    {
        if (audioAmbiente != null && audioAmbiente.isPlaying)
        {
            audioAmbiente.Stop();
        }
    }

    void ReproducirClip(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    void CargarSonidos()
    {
        if (sonidoPiedraGrande == null)
        {
            sonidoPiedraGrande = Resources.Load<AudioClip>("Audio/piedra_grande");
        }

        if (sonidoPiedraPequena == null)
        {
            sonidoPiedraPequena = Resources.Load<AudioClip>("Audio/piedra_pequena");
        }

        if (sonidoBonus == null)
        {
            sonidoBonus = Resources.Load<AudioClip>("Audio/bonus");
        }

        if (sonidoSalto == null)
        {
            sonidoSalto = Resources.Load<AudioClip>("Audio/salto");
        }

        if (sonidoGameOver == null)
        {
            sonidoGameOver = Resources.Load<AudioClip>("Audio/game_over");
        }

        if (sonidoAmbiente == null)
        {
            sonidoAmbiente = Resources.Load<AudioClip>("Audio/ambiente");
        }
    }

    void ConfigurarAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length > 1)
        {
            audioAmbiente = sources[1];
        }
        else
        {
            audioAmbiente = gameObject.AddComponent<AudioSource>();
        }

        audioAmbiente.playOnAwake = false;
        audioAmbiente.loop = true;
    }
}
