using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0649

public class ExampleGUITest : MonoBehaviour
    {
    [System.Serializable]
    private class DebugValues
        {
        public string name;
        public float value;

        public DebugValues(string name)
            {
            this.name = name;
            }

        public void DrawAsGUI(GUIStyle labelStyle, GUIStyle valueStyle)
            {
            GUILayout.BeginHorizontal ();
                {
                GUILayout.Label (name, labelStyle);
                GUILayout.Label (value.ToString(), valueStyle);
                }
            GUILayout.EndHorizontal ();
            }
        }

    [SerializeField]
    private EditorStyleScriptable editorStyles;

    [SerializeField]
    private GUIStyle headerStyle, labelStyle, valueStyle;

    private DebugValues[] values = new DebugValues[3]
        {
        new DebugValues("Delta"),
        new DebugValues("Frame Count"),
        new DebugValues("Time"),
        };

    // Start is called before the first frame update
    private void Start()
        {
        if (editorStyles == null)
            {
            Debug.LogWarning ("EditorStyles is not assigned!");
            return;
            }

        headerStyle = editorStyles.GetStyle ("ObjectFieldThumb");
        headerStyle.alignment = TextAnchor.MiddleCenter;

        labelStyle = editorStyles.GetStyle ("BoldLabel");
        valueStyle = editorStyles.GetStyle ("HelpBox");
        }

    // Update is called once per frame
    private void Update()
        {
        values[0].value = Time.deltaTime;
        values[1].value = Time.frameCount;
        values[2].value = Time.time;
        }

    private void OnGUI()
        {
        if (editorStyles == null)
            return;

        //19 is the height of a field (normally)
        //So 19*3 for each of the floats, then another 19 for the header
        Rect rect = new Rect (10f, 10f, Screen.width - 20f, 19f * 4f);

        //Draw group of values, can call GUI.skin still too for convenience
        GUI.BeginGroup (rect, GUI.skin.box);
            {
            //Use GUILayout to auto layout
            GUILayout.Label ("Time values", headerStyle);

            foreach (var value in values)
                value.DrawAsGUI (labelStyle, valueStyle);
            }
        GUI.EndGroup ();
        }
    }
