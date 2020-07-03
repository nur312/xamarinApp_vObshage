using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appUI.pages
{
    /// <summary>
    /// Страница с подсказками.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HelpPage : ContentPage
    {
        /// <summary>
        /// Установка начальных значений.
        /// </summary>
        public HelpPage()
        {
            InitializeComponent();
        }
    }
}