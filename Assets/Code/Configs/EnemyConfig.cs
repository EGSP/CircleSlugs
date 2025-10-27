using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    public Enemy Prefab;

    /// <summary>
    /// То насколько мощный этот враг в разрезе всей игры. 
    /// Число используется системой спавна.
    /// </summary>
    public int Power;
}

[CustomEditor(typeof(EnemyConfig))]
public class EnemyConfigEditor : Editor
{
    private Editor prefabEditor;

    public override void OnInspectorGUI()
    {
        // Рисуем стандартные поля
        DrawDefaultInspector();

        EnemyConfig config = (EnemyConfig)target;

        if (config.Prefab != null)
        {
            GUILayout.Space(10);
            GUILayout.Label("Prefab Preview", EditorStyles.boldLabel);

            if (prefabEditor == null || prefabEditor.target != config.Prefab.gameObject)
            {
                if (prefabEditor != null)
                    DestroyImmediate(prefabEditor);

                prefabEditor = Editor.CreateEditor(config.Prefab.gameObject);
            }

            if (prefabEditor != null)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                // Интерактивное превью - можно вращать мышкой!
                prefabEditor.OnInteractivePreviewGUI(
                    GUILayoutUtility.GetRect(256, 256),
                    EditorStyles.helpBox
                );
                GUILayout.EndVertical();
            }
        }
    }

    private void OnDisable()
    {
        // Очищаем редактор при закрытии инспектора
        if (prefabEditor != null)
        {
            DestroyImmediate(prefabEditor);
            prefabEditor = null;
        }
    }
}