using UnityEngine;
using System.Collections;

public class Cerebro_Inimigo : MonoBehaviour
{
    [Header("--- PERÍMETROS (Sua Imagem) ---")]
    public float raioPatrulha = 5f;
    public float raioVisao = 8f;
    public float raioPerseguicao = 10f;

    [Header("--- CONFIGURAÇÕES ---")]
    public float velocidade = 2.5f;
    public LayerMask camadaObstaculo;

    private Vector3 pontoInicial;
    private Transform jogador;
    private bool movendo = false;
    private Vector3 ultimaDirecaoDesvio;

    void Start()
    {
        pontoInicial = transform.position;
        // Busca o jogador na cena
        GameObject objJogador = GameObject.Find("Jogador");
        if (objJogador != null) jogador = objJogador.transform;

        StartCoroutine(FluxoIA());
    }

    IEnumerator FluxoIA()
    {
        while (true)
        {
            if (jogador == null) yield break;

            float distanciaJogador = Vector3.Distance(transform.position, jogador.position);

            // Lógica de Perseguição
            if (distanciaJogador <= raioVisao && distanciaJogador <= raioPerseguicao)
            {
                yield return StartCoroutine(MoverNoGrid(jogador.position));
            }
            else
            {
                // Patrulha
                Vector3 destinoAleatorio = pontoInicial + (Vector3)Random.insideUnitCircle * raioPatrulha;
                yield return StartCoroutine(MoverNoGrid(destinoAleatorio));
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }
            yield return null;
        }
    }

    IEnumerator MoverNoGrid(Vector3 destinoFinal)
    {
        if (movendo) yield break;
        movendo = true;

        Vector3 direcaoDesejada = CalcularPassoGrid(destinoFinal);
        Vector3 posicaoAlvo = transform.position + direcaoDesejada;

        // Se o caminho direto está livre, ele vai
        if (CaminhoEstaLivre(posicaoAlvo))
        {
            yield return StartCoroutine(ExecutarMovimento(posicaoAlvo));
            ultimaDirecaoDesvio = Vector3.zero; // Limpa o desvio se o caminho está livre
        }
        else
        {
            // TENTA DESVIAR: Se o X está bloqueado, tenta Y e vice-versa
            Vector3 desvioA = (direcaoDesejada.x != 0) ? new Vector3(0, 1, 0) : new Vector3(1, 0, 0);
            Vector3 desvioB = -desvioA;

            // Tenta primeiro a direção que ele já estava usando para desviar (evita oscilação)
            Vector3 escolha = (ultimaDirecaoDesvio == desvioB) ? desvioB : desvioA;
            Vector3 outraEscolha = (escolha == desvioA) ? desvioB : desvioA;

            if (CaminhoEstaLivre(transform.position + escolha))
            {
                ultimaDirecaoDesvio = escolha;
                yield return StartCoroutine(ExecutarMovimento(transform.position + escolha));
            }
            else if (CaminhoEstaLivre(transform.position + outraEscolha))
            {
                ultimaDirecaoDesvio = outraEscolha;
                yield return StartCoroutine(ExecutarMovimento(transform.position + outraEscolha));
            }
            else
            {
                yield return new WaitForSeconds(0.3f); // Cercado
            }
        }

        movendo = false;
    }

    bool CaminhoEstaLivre(Vector3 posicao)
    {
        // Usa OverlapPoint com um pequeno ajuste para "encostar" melhor sem bugar
        return Physics2D.OverlapPoint(posicao, camadaObstaculo) == null;
    }

    IEnumerator ExecutarMovimento(Vector3 destino)
    {
        while (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidade * Time.deltaTime);
            yield return null;
        }
        transform.position = destino;
    }

    Vector3 CalcularPassoGrid(Vector3 destino)
    {
        Vector3 diff = destino - transform.position;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
            return new Vector3(diff.x > 0 ? 1 : -1, 0, 0);
        else
            return new Vector3(0, diff.y > 0 ? 1 : -1, 0);
    }
}