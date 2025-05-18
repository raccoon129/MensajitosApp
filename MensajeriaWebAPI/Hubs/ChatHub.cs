using COMMON.Entidades;
using Microsoft.AspNetCore.SignalR;

namespace MensajeriaWebAPI.Hubs
{
    public class ChatHub : Hub
    {
        // Dictionary para mapear usuarios a sus connectionIds
        private static Dictionary<int, string> _usuariosConectados = new Dictionary<int, string>();

        // Método que se llama cuando un usuario se conecta
        public async Task ConectarUsuario(int idUsuario)
        {
            if (!_usuariosConectados.ContainsKey(idUsuario))
            {
                _usuariosConectados.Add(idUsuario, Context.ConnectionId);
            }
            else
            {
                _usuariosConectados[idUsuario] = Context.ConnectionId;
            }

            await Clients.All.SendAsync("UsuarioConectado", idUsuario);
        }

        // Método para enviar un mensaje a un usuario específico
        // Método para enviar un mensaje a un usuario específico
        public async Task EnviarMensaje(Mensaje mensaje)
        {
            try
            {
                // Registrar diagnóstico
                Console.WriteLine($"Enviando mensaje de {mensaje.emisor_id} a {mensaje.receptor_id}: {mensaje.contenido}");

                // Si el receptor está online, le enviamos el mensaje directamente
                if (_usuariosConectados.TryGetValue(mensaje.receptor_id, out string connectionId))
                {
                    Console.WriteLine($"Usuario {mensaje.receptor_id} está conectado, enviando mensaje directamente");
                    await Clients.Client(connectionId).SendAsync("RecibirMensaje", mensaje);
                }
                else
                {
                    Console.WriteLine($"Usuario {mensaje.receptor_id} no está conectado, solo notificando al emisor");
                }

                // También notificamos al remitente que el mensaje fue enviado
                await Clients.Caller.SendAsync("MensajeEnviado", mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EnviarMensaje: {ex.Message}");
                throw;
            }
        }

        // Método que se llama cuando un usuario se desconecta
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var usuario = _usuariosConectados.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (usuario.Value != null)
            {
                _usuariosConectados.Remove(usuario.Key);
                Clients.All.SendAsync("UsuarioDesconectado", usuario.Key);
            }

            return base.OnDisconnectedAsync(exception);
        }
    }

}
