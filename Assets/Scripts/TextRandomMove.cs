using UnityEngine;

public class TextRandomMove : MonoBehaviour
{
    private float velocidad = 25f;
    private Vector2 direccion;
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();

        // Elige una direcci�n aleatoria al inicio
        direccion = Random.onUnitSphere;

        // Normaliza la direcci�n para garantizar que tenga una magnitud de 1
        direccion.Normalize();
    }

    void Update()
    {
        // Mueve el objeto en la direcci�n aleatoria
        rt.anchoredPosition += direccion * velocidad * Time.deltaTime;

        // Destruye el objeto despu�s de un tiempo.
        Destroy(gameObject, 1f);
    }
}
