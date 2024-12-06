using Microsoft.Maui.Controls;

namespace trylang
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("CalendarPage", typeof(trylang.View.CalendarPage));
            Routing.RegisterRoute("InsightsPage", typeof(trylang.View.InsightsPage));
            Routing.RegisterRoute("RelaxPage", typeof(trylang.View.RelaxPage));
            Routing.RegisterRoute("EditMoodPage", typeof(EditMoodPage));
            Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
            Routing.RegisterRoute("SpinnerPage", typeof(SpinnerPage));
            Routing.RegisterRoute("PopItPage", typeof(PopItPage));
            Routing.RegisterRoute("BouncyBallsPage", typeof(BouncyBallsPage));
            Routing.RegisterRoute("RipplesPage", typeof(RipplesPage));
            Routing.RegisterRoute("FidgetPage", typeof(FidgetPage));
            Routing.RegisterRoute("AffirmationPage", typeof(AffirmationPage));
            Routing.RegisterRoute("AccountPage", typeof(AccountPage));

        }

    }
}