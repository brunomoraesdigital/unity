using UnityEngine;

public class LogicaPersonagem : MonoBehaviour
{
    public int pv = 100;
    public string nomeDoGolpe = "Corte Rápido";
    public int dano = 20;
    public int cura = 20;
    public float velocidade = 0.1f;
    public float forcaDoPulo = 1.0f;
    public int direcaoDoPuloX = 0;
    public int direcaoDoPuloY = 0;

    private void Update()
    {

        if (pv > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                float puloX = direcaoDoPuloX * forcaDoPulo;
                float puloY = (direcaoDoPuloY == 0) ? forcaDoPulo : direcaoDoPuloY * forcaDoPulo;

                transform.Translate(puloX, puloY, 0);
                Debug.Log("Agreffoz saltou na direção: " + direcaoDoPuloX + ", " + direcaoDoPuloY);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                transform.Translate(velocidade, 0, 0);
                direcaoDoPuloX = 1;
                direcaoDoPuloY = 0;

                Debug.Log("Agreffoz se moveu para direita no eixo x!");
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.Translate(-velocidade, 0, 0);
                direcaoDoPuloX = -1;
                direcaoDoPuloY = 0;

                Debug.Log("Agreffoz se moveu para esquerda no eixo x!");
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                transform.Translate(0, velocidade, 0);
                direcaoDoPuloX = 0;
                direcaoDoPuloY = 1;

                Debug.Log("Agreffoz se moveu para cima no eixo y!");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.Translate(0, -velocidade, 0);
                direcaoDoPuloX = 0;
                direcaoDoPuloY = -1;

                Debug.Log("Agreffoz se moveu para baixo no eixo y!");
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Agreffoz usou " + nomeDoGolpe + " e causou " + dano + " de dano!");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                int vidaAposDano = pv - dano;

                Debug.Log("Agreffoz recebeu um golpe e sofreu " + dano + " de dano!");

                pv = vidaAposDano;
                Debug.Log("PV: "+pv+"/100");

            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                int vidaAposCura = pv + cura;

                if (vidaAposCura > 100)
                {
                    int excessoDeCura = vidaAposCura - 100;
                    int curaAplicada = cura - excessoDeCura;

                    Debug.Log("Agreffoz usou cura em si mesmo e recuperou " + curaAplicada + " de PV");
                    
                    pv = 100;
                    Debug.Log("PV: " + pv + "/100");
                }
                else
                {
                    Debug.Log("Agreffoz usou cura em si mesmo e recuperou " + cura + " de PV");

                    pv = vidaAposCura;
                    Debug.Log("PV: " + pv + "/100");

                }
            }

        } else 
        {
            Debug.Log("GAME OVER!");
            gameObject.SetActive(false);
        }
    }

}