using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using COMMON.Entidades;
using System.Threading.Tasks;
using MensajeriaMAUI.Modelos;
using COMMON;

namespace MensajeriaMAUI.Servicios
{

    public class ApiService
    {
        
        

    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiService()
    {
        // Crear un HttpClient con manejo de certificados inseguros para desarrollo
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = 
            (message, cert, chain, errors) => true; // SOLO PARA DESARROLLO
        
        _httpClient = new HttpClient(handler);
        
        // Usar la URL desde los parámetros comunes
        _baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? Params.UrlAPI.Replace("localhost", "10.0.2.2") + "api"
            : Params.UrlAPI + "api";
            
        Console.WriteLine($"URL de API configurada: {_baseUrl}");
    }

    public async Task<List<Usuario>> ObtenerUsuariosAsync()
    {
        try
        {
            Console.WriteLine($"Obteniendo usuarios de: {_baseUrl}/Usuario");
            var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario");
            
            if (response.IsSuccessStatusCode)
            {
                var usuarios = await response.Content.ReadFromJsonAsync<List<Usuario>>();
                return usuarios ?? new List<Usuario>();
            }
            else
            {
                Console.WriteLine($"Error HTTP: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                return new List<Usuario>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener usuarios: {ex.Message}");
            return new List<Usuario>();
        }
    }

    public async Task<Usuario?> LoginAsync(string nombreUsuario, string contrasena)
    {
        try
        {
            var loginModel = new LoginModelo
            {
                NombreUsuario = nombreUsuario,
                Contrasena = contrasena
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginModel),
                Encoding.UTF8,
                "application/json");

            Console.WriteLine($"Intentando login en: {_baseUrl}/Usuario/login");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Usuario/login", content);
            
            Console.WriteLine($"Respuesta de login: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Usuario>();
            }
            else
            {
                Console.WriteLine($"Error en login: {await response.Content.ReadAsStringAsync()}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción en login: {ex.Message}");
            return null;
        }
    }

    public async Task<Usuario> RegistrarUsuarioAsync(Usuario usuario)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(usuario),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/Usuario", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Usuario>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al registrar usuario: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Mensaje>> ObtenerMensajesAsync(int idUsuario)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Mensaje");
            response.EnsureSuccessStatusCode();
            var mensajes = await response.Content.ReadFromJsonAsync<List<Mensaje>>();
            return mensajes.Where(m => m.emisor_id == idUsuario || m.receptor_id == idUsuario).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener mensajes: {ex.Message}");
            return new List<Mensaje>();
        }
    }

    public async Task<Mensaje> EnviarMensajeAsync(Mensaje mensaje)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(mensaje),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_baseUrl}/Mensaje", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Mensaje>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar mensaje: {ex.Message}");
            return null;
        }
    }

    
}
}
