using UnityEngine;

public class TextRandomMove : MonoBehaviour
{
    private float velocidad = 25f;
    private Vector2 direccion;
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();

        // Elige una dirección aleatoria al inicio
        direccion = Random.onUnitSphere;

        // Normaliza la dirección para garantizar que tenga una magnitud de 1
        direccion.Normalize();
    }

    void Update()
    {
        // Mueve el objeto en la dirección aleatoria
        rt.anchoredPosition += direccion * velocidad * Time.deltaTime;

        // Destruye el objeto después de un tiempo.
        Destroy(gameObject, 1f);
    }
}
