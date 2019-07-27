using multilanguage;
using System.IO;
using System.Text;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace mk.editor.multilanguage
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MultiTextMeshPro))]
    public class MultiTextMeshProEditor : TMP_UiEditorPanel
    {
        public const string FileExtension = ".txt";
        public static readonly string MultiLanguagePath = "/Resources/Config/Multilanguage/";
        public static readonly string OutputPath = "/Resources2/MultiLanguage/";
        public static readonly string[] languages = new string[] { "EN", "CN", "TW" };

        [MenuItem("GameObject/MKUI/MultiTextMeshPro", false, 0)]
        public static void AddText()
        {
            // Create a custom game object
            GameObject go = new GameObject("MultiTextMeshPro");

            // Set the selection object as the parent 
            GameObjectUtility.SetParentAndAlign(go, Selection.activeGameObject);

            // Register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
            go.AddComponent<RectTransform>();
            go.AddComponent<MultiTextMeshPro>();
        }

        [MenuItem("Tools/UI/Merge Language Sheet")]
        public static void GenerateFontAtlas()
        {
            for (int i=0; i<languages.Length; i++)
            {
                var targetPath = Application.dataPath + OutputPath + "All_" + languages[i] + FileExtension;
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.Create(targetPath).Close();

                // TODO: 可以優化
                var files = Directory.GetFiles(Application.dataPath + MultiLanguagePath + "BaseString" + FileExtension);
                string text = File.ReadAllText(files[0]);
                File.AppendAllText(targetPath, text, Encoding.UTF8);

                files = Directory.GetFiles(Application.dataPath + MultiLanguagePath, "*" + languages[i] + FileExtension);
                for (int j = 0; j < files.Length; j++)
                {
                    text = File.ReadAllText(files[j]);
                    File.AppendAllText(targetPath, text, Encoding.UTF8);
                }
                File.AppendAllText(targetPath, "#", Encoding.UTF8);
            }
            AssetDatabase.Refresh();
        }

        protected SerializedProperty materialProp = null;

        protected MultiTextMeshPro textMeshPro;
        protected LanguageMgr.FileNames preFile = LanguageMgr.FileNames.None;
        protected string preKey = string.Empty;
        protected int preLanguage = 0;
        protected Material perMaterial = null;

        protected void Awake()
        {
            textMeshPro = target as MultiTextMeshPro;
            materialProp = serializedObject.FindProperty("SpecificMaterial");
        }

        public override void OnInspectorGUI()
        {
            if (preKey != textMeshPro.Key || preFile != textMeshPro.FileName)
            {
                LanguageMgr.LoadModule(textMeshPro.FileName);
                textMeshPro.GetStringAndSetText(textMeshPro.Key, textMeshPro.FileName);
                preKey = textMeshPro.Key;
                preFile = textMeshPro.FileName;
            }

            textMeshPro.Key = EditorGUILayout.TextField("Key ID", textMeshPro.Key);
            textMeshPro.FileName = (LanguageMgr.FileNames)EditorGUILayout.EnumPopup("File Name", textMeshPro.FileName);

            if (preLanguage != textMeshPro.ChooseLanguage)
            {
                LanguageMgr.instance.MLang = textMeshPro.ChooseLanguage;
                LanguageMgr.instance.Init();
                LanguageMgr.LoadModule(textMeshPro.FileName);
                textMeshPro.GetStringAndSetText(textMeshPro.Key, textMeshPro.FileName);
                preLanguage = textMeshPro.ChooseLanguage;
            }
            textMeshPro.ChooseLanguage = EditorGUILayout.Popup("Language", textMeshPro.ChooseLanguage, LanguageMgr.instance.LanguageList);

            EditorGUILayout.PropertyField(materialProp);
            if (materialProp.objectReferenceValue != textMeshPro.SpecificMaterial)
            {
                serializedObject.ApplyModifiedProperties();
                textMeshPro.SetMultiLanguageMaterial();
            }

            base.OnInspectorGUI();
        }
    }
}