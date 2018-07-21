using System;
using EZData;
using TruePong.Utils;

namespace TruePong.UI {
    public class CommonDialogButton : Context {
        #region Property Name
        private readonly Property<String> _privateNameProperty = new Property<String>();
        public Property<String> NameProperty { get { return _privateNameProperty; } }
        public String Name {
            get { return NameProperty.GetValue(); }
            private set { NameProperty.SetValue(value); }
        }
        #endregion

        private readonly Action<CommonDialogButton> onClick;

        public CommonDialogButton(String name, Action<CommonDialogButton> onClick) {
            Name = name;
            this.onClick = onClick;
        }

        public void OnClick() {
            onClick.TryCall(this);
        }
    }

    public class CommonDialog : Window {
        #region Property Title
        private readonly Property<String> _privateTitleProperty = new Property<String>();
        public Property<String> TitleProperty { get { return _privateTitleProperty; } }
        public String Title {
            get { return TitleProperty.GetValue(); }
            set { TitleProperty.SetValue(value); }
        }
        #endregion
        
        #region Property Message
        private readonly Property<String> _privateMessageProperty = new Property<String>();
        public Property<String> MessageProperty { get { return _privateMessageProperty; } }
        public String Message {
            get { return MessageProperty.GetValue(); }
            set { MessageProperty.SetValue(value); }
        }
        #endregion

        #region Collection Buttons
        private readonly Collection<CommonDialogButton> _privateButtons = new Collection<CommonDialogButton>(false);
        public Collection<CommonDialogButton> Buttons {
            get { return _privateButtons; }
        }
        #endregion

        public CommonDialog(String title, String message) : base("UI/CommonDialog/CommonDialog") {
            Title = title;
            Message = message;
        }

        public void AddButton(String name, Action onClick) {
            var button = new CommonDialogButton(name, (btn) => {
                onClick.TryCall();
                Destroy();
            });
            Buttons.Add(button);
        }
    }
}