using UnityEngine;
using UnityEditor;

public class YuME_editorConfig : EditorWindow
{
    public static YuME_editorData editorData;

    [MenuItem("Window/Yuponic/Utils/Editor Config")]
    static void Initialize()
    {
        YuME_editorConfig editorConfig = EditorWindow.GetWindow<YuME_editorConfig>(true, "Editor Config");
        editorConfig.titleContent.text = "Editor Config";
    }

    void OnEnable()
    {
        editorData = ScriptableObject.CreateInstance<YuME_editorData>();
        string[] guids = AssetDatabase.FindAssets("YuME_editorSetupData");
        editorData = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(YuME_editorData)) as YuME_editorData;
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Cursor Color Settings", EditorStyles.boldLabel);

        editorData.brushCursorColor = EditorGUILayout.ColorField("Brush Cursor Color", editorData.brushCursorColor);
        editorData.pickCursorColor = EditorGUILayout.ColorField("Brush Picker Color", editorData.pickCursorColor);
        editorData.eraseCursorColor = EditorGUILayout.ColorField("Brush Erase Color", editorData.eraseCursorColor);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Icon Size", EditorStyles.boldLabel);
        editorData.iconWidth = (int)EditorGUILayout.Slider("Icon Size", editorData.iconWidth, 16f, 64f);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Invert Mouse wheel scrolling", EditorStyles.boldLabel);
        editorData.invertMouseWheel = EditorGUILayout.Toggle("Invert Mouse Wheel", editorData.invertMouseWheel);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Global Scale Setting", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Warning. Changing the grid scale will have a knock on effect to all your maps. The default scale is 1. If you are seeing issues with your maps, please reset the scale.", MessageType.Warning);
        editorData.gridScaleFactor = EditorGUILayout.Slider("Grid Size", editorData.gridScaleFactor, 1f, 48f);
        editorData.gridScaleFactor = editorData.gridScaleFactor * 2;
        editorData.gridScaleFactor = Mathf.Round(editorData.gridScaleFactor) / 2;
        if(editorData.gridScaleFactor < 1) { editorData.gridScaleFactor = 1; }
        if(editorData.gridScaleFactor > 48) { editorData.gridScaleFactor = 48; }


        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Grid Color Settings", EditorStyles.boldLabel);

        editorData.gridColorNormal = EditorGUILayout.ColorField("Grid Color", editorData.gridColorNormal);
        editorData.gridColorFill = EditorGUILayout.ColorField("Fill Color", editorData.gridColorFill);
        editorData.gridColorBorder = EditorGUILayout.ColorField("Border Color", editorData.gridColorBorder);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Grid Offset", EditorStyles.boldLabel);

        YuME_mapEditor.gridOffset = (float)EditorGUILayout.Slider("Grid Offset", YuME_mapEditor.gridOffset, -0.25f, 0.25f);

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Layer Names", EditorStyles.boldLabel);

        for (int i = 0; i < YuME_mapEditor.editorData.layerNames.Count; i++)
        {
            YuME_mapEditor.editorData.layerNames[i] = EditorGUILayout.TextField("Layer " + (i + 1),YuME_mapEditor.editorData.layerNames[i]);
        }
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }

    }
}
