using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu ()]
public class EditorStyleScriptable : ScriptableObject, ISerializationCallbackReceiver
    {
    [Serializable]
    public class GUIStyleRef
        {
        public string name;
        public GUIStyle style;

        public GUIStyleRef(GUIStyle style)
            {
            this.name = style.name;
            this.style = style;
            }
        }

    [SerializeField]
    private List<GUIStyleRef> editorStyles = new List<GUIStyleRef> ();
    private static Dictionary<string, GUIStyle> styleLookup;

    /// <summary>
    /// Current style count
    /// </summary>
    public int StyleCount => editorStyles.Count;

    /// <summary>
    /// Add a style to the collection.
    /// </summary>
    public void AddStyle(GUIStyle style)
        {
        if (styleLookup.ContainsKey (style.name))
            return;

        GUIStyleRef styleRef = new GUIStyleRef (style);
        editorStyles.Add (styleRef);
        styleLookup.Add (styleRef.name, style);
        }

    /// <summary>
    /// Add multiple styles. Will call <see cref="AddStyle(GUIStyle)"/> for each one.
    /// </summary>
    public void AddStyles(IEnumerable<GUIStyle> styles)
        {
        foreach (var style in styles)
            AddStyle (style);
        }

    /// <summary>
    /// Clear all style information.
    /// </summary>
    public void ClearStyles()
        {
        editorStyles.Clear ();
        styleLookup?.Clear ();
        }

    /// <summary>
    /// Find a style with the given name.
    /// </summary>
    /// <param name="name">Name of the style to find</param>
    public GUIStyle GetStyle(string name)
        {
        if (string.IsNullOrEmpty (name))
            return null;

        if (styleLookup.ContainsKey (name))
            return styleLookup[name];

        return null;
        }

    public int GetStyleIndex(string name)
        {
        return editorStyles.FindIndex (s => s.name == name);
        }

    /// <summary>
    /// Return all styles found
    /// </summary>
    public IEnumerable<GUIStyleRef> GetStyles()
        {
        return editorStyles;
        }

    #region Serialization

    public void OnBeforeSerialize()
        {
        if (styleLookup == null)
            styleLookup = new Dictionary<string, GUIStyle> ();
        }

    public void OnAfterDeserialize()
        {
        if (styleLookup == null)
            styleLookup = new Dictionary<string, GUIStyle> ();

        //Add the style or update
        foreach (var style in editorStyles)
            {
            if (!styleLookup.ContainsKey (style.name))
                styleLookup.Add (style.name, style.style);
            else
                styleLookup[style.name] = style.style;
            }
        }

    #endregion
    }