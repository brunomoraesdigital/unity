using UnityEngine;

public class Cerebro_Inimigo : MonoBehaviour
{
    [Header("--- CONFIGURAÇÕES ---")]
    public string nomeInimigo = "Monstro Bravo";
    public float velocidade = 3f; // Um pouco mais lento que o jogador

    private Transform alvo; // O Jogador
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // No Big Ball of Mud, o inimigo "procura" o jogador pelo nome do objeto
        GameObject jogadorObj = GameObject.Find("Jogador");

        if (jogadorObj != null)
        {
            alvo = jogadorObj.transform;
        }
    }

    void FixedUpdate()
    {
        if (alvo != null)
        {
            // Calcula a direção do jogador
            Vector2 direcao = (alvo.position - transform.position).normalized;

            // Move o inimigo em direção ao jogador
            rb.MovePosition(rb.position + direcao * velocidade * Time.fixedDeltaTime);
        }
    }
}