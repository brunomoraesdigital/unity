using UnityEngine;
using UnityEngine.InputSystem; // Sistema moderno do Unity 6

public class Cerebro_Jogador : MonoBehaviour
{
    public float velocidade = 5f;
    private Rigidbody2D rb;
    private Vector2 entradaMovimento;

    void Start()
    {
        // Pega o componente Rigidbody2D que configuramos no Inspector
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Lê o estado atual do teclado de forma direta (API Moderna)
        Keyboard teclado = Keyboard.current;

        if (teclado != null)
        {
            float moverX = 0;
            float moverY = 0;

            if (teclado.wKey.isPressed) moverY = 1;
            if (teclado.sKey.isPressed) moverY = -1;
            if (teclado.aKey.isPressed) moverX = -1;
            if (teclado.dKey.isPressed) moverX = 1;

            entradaMovimento = new Vector2(moverX, moverY).normalized;
        }
    }

    void FixedUpdate()
    {
        // Aplica o movimento na física para permitir o deslize nos obstáculos
        rb.MovePosition(rb.position + entradaMovimento * velocidade * Time.fixedDeltaTime);
    }
}