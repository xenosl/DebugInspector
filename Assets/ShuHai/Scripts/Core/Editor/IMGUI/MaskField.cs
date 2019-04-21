using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Bitwise;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Editor.IMGUI
{
    using OptionTuple = Tuple<string, ulong>;

    public sealed class MaskField
    {
        public string Label;

        #region Construct

        public MaskField(IEnumerable<string> options) : this(string.Empty, 0ul, options) { }

        public MaskField(ulong value, IEnumerable<string> optionNames) : this(string.Empty, value, optionNames) { }

        public MaskField(string label, IEnumerable<string> optionNames) : this(label, 0ul, optionNames) { }

        public MaskField(string label, ulong value, IEnumerable<string> optionNames)
        {
            Construct(label, value, CreateOptions(optionNames));
        }

        public MaskField(IEnumerable<OptionTuple> options) : this(string.Empty, 0ul, options) { }

        public MaskField(ulong value, IEnumerable<OptionTuple> options) : this(string.Empty, value, options) { }

        public MaskField(string label, IEnumerable<OptionTuple> options) : this(label, 0ul, options) { }

        public MaskField(string label, ulong value, IEnumerable<OptionTuple> options)
        {
            Construct(label, value, CreateOptions(options));
        }

        private void Construct(string label, ulong value, IEnumerable<Option> options)
        {
            Label = label;
            SetValue(value, false, false);
            SetOptions(options, false);

            UpdateUpToValue();
            UpdateUpToOptions();
        }

        #endregion

        #region GUI

        public string GetPopupButtonText()
        {
            if (SelectedCount == 0)
                return OptionNoneName;
            if (SelectedCount == 1)
                return GetOption(GetSelectedIndex(0)).Name;
            if (SelectedCount > 1 && SelectedCount < OptionCount - SpecialOptionCountInOptionList)
                return "Mixed ...";
            // SelectedOptionCount >= OptionCount - SpecialOptionCountInOptionList
            return OptionAllName;
        }

        public ulong GUI(GUIContent popupButtonContent = null, GUIStyle style = null)
        {
            bool hasLabel = !string.IsNullOrEmpty(Label);
            BeginDraw(hasLabel);

            if (popupButtonContent == null)
            {
                PopupButtonContent.text = GetPopupButtonText();
                popupButtonContent = PopupButtonContent;
            }

            style = style ?? EditorStyles.popup;
            var buttonRect = GUILayoutUtility.GetRect(popupButtonContent, style);
            if (UnityEngine.GUI.Button(buttonRect, popupButtonContent, style))
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(OptionNoneName), AreNoneSelected(), OnOptionMenuItemNoneClick);
                menu.AddItem(new GUIContent(OptionAllName), AreAllSelected(), OnOptionMenuItemAllClick);

                for (int i = 0; i < OptionCount; ++i)
                {
                    if (i == optionNoneIndex || i == optionAllIndex)
                        continue;
                    var option = GetOption(i);
                    menu.AddItem(new GUIContent(option.Name), option.Selected, OnOptionMenuItemClick, i);
                }
                menu.DropDown(buttonRect);
            }

            EndDraw(hasLabel);
            return Value;
        }

        private readonly GUIContent PopupButtonContent = new GUIContent();

        private void BeginDraw(bool hasLabel)
        {
            if (!hasLabel)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(Label);
        }

        private void EndDraw(bool hasLabel)
        {
            if (!hasLabel)
                return;

            EditorGUILayout.EndHorizontal();
        }

        private void OnOptionMenuItemNoneClick() { UnselectAll(); }

        private void OnOptionMenuItemAllClick() { SelectAll(); }

        private void OnOptionMenuItemClick(object indexObj)
        {
            var option = GetOption((int)indexObj);
            option.Selected = !option.Selected;
        }

        #endregion GUI

        #region Value

        public const int BitCount = sizeof(ulong) * 8;

        public event Action<ulong> ValueChanged;

        public ulong Value { get { return value; } set { SetValue(value); } }

        private ulong value;

        private void SetValue(ulong value, bool updateRelatedData = true, bool sendNotification = true)
        {
            if (value == this.value)
                return;

            var old = this.value;
            this.value = value;

            if (updateRelatedData)
                UpdateUpToValue();

            if (sendNotification && ValueChanged != null)
                ValueChanged(old);
        }

        private void UpdateUpToValue() { UpdateSelectedIndices(); }

        #endregion

        #region Selection

        public void UnselectAll() { Value = 0; }

        public void SelectAll() { Value = allSelectedValue; }

        public bool AreAllSelected() { return Value.HasFlag(allSelectedValue); }

        public bool AreNoneSelected() { return (Value & allSelectedValue) == 0; }

        private ulong allSelectedValue;

        private void UpdateAllSelectedValue()
        {
            allSelectedValue = Options.Select(o => o.Value).Aggregate((a, v) => a | v);
        }

        #region Selected Indices

        public int SelectedCount { get { return selectedIndices.Count; } }

        public IEnumerable<int> SelectedIndices { get { return selectedIndices; } }

        public int GetSelectedIndex(int index) { return selectedIndices.At(index, -1); }

        private readonly List<int> selectedIndices = new List<int>(BitCount);

        private void UpdateSelectedIndices()
        {
            selectedIndices.Clear();

            for (int i = 0; i < OptionCount; i++)
            {
                if (i == optionNoneIndex || i == optionAllIndex)
                    continue;
                var option = GetOption(i);
                if (option.Selected)
                    selectedIndices.Add(i);
            }
        }

        #endregion Selected Indices

        #endregion Selection

        #region Options

        public int OptionCount { get { return options.Count; } }

        public IEnumerable<Option> Options { get { return options; } }

        public void SetOptions(params string[] optionNames) { SetOptions((IEnumerable<string>)optionNames); }
        public void SetOptions(IEnumerable<string> optionNames) { SetOptions(optionNames, true); }
        public void SetOptions(IEnumerable<OptionTuple> options) { SetOptions(options, true); }

        public Option GetOption(int index) { return options[index]; }

        private List<Option> options;

        private void SetOptions(IEnumerable<string> optionNames, bool updateRelatedData)
        {
            SetOptions(CreateOptions(optionNames), updateRelatedData);
        }

        private void SetOptions(IEnumerable<OptionTuple> options, bool updateRelatedData)
        {
            SetOptions(CreateOptions(options), updateRelatedData);
        }

        private void SetOptions(IEnumerable<Option> options, bool updateRelatedData = true)
        {
            Ensure.Argument.NotNull(options, "options");
            Ensure.Argument.Satisfy(options.Count() <= BitCount, "options", "Too many options.");

            this.options = new List<Option>(options);

            if (updateRelatedData)
                UpdateUpToOptions();
        }

        private void UpdateUpToOptions()
        {
            UpdateAllSelectedValue();
            UpdateSpecialOptions();
        }

        private IEnumerable<Option> CreateOptions(IEnumerable<string> optionNames)
        {
            return optionNames.Select((n, i) => new Option(this, n, 1ul << i));
        }

        private IEnumerable<Option> CreateOptions(IEnumerable<OptionTuple> options)
        {
            return options.Select(t => new Option(this, t.Item1, t.Item2));
        }

        #region Special Options

        public const string OptionNoneName = "None";
        public const string OptionAllName = "All";

        public int SpecialOptionCountInOptionList
        {
            get
            {
                var count = 0;
                if (OptionNoneExistsInOptionList)
                    count++;
                if (OptionAllExistsInOptionList)
                    count++;
                return count;
            }
        }

        public bool OptionNoneExistsInOptionList { get { return optionNoneIndex >= 0; } }
        public Option OptionNone { get { return optionNone; } }

        public bool OptionAllExistsInOptionList { get { return optionAllIndex >= 0; } }
        public Option OptionAll { get { return optionAll; } }

        private int optionNoneIndex;
        private Option optionNone;

        private int optionAllIndex;
        private Option optionAll;

        private void UpdateSpecialOptions()
        {
            UpdateSpecialOptions(OptionNoneName, 0, out optionNone, out optionNoneIndex);
            UpdateSpecialOptions(OptionAllName, allSelectedValue, out optionAll, out optionAllIndex);
        }

        private void UpdateSpecialOptions(string name, ulong value, out Option option, out int index)
        {
            index = options.FindIndex(o => o.Value == value);
            option = index >= 0 ? GetOption(index) : new Option(this, name, value);
        }

        #endregion

        public class Option
        {
            public readonly MaskField Owner;

            public readonly string Name;
            public readonly ulong Value;

            public bool Selected
            {
                get { return Owner.Value.HasFlag(Value); }
                set
                {
                    var old = Owner.Value;
                    Owner.Value = value ? old | Value : old & ~Value;
                }
            }

            internal Option(MaskField owner, string name, ulong value)
            {
                Ensure.Argument.NotNull(owner, "owner");
                Ensure.Argument.NotNullOrEmpty(name, "name");

                Owner = owner;
                Name = name;
                Value = value;
            }
        }

        #endregion
    }
}