using UnityEngine;
using UnityEngine.AI;

public class CerebroInimigo : MonoBehaviour
{
    [SerializeField] Transform JOGADOR;
    private NavMeshAgent agent;

    private Vector3 pontoRespawn;
    [SerializeField] float raioPatrulha = 3f;
    [SerializeField] float raioVisao = 4f;
    [SerializeField] float raioPerseguicao = 8f;
    [SerializeField] float distanciaAtaque = 1.2f; // Distância para parar de empurrar

    private bool estaPerseguindo = false;

    [SerializeField] float tempoEsperaPatrulha = 2f;
    private float cronometroPatrulha;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        pontoRespawn = transform.position;
        cronometroPatrulha = tempoEsperaPatrulha;

        // Trava a física para não ser empurrado pelo jogador
        if (GetComponent<Rigidbody2D>() != null)
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void Update()
    {
        if (JOGADOR == null) return;

        float distanciaProJogador = Vector3.Distance(transform.position, JOGADOR.position);
        float distanciaJogadorProRespawn = Vector3.Distance(pontoRespawn, JOGADOR.position);

        // LÓGICA DA CHAVE
        if (distanciaProJogador <= raioVisao) estaPerseguindo = true;
        if (distanciaJogadorProRespawn > raioPerseguicao) estaPerseguindo = false;

        if (estaPerseguindo)
        {
            // --- AQUI ESTÁ A CORREÇÃO DA PARADA ---
            if (distanciaProJogador <= distanciaAtaque)
            {
                agent.isStopped = true;       // Trava o movimento
                agent.velocity = Vector3.zero; // Mata o empurrão/inércia
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(JOGADOR.position);
            }
        }
        else
        {
            agent.isStopped = false; // Garante que ele possa voltar a andar
            ExecutarPatrulhaVadiagem();
        }

        AjustarRotacaoVisual();
    }

    void ExecutarPatrulhaVadiagem()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            cronometroPatrulha -= Time.deltaTime;
            if (cronometroPatrulha <= 0)
            {
                Vector2 pontoAleatorio = Random.insideUnitCircle * raioPatrulha;
                Vector3 destinoFinal = pontoRespawn + new Vector3(pontoAleatorio.x, pontoAleatorio.y, 0);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(destinoFinal, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(centro, raioPatrulha);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centro, raioPerseguicao);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioVisao);

        // Desenha o raio de ataque em branco para você ajustar
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}