using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using COMMON.Entidades;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace Mensajitos.Servicios
{
    public class ServicioSignalR
    {
        private HubConnection _conexion;
        
        // Eventos para notificar a la UI
        public event Action<Mensaje> MensajeRecibido;
        public event Action<int> UsuarioConectado;
        public event Action<int> UsuarioDesconectado;
        
        public bool EstaConectado => _conexion?.State == HubConnectionState.Connected;
        
        public async Task IniciarConexion(int idUsuario)
        {
            try
            {
                // Crear conexión al hub utilizando la URL definida en Params
                _conexion = new HubConnectionBuilder()
                    .WithUrl($"{COMMON.Params.UrlAPI}chathub")
                    .WithAutomaticReconnect()
                    .Build();
                
                // Configurar los manejadores de eventos del hub
                ConfigurarEventos();
                
                // Iniciar la conexión
                await _conexion.StartAsync();
                
                // Notificar al servidor que el usuario está conectado
                await _conexion.InvokeAsync("ConectarUsuario", idUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al conectar con SignalR: {ex.Message}");
                throw;
            }
        }
        
        private void ConfigurarEventos()
        {
            // Cuando recibimos un mensaje
            _conexion.On<Mensaje>("RecibirMensaje", (mensaje) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MensajeRecibido?.Invoke(mensaje);
                });
            });
            
            // Cuando un usuario se conecta
            _conexion.On<int>("UsuarioConectado", (idUsuario) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UsuarioConectado?.Invoke(idUsuario);
                });
            });
            
            // Cuando un usuario se desconecta
            _conexion.On<int>("UsuarioDesconectado", (idUsuario) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UsuarioDesconectado?.Invoke(idUsuario);
                });
            });
        }
        
        public async Task EnviarMensaje(Mensaje mensaje)
        {
            if (EstaConectado)
            {
                await _conexion.InvokeAsync("EnviarMensaje", mensaje);
            }
        }
        
        public async Task Desconectar()
        {
            if (EstaConectado)
            {
                await _conexion.StopAsync();
            }
        }
    }
}
