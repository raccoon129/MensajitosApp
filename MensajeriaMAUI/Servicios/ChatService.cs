using COMMON.Entidades;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;


namespace MensajeriaMAUI.Servicios
{
    public class ChatService
    {
        private readonly HubConnection _hubConnection;
        private Usuario _usuarioActual;
        public ObservableCollection<Mensaje> MensajesRecibidos { get; set; } = new ObservableCollection<Mensaje>();

        public event Action<Mensaje> MensajeRecibido;
        public event Action<int> UsuarioConectado;
        public event Action<int> UsuarioDesconectado;

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public ChatService()
        {
            string url = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:5253/chathub"
                : "https://localhost:5253/chathub";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<Mensaje>("RecibirMensaje", (mensaje) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MensajesRecibidos.Add(mensaje);
                    MensajeRecibido?.Invoke(mensaje);
                });
            });

            _hubConnection.On<int>("UsuarioConectado", (idUsuario) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UsuarioConectado?.Invoke(idUsuario);
                });
            });

            _hubConnection.On<int>("UsuarioDesconectado", (idUsuario) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UsuarioDesconectado?.Invoke(idUsuario);
                });
            });
        }

        public async Task ConectarAsync(Usuario usuario)
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                _usuarioActual = usuario;
                await _hubConnection.InvokeAsync("ConectarUsuario", usuario.id_usuario);
            }
        }

        public async Task EnviarMensajeAsync(Mensaje mensaje)
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("EnviarMensaje", mensaje);
            }
        }

        public async Task DesconectarAsync()
        {
            if (_hubConnection.State != HubConnectionState.Disconnected)
            {
                await _hubConnection.StopAsync();
            }
        }
    }
}
