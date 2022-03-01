using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Metadata.Extension.Editor
{
    public class AddMeshEditor : EditorWindow
    {
        [SerializeField]
        private List<string> keys, values;

        [MenuItem("Tools/Add mesh collider to mesh")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            AddMeshEditor window = (AddMeshEditor)GetWindow(typeof(AddMeshEditor), false, "Add mesh collider to mesh", true);
            window.Show();
        }

        private void OnEnable()
        {
            keys ??= new List<string>();
            values ??= new List<string>();
        }

        private void OnGUI()
        {
            GUILayout.Label("Enter the key and value below:", EditorStyles.boldLabel);
            SerializedObject so = new SerializedObject(this);
            so.Update();
            var keySerializedObj = so.FindProperty("keys");
            var valueSerializedObj = so.FindProperty("values");
            float defaultLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50f;
            EditorGUILayout.Space();
            for (int i = 0; i < keys.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var existingElement = keySerializedObj.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(existingElement, new GUIContent($"Key {i + 1}"));
                GUILayout.Space(10f);
                existingElement = valueSerializedObj.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(existingElement, new GUIContent($"Value {i + 1}"));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUIUtility.labelWidth = defaultLabelWidth;
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            
           
            
            if (GUILayout.Button("Add"))
            {
                keys ??= new List<string>();
                keys.Add(string.Empty);
                values ??= new List<string>();
                values.Add(string.Empty);
            }
            
            GUI.enabled = keys.Count > 0;

            if (GUILayout.Button("Remove last"))
            {
                keys ??= new List<string>();
                keys.RemoveAt(keys.Count - 1);
                values ??= new List<string>();
                values.RemoveAt(values.Count - 1);
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            so.ApplyModifiedProperties(); // Remember to apply modified properties
            so.Update();
            GUI.enabled = keys.Count > 0 && !keys.Any(string.IsNullOrEmpty) && !values.Any(string.IsNullOrEmpty);
            if (GUILayout.Button("Add Mesh Collider"))
            {
                var allMetaDataObjs = FindObjectsOfType<UnityEngine.Reflect.Metadata>();
                foreach (var t in allMetaDataObjs)
                {
                    if (t.GetComponent<MeshRenderer>() == null) continue;
                    if (t.parameters.Keys.Any(x => keys.Any(y => y == x)))
                    {
                        for (var j = 0; j < keys.Count; j++)
                        {
                            if(!t.parameters.ContainsKey(keys[j])) continue;
                            var value = t.parameters[keys[j]].value;
                            if (!string.Equals(values[j], value) || t.GetComponent<MeshCollider>() != null) continue;
                            t.gameObject.AddComponent<MeshCollider>();
                        }
                    }
                }
    #if PIXYZ
                var piXYZMetaData = FindObjectsOfType<Pixyz.ImportSDK.Metadata>();
                foreach (var t in piXYZMetaData)
                {
                    if (t.GetComponent<MeshRenderer>() == null) continue;
                    
                    if (t.getProperties().Keys.Any(x => keys.Any(y => string.Equals(y, x))))
                    {
                        for (var i = 0; i < keys.Count; i++)
                        {
                            if(!t.getProperties().ContainsKey(keys[i])) continue;
                            var value = t.getProperty(keys[i]);
                            if (!string.Equals(values[i], value) || t.GetComponent<MeshCollider>() != null) continue;
                            t.gameObject.AddComponent<MeshCollider>();
                        }
                    }
                }
    #endif
            }
        }
    }
}

