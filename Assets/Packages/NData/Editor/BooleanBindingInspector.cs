using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BooleanBinding), true)]
[CanEditMultipleObjects]
public class BooleanBindingInspector : Editor {
    private Type type;
    private String typeName;
    public static Assembly assembly;

    public void OnEnable() {
        if (assembly == null) {
            assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.StartsWith("Assembly-CSharp,"));
        }
    }

    public override void OnInspectorGUI() {
        var binding = (BooleanBinding)target;

        if (binding.CheckType == BooleanBinding.CHECK_TYPE.EQUAL_TO_STRING) {
            if (targets.Length == 1) {
                DrawDefaultInspector();
                binding.StringReference = EditorGUILayout.TextField("String Reference", binding.StringReference);
                binding.EvaluateExpression = false;
            }
        }

        else if (binding.CheckType == BooleanBinding.CHECK_TYPE.ENUM) {
            DrawDefaultInspector();

            if (targets.Length == 1) {
                EditorGUILayout.Space();
                DrawEnumBindingFields(binding);
            }
        } 
        
        else {
            DrawDefaultInspector();
            binding.StringReference = "";
            binding.EnumType = "";
            if (binding.EnumValues != null) {
                binding.EnumValues.Clear();
            }
        }
    }

    private void DrawEnumBindingFields(BooleanBinding binding) {
        if (String.IsNullOrEmpty(typeName)) {
            type = GetType(binding.EnumType);

            if (type == null) {
                String tpName = binding.EnumType;
                int idx = tpName.LastIndexOf('.');
                if (idx >= 0) {
                    String tp = tpName.Substring(idx+1, tpName.Length - idx - 1);
                    Type[] types = GetTypeByName(tp);
                    if (types.Length == 1) {
                        type = types[0];
                    }
                }
            }

            if (type != null) {
                if (GetTypeByName(type.Name).Length > 1) {
                    typeName = type.FullName;
                } else {
                    typeName = type.Name;
                }
            } 
        }

        typeName = EditorGUILayout.TextField("Type", typeName);

        if (type == null || (type != null && type.Name != typeName && type.FullName != typeName)) {
            if (!String.IsNullOrEmpty(typeName)) {
                if (typeName.Contains(".")) {
                    type = GetType(typeName);
                    if (type == null) {
                        ShowWarning("Type not found");
                    } else {
                        binding.EnumType = type.FullName;
                    }
                } else {
                    Type[] types = GetTypeByName(typeName);
                    if (types.Length == 0) {
                        ShowWarning("Type not found");
                        type = null;
                    } else if (types.Length > 1) {
                        ShowWarning("There are more than 1 type");
                        type = null;
                    } else {
                        type = types[0];
                        binding.EnumType = type.FullName;
                    }
                }
            }
        }

        if (type != null) {
            EditorGUILayout.BeginVertical("Box");
            var enumValues = Enum.GetValues(type);
            foreach (var enumValue in enumValues) {
                String valueName = enumValue.ToString();
                bool oldValue = binding.EnumValues.Contains((int) enumValue);
                bool newValue = EditorGUILayout.Toggle(valueName, oldValue);

                if (newValue != oldValue) {
                    if (newValue) {
                        binding.EnumValues.Add((int) enumValue);
                    } else {
                        binding.EnumValues.Remove((int) enumValue);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        } else {
            binding.EnumValues.Clear();
        }
    }

    private void ShowWarning(String message) {
        GUI.color = Color.yellow;
        EditorGUILayout.HelpBox(message, MessageType.Warning, true);
        GUI.color = Color.white;
    }

    private static Type[] GetTypeByName(String name) {
        List<Type> returnVal = new List<Type>();

        Type[] assemblyTypes = assembly.GetTypes();
        for (int j = 0; j < assemblyTypes.Length; j++) {
            if (assemblyTypes[j].Name == name && assemblyTypes[j].IsEnum) {
                returnVal.Add(assemblyTypes[j]);
            }
        }

        return returnVal.ToArray();
    }

    private static Type GetType(String name) {
        if (String.IsNullOrEmpty(name)) return null;

        var type = assembly.GetType(name);
        if (type != null && type.IsEnum) return type;
        return null;
    }
}
