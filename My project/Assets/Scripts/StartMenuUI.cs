using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public GameObject menuPrincipal;
    public TMP_InputField campoNombre;
    public TMP_FontAsset fuenteUI;
    public Image previewPersonaje;
    public TMP_Text textoPersonaje;
    public int indicePersonaje;

    public PlayerAppearance playerAppearance;
    public GameSession gameSession;

    private bool menuActive = true;

    public void Init()
    {
        ConfigurarCampoNombre();
        if (menuPrincipal != null)
        {
            CrearSelectorPersonaje(menuPrincipal.transform);
        }

        ActualizarPreviewPersonaje();
    }

    public void SetMenuActive(bool value)
    {
        menuActive = value;
        if (menuPrincipal != null)
        {
            menuPrincipal.SetActive(value);
        }
    }

    public string GetPlayerName()
    {
        if (campoNombre != null && !string.IsNullOrWhiteSpace(campoNombre.text))
        {
            return campoNombre.text.Trim();
        }

        return "Jugador";
    }

    public int GetCharacterIndex()
    {
        return indicePersonaje;
    }

    void Update()
    {
        if (!menuActive)
        {
            return;
        }

        bool nombreEnfocado = campoNombre != null && campoNombre.isFocused;

        if (!nombreEnfocado)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CambiarPersonaje(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CambiarPersonaje(1);
            }
        }

        bool enter = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
        bool xLibre = Input.GetKeyDown(KeyCode.X) && !nombreEnfocado;

        if ((enter || xLibre) && gameSession != null)
        {
            gameSession.StartGame();
        }
    }

    void CambiarPersonaje(int direccion)
    {
        indicePersonaje = (indicePersonaje + direccion + 2) % 2;
        ActualizarPreviewPersonaje();
    }

    void ActualizarPreviewPersonaje()
    {
        if (playerAppearance == null)
        {
            return;
        }

        Sprite preview = playerAppearance.ObtenerSpritePreview(indicePersonaje);
        if (previewPersonaje != null && preview != null)
        {
            previewPersonaje.sprite = preview;
            previewPersonaje.preserveAspect = true;
        }

        if (textoPersonaje != null)
        {
            textoPersonaje.text = indicePersonaje == 0
                ? "> Knight <   Snake"
                : "Knight   > Snake <";
        }
    }

    void ConfigurarCampoNombre()
    {
        if (campoNombre == null && menuPrincipal != null)
        {
            campoNombre = menuPrincipal.GetComponentInChildren<TMP_InputField>(true);
        }

        if (campoNombre == null)
        {
            GameObject existente = GameObject.Find("CampoNombre");
            if (existente != null)
            {
                campoNombre = existente.GetComponent<TMP_InputField>();
            }
        }

        if (campoNombre != null)
        {
            campoNombre.navigation = new Navigation { mode = Navigation.Mode.None };
            campoNombre.ActivateInputField();
        }
    }

    void CrearSelectorPersonaje(Transform parent)
    {
        if (previewPersonaje == null)
        {
            GameObject previewGo = new GameObject("PreviewPersonaje");
            previewGo.transform.SetParent(parent, false);
            RectTransform rt = previewGo.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(120f, 120f);
            rt.anchoredPosition = new Vector2(0f, -220f);
            previewPersonaje = previewGo.AddComponent<Image>();
            previewPersonaje.color = Color.white;
            previewPersonaje.preserveAspect = true;
        }

        if (textoPersonaje == null)
        {
            textoPersonaje = CrearTexto(parent, "TextoPersonaje", new Vector2(0f, -300f),
                TextAlignmentOptions.Center, "> Knight <   Snake");
            RectTransform rt = textoPersonaje.rectTransform;
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(700f, 50f);
            rt.anchoredPosition = new Vector2(0f, -300f);
            textoPersonaje.fontSize = 28;
        }
    }

    TMP_Text CrearTexto(Transform parent, string nombre, Vector2 pos, TextAlignmentOptions align, string texto)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(420f, 80f);
        rt.anchoredPosition = pos;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = 42;
        tmp.alignment = align;
        tmp.color = Color.white;
        tmp.raycastTarget = false;

        if (fuenteUI != null)
        {
            tmp.font = fuenteUI;
        }

        return tmp;
    }
}
