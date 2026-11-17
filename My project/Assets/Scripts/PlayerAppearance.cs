using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    public RuntimeAnimatorController controladorCaballero;
    public RuntimeAnimatorController controladorSerpiente;
    public Sprite spriteCaballero;
    public Sprite spriteSerpiente;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        animator = GetComponent<Animator>();
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

    public void SetSaltando(bool valor)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            return;
        }

        animator.SetBool("estaSaltando", valor);
        animator.SetBool("isJumping", valor);
    }
}
