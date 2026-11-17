using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudUI : MonoBehaviour
{
    public GameObject menugameOver;
    public TMP_Text textoPuntaje;
    public TMP_Text textoNombre;
    public TMP_FontAsset fuenteUI;
    public GameObject menuPrincipal;

    public string nombreJugador = "Player";

    public void Init()
    {
        if (fuenteUI == null && menuPrincipal != null)
        {
            TMP_Text menuText = menuPrincipal.GetComponentInChildren<TMP_Text>(true);
            if (menuText != null)
            {
                fuenteUI = menuText.font;
            }
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        if (textoPuntaje == null)
        {
            textoPuntaje = CrearTextoHUD(canvas.transform, "TextoPuntaje", new Vector2(-20f, -20f),
                TextAlignmentOptions.TopRight, "Puntaje: 0");
            RectTransform rt = textoPuntaje.rectTransform;
            rt.anchorMin = new Vector2(1f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(-20f, -20f);
        }

        if (textoNombre == null)
        {
            textoNombre = CrearTextoHUD(canvas.transform, "TextoNombre", new Vector2(20f, -20f),
                TextAlignmentOptions.TopLeft, nombreJugador);
            RectTransform rt = textoNombre.rectTransform;
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(0f, 1f);
            rt.pivot = new Vector2(0f, 1f);
            rt.anchoredPosition = new Vector2(20f, -20f);
        }

        ShowGameOver(false);
        ActualizarHUD();
    }

    public void SetScore(int puntaje)
    {
        if (textoPuntaje != null)
        {
            textoPuntaje.text = "Puntaje: " + puntaje;
        }
    }

    public void SetPlayerName(string nombre)
    {
        nombreJugador = nombre;
        if (textoNombre != null)
        {
            textoNombre.text = nombreJugador;
        }
    }

    public void ShowGameOver(bool show)
    {
        if (menugameOver != null)
        {
            menugameOver.SetActive(show);
        }
    }

    public void ActualizarHUD()
    {
        if (textoNombre != null)
        {
            textoNombre.text = nombreJugador;
        }
    }

    TMP_Text CrearTextoHUD(Transform parent, string nombre, Vector2 pos, TextAlignmentOptions align, string texto)
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
