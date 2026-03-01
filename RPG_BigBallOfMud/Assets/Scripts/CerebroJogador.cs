using UnityEngine;
using UnityEngine.InputSystem; // Necessário para o novo sistema da Unity 6

public class CerebroJogador : MonoBehaviour
{
    public float velocidade = 5f;
    private Rigidbody2D rb;
    private Vector2 entrada;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // FUNCIONALIDADE: NASCIMENTO (SPAWN)
        // O código procura o objeto vazio que você criou na Hierarchy
        GameObject pontoDeNascimento = GameObject.Find("PontoDeSpawn");

        if (pontoDeNascimento != null)
        {
            // Teletransporta o círculo azul para o local exato do objeto
            transform.position = pontoDeNascimento.transform.position;
        }
        else
        {
            // Aviso caso você esqueça de criar ou renomear o objeto
            Debug.LogWarning("Aviso: Objeto 'PontoDeSpawn' não encontrado na Hierarchy!");
        }
    }

    void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado != null)
        {
            float x = 0;
            float y = 0;

            if (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) y = 1;
            if (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) y = -1;
            if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) x = -1;
            if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) x = 1;

            entrada = new Vector2(x, y);
            // Ataque 1: Botão J ou Clique Esquerdo (Comum)
            if (teclado.jKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                Debug.Log("Ataque 1: Golpe Rápido desferido!");
            }

            // Ataque 2: Botão K (Forte 1)
            if (teclado.kKey.wasPressedThisFrame)
            {
                Debug.Log("Ataque 2: Golpe Pesado desferido!");
            }

            // Ataque 3: Botão L (Forte 2)
            if (teclado.lKey.wasPressedThisFrame)
            {
                Debug.Log("Ataque 3: Explosão de Energia!");
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + entrada.normalized * velocidade * Time.fixedDeltaTime);
    }
}