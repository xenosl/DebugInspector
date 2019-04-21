using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using ShuHai.Editor.IMGUI.Controls;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    public class StackFrameField
    {
        public StackFrameField() : this(null) { }

        public StackFrameField(StackFrame value)
        {
            Value = value;
            valueControl.MouseDoubleClick += ValueControlDoubleClick;
        }

        #region GUI

        public void GUI() { ValueControlGUI(); }

        #endregion GUI

        #region Value

        public StackFrame Value
        {
            get { return value; }
            set
            {
                if (value == this.value)
                    return;

                this.value = value;

                valueText = null;
                valueControlTextDirty = true;
                fileExists = null;
            }
        }

        private StackFrame value;

        #region Text

        public string ValueText { get { return valueText ?? (valueText = BuildValueText(false)); } }

        private string valueText;

        private string BuildValueText(bool boldMethod)
        {
            if (Value == null)
                return string.Empty;

            var builder = new StringBuilder();

            if (boldMethod)
                builder.Append("<b>");
            builder.Append(GetMethodName());
            if (boldMethod)
                builder.Append("</b>");
            builder.Append(' ');

            builder.Append(Value.GetFileName())
                .Append(':')
                .Append(Value.GetFileLineNumber());

            return builder.ToString();
        }

        private string GetMethodName()
        {
            if (Environment.Version.Major >= 4)
            {
                var text = Value.ToString();
                int methodEndIndex = text.IndexOf(" at offset", StringComparison.Ordinal);
                return text.Remove(methodEndIndex);
            }
            else // Value.GetMethod() may cause unity crash on .Net 4
            {
                var method = Value.GetMethod();
                return method != null ? method.ToString(true) : "<unknown method>";
            }
        }

        #endregion Text

        #region Control

        private bool valueControlTextDirty = true;

        private readonly Label valueControl = new Label(null) { Selectable = true, RichText = true };

        private void UpdateValueControlText()
        {
            if (!valueControlTextDirty)
                return;

            valueControl.Text = BuildValueText(true);
            valueControlTextDirty = false;
        }

        private void ValueControlGUI()
        {
            UpdateValueControlText();
            valueControl.GUI();
        }

        private void ValueControlDoubleClick(Control control, Event evt)
        {
            if (FileExists)
                OpenFile();
        }

        #endregion Control

        #endregion Value

        #region File

        public event Action FileOpen;

        public bool FileExists
        {
            get
            {
                if (fileExists == null)
                    fileExists = Value != null && File.Exists(Value.GetFileName());
                return fileExists.Value;
            }
        }

        private bool? fileExists;

        private void OpenFile()
        {
            var assetPath = ProjectPaths.AssetOf(Value.GetFileName());
            var file = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
            if (file == null)
                return;

            if (AssetDatabase.OpenAsset(file, Value.GetFileLineNumber()))
                FileOpen.NPInvoke();
        }

        #endregion File
    }
}