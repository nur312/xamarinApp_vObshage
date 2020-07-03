using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;


namespace appUI
{
    /// <summary>
    /// Класс предостовлящий массив вкладок в нижней чачти экрана.
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        /// <summary>
        /// Установка начальных значений.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            Children.Add(new NavigationPage(new pages.PersonalPage())
            {
                IconImageSource = "manuser.png"
            });
            Children.Add(new NavigationPage(new pages.GeneralPage())
            {
                IconImageSource = "users.png"
            });
        }
    }
}
