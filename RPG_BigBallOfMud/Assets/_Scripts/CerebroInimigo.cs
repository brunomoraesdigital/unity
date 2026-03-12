using UnityEngine;
using UnityEngine.AI;

public class CerebroInimigo : MonoBehaviour
{
    [SerializeField] Transform JOGADOR;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Mantém o sprite "de pé" no mundo 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (JOGADOR != null)
        {
            // Define o destino no mapa de navegação
            agent.SetDestination(JOGADOR.position);

            // LOGICA DE ROTAÇÃO: Aponta o indicador para a direção do movimento
            AjustarRotacaoVisual();
        }
    }

    void AjustarRotacaoVisual()
    {
        // Só rotaciona se o monstro estiver realmente se movendo
        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            // Calcula o ângulo baseado no vetor de velocidade do NavMesh
            float angulo = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;

            // Aplica a rotação no Transform (afeta o triângulo filho)
            transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
    }
}