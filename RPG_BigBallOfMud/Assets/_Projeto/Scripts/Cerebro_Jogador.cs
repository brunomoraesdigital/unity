using UnityEngine;
using UnityEngine.InputSystem;

public class Cerebro_Jogador : MonoBehaviour
{
    [Header("--- ATRIBUTOS ---")]
    public string nomeDoPersonagem = "Aventureiro";
    public int nivel = 1;
    public float hpAtual = 100f;
    public float hpMaximo = 100f;

    [Header("--- MOVIMENTAÇÃO ---")]
    public float velocidade = 5f;

    // Privados e fixos para não poluir o Inspector
    private float spawnX = 0f;
    private float spawnY = 0f;
    private float spawnZ = -1f;

    private float cooldownGeral = 0.5f;
    private float tempoProximoAtaque;

    private Rigidbody2D rb;
    private Vector2 entradaMovimento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Posiciona no início
        transform.position = new Vector3(spawnX, spawnY, spawnZ);
    }

    void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        // 1. Movimentação (W, A, S, D)
        float moverX = 0;
        float moverY = 0;
        if (teclado.wKey.isPressed) moverY = 1;
        if (teclado.sKey.isPressed) moverY = -1;
        if (teclado.aKey.isPressed) moverX = -1;
        if (teclado.dKey.isPressed) moverX = 1;
        entradaMovimento = new Vector2(moverX, moverY).normalized;

        // 2. Ações (Ataques e Pulo)
        if (Time.time > tempoProximoAtaque)
        {
            if (teclado.jKey.wasPressedThisFrame)
            {
                Debug.Log($"{nomeDoPersonagem} executou ATAQUE COMUM (J)!");
                tempoProximoAtaque = Time.time + cooldownGeral;
            }
            else if (teclado.kKey.wasPressedThisFrame)
            {
                Debug.Log($"{nomeDoPersonagem} executou ATAQUE FORTE 1 (K)!");
                tempoProximoAtaque = Time.time + 1.5f; // Cooldown fixo no código
            }
            else if (teclado.lKey.wasPressedThisFrame)
            {
                Debug.Log($"{nomeDoPersonagem} executou ATAQUE FORTE 2 (L)!");
                tempoProximoAtaque = Time.time + 3.0f;
            }
            else if (teclado.spaceKey.wasPressedThisFrame)
            {
                Debug.Log($"{nomeDoPersonagem} Ppulou! (Espaço)");
                // Sem efeito visual por enquanto, apenas log
            }
        }

        // 3. Sistema de Morte
        if (hpAtual <= 0)
        {
            hpAtual = hpMaximo;
            transform.position = new Vector3(spawnX, spawnY, spawnZ);
            Debug.Log("💀 Renascendo...");
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + entradaMovimento * velocidade * Time.fixedDeltaTime);
    }
}