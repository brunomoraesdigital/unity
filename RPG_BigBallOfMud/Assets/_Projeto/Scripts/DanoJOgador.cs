using UnityEngine;

public class DanoJogador : MonoBehaviour
{
    // Ao entrar em um Trigger (Gatilho)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem encostou tem a Tag "Player"
        if (other.CompareTag("Player"))
        {
            // Move o jogador de volta para a posição (0, 0)
            other.transform.position = new Vector3(0, 0, 0);
            Debug.Log("O Jogador tocou no perigo e voltou ao início!");
        }
    }
}