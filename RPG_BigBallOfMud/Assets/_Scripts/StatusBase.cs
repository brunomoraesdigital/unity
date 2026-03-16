using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusBase : MonoBehaviour
{
    [Header("Componentes UI")]
    // Como RectTransform, o Unity permite arrastar o objeto da Hierarquia
    public RectTransform objetoFillPV;
    public RectTransform objetoFillPM;
    public TextMeshProUGUI textoAtributos;

    [Header("Valores")]
    public float pvAtual = 100f;
    public float pvMax = 100f;
    public float pmAtual = 100f;
    public float pmMax = 100f;
    public int ataque = 50;
    public int defesa = 50;

    private float larguraOriginal = 160f;
    private Transform alvo;
    private Vector3 offset;

    void Start()
    {
        alvo = transform.parent;
        offset = transform.localPosition;
        transform.SetParent(null);
    }

    void LateUpdate()
    {
        if (alvo == null) return;
        transform.position = alvo.position + offset;
        transform.rotation = Quaternion.identity;

        // CÁLCULO PRECISO: pvAtual / pvMax
        if (objetoFillPV != null)
        {
            float pct = Mathf.Clamp01(pvAtual / pvMax);
            // Ajusta o Width (largura) baseado no cálculo
            objetoFillPV.sizeDelta = new Vector2(larguraOriginal * pct, objetoFillPV.sizeDelta.y);
        }

        if (objetoFillPM != null)
        {
            float pct = Mathf.Clamp01(pmAtual / pmMax);
            objetoFillPM.sizeDelta = new Vector2(larguraOriginal * pct, objetoFillPM.sizeDelta.y);
        }

        if (textoAtributos != null)
        {
            textoAtributos.text = "ATQ: " + ataque.ToString("000") + "  DEF: " + defesa.ToString("000");
        }
    }
}