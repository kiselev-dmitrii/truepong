using EZData;

namespace TruePong.Utils {
    public class Popup : Context {
        #region Property IsVisible
        private readonly Property<bool> _privateIsVisibleProperty = new Property<bool>();
        public Property<bool> IsVisibleProperty { get { return _privateIsVisibleProperty; } }
        public bool IsVisible {
            get { return IsVisibleProperty.GetValue(); }
            set { IsVisibleProperty.SetValue(value); }
        }
        #endregion

        public Popup() {
            IsVisible = false;
        }

        public void SetVisible(bool isVisible) {
            IsVisible = isVisible;
        }

        public void Show() {
            SetVisible(true);
        }

        public void Hide() {
            SetVisible(false);
        }
    }
}
