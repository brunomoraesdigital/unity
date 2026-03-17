using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CerebroNPC : MonoBehaviour
{
    public enum TipoNPC { Aventureiro, Mercador, AssistenteGuilda }
    [Header("Configurações do NPC")]
    public TipoNPC tipoDesteNPC;

    private int contadorTeste = 0;
    [SerializeField] Transform JOGADOR;
    private NavMeshAgent agent;

    private Vector3 pontoRespawn;
    [SerializeField] float raioVadiagem = 2f;
    [SerializeField] float raioVisao = 3f;

    [SerializeField] float tempoEsperaPatrulha = 3f;
    private float cronometroPatrulha;
    private bool jaFalou = false;

    // Controle de dicas para o Aventureiro
    private int ultimaDicaIndice = -1;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        pontoRespawn = transform.position;
        cronometroPatrulha = tempoEsperaPatrulha;

        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        if (JOGADOR == null) return;

        float distanciaProJogador = Vector2.Distance(transform.position, JOGADOR.position);

        if (distanciaProJogador <= raioVisao)
        {
            if (!jaFalou)
            {
                ExecutarFalaRPG();
                jaFalou = true;
            }
            agent.isStopped = true;
            OlharSuaveParaOJogador(); // NOVO: Olha para você suavemente
        }
        else
        {
            agent.isStopped = false;
            jaFalou = false;
            ExecutarVadiagemNPC();
            AjustarRotacaoVisual(); // Volta a olhar para onde caminha
        }
    }

    void ExecutarFalaRPG()
    {
        string mensagem = "";
        contadorTeste++;

        if (tipoDesteNPC == TipoNPC.Aventureiro)
        {
            // Lógica de dicas aleatórias sem repetir a anterior
            string[] dicas = {
                "Lobo cercam o norte. Mantenha o aço afiado!",
                "Cuidado com os ratos. Eles parecem calmos, mas atacam se chegar perto!",
                "Não perca tempo com as galinhas. Vão fugir antes de você atacar.",
                "Coelhos são medrosos. Correm ao menor sinal de perigo."
            };

            int novoIndice;
            do
            {
                novoIndice = Random.Range(0, dicas.Length);
            } while (novoIndice == ultimaDicaIndice);

            ultimaDicaIndice = novoIndice;
            mensagem = "AVENTUREIRO: " + dicas[novoIndice] + " (" + contadorTeste + ")";
        }
        else if (tipoDesteNPC == TipoNPC.Mercador)
        {
            mensagem = "MERCADOR: Pelas barbas de Odin! Deseja trocar seu ouro por algo útil? (" + contadorTeste + ")";
        }
        else
        {
            mensagem = "ASSISTENTE: Bem-vindo à Guilda. Procuras missões ou suporte? (" + contadorTeste + ")";
        }

        if (GerenteConsole.instancia != null)
            GerenteConsole.instancia.EscreverNoConsole(mensagem);
    }

    void OlharSuaveParaOJogador()
    {
        Vector3 direcao = (JOGADOR.position - transform.position).normalized;
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        Quaternion rotacaoAlvo = Quaternion.Euler(0, 0, angulo);
        // 5f é a velocidade da suavidade
        transform.rotation = Quaternion.Lerp(transform.rotation, rotacaoAlvo, Time.deltaTime * 5f);
    }

    void ExecutarVadiagemNPC()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            cronometroPatrulha -= Time.deltaTime;
            if (cronometroPatrulha <= 0)
            {
                Vector2 pontoAleatorio = Random.insideUnitCircle * raioVadiagem;
                Vector3 destinoFinal = pontoRespawn + new Vector3(pontoAleatorio.x, pontoAleatorio.y, 0);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(destinoFinal, out hit, 1.0f, NavMesh.AllAreas))
                    agent.SetDestination(hit.position);

                cronometroPatrulha = tempoEsperaPatrulha;
            }
        }
    }

    void AjustarRotacaoVisual()
    {
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            float angulo = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centro = Application.isPlaying ? pontoRespawn : transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centro, raioVadiagem);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioVisao);
    }
}