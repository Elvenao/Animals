// ===========================================================================
//  OrganizadorJerarquiaPro.cs
//  Ubicación: Assets/Editor/OrganizadorJerarquiaPro.cs
//  Menú:       Tools → 🗂️ Organizador de Jerarquía Profesional
// ===========================================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OrganizadorJerarquiaPro : EditorWindow
{
    private static readonly string[] RAICES = new[]
    {
        "[ 🔧 SISTEMA ]",
        "[ 🌍 ENTORNO ]",
        "[ 💡 ILUMINACIÓN ]",
        "[ 🤖 IA / TRÁFICO ]",
        "[ 👤 PERSONAJES ]",
        "[ 🥽 XR / JUGADOR ]",
        "[ 🎵 AUDIO ]",
        "[ 🖥️ UI / HUD ]",
        "[ ⚙️ DEBUG ]"
    };

    private static readonly string[] SUBS_SISTEMA = new[] { "Managers", "Eventos_Globales", "Input", "Red_y_Servicios" };
    private static readonly string[] SUBS_PERSONAJES = new[] { "NPCs_Principales", "NPCs_Secundarios", "Animales", "Criaturas" };
    private static readonly string[] SUBS_ILUMINACION = new[] { "Luces_Base", "Efectos_VFX", "Post_Process" };
    private static readonly string[] SUBS_TRAFICO = new[] { "Sistemas_IA", "Señalamiento_y_Semaforos", "Vehiculos_Dinamicos" };
    
    private static readonly string[] SUBS_ENTORNO = new[] { 
        "Banquetas_y_Aceras", "Pavimento_y_Calles", "Edificios_y_Hospital", 
        "Estructuras_Modulares", "Cercados_y_Vallas", "Props_Decorativos", 
        "Props_Interactivos", "Vegetación", "Terreno", "Agua", "Cielo_y_Ambiente" 
    };

    // =========================================================================
    //  DICCIONARIO DE REGLAS UNIVERSALES
    // =========================================================================
    private static readonly ReglaQuirurgica[] REGLAS = new ReglaQuirurgica[]
    {
        new ReglaQuirurgica("[ ⚙️ DEBUG ]", null, new[] { "debug","test","prueba","temporal","gizmo","editor_","_temp","_dev" }),
        new ReglaQuirurgica("[ 🖥️ UI / HUD ]", null, new[] { "canvas","hud","panel","menu","menú","popup","tooltip","overlay" }),
        new ReglaQuirurgica("[ 🎵 AUDIO ]", null, new[] { "audio","sound","sonido","musica","música","music","ambient","sfx","bgm" }),
        new ReglaQuirurgica("[ 🥽 XR / JUGADOR ]", null, new[] { "xr origin","xrorigin","xr rig","xrrig","player","jugador","rig","main camera","camera","hand","controller" }),
        
        // Tráfico e IA
        new ReglaQuirurgica("[ 🤖 IA / TRÁFICO ]", "Sistemas_IA", new[] { "navmesh","navmeshsurface","navmeshagent","waypoint","punto_ruta","ruta","route","path","aipath","node_ai" }),
        new ReglaQuirurgica("[ 🤖 IA / TRÁFICO ]", "Señalamiento_y_Semaforos", new[] { "sign_traffic", "street_sign", "stop_sign", "prop_sign", "semaforo", "semáforo", "traffic_light" }),
        new ReglaQuirurgica("[ 🤖 IA / TRÁFICO ]", "Vehiculos_Dinamicos", new[] { "sm_veh", "car", "truck", "sedan", "vehicle", "ambulance", "police", "taxi", "bus", "moto", "bike" }),

        // Personajes y Animales
        new ReglaQuirurgica("[ 👤 PERSONAJES ]", "NPCs_Principales", new[] { "npc","convai","amelia","doctor","enfermero","enfermera","recepcionista" }),
        new ReglaQuirurgica("[ 👤 PERSONAJES ]", "NPCs_Secundarios", new[] { "civil","civilian","peatón","pedestrian","npc_sec","paciente" }),
        new ReglaQuirurgica("[ 👤 PERSONAJES ]", "Animales", new[] { "dog","perro","cat","gato","bird","pájaro","rabbit","conejo","animal","pet" }),
        new ReglaQuirurgica("[ 👤 PERSONAJES ]", "Criaturas", new[] { "creature","criatura","monster","creatureflee","creaturegrabable" }),
        
        // Iluminación y VFX
        new ReglaQuirurgica("[ 💡 ILUMINACIÓN ]", "Efectos_VFX", new[] { "fx", "fx_", "particlesystem", "particle", "efecto", "vfx", "smoke", "fire", "spark" }),
        new ReglaQuirurgica("[ 💡 ILUMINACIÓN ]", "Post_Process", new[] { "volume", "postprocess", "post_process", "reflection probe", "lightprobe" }),
        new ReglaQuirurgica("[ 💡 ILUMINACIÓN ]", "Luces_Base", new[] { "directional light", "point light", "spot light", "area light", "luz_", "_light" }),
        
        // Entorno Urbano y Naturaleza
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Agua", new[] { "water","agua","ocean","river","lake","pool","fountain" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Vegetación", new[] { "tree","arbol","árbol","bush","arbusto","grass","hierba","plant","flower","sm_tree","sm_plant","sm_grass" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Terreno", new[] { "terrain","terreno","ground","landscape" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Cielo_y_Ambiente", new[] { "sky","cielo","skybox","fog","niebla","cloud" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Props_Interactivos", new[] { "door","puerta","button","boton","lever","palanca","switch","interactable" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Banquetas_y_Aceras", new[] { "sidewalk", "banqueta", "curb", "acera", "crosswalk" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Pavimento_y_Calles", new[] { "sm_road", "paviment", "pavimento", "street", "carretera", "calle", "road", "ramp", "rampa" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Edificios_y_Hospital", new[] { "sm_bld", "building", "edificio", "hospital" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Cercados_y_Vallas", new[] { "envfence", "fence", "reja", "valla", "cerca", "wall_fence" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Estructuras_Modulares", new[] { "sm_str", "sm_floor", "sm_wall", "sm_ceil", "wall", "floor", "ceiling", "roof", "stairs", "corridor", "france", "subwayentrance", "bridge", "puente", "metro", "tunnel", "túnel", "arch", "columna", "pillar", "structure" }),
        new ReglaQuirurgica("[ 🌍 ENTORNO ]", "Props_Decorativos", new[] { "sm_prop","prop_","_prop","box","caja","chair","silla","table","mesa","bench","shelf","cabinet","sofa","lamp","sign","trash","barrel","crate","furniture" }),

        // Sistema
        new ReglaQuirurgica("[ 🔧 SISTEMA ]", "Input", new[] { "inputsystem","eventsystem" }),
        new ReglaQuirurgica("[ 🔧 SISTEMA ]", "Red_y_Servicios", new[] { "network","server","client","multiplayer" }),
        new ReglaQuirurgica("[ 🔧 SISTEMA ]", "Managers", new[] { "manager","system","controller","bootstrap","gamecontroller","initializer","singleton" }),
    };

    private Vector2 _scrollLog;
    private readonly List<string> _log = new List<string>();
    private int _cMovidos, _cUbicados, _cSinRegla;

    [MenuItem("Tools/🗂️ Organizador de Jerarquía Profesional")]
    public static void AbrirVentana()
    {
        var w = GetWindow<OrganizadorJerarquiaPro>("🗂️ Jerarquía Quirúrgica Total");
        w.minSize = new Vector2(550, 400);
        w.Show();
    }

    private void OnGUI()
    {
        GUILayout.Space(8);
        EditorGUILayout.HelpBox("SISTEMA DE DISECCIÓN QUIRÚRGICA UNIVERSAL TOTAL\n\n" +
            "• SEPARACIÓN ABSOLUTA: Agrupa de forma automática TODOS los elementos de la escena por su nombre exacto de modelo.\n" +
            "• Carpetas dinámicas generadas al vuelo para props, edificios, vallas, carros, luces y vegetación.\n" +
            "• Optimizado para un control quirúrgico de escenas masivas (+6,000 objetos).", MessageType.Info);
        
        GUILayout.Space(10);

        var colorAntes = GUI.backgroundColor;
        GUI.backgroundColor = new Color(0.25f, 0.75f, 1f);
        if (GUILayout.Button("▶  Ejecutar Clasificación Quirúrgica Universal", GUILayout.Height(40)))
        {
            _log.Clear();
            _cMovidos = _cUbicados = _cSinRegla = 0;
            EjecutarOrdenQuirurgicoTotal();
        }
        GUI.backgroundColor = colorAntes;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Consola de operaciones masivas:", EditorStyles.boldLabel);
        _scrollLog = EditorGUILayout.BeginScrollView(_scrollLog, EditorStyles.helpBox);
        foreach (var linea in _log)
        {
            Color c = linea.StartsWith("✅") ? new Color(0.4f, 0.9f, 0.4f) :
                      linea.StartsWith("⚠") ? new Color(1f, 0.8f, 0.2f) :
                      linea.StartsWith("❌") ? new Color(1f, 0.4f, 0.4f) : EditorStyles.label.normal.textColor;
            
            var estilo = new GUIStyle(EditorStyles.label) { normal = { textColor = c } };
            GUILayout.Label(linea, estilo);
        }
        EditorGUILayout.EndScrollView();
    }

    private void EjecutarOrdenQuirurgicoTotal()
    {
        Scene escena = SceneManager.GetActiveScene();
        if (!escena.isLoaded) { Log("❌ Error: No hay ninguna escena activa cargada."); return; }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Disección Quirúrgica Universal");
        int grupoUndo = Undo.GetCurrentGroup();

        Log($"── Iniciando segmentación atómica total en: \"{escena.name}\" ──");

        // 1. Inicializar contenedores principales
        var mapaRaices = new Dictionary<string, GameObject>();
        foreach (var r in RAICES) mapaRaices[r] = ObtenerOCrearRaiz(r, grupoUndo);

        var mapaSubs = new Dictionary<string, Dictionary<string, GameObject>>();
        CrearEstructurasEstaticas("[ 🔧 SISTEMA ]", SUBS_SISTEMA, mapaRaices, mapaSubs, grupoUndo);
        CrearEstructurasEstaticas("[ 👤 PERSONAJES ]", SUBS_PERSONAJES, mapaRaices, mapaSubs, grupoUndo);
        CrearEstructurasEstaticas("[ 💡 ILUMINACIÓN ]", SUBS_ILUMINACION, mapaRaices, mapaSubs, grupoUndo);
        CrearEstructurasEstaticas("[ 🤖 IA / TRÁFICO ]", SUBS_TRAFICO, mapaRaices, mapaSubs, grupoUndo);
        CrearEstructurasEstaticas("[ 🌍 ENTORNO ]", SUBS_ENTORNO, mapaRaices, mapaSubs, grupoUndo);

        // 2. Captura masiva del universo de la escena (6K+ objetos)
        Transform[] todosLosTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        List<GameObject> listaProcesamiento = new List<GameObject>();

        for (int i = 0; i < todosLosTransforms.Length; i++)
        {
            GameObject go = todosLosTransforms[i].gameObject;
            if (go.hideFlags != HideFlags.None || go.scene != escena || EsContenedor(go.name)) continue;          
            listaProcesamiento.Add(go);
        }

        // Cache para el empaquetado de subcarpetas por nombre de asset de forma universal
        var cacheCarpetasNombre = new Dictionary<string, Transform>();

        int total = listaProcesamiento.Count;
        for (int i = 0; i < total; i++)
        {
            if (i % 120 == 0)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Disección Universal Activa", $"Filtrando a nivel atómico {i}/{total}: {listaProcesamiento[i].name}", (float)i / total))
                {
                    EditorUtility.ClearProgressBar();
                    Log("⚠ Operación abortada de forma segura por el usuario.");
                    FinalizarOperacion(escena, grupoUndo);
                    return;
                }
            }
            MudarObjetoQuirurgicoUniversal(listaProcesamiento[i], mapaRaices, mapaSubs, cacheCarpetasNombre, grupoUndo);
        }

        EditorUtility.ClearProgressBar();
        FinalizarOperacion(escena, grupoUndo);
    }

    private void MudarObjetoQuirurgicoUniversal(GameObject go, Dictionary<string, GameObject> mapaRaices, Dictionary<string, Dictionary<string, GameObject>> mapaSubs, Dictionary<string, Transform> cacheCarpetasNombre, int grupoUndo)
    {
        if (go == null) return;

        (string raiz, string subBase) = DeterminarDestino(go.name);
        if (raiz == null) { _cSinRegla++; return; }

        Transform padreContenedor = null;
        if (subBase != null && mapaSubs.TryGetValue(raiz, out var dicSub) && dicSub.TryGetValue(subBase, out var goSub))
        {
            padreContenedor = goSub.transform;
        }
        else if (mapaRaices.TryGetValue(raiz, out var goRaiz))
        {
            padreContenedor = goRaiz.transform;
        }

        if (padreContenedor == null) return;

        // =========================================================================
        //  DISECCIÓN QUIRÚRGICA UNIVERSAL (MÁXIMO DETALLE PARA TODO OBJETO)
        // =========================================================================
        string nombreLimpioCarpeta = ExtraerNombreLimpioBase(go.name);
        string claveCache = $"{raiz}_{subBase}_{nombreLimpioCarpeta}";

        Transform padreFinal = padreContenedor;

        // Regla universal: Generamos subcarpeta única para cada tipo de objeto individual
        if (!cacheCarpetasNombre.TryGetValue(claveCache, out padreFinal))
        {
            GameObject carpetaExistente = null;
            for (int i = 0; i < padreContenedor.childCount; i++)
            {
                if (padreContenedor.GetChild(i).name == nombreLimpioCarpeta)
                {
                    carpetaExistente = padreContenedor.GetChild(i).gameObject;
                    break;
                }
            }

            if (carpetaExistente == null)
            {
                carpetaExistente = new GameObject(nombreLimpioCarpeta);
                carpetaExistente.transform.SetParent(padreContenedor, false);
                carpetaExistente.transform.localPosition = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(carpetaExistente, $"Crear subcarpeta universal {nombreLimpioCarpeta}");
            }
            padreFinal = carpetaExistente.transform;
            cacheCarpetasNombre[claveCache] = padreFinal;
        }

        if (go.transform.parent == padreFinal || go == padreFinal.gameObject) { _cUbicados++; return; }

        // Mover preservando coordenadas del mundo intactas
        Undo.SetTransformParent(go.transform, padreFinal, $"Mover Universal {go.name}");
        _cMovidos++;
    }

    private string ExtraerNombreLimpioBase(string nombreOriginal)
    {
        string n = nombreOriginal;
        if (n.Contains("(")) n = n.Split('(')[0];
        if (n.Contains("Instance")) n = n.Replace("Instance", "");
        return n.Trim();
    }

    private GameObject ObtenerOCrearRaiz(string nombre, int grupoUndo)
    {
        foreach (var go in SceneManager.GetActiveScene().GetRootGameObjects()) if (go.name == nombre) return go;
        GameObject nuevo = new GameObject(nombre);
        nuevo.transform.position = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(nuevo, $"Crear {nombre}");
        return nuevo;
    }

    private void CrearEstructurasEstaticas(string raiz, string[] subs, Dictionary<string, GameObject> mapaRaices, Dictionary<string, Dictionary<string, GameObject>> mapaSubs, int grupoUndo)
    {
        if (!mapaRaices.TryGetValue(raiz, out var goRaiz)) return;
        
        if (!mapaSubs.TryGetValue(raiz, out var dic))
        {
            dic = new Dictionary<string, GameObject>();
            mapaSubs[raiz] = dic;
        }

        foreach (var s in subs)
        {
            GameObject subGO = null;
            for (int i = 0; i < goRaiz.transform.childCount; i++)
            {
                if (goRaiz.transform.GetChild(i).name == s) { subGO = goRaiz.transform.GetChild(i).gameObject; break; }
            }

            if (subGO == null)
            {
                subGO = new GameObject(s);
                subGO.transform.SetParent(goRaiz.transform, false);
                subGO.transform.localPosition = Vector3.zero;
                Undo.RegisterCreatedObjectUndo(subGO, $"Crear sub {s}");
            }
            dic[s] = subGO;
        }
    }

    private (string raiz, string sub) DeterminarDestino(string nombre)
    {
        string lower = nombre.ToLowerInvariant();
        foreach (var regla in REGLAS)
        {
            foreach (var palabra in regla.Palabras) if (lower.Contains(palabra)) return (regla.Raiz, regla.SubCarpeta);
        }
        return (null, null);
    }

    private bool EsContenedor(string nombre)
    {
        foreach (var r in RAICES) if (r == nombre) return true;
        foreach (var sub in SUBS_SISTEMA) if (sub == nombre) return true;
        foreach (var sub in SUBS_PERSONAJES) if (sub == nombre) return true;
        foreach (var sub in SUBS_ILUMINACION) if (sub == nombre) return true;
        foreach (var sub in SUBS_TRAFICO) if (sub == nombre) return true;
        foreach (var sub in SUBS_ENTORNO) if (sub == nombre) return true;
        return false;
    }

    private void FinalizarOperacion(Scene escena, int grupoUndo)
    {
        EditorSceneManager.MarkSceneDirty(escena);
        Undo.CollapseUndoOperations(grupoUndo);
        Log("──────────────────────────────────────────────────────────────");
        Log($"📊 INFORME DE SEPARACIÓN QUIRÚRGICA UNIVERSAL:");
        Log($"   ✅ Clones empaquetados y aislados por tipo: {_cMovidos}");
        Log($"   ✨ Elementos ya validados en posición:       {_cUbicados}");
        Log($"   ⚠  Objetos sin regla (permanecen sueltos):   {_cSinRegla}");
        Log("──────────────────────────────────────────────────────────────");
    }

    private void Log(string msg) => _log.Add(msg);

    private class ReglaQuirurgica
    {
        public readonly string Raiz;
        public readonly string SubCarpeta;
        public readonly string[] Palabras;
        public ReglaQuirurgica(string raiz, string sub, string[] palabras) { Raiz = raiz; SubCarpeta = sub; Palabras = palabras; }
    }
}
