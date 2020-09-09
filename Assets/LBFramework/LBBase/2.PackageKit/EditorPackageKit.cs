using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LBFramework.PackageKit
{
    public static class FrameworkMenuItems
    {
        public const string Preferences = "LBFramework/Preferences... %e";
        public const string PackageKit = "LBFramework/PackageKit... %#e";

        public const string Feedback = "LBFramework/Feedback";
    }

    public static class FrameworkMenuItemsPriorities
    {
        public const int Preferences = 1;

        public const int Feedback = 11;
    }
    public abstract class IMGUIEditorWindow : EditorWindow
    {
        public static T Create<T>(bool utility, string title = null) where T : IMGUIEditorWindow
        {
            return string.IsNullOrEmpty(title) ? GetWindow<T>(utility) : GetWindow<T>(utility, title);
        }
        
        private readonly List<IView> mChildren = new List<IView>();
        private bool mVisible = true;
        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public void AddChild(IView childView)
        {
            mChildren.Add(childView);
        }

        public void RemoveChild(IView childView)
        {
            
            mChildren.Remove(childView);
        }

        public List<IView> Children
        {
            get { return mChildren; }
        }

        public void RemoveAllChidren()
        {
            mChildren.Clear();
        }

        public abstract void OnClose();
        
        public abstract void OnUpdate();

        private void OnDestroy()
        {
            OnClose();
        }

        protected abstract void Init();
        private bool mInited = false;

        public virtual void OnGUI()
        {
            if (!mInited)
            {
                Init();
                mInited = true;
            }

            OnUpdate();

            if (Visible)
            {
                mChildren.ForEach(childView => childView.DrawGUI());
            }
        }
    }
    public interface IView : IDisposable
    {
        void Show();

        void Hide();

        void DrawGUI();

        ILayout Parent { get; set; }

        GUIStyleProperty Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();
    }

    public static class ViewExtensions
    {
        public static TView Do<TView>(this TView self, Action<TView> onDo) where TView : IView
        {
            onDo(self);
            return self;
        }
    }
    public interface ILayout : IView
    {
        ILayout AddChild(IView view);

        void RemoveChild(IView view);

        void Clear();
    }
    public static class WindowExtension
    {
        public static T PushCommand<T>(this T view, Action command) where T : IView
        {
            RenderEndCommandExecuter.PushCommand(command);
            return view;
        }
    }
    public class RenderEndCommandExecuter
    {
        private static Queue<Action> mPrivateCommands = new Queue<Action>();

        private static Queue<Action> mCommands
        {
            get { return mPrivateCommands; }
        }

        public static void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        public static void ExecuteCommand()
        {
            while (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }
    }
    public class GUIStyleProperty
    {
        private readonly Func<GUIStyle> mCreator;


        private Action<GUIStyle> mOperations = (style) => { };

        public GUIStyleProperty Set(Action<GUIStyle> operation)
        {
            mOperations += operation;
            return this;
        }

        public GUIStyleProperty(Func<GUIStyle> creator)
        {
            mCreator = creator;
        }

        private GUIStyle mValue = null;

        public GUIStyle Value
        {
            get
            {
                if (mValue == null)
                {
                    mValue = mCreator.Invoke();
                    mOperations(mValue);
                }

                return mValue;
            }
            set { mValue = value; }
        }
    }
}