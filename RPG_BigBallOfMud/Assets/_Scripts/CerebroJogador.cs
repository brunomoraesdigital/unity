using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class CerebroJogador : MonoBehaviour
{
    public float velocidadeMovimento = 5f;
    private Rigidbody2D componenteFisica;
    private NavMeshAgent agenteNav;
    private Vector2 direcaoInput;

    [Header("Configurações de Ataque")]
    private float tempoExibicaoAtaque = 0.2f;
    private float alcanceAtaque = 1.6f; // Mesma distância do monstro

    private Transform alvoPerseguicao;
    private float cronometroAtaqueAuto;

    void Start()
    {
        componenteFisica = GetComponent<Rigidbody2D>();
        agenteNav = GetComponent<NavMeshAgent>();

        if (agenteNav != null)
        {
            agenteNav.updateRotation = false;
            agenteNav.updateUpAxis = false;
            agenteNav.speed = velocidadeMovimento;
        }
    }

    void Update()
    {
        if (GerenteConsole.instancia != null && GerenteConsole.instancia.EstaDigitando())
        {
            if (agenteNav != null && agenteNav.isOnNavMesh) agenteNav.isStopped = true;
            return;
        }

        MoverERotacionar();

        if (Keyboard.current.fKey.wasPressedThisFrame) { ExecutarAcerto(); }

        if (Mouse.current.leftButton.wasPressedThisFrame) { DetectarCliqueMouse(); }

        if (alvoPerseguicao != null) { ExecutarAutoCaca(); }
    }

    void DetectarCliqueMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("Inimigo"))
        {
            alvoPerseguicao = hit.collider.transform;
            if (agenteNav != null && agenteNav.isOnNavMesh) agenteNav.isStopped = false;

            // Mensagem de início de caça
            GerenteConsole.instancia.EscreverNoConsole("Você marcou o " + alvoPerseguicao.name + " como alvo!");
        }
        else
        {
            alvoPerseguicao = null;
            if (agenteNav != null && agenteNav.isOnNavMesh)
            {
                agenteNav.isStopped = false;
                agenteNav.SetDestination(new Vector3(mousePos.x, mousePos.y, 0));
            }
        }
    }

    void ExecutarAutoCaca()
    {
        float distancia = Vector2.Distance(transform.position, alvoPerseguicao.position);

        // CORREÇÃO: Olha sempre para o alvo na perseguição
        Vector3 direcaoAlvo = (alvoPerseguicao.position - transform.position).normalized;
        float anguloAlvo = Mathf.Atan2(direcaoAlvo.y, direcaoAlvo.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, anguloAlvo);

        if (distancia > alcanceAtaque)
        {
            if (agenteNav != null && agenteNav.isOnNavMesh)
            {
                agenteNav.isStopped = false;
                agenteNav.SetDestination(alvoPerseguicao.position);
            }
        }
        else
        {
            if (agenteNav != null && agenteNav.isOnNavMesh)
            {
                agenteNav.isStopped = true;
                agenteNav.velocity = Vector3.zero; // Para o corpo físico
            }

            cronometroAtaqueAuto += Time.deltaTime;
            if (cronometroAtaqueAuto >= 1f)
            {
                ExecutarAcerto();
                cronometroAtaqueAuto = 0;
            }
        }
    }

    void MoverERotacionar()
    {
        float x = 0; float y = 0;
        bool temTeclado = false;

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) { y = 1; temTeclado = true; }
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) { y = -1; temTeclado = true; }
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) { x = -1; temTeclado = true; }
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) { x = 1; temTeclado = true; }

        if (temTeclado)
        {
            alvoPerseguicao = null;
            if (agenteNav != null && agenteNav.isOnNavMesh) agenteNav.isStopped = true;
            direcaoInput = new Vector2(x, y);
            float angulo = Mathf.Atan2(direcaoInput.y, direcaoInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
        else if (agenteNav != null && agenteNav.isOnNavMesh && !agenteNav.isStopped && agenteNav.hasPath)
        {
            if (agenteNav.velocity.sqrMagnitude > 0.01f)
            {
                float angulo = Mathf.Atan2(agenteNav.velocity.y, agenteNav.velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angulo);
            }
            direcaoInput = Vector2.zero;
        }
        else { direcaoInput = Vector2.zero; }
    }

    void FixedUpdate()
    {
        if (agenteNav == null || !agenteNav.isOnNavMesh || agenteNav.isStopped)
        {
            componenteFisica.linearVelocity = direcaoInput.normalized * velocidadeMovimento;
        }
    }

    void ExecutarAcerto()
    {
        GameObject bastao = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(bastao.GetComponent<MeshCollider>());
        bastao.transform.SetParent(this.transform);
        bastao.transform.localPosition = new Vector3(1.2f, 0, -0.1f); // Z para cima
        bastao.transform.localRotation = Quaternion.identity;
        bastao.transform.localScale = new Vector3(1.5f, 0.3f, 1f);
        bastao.GetComponent<Renderer>().material.color = new Color(0.5f, 0.8f, 1f);
        Destroy(bastao, tempoExibicaoAtaque);

        Vector2 pontoAtaque = (Vector2)transform.position + (Vector2)(transform.right * 1.5f);
        Collider2D[] atingidos = Physics2D.OverlapBoxAll(pontoAtaque, new Vector2(1.5f, 0.5f), transform.eulerAngles.z);

        foreach (var hit in atingidos)
        {
            if (hit.CompareTag("Inimigo"))
            {
                CerebroInimigo inimigo = hit.GetComponent<CerebroInimigo>();
                if (inimigo != null) inimigo.ReceberAcerto();
            }
        }
    }
}