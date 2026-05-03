using UnityEngine;
using UnityEngine.EventSystems; // ¡Necesario para el Drag & Drop!

public class ZonaBasura : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // Si el objeto que arrastramos tiene un componente, lo detectamos
        if (eventData.pointerDrag != null)
        {
            GameObject mensaje = eventData.pointerDrag;

            // Verificamos si es un mensaje del Ex
            if (mensaje.CompareTag("MensajeRojo"))
            {
                Debug.Log("¡Mensaje enviado a la basura!");
                
                // Si queremos que se queden ahí acumulados
                mensaje.transform.SetParent(this.transform);
                mensaje.transform.localPosition = Vector3.zero; // Opcional: Centrarlo
                
                // Si solo queremos destruirlo
                // Destroy(mensaje);
            }
        }
    }
}