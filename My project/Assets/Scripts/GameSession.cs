using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject menugameOver;

    public float velocidad = 2f;
    public GameObject col;
    public GameObject piedra1;
    public GameObject piedra2;
    public Renderer fondo;

    public bool finJuego;
    public bool inicio;

    public Transform jugador;
    public string nombreJugador = "Jugador";

    public TMP_InputField campoNombre;
    public TMP_FontAsset fuenteUI;
    public Image previewPersonaje;
    public TMP_Text textoPersonaje;
    public int indicePersonaje;

    public float minGapRocas = 5f;
    public float maxGapRocas = 8f;

    public StartMenuUI startMenuUI;
    public HudUI hudUI;
    public GroundScroller groundScroller;
    public BackgroundScroller backgroundScroller;
    public ObstacleSpawner obstacleSpawner;
    public ScoreTracker scoreTracker;
    public AudioEffects audioEffects;
    public PlayerAppearance playerAppearance;
    public PlayerManager playerManager;

    private bool gameOverHandled;

    void Start()
    {
        EnsurePlayerReference();
        EnsureComponents();
        ConfigureComponents();
        InitializeComponents();
        SetRunning(false);
    }

    void Update()
    {
        if (!inicio || !finJuego)
        {
            return;
        }

        HandleGameOver();

        if (Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void StartGame()
    {
        if (inicio)
        {
            return;
        }

        nombreJugador = startMenuUI != null ? startMenuUI.GetPlayerName() : GetPlayerNameFallback();
        indicePersonaje = startMenuUI != null ? startMenuUI.GetCharacterIndex() : indicePersonaje;

        if (playerAppearance != null)
        {
            playerAppearance.AplicarPersonaje(indicePersonaje);
        }

        inicio = true;
        finJuego = false;
        gameOverHandled = false;

        if (hudUI != null)
        {
            hudUI.SetPlayerName(nombreJugador);
            hudUI.ShowGameOver(false);
        }

        if (scoreTracker != null)
        {
            scoreTracker.ResetScore();
        }

        if (startMenuUI != null)
        {
            startMenuUI.SetMenuActive(false);
        }

        if (audioEffects != null)
        {
            audioEffects.StartAmbience();
        }

        SetRunning(true);
    }

    public void EndGame()
    {
        if (finJuego)
        {
            return;
        }

        finJuego = true;
        ReproducirSonidoGameOver();
    }

    public void ReproducirSonidoSalto()
    {
        if (audioEffects != null)
        {
            audioEffects.PlayJump();
        }
    }

    public void ReproducirSonidoGameOver()
    {
        if (audioEffects != null)
        {
            audioEffects.PlayGameOver();
        }
    }

    private void HandleGameOver()
    {
        if (gameOverHandled)
        {
            return;
        }

        gameOverHandled = true;
        SetRunning(false);

        if (hudUI != null)
        {
            hudUI.ShowGameOver(true);
        }

        if (audioEffects != null)
        {
            audioEffects.StopAmbience();
        }
    }

    private void SetRunning(bool value)
    {
        if (groundScroller != null)
        {
            groundScroller.SetRunning(value);
        }

        if (backgroundScroller != null)
        {
            backgroundScroller.SetRunning(value);
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetRunning(value);
        }

        if (scoreTracker != null)
        {
            scoreTracker.SetScoringEnabled(value);
        }
    }

    private void EnsurePlayerReference()
    {
        if (jugador != null)
        {
            return;
        }

        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            jugador = playerObj.transform;
        }
    }

    private void EnsureComponents()
    {
        startMenuUI = EnsureComponent(startMenuUI);
        hudUI = EnsureComponent(hudUI);
        groundScroller = EnsureComponent(groundScroller);
        backgroundScroller = EnsureComponent(backgroundScroller);
        obstacleSpawner = EnsureComponent(obstacleSpawner);
        scoreTracker = EnsureComponent(scoreTracker);
        audioEffects = EnsureComponent(audioEffects);

        if (jugador != null)
        {
            playerManager = jugador.GetComponent<PlayerManager>();
            playerAppearance = jugador.GetComponent<PlayerAppearance>();
            if (playerAppearance == null)
            {
                playerAppearance = jugador.gameObject.AddComponent<PlayerAppearance>();
            }
        }
    }

    private T EnsureComponent<T>(T component) where T : Component
    {
        if (component != null)
        {
            return component;
        }

        T existing = GetComponent<T>();
        if (existing != null)
        {
            return existing;
        }

        return gameObject.AddComponent<T>();
    }

    private void ConfigureComponents()
    {
        if (playerManager != null)
        {
            playerManager.gameSession = this;
            CopyAppearanceDefaults(playerManager);
        }

        if (startMenuUI != null)
        {
            startMenuUI.menuPrincipal = menuPrincipal;
            startMenuUI.campoNombre = campoNombre;
            startMenuUI.fuenteUI = fuenteUI;
            startMenuUI.previewPersonaje = previewPersonaje;
            startMenuUI.textoPersonaje = textoPersonaje;
            startMenuUI.indicePersonaje = indicePersonaje;
            startMenuUI.playerAppearance = playerAppearance;
            startMenuUI.gameSession = this;
        }

        if (hudUI != null)
        {
            hudUI.menugameOver = menugameOver;
            hudUI.fuenteUI = fuenteUI;
            hudUI.menuPrincipal = menuPrincipal;
            hudUI.nombreJugador = nombreJugador;
        }

        if (groundScroller != null)
        {
            groundScroller.velocidad = velocidad;
            groundScroller.col = col;
        }

        if (backgroundScroller != null)
        {
            backgroundScroller.velocidad = velocidad;
            backgroundScroller.fondo = fondo;
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.velocidad = velocidad;
            obstacleSpawner.piedra1 = piedra1;
            obstacleSpawner.piedra2 = piedra2;
            obstacleSpawner.minGapRocas = minGapRocas;
            obstacleSpawner.maxGapRocas = maxGapRocas;
        }

        if (scoreTracker != null)
        {
            scoreTracker.jugador = jugador;
            scoreTracker.audioEffects = audioEffects;
            scoreTracker.hudUI = hudUI;
            scoreTracker.obstacleSpawner = obstacleSpawner;
            scoreTracker.playerController = playerManager;
        }
    }

    private void InitializeComponents()
    {
        if (hudUI != null)
        {
            hudUI.Init();
        }

        if (startMenuUI != null)
        {
            startMenuUI.Init();
        }

        if (groundScroller != null)
        {
            groundScroller.Init();
        }

        if (obstacleSpawner != null)
        {
            obstacleSpawner.Init();
        }

        if (scoreTracker != null)
        {
            scoreTracker.ResetScore();
        }
    }

    private string GetPlayerNameFallback()
    {
        if (campoNombre != null && !string.IsNullOrWhiteSpace(campoNombre.text))
        {
            return campoNombre.text.Trim();
        }

        return "Jugador";
    }

    private void CopyAppearanceDefaults(PlayerManager source)
    {
        if (playerAppearance == null || source == null)
        {
            return;
        }

        playerAppearance.controladorCaballero = source.controladorCaballero;
        playerAppearance.controladorSerpiente = source.controladorSerpiente;
        playerAppearance.spriteCaballero = source.spriteCaballero;
        playerAppearance.spriteSerpiente = source.spriteSerpiente;
    }
}
