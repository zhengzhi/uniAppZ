using UnityEngine;
using System.Collections;
using UnityEditor;

namespace multilanguage
{
    [CustomEditor(typeof(MultiMeshText))]
    public class MultiMeshTextEditor : Editor
    {
        string preKey = "";
        LanguageMgr.FileNames preFile = LanguageMgr.FileNames.None;
        int preLanguage = 0;

        public override void OnInspectorGUI()
        {
            MultiMeshText multitext = target as MultiMeshText;

            if (preKey != multitext.Key || preFile != multitext.FileName)
            {
                LanguageMgr.LoadModule(multitext.FileName);
                multitext.text = multitext.GetStringAndSetText();
                preKey = multitext.Key;
                preFile = multitext.FileName;
            }
            multitext.Key = EditorGUILayout.TextField("Key ID : ", multitext.Key);
            multitext.FileName = (LanguageMgr.FileNames)EditorGUILayout.EnumPopup("File Name : ", multitext.FileName);

            if (preLanguage != multitext.ChooseLanguage)
            {
                LanguageMgr.instance.MLang = multitext.ChooseLanguage;
                LanguageMgr.instance.Init();
                LanguageMgr.LoadModule(multitext.FileName);
                multitext.text = multitext.GetStringAndSetText();
                preLanguage = multitext.ChooseLanguage;
            }
            multitext.ChooseLanguage = EditorGUILayout.Popup(multitext.ChooseLanguage, LanguageMgr.instance.LanguageList);
        }

    }
}
