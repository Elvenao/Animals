using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIVirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class Event : UnityEvent { }

    [Header("Configuración Teclado")]
    public KeyCode teclaDelTeclado = KeyCode.None; // Aquí asignarás la tecla en el Inspector

    [Header("Output")]
    public BoolEvent buttonStateOutputEvent;
    public Event buttonClickOutputEvent;

    // --- NUEVO: Escuchar el teclado ---
    void Update()
    {
        // Si no hemos asignado ninguna tecla, no hacemos nada
        if (teclaDelTeclado == KeyCode.None) return;

        // Al presionar la tecla
        if (Input.GetKeyDown(teclaDelTeclado))
        {
            OutputButtonStateValue(true);
        }

        // Al soltar la tecla
        if (Input.GetKeyUp(teclaDelTeclado))
        {
            OutputButtonStateValue(false);
            OutputButtonClickEvent();
        }
    }
    // ----------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        OutputButtonStateValue(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OutputButtonStateValue(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OutputButtonClickEvent();
    }

    void OutputButtonStateValue(bool buttonState)
    {
        buttonStateOutputEvent.Invoke(buttonState);
    }

    void OutputButtonClickEvent()
    {
        buttonClickOutputEvent.Invoke();
    }

}
