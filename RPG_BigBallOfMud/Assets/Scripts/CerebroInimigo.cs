using UnityEngine;
using System.Collections;

public class CerebroInimigo : MonoBehaviour
{
    [Header("--- ZONAS FIXAS ---")]
    public float raioPatrulha = 3f;
    public float raioPerseguicao = 6f;

    [Header("--- ZONA MÓVEL ---")]
    public float raioVisao = 3f;

    [Header("--- STATUS ---")]
    public float velocidade = 2f;
    public float hpAtual = 500f; // ❤️ Vida total

    private Rigidbody2D rb;
    private Transform alvoJogador;
    private Vector2 pontoInicial;
    private Vector2 destinoPatrulha;

    private bool estaPerseguindo = false;
    private bool estaEsperando = false;
    private float cronometroAbortar = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pontoInicial = transform.position;
        destinoPatrulha = pontoInicial;

        GameObject p = GameObject.Find("Jogador");
        if (p != null) alvoJogador = p.transform;
    }

    void FixedUpdate()
    {
        if (alvoJogador == null) return;

        float distCorpoAoJogador = Vector2.Distance(transform.position, alvoJogador.position);
        float distSpawnAoJogador = Vector2.Distance(pontoInicial, alvoJogador.position);

        if (distCorpoAoJogador <= raioVisao && distSpawnAoJogador <= raioPerseguicao)
        {
            if (!estaPerseguindo)
            {
                estaPerseguindo = true;
                StopAllCoroutines();
                estaEsperando = false;
            }
        }
        else
        {
            if (estaPerseguindo)
            {
                estaPerseguindo = false;
                destinoPatrulha = pontoInicial;
            }
        }

        if (estaPerseguindo)
            MoverPara(alvoJogador.position);
        else
            ExecutarPatrulha();
    }

    // --- SISTEMA DE COMBATE ---

    // ⚔️ Esta é a função que o jogador vai "chamar"
    public void ReceberDano(float dano)
    {
        hpAtual -= dano;
        Debug.Log("Inimigo levou dano! HP: " + hpAtual);

        if (hpAtual <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject); // Remove o inimigo da cena
    }

    // --- MOVIMENTAÇÃO ---

    void ExecutarPatrulha()
    {
        float distDestino = Vector2.Distance(transform.position, destinoPatrulha);
        cronometroAbortar += Time.fixedDeltaTime;

        if (distDestino < 0.3f || cronometroAbortar > 3f)
        {
            if (!estaEsperando) StartCoroutine(EsperarEPatroar());
        }
        else
        {
            MoverPara(destinoPatrulha);
        }
    }

    void MoverPara(Vector2 alvo)
    {
        Vector2 direcao = (alvo - (Vector2)transform.position).normalized;
        rb.linearVelocity = direcao * velocidade;
    }

    IEnumerator EsperarEPatroar()
    {
        estaEsperando = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1.0f);
        destinoPatrulha = pontoInicial + (Random.insideUnitCircle * raioPatrulha);
        cronometroAbortar = 0f;
        estaEsperando = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, raioVisao);

        Vector2 centro = Application.isPlaying ? pontoInicial : (Vector2)transform.position;
        Gizmos.color = new Color(1, 0.5f, 0);
        Gizmos.DrawWireSphere(centro, raioPatrulha);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centro, raioPerseguicao);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, destinoPatrulha);
        }
    }
}