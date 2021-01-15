<h1 align="center">
  Editor Styles Scriptable
</h1>
<h4 align="center">
Small utility addon for Unity to have all EditorStyles cached in a ScriptableObject with previews. Can use the GUIStyles for custom editors, tools or even at game runtime. 
</h4>

## About

Just while doing some quick debugging, I got really fed up of having to create GUIStyles with tiny changes that were similar to EditorStyles. Now it's super convenient to see the styles and also have them instantly rather than having to create/reference GUIStyles in OnGUI methods.

<p align="center" >
<img src="https://i.imgur.com/JgqEcPP.png">
</p>

## Usage

#### EditorStyleScriptable
The ScriptableObject with all the EditorStyles. Provides some methods for clearing all styles (generally for the editor), adding styles and also finding styles at runtime:

```cs
public class ExampleGUITest : MonoBehaviour
{
  [SerializeField]
  private EditorStyleScriptable editorStyles;
  
  [SerializeField]
  private GUIStyle headerStyle;
    
  public void Start()
  {
    //Find the style
    headerStyle = editorStyles.GetStyle("BoldLabel");
  }

  public void OnGUI()
  {
    GUI.Label (new Rect(10, 10, 100, 100), "Time values", headerStyle);
  }
}
```

#### EditorStyleEditor
The editor class containing all reflection behaviour to find the styles. Also custom draws and displays the examples in the ScriptableObject.

#### ExampleGUITest
Example provided showing a slightly more expansive version of the above.

## Feedback

If there are any problems, or requests to expand upon this, feel free to add an issue or pull request!
