using UnityEngine;
using UnityEngine.InputSystem;

public class CerebroJogador : MonoBehaviour
{
    public enum TipoArma { Espada, Arco, Cajado }

    [Header("--- EQUIPAMENTO ---")]
    public TipoArma armaEquipada = TipoArma.Espada;
    public int atk_arma = 10;

    [Header("--- PROGRESSO E PONTOS ---")]
    [Range(1, 100)] public int NV = 1;
    public int pontosDisponiveis;

    [Header("--- ATRIBUTOS PRIMÁRIOS ---")]
    [Range(1, 100)] public int FOR = 1;
    [Range(1, 100)] public int AGI = 1;
    [Range(1, 100)] public int VIT = 1;
    [Range(1, 100)] public int INT = 1;
    [Range(1, 100)] public int DES = 1;
    [Range(1, 100)] public int SOR = 1;

    private int fOld, aOld, vOld, iOld, dOld, sOld, nvOld;

    [Header("--- STATUS SECUNDÁRIOS ---")]
    public int vida_max; public int mana_max;
    public int atk_fis, atk_mag, atk_dist, defesa;
    public int vel_ataque, esquiva, precisao, acerto_critico, esquiva_perfeita, carga_max;

    private Rigidbody2D rb;
    private Vector2 entrada;
    private float tempoReuso = 0;
    public float velocidadeBase = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SalvarEstadoAnterior();
        CalcularStatus();

        GameObject spawn = GameObject.Find("PontoDeSpawn");
        if (spawn != null) transform.position = spawn.transform.position;
    }

    void Update()
    {
        CalcularStatus();
        GerenciarInputs();
    }

    void OnValidate()
    {
        CalcularStatus();
    }

    void CalcularStatus()
    {
        int totalGanhos = NV - 1;
        int gastos = (FOR - 1) + (AGI - 1) + (VIT - 1) + (INT - 1) + (DES - 1) + (SOR - 1);

        if (gastos > totalGanhos)
        {
            FOR = fOld; AGI = aOld; VIT = vOld;
            INT = iOld; DES = dOld; SOR = sOld;
            NV = nvOld;
            gastos = (FOR - 1) + (AGI - 1) + (VIT - 1) + (INT - 1) + (DES - 1) + (SOR - 1);
        }
        else
        {
            SalvarEstadoAnterior();
        }

        pontosDisponiveis = totalGanhos - gastos;

        // Fórmulas com Atk Arma (Sempre Inteiros)
        atk_mag = (int)((INT * 4) + (DES / 2f) + SOR) + atk_arma;
        atk_fis = (int)((FOR * 2) + (DES / 2f) + SOR) + atk_arma;
        atk_dist = (int)(DES + (AGI / 2f) + SOR) + atk_arma;

        vel_ataque = (int)((AGI * 2) + DES);
        esquiva = (int)((AGI * 2) + (DES / 2f) + SOR);
        precisao = (int)((DES * 2) + (AGI / 2f) + SOR);
        acerto_critico = (int)(Mathf.Floor(SOR / 2f) * Mathf.Floor(DES / 10f));
        esquiva_perfeita = (int)(Mathf.Floor(SOR / 2f) * Mathf.Floor(AGI / 10f));
        vida_max = (int)((VIT * 3 + NV * 2) * 100);
        mana_max = (int)((INT * 3 + NV * 2) * 100);
        defesa = (int)((VIT * 3) + (FOR / 2f) + (NV * 2));
        carga_max = (int)(((FOR * 10) + NV) * 10);
    }

    void SalvarEstadoAnterior()
    {
        fOld = FOR; aOld = AGI; vOld = VIT;
        iOld = INT; dOld = DES; sOld = SOR;
        nvOld = NV;
    }

    void GerenciarInputs()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        float x = (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) ? -1 : (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) ? 1 : 0;
        float y = (teclado.wKey.isPressed || teclado.upArrowKey.isPressed) ? 1 : (teclado.sKey.isPressed || teclado.downArrowKey.isPressed) ? -1 : 0;
        entrada = new Vector2(x, y);

        if (tempoReuso > 0) tempoReuso -= Time.deltaTime;
        if (tempoReuso <= 0)
        {
            if (teclado.jKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) ProcessarAtaque(1);
            if (teclado.kKey.wasPressedThisFrame) ProcessarAtaque(2);
            if (teclado.lKey.wasPressedThisFrame) ProcessarAtaque(3);
        }
    }

    void ProcessarAtaque(int b)
    {
        int danoFinal = 0;
        string nomeGolpe = "";

        // Define o dano baseado no botão e na arma
        if (armaEquipada == TipoArma.Espada)
        {
            if (b == 1) { danoFinal = atk_fis; nomeGolpe = "Golpe Rápido"; }
            if (b == 2) { danoFinal = (int)(atk_fis * 1.5f); nomeGolpe = "Golpe Fulminante"; tempoReuso = 1f; }
            if (b == 3) { danoFinal = (int)(atk_fis * 2f); nomeGolpe = "Impacto Explosivo"; tempoReuso = 3f; }
        }
        // ... (você pode repetir a lógica para Arco e Cajado depois)

        Debug.Log($"{nomeGolpe}! Dano: {danoFinal}");

        // --- A MÁGICA ACONTECE AQUI: ---
        // Cria um círculo invisível de 1.5 metros ao redor do jogador para detectar inimigos
        Collider2D[] atingidos = Physics2D.OverlapCircleAll(transform.position, 1.5f);

        foreach (Collider2D col in atingidos)
        {
            // Se o que atingimos tiver a Tag "Inimigo"
            if (col.CompareTag("Inimigo"))
            {
                CerebroInimigo scriptInimigo = col.GetComponent<CerebroInimigo>();
                if (scriptInimigo != null)
                {
                    scriptInimigo.ReceberDano(danoFinal); // Manda o dano para o inimigo!
                }
            }
        }
    }

    // Isso serve para você enxergar o tamanho do seu ataque na aba Scene (Círculo Azul)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + entrada.normalized * velocidadeBase * Time.fixedDeltaTime);
    }
}