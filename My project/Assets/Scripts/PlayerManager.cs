using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float fuerzaSalto;

    public bool EnSuelo => enSuelo;

    public RuntimeAnimatorController controladorCaballero;
    public RuntimeAnimatorController controladorSerpiente;
    public Sprite spriteCaballero;
    public Sprite spriteSerpiente;

    private Rigidbody2D rigidbody2D;
    public GameSession gameSession;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool enSuelo = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (controladorCaballero == null && animator != null)
        {
            controladorCaballero = animator.runtimeAnimatorController;
        }

        if (spriteCaballero == null && spriteRenderer != null)
        {
            spriteCaballero = spriteRenderer.sprite;
        }
    }

    void Update()
    {
        if (gameSession == null || !gameSession.inicio || gameSession.finJuego)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && enSuelo)
        {
            enSuelo = false;
            SetSaltando(true);
            rigidbody2D.AddForce(new Vector2(0, fuerzaSalto));
            gameSession.ReproducirSonidoSalto();
        }
    }

    public void AplicarPersonaje(int indice)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        bool usarSerpiente = indice == 1;

        RuntimeAnimatorController controlador = usarSerpiente ? controladorSerpiente : controladorCaballero;
        Sprite sprite = usarSerpiente ? spriteSerpiente : spriteCaballero;

        if (controlador != null)
        {
            animator.runtimeAnimatorController = controlador;
        }

        if (sprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }

        SetSaltando(false);
    }

    public Sprite ObtenerSpritePreview(int indice)
    {
        return indice == 1 ? spriteSerpiente : spriteCaballero;
    }

    void SetSaltando(bool valor)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool("estaSaltando", valor);
        animator.SetBool("isJumping", valor);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            enSuelo = true;
            SetSaltando(false);
        }

        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            gameSession.EndGame();
        }
    }
}
