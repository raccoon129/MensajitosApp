namespace MensajeriaMAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert("Cerrar Sesión", 
                "¿Estás seguro de que deseas cerrar sesión?", "Sí", "No");
                
            if (confirmar)
            {
                await App.CerrarSesion();
            }
        }
    }
}
