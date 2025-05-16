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
        public async Task EnviarMensaje(Mensaje mensaje)
        {
            // Si el receptor está online, le enviamos el mensaje directamente
            if (_usuariosConectados.TryGetValue(mensaje.receptor_id, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("RecibirMensaje", mensaje);
            }

            // También notificamos al remitente que el mensaje fue enviado
            await Clients.Caller.SendAsync("MensajeEnviado", mensaje);
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
