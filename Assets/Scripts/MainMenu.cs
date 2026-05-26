using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelPrincipal;
    public GameObject panelInstrucciones;
    public GameObject panelCreditos;

    void Start()
    {
        // Al iniciar, nos aseguramos de que solo el menú principal esté activo
        RegresarAlMenu();
    }

    void Update()
    {
        // Soporte para el botón "Atrás" del celular (tecla Escape)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelInstrucciones.activeSelf || panelCreditos.activeSelf)
            {
                RegresarAlMenu();
            }
            else
            {
                Salir();
            }
        }
    }

    // --- NAVEGACIÓN ---

    public void IniciarSimulador()
    {
        SceneManager.LoadScene(1); // Carga la escena de AR
    }

    public void AbrirInstrucciones()
    {
        panelPrincipal.SetActive(false);
        panelInstrucciones.SetActive(true);
        panelCreditos.SetActive(false);
    }

    public void AbrirCreditos()
    {
        panelPrincipal.SetActive(false);
        panelInstrucciones.SetActive(false);
        panelCreditos.SetActive(true);
    }

    public void RegresarAlMenu()
    {
        panelPrincipal.SetActive(true);
        panelInstrucciones.SetActive(false);
        panelCreditos.SetActive(false);
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del simulador...");
    }
}