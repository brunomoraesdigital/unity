using UnityEngine;
using UnityEngine.InputSystem; // Sistema novo

public class MovimentoJogador : MonoBehaviour
{
    public float velocidade = 10f;
    public Rigidbody2D rb;

    void FixedUpdate()
    {
        // Forma direta de ler o teclado no Sistema Novo
        Vector2 teclas = Vector2.zero;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) teclas.y = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) teclas.y = -1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) teclas.x = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) teclas.x = 1;

        // Aplica a física moderna do Unity 6
        rb.linearVelocity = teclas.normalized * velocidade;
    }
}