using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MisadventureEditor.GameManager
{
    public class ScriptableObjectDrawer<T> where T : ScriptableObject
    {
        [LabelWidth(50)]
        [PropertyOrder(-1)]
        [LabelText("Name")]
        [PropertySpace(5f)]
        [TitleGroup("Tools")]
        [HorizontalGroup("Tools/Main")]
        [VerticalGroup("Tools/Main/Vertical")]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [ShowInInspector]
        protected string NameForNew { get; set; }

        [HideLabel]
        [PropertySpace]
        [HorizontalGroup("Tools/Main/Vertical/General/Rename", Order = 2)]
        [PropertyOrder(0)]
        [InlineButton("RenameSelected", "Rename Object")]
        [ShowInInspector]
        [GUIColor(0.65f, 0.65f, 1f)]
        protected virtual string RenameName { get; set; }

        [PropertySpace(5)]
        [PropertyOrder(-1)]
        [BoxGroup("Tools/Main/Vertical/General")]
        [Button]
        protected void Select() => Selection.activeObject = Selected;

        [PropertySpace(10f)]
        [PropertyOrder(1)]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        [ShowInInspector]
        [ShowIf("ShowSelected")]
        [TitleGroup("Tools/Main/Vertical/Settings")]
        protected T Selected { get; set; }

        protected virtual bool ShowSelected => true;

        public virtual string Path { get; protected set; }

        [PropertyOrder(0)]
        [BoxGroup("Tools/Main/Vertical/Create")]
        [PropertySpace(5, 5)]
        [Button]
        [GUIColor(0, 0.9f, 0)]
        public virtual void CreateNew()
        {
            if (NameForNew == "") return;

            var newItem = ScriptableObject.CreateInstance<T>();
            newItem.name = "New " + typeof(T);

            if (Path == "") Path = "Assets/_Project";

            AssetDatabase.CreateAsset(newItem, Path + "\\" + NameForNew + ".asset");
            AssetDatabase.SaveAssets();

            NameForNew = "";

            SetSelected(newItem);
        }

        public virtual void CreateNew(string overrideName, string overridePath)
        {
            if (overrideName == "") return;
            if (overridePath == "") return;

            var newItem = ScriptableObject.CreateInstance<T>();
            newItem.name = overrideName;

            AssetDatabase.CreateAsset(newItem, $"{overridePath}/{overrideName}.asset");
            AssetDatabase.SaveAssets();

            NameForNew = "";

            SetSelected(newItem);
        }

        public virtual void RenameSelected()
        {
            if (RenameName == "") return;
            if (!Selected) return;

            var assetPath = AssetDatabase.GetAssetPath(Selected);
            AssetDatabase.RenameAsset(assetPath, RenameName);

            AssetDatabase.SaveAssets();
        }

        [LabelText("Delete Object")]
        [PropertyOrder(2)]
        [BoxGroup("Tools/Main/Vertical/General")]
        [Button]
        [GUIColor(1, 0.2f, 0)]
        [PropertySpace(SpaceAfter = 5)]
        public virtual void DeleteSelected()
        {
            if (!Selected) return;

            var assetPath = AssetDatabase.GetAssetPath(Selected);
            AssetDatabase.DeleteAsset(assetPath);
            AssetDatabase.SaveAssets();
        }

        public virtual void SetSelected(object item)
        {
            var attempt = item as T;
            if (!attempt) return;

            Selected = attempt;
            NameForNew = Selected.name;
            RenameName = Selected.name;
        }

        public virtual void SetPath(string newPath)
        {
            Path = newPath;
        }

        protected string GetCurrentSubfolder()
        {
            if (!Selected) return "";

            var selectedDirectory = GetCurrentDirectory();

            if (System.IO.Path.GetFullPath(selectedDirectory) ==
                System.IO.Path.GetFullPath(Path)) return "";

            var subfolder = System.IO.Path.GetFileName(selectedDirectory);

            return subfolder;
        }

        protected string GetCurrentDirectory()
        {
            if (!Selected) return "";

            var selectedPath = AssetDatabase.GetAssetPath(Selected);
            var selectedDirectory = System.IO.Path.GetDirectoryName(selectedPath);

            return selectedDirectory;
        }
    }
}