using System;
using EZData;
using TruePong.Utils;

namespace TruePong.UI {
    public class WaitWindow : Window {
        #region Property Message
        private readonly Property<String> _privateMessageProperty = new Property<String>();
        public Property<String> MessageProperty { get { return _privateMessageProperty; } }
        public String Message {
            get { return MessageProperty.GetValue(); }
            set { MessageProperty.SetValue(value); }
        }
        #endregion

        public WaitWindow(String message) : base("UI/WaitWindow") {
            Message = message;
        }
    }
}