using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;


[CustomEditor (typeof (EditorStyleScriptable))]
public class EditorStyleEditor : Editor
    {
    private enum ExampleType { BUTTON, LABEL, POPUP, TEXT_FIELD, TOGGLE, SELECTABLE }
    private ExampleType exampleType = ExampleType.LABEL;

    // === Reference ===
    private EditorStyleScriptable cache;
    private SerializedProperty styleProperty;
    private List<SerializedProperty> serializedStyles;

    // === Display ===
    private readonly string exampleText = "GUIStyle Example";
    private string styleLookupTest;
    private GUIStyle styleLookup;

    // === Reflection ===
    private EditorStyles stylesClass;

    private Type styleType = typeof (EditorStyles);
    private FieldInfo[] styleFields;

    private List<GUIStyle> foundStyles;

    private readonly BindingFlags methodBindings = BindingFlags.NonPublic | BindingFlags.Instance;
    private readonly BindingFlags fieldBindings = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

    // === Helpers ===
    private bool HasStyles => cache?.StyleCount > 0;

    private void OnEnable()
        {
        cache = target as EditorStyleScriptable;

        foundStyles = new List<GUIStyle> ();
        foreach (var style in cache.GetStyles ())
            foundStyles.Add (style.style);
        }

    public override void OnInspectorGUI()
        {
        serializedObject.UpdateIfRequiredOrScript ();

        if (styleProperty == null)
            {
            styleProperty = serializedObject.FindProperty ("editorStyles");
            CacheSerializedStyles ();
            }

        DrawHeaderGUI ();

        if (foundStyles == null || foundStyles.Count == 0)
            return;

        DrawContentGUI ();
        }

    private void DrawHeaderGUI()
        {
        styleLookupTest = EditorGUILayout.DelayedTextField ("Style Lookup", styleLookupTest);
        styleLookup = cache.GetStyle (styleLookupTest);

        if (styleLookup != null)
            {
            int index = cache.GetStyleIndex (styleLookupTest);
            DrawStyle (styleProperty.GetArrayElementAtIndex (index), foundStyles[index]);
            }
        else
            {
            EditorGUILayout.LabelField ("Search for GUIStyle above to display");
            }

        EditorGUILayout.BeginVertical (EditorStyles.helpBox);
            {
            EditorGUILayout.BeginHorizontal ();
                {
                if (GUILayout.Button ("Get Styles"))
                    GetStyles ();

                if (GUILayout.Button ("Clear Styles"))
                    ClearAll ();
                }
            EditorGUILayout.EndHorizontal ();
            }
        EditorGUILayout.EndVertical ();
        }

    private void DrawContentGUI()
        {
        //Display styles and display example GUIStyle
        EditorGUILayout.BeginVertical (EditorStyles.helpBox);
            {
            string info = HasStyles
                  ? "Editor Styles"
                  : "No styles found, click the buttonto get all editor styles.";

            EditorGUILayout.BeginHorizontal ();
                {
                EditorGUILayout.LabelField (info, EditorStyles.boldLabel);
                exampleType = (ExampleType)EditorGUILayout.EnumPopup (exampleType);
                }
            EditorGUILayout.EndHorizontal ();

            EditorGUILayout.Space ();

            EditorGUI.indentLevel++;
                {
                for (int i = 0; i < serializedStyles.Count; i++)
                    DrawStyle (serializedStyles[i], foundStyles[i]);
                }
            EditorGUI.indentLevel--;
            }
        EditorGUILayout.EndVertical ();
        }

    /// <summary>
    /// Finds all <see cref="GUIStyles"/> from the EditorStyle class and adds them to the target scriptable.
    /// </summary>
    private void GetStyles()
        {
        cache.ClearStyles ();

        if (stylesClass == null)
            stylesClass = Activator.CreateInstance<EditorStyles> ();

        //Get method and initialize the styles. This just makes sure the styles are all setup prior
        MethodInfo initMethod = styleType.GetMethod ("InitSharedStyles", methodBindings);
        initMethod.Invoke (stylesClass, null);

        //Get fields required
        styleFields = styleType.GetFields (fieldBindings);

        //Add all styles to list for convenient sorting
        foundStyles = new List<GUIStyle> ();

        foreach (var item in styleFields)
            {
            object val = item.GetValue (stylesClass);

            if (val is GUIStyle)
                foundStyles.Add (val as GUIStyle);
            }

        foundStyles.OrderBy (style => style.name);
        cache.AddStyles (foundStyles);

        serializedObject.Update ();

        CacheSerializedStyles ();
        }

    #region Draw Styles

    private void DrawExample(GUIStyle style)
        {
        switch (exampleType)
            {
            case ExampleType.BUTTON:
                GUILayout.Button (exampleText, style);
                break;

            case ExampleType.LABEL:
                EditorGUILayout.LabelField (exampleText, style);
                break;

            case ExampleType.POPUP:
                EditorGUILayout.Popup (0, new string[] { "One", "Two", "Three" }, style);

                break;

            case ExampleType.TEXT_FIELD:
                EditorGUILayout.TextField (exampleText, style);
                break;

            case ExampleType.TOGGLE:
                EditorGUILayout.ToggleLeft (exampleText, true, style);
                break;

            case ExampleType.SELECTABLE:
                EditorGUILayout.SelectableLabel (exampleText, style);

                break;


            }
        }

    private void DrawStyle(SerializedProperty property, GUIStyle style)
        {
        EditorGUILayout.BeginHorizontal ();
            {
            property.isExpanded = EditorGUILayout.Foldout (property.isExpanded, property.displayName, true);

            GUILayout.FlexibleSpace ();

            DrawExample (style);
            }
        EditorGUILayout.EndHorizontal ();

        //Do this so the whole window width can be used for the GUIStyle settings
        if (property.isExpanded)
            {
            EditorGUI.indentLevel++;
            DrawChildren (property);
            EditorGUI.indentLevel--;
            }
        }

    private void DrawChildren(SerializedProperty parent)
        {
        bool checkChildren = true;

        SerializedProperty copy = parent.Copy ();
        int index = 0;
        while (copy.NextVisible (checkChildren))
            {
            if (SerializedProperty.EqualContents (copy, parent.GetEndProperty ()))
                break;

            EditorGUILayout.PropertyField (copy, true);

            if (!checkChildren)
                index++;

            checkChildren = false;
            }
        }

    #endregion

    #region Helpers

    private void CacheSerializedStyles()
        {
        serializedStyles = new List<SerializedProperty> ();

        for (int i = 0; i < styleProperty.arraySize; i++)
            serializedStyles.Add (styleProperty.GetArrayElementAtIndex (i));
        }

    private void ClearAll()
        {
        cache.ClearStyles ();
        serializedStyles.Clear ();
        foundStyles.Clear ();
        }

    #endregion
    }