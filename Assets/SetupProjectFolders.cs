// ============================================================
//  OrganizadorProyecto.cs
//  Colocar en: Assets/Editor/OrganizadorProyecto.cs
//
//  Acceso: barra superior Unity  →  Tools → Organizar Proyecto
//
//  REGLAS IMPLEMENTADAS:
//  1. Nunca toca: Convai, SyntyStudios, TextMesh Pro, XR, XRI,
//     VRTemplateAssets, Samples, Bublisher, ithappy, StarterAssets,
//     Plugins, Settings, Resources, ThirdParty, TutorialInfo.
//  2. Usa AssetDatabase.MoveAsset() — mueve el .meta automáticamente.
//  3. Crea (si falta) la carpeta _Project con la estructura limpia.
//  4. Mudanza inteligente por tipo/nombre de archivo.
// ============================================================

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class OrganizadorProyecto : EditorWindow
{
    // ── Resultado del proceso ─────────────────────────────────────────────
    private static readonly List<string> _log = new List<string>();
    private Vector2 _scroll;

    // ── Carpetas de terceros / SDKs que JAMÁS se tocan ───────────────────
    private static readonly HashSet<string> CARPETAS_PROTEGIDAS = new HashSet<string>(
        System.StringComparer.OrdinalIgnoreCase)
    {
        "Convai", "SyntyStudios", "TextMesh Pro", "XR", "XRI",
        "VRTemplateAssets", "Samples", "Bublisher", "ithappy",
        "StarterAssets", "Plugins", "Settings", "Resources",
        "ThirdParty", "TutorialInfo", "_Project", "_Recovery",
        "Editor"   // Evitar mover scripts de Editor ya ubicados
    };

    // ── Heurística nombre → subcarpeta de Scripts ────────────────────────
    private static readonly Dictionary<string, string> NOMBRE_A_SCRIPTS_SUBCARPETA =
        new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
    {
        // Managers
        { "MainMenu",        "Managers" },
        { "Pause",           "Managers" },
        { "CinemachineBrain","Managers" },
        { "ZoomCamera",      "Managers" },
        // Gameplay
        { "Teleport",        "Gameplay" },
        { "CreatureFlee",    "Gameplay" },
        { "CreatureGrabable","Gameplay" },
    };

    // =====================================================================
    //  MENÚ
    // =====================================================================
    [MenuItem("Tools/Organizar Proyecto")]
    public static void AbrirVentana()
    {
        var ventana = GetWindow<OrganizadorProyecto>("Organizar Proyecto");
        ventana.minSize = new Vector2(520, 420);
        ventana.Show();
    }

    // =====================================================================
    //  GUI
    // =====================================================================
    private void OnGUI()
    {
        GUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Este script mueve los archivos sueltos de Assets/ hacia Assets/_Project/\n" +
            "usando AssetDatabase.MoveAsset() para conservar todas las referencias.\n\n" +
            "⚠  Haz un COMMIT / BACKUP antes de continuar.",
            MessageType.Warning);

        GUILayout.Space(6);

        if (GUILayout.Button("▶  Ejecutar reorganización", GUILayout.Height(36)))
        {
            _log.Clear();
            EjecutarReorganizacion();
        }

        GUILayout.Space(6);
        EditorGUILayout.LabelField("Log de operaciones:", EditorStyles.boldLabel);

        _scroll = EditorGUILayout.BeginScrollView(_scroll,
            GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        foreach (var linea in _log)
        {
            bool esError  = linea.StartsWith("❌");
            bool esOk     = linea.StartsWith("✅");
            bool esInfo   = linea.StartsWith("──");

            GUIStyle estilo = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                normal   = { textColor = esError ? Color.red
                                       : esOk    ? new Color(0.2f, 0.8f, 0.2f)
                                       : esInfo  ? Color.cyan
                                                 : EditorStyles.label.normal.textColor }
            };
            GUILayout.Label(linea, estilo);
        }

        EditorGUILayout.EndScrollView();

        if (_log.Count > 0 && GUILayout.Button("Limpiar log"))
            _log.Clear();
    }

    // =====================================================================
    //  LÓGICA PRINCIPAL
    // =====================================================================
    private static void EjecutarReorganizacion()
    {
        Log("── INICIO DE REORGANIZACIÓN ─────────────────────────────────");

        // 1. Crear estructura _Project
        CrearEstructura();

        // 2. Mover archivos sueltos en Assets/
        MoverArchivosSueltosEnRaiz();

        // 3. Mover contenido de Assets/Scripts/
        MoverScriptsFolderExistente();

        // 4. Mover contenido de Assets/Scenes/
        MoverScenesFolderExistente();

        // 5. Mover contenido de Assets/Materials/ (raíz)
        MoverMaterialsFolderExistente();

        // 6. Mover contenido de Assets/Images/
        MoverImagesFolderExistente();

        // 7. Mover modelos 3D sueltos (hospital7.*)
        MoverModelosSueltos();

        // 8. Mover Assets/Texturas/
        MoverTexturasFolderExistente();

        // 9. Refrescar base de datos
        AssetDatabase.Refresh();
        Log("── REORGANIZACIÓN COMPLETADA ────────────────────────────────");
        Log($"   Total de operaciones: {_log.Count} líneas en log.");

        Debug.Log("[OrganizadorProyecto] Reorganización completada. Revisa el log en la ventana.");
    }

    // =====================================================================
    //  PASO 1: Crear estructura _Project
    // =====================================================================
    private static void CrearEstructura()
    {
        Log("── Creando estructura _Project ...");
        string[] carpetas = {
            "Assets/_Project",
            "Assets/_Project/Animations",
            "Assets/_Project/Audio",
            "Assets/_Project/Audio/Music",
            "Assets/_Project/Audio/SFX",
            "Assets/_Project/Prefabs",
            "Assets/_Project/Scenes",
            "Assets/_Project/Scenes/_Dev",
            "Assets/_Project/Scenes/Production",
            "Assets/_Project/Scripts",
            "Assets/_Project/Scripts/AI",
            "Assets/_Project/Scripts/Managers",
            "Assets/_Project/Scripts/Gameplay",
            "Assets/_Project/Shaders",
            "Assets/_Project/Materials",
            "Assets/_Project/Textures",
        };

        foreach (var carpeta in carpetas)
        {
            if (!AssetDatabase.IsValidFolder(carpeta))
            {
                // Crear la carpeta padre / hija
                string parent = Path.GetDirectoryName(carpeta).Replace('\\', '/');
                string child  = Path.GetFileName(carpeta);
                string guid   = AssetDatabase.CreateFolder(parent, child);
                Log(guid.Length > 0
                    ? $"✅ Carpeta creada: {carpeta}"
                    : $"❌ No se pudo crear: {carpeta}");
            }
        }
    }

    // =====================================================================
    //  PASO 2: Archivos sueltos directamente en Assets/
    //          (sin subcarpeta, es decir, son hijos directos)
    // =====================================================================
    private static void MoverArchivosSueltosEnRaiz()
    {
        Log("── Archivos sueltos en Assets/ ...");

        // Sólo archivos directos (no recursivo)
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets" });

        foreach (var guid in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);

            // Sólo archivos en la raíz inmediata de Assets
            if (!EsHijoDirectoDeAssets(ruta)) continue;
            if (AssetDatabase.IsValidFolder(ruta))   continue;  // es carpeta
            if (EsMetaFile(ruta))                    continue;  // .meta lo maneja Unity

            string nombre    = Path.GetFileName(ruta);
            string extension = Path.GetExtension(ruta).ToLower();
            string destino   = DeterminarDestino(ruta, nombre, extension, EsEnAssets: true);

            if (destino != null)
                MoverAsset(ruta, destino + "/" + nombre);
        }
    }

    // =====================================================================
    //  PASO 3: Mover Assets/Scripts/ → _Project/Scripts/
    // =====================================================================
    private static void MoverScriptsFolderExistente()
    {
        const string ORIGEN = "Assets/Scripts";
        if (!AssetDatabase.IsValidFolder(ORIGEN)) return;
        Log("── Moviendo Assets/Scripts/ ...");

        string[] guids = AssetDatabase.FindAssets("t:Script", new[] { ORIGEN });

        foreach (var guid in guids)
        {
            string ruta   = AssetDatabase.GUIDToAssetPath(guid);
            string nombre = Path.GetFileName(ruta);
            string ext    = Path.GetExtension(ruta).ToLower();
            if (ext != ".cs") continue;

            string sub    = InferirSubcarpetaScript(nombre);
            string destino = $"Assets/_Project/Scripts/{sub}/{nombre}";
            MoverAsset(ruta, destino);
        }
    }

    // =====================================================================
    //  PASO 4: Mover Assets/Scenes/ → _Project/Scenes/Production/
    //          (sólo las escenas de trabajo, no tocar las de SDKs)
    // =====================================================================
    private static void MoverScenesFolderExistente()
    {
        const string ORIGEN = "Assets/Scenes";
        if (!AssetDatabase.IsValidFolder(ORIGEN)) return;
        Log("── Moviendo Assets/Scenes/ ...");

        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { ORIGEN });

        foreach (var guid in guids)
        {
            string ruta   = AssetDatabase.GUIDToAssetPath(guid);
            string nombre = Path.GetFileName(ruta);
            // Escenas de Convai / Samples ya están en sus carpetas; aquí
            // sólo llegan las de Assets/Scenes/ por el filtro de búsqueda
            string destino = $"Assets/_Project/Scenes/Production/{nombre}";
            MoverAsset(ruta, destino);
        }

        // Mover también la escena suelta MainMenu.unity que está en la raíz
        MoverSiExiste("Assets/MainMenu.unity",
                      "Assets/_Project/Scenes/Production/MainMenu.unity");
    }

    // =====================================================================
    //  PASO 5: Mover Assets/Materials/ → _Project/Materials/
    // =====================================================================
    private static void MoverMaterialsFolderExistente()
    {
        const string ORIGEN = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(ORIGEN)) return;
        Log("── Moviendo Assets/Materials/ ...");

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { ORIGEN });

        foreach (var guid in guids)
        {
            string ruta   = AssetDatabase.GUIDToAssetPath(guid);
            string nombre = Path.GetFileName(ruta);
            MoverAsset(ruta, $"Assets/_Project/Materials/{nombre}");
        }
    }

    // =====================================================================
    //  PASO 6: Mover Assets/Images/ →
    //          .png / .jpg  → _Project/Textures/
    //          .mat         → _Project/Materials/
    // =====================================================================
    private static void MoverImagesFolderExistente()
    {
        const string ORIGEN = "Assets/Images";
        if (!AssetDatabase.IsValidFolder(ORIGEN)) return;
        Log("── Moviendo Assets/Images/ ...");

        // Imágenes (incluyendo las de la subcarpeta Images/Materials/)
        string[] guids = AssetDatabase.FindAssets("", new[] { ORIGEN });

        foreach (var guid in guids)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(guid);
            if (AssetDatabase.IsValidFolder(ruta)) continue;

            string nombre = Path.GetFileName(ruta);
            string ext    = Path.GetExtension(ruta).ToLower();

            string carpetaDestino = ext == ".mat"
                ? "Assets/_Project/Materials"
                : "Assets/_Project/Textures";

            MoverAsset(ruta, $"{carpetaDestino}/{nombre}");
        }
    }

    // =====================================================================
    //  PASO 7: Modelos sueltos (hospital7.fbx / hospital7.prefab)
    // =====================================================================
    private static void MoverModelosSueltos()
    {
        Log("── Moviendo modelos 3D sueltos ...");
        MoverSiExiste("Assets/hospital7.fbx",    "Assets/_Project/Prefabs/hospital7.fbx");
        MoverSiExiste("Assets/hospital7.prefab", "Assets/_Project/Prefabs/hospital7.prefab");
    }

    // =====================================================================
    //  PASO 8: Assets/Texturas/ → _Project/Textures/
    // =====================================================================
    private static void MoverTexturasFolderExistente()
    {
        const string ORIGEN = "Assets/Texturas";
        if (!AssetDatabase.IsValidFolder(ORIGEN)) return;
        Log("── Moviendo Assets/Texturas/ ...");

        string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { ORIGEN });

        foreach (var guid in guids)
        {
            string ruta   = AssetDatabase.GUIDToAssetPath(guid);
            string nombre = Path.GetFileName(ruta);
            MoverAsset(ruta, $"Assets/_Project/Textures/{nombre}");
        }
    }

    // =====================================================================
    //  UTILIDADES
    // =====================================================================

    /// <summary>
    /// Determina la carpeta de destino de un archivo suelto en la raíz de Assets/.
    /// Devuelve null si no debe moverse.
    /// </summary>
    private static string DeterminarDestino(string ruta, string nombre, string ext,
                                             bool EsEnAssets = false)
    {
        // Archivos que no queremos tocar
        if (nombre == "SetupProjectFolders.cs") return null;
        if (ext == ".cs" && EsEnAssets)
        {
            string sub = InferirSubcarpetaScript(nombre);
            return $"Assets/_Project/Scripts/{sub}";
        }
        if (ext == ".unity") return "Assets/_Project/Scenes/Production";
        if (ext == ".prefab") return "Assets/_Project/Prefabs";
        if (ext == ".fbx")   return "Assets/_Project/Prefabs";
        if (ext == ".mat")   return "Assets/_Project/Materials";
        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".exr")
            return "Assets/_Project/Textures";
        if (ext == ".anim")  return "Assets/_Project/Animations";
        if (ext == ".shader" || ext == ".shadergraph" || ext == ".compute")
            return "Assets/_Project/Shaders";

        // No se mueve (asset desconocido, .asset, .txt, etc.)
        return null;
    }

    /// <summary>
    /// Deduce la subcarpeta de Scripts en función del nombre del archivo.
    /// Reglas: Managers > AI > Gameplay (por defecto).
    /// </summary>
    private static string InferirSubcarpetaScript(string nombreConExtension)
    {
        string sinExt = Path.GetFileNameWithoutExtension(nombreConExtension);

        if (NOMBRE_A_SCRIPTS_SUBCARPETA.TryGetValue(sinExt, out string sub))
            return sub;

        // Heurística por sufijos comunes
        string lower = sinExt.ToLower();
        if (lower.Contains("manager") || lower.Contains("menu")  ||
            lower.Contains("controller") || lower.Contains("camera") ||
            lower.Contains("pause")   || lower.Contains("settings"))
            return "Managers";

        if (lower.Contains("ai") || lower.Contains("npc") || lower.Contains("brain") ||
            lower.Contains("behavior") || lower.Contains("flee") ||
            lower.Contains("wander"))
            return "AI";

        // Por defecto → Gameplay
        return "Gameplay";
    }

    /// <summary>
    /// Mueve un asset usando AssetDatabase.MoveAsset().
    /// Crea la carpeta destino si no existe.
    /// </summary>
    private static void MoverAsset(string origen, string destino)
    {
        if (!File.Exists(origen) && !AssetDatabase.IsValidFolder(origen))
        {
            Log($"❌ Origen no existe: {origen}");
            return;
        }

        // Si ya está en el lugar correcto, no hacer nada
        if (origen == destino) return;

        // Si el destino ya existe, añadir sufijo para no sobreescribir
        if (File.Exists(destino))
        {
            Log($"⚠  Ya existe en destino, se omite: {destino}");
            return;
        }

        // Asegurar que la carpeta de destino existe
        string carpetaDestino = Path.GetDirectoryName(destino).Replace('\\', '/');
        CrearCarpetaRecursiva(carpetaDestino);

        string error = AssetDatabase.MoveAsset(origen, destino);

        if (string.IsNullOrEmpty(error))
            Log($"✅ {origen}  →  {destino}");
        else
            Log($"❌ Error moviendo '{origen}': {error}");
    }

    /// <summary>Shortcut para mover si el archivo existe.</summary>
    private static void MoverSiExiste(string origen, string destino)
    {
        if (File.Exists(origen))
            MoverAsset(origen, destino);
    }

    /// <summary>
    /// Crea recursivamente la carpeta usando AssetDatabase.CreateFolder().
    /// </summary>
    private static void CrearCarpetaRecursiva(string carpeta)
    {
        if (AssetDatabase.IsValidFolder(carpeta)) return;

        string padre = Path.GetDirectoryName(carpeta).Replace('\\', '/');
        CrearCarpetaRecursiva(padre);

        string nombre = Path.GetFileName(carpeta);
        AssetDatabase.CreateFolder(padre, nombre);
    }

    /// <summary>
    /// Devuelve true si la ruta es un hijo directo de Assets/
    /// (no está anidado en ninguna subcarpeta).
    /// </summary>
    private static bool EsHijoDirectoDeAssets(string ruta)
    {
        // "Assets/algo.cs"  →  sí
        // "Assets/Scripts/algo.cs" →  no
        if (!ruta.StartsWith("Assets/")) return false;
        string sinPrefijo = ruta.Substring("Assets/".Length);
        return !sinPrefijo.Contains('/');
    }

    private static bool EsMetaFile(string ruta) =>
        ruta.EndsWith(".meta", System.StringComparison.OrdinalIgnoreCase);

    private static void Log(string msg)
    {
        _log.Add(msg);
        // También lo enviamos a la consola para que quede en el log de Unity
        if (msg.StartsWith("❌"))
            Debug.LogWarning("[Organizar] " + msg);
        else
            Debug.Log("[Organizar] " + msg);
    }
}
