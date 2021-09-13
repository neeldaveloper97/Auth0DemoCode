
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using SimpleLogin.Client.Helpers;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleLogin.Client.Services
{
    public interface IHttpService
    {
        Task<T> Get<T>(string uri, bool addjwt);
        Task<T> Post<T>(string uri, object value, bool addjwt);

    }

    public class HttpService : IHttpService
    {
        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;
        private IConfiguration _configuration;

        public HttpService(
            HttpClient httpClient,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService,
            IConfiguration configuration
        )
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _configuration = configuration;
        }

        public async Task<T> Get<T>(string uri, bool addjwt)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await sendRequest<T>(request, addjwt);
        }

        public async Task<T> Post<T>(string uri, object value, bool addjwt)
        {
            var request = createRequest(HttpMethod.Post, uri, value);
            return await sendRequest<T>(request, addjwt);
        }

        // helper methods
        private HttpRequestMessage createRequest(HttpMethod method, string uri, object value = null)
        {
            var request = new HttpRequestMessage(method, uri);
            if (value != null)
                request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
            return request;
        }

        private async Task sendRequest(HttpRequestMessage request)
        {
            await addJwtHeader(request);

            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("account/logout");
                return;
            }

            await handleErrors(response);
        }

        private async Task<T> sendRequest<T>(HttpRequestMessage request, bool addjwt)
        {

            if (addjwt)
            { await addJwtHeader(request); }


            // send request
            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                //_navigationManager.NavigateTo("account/logout");
                return default;
            }

            await handleErrors(response);

            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new StringConverter());
            return await response.Content.ReadFromJsonAsync<T>(options);
        }

        private async Task addJwtHeader(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var token = await _localStorageService.GetItem<string>("token");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task handleErrors(HttpResponseMessage response)
        {
            // throw exception on error response
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                throw new Exception(error["message"]);
            }
        }
    }
}