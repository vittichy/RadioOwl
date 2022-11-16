using System.Windows;

namespace RadioOwl.Forms.Extension
{
    /// <summary>
    /// zajisteni nastaveni focusu na zvoleny UIElement - jinak to ve WPF samo od sebe nejde? a po startu nema form focus nikde :-/
    /// 
    /// viz: Set focus on textbox in WPF from view model (C#)
    /// http://stackoverflow.com/questions/1356045/set-focus-on-textbox-in-wpf-from-view-model-c
    /// </summary>
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusExtension), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = (UIElement)d;
            if ((bool)e.NewValue)
            {
                uie.Focus(); // Don't care about false values.
            }
        }
    }
}
