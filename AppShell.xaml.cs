namespace Mensajitos
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            // Registrar rutas para navegaci�n
            Routing.RegisterRoute(nameof(Paginas.MensajesRecientesPage), typeof(Paginas.MensajesRecientesPage));
            Routing.RegisterRoute(nameof(Paginas.RegistroPage), typeof(Paginas.RegistroPage));
        }
    }
}