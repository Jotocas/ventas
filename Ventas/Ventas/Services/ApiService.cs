namespace Ventas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Plugin.Connectivity;
    using Common.Models;
    using System.Text;

    public class ApiService
    {
        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message ="No tienes Internet"//Languages.TurnOnInternet,
                };
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message ="No hay Conexion con google.com" //Languages.NoInternet,
                };
            }

            return new Response
            {
                IsSuccess = true,
            };
        }

        public async Task<Response> Post<T>(
   string urlBase,
   string servicePrefix,
   string controller,
   T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(
                    request, Encoding.UTF8,
                    "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PostAsync(url,content);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(result);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = obj,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> GetList<T>(
      string urlBase,
      string servicePrefix,
      string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                 var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.GetAsync("http://192.168.43.119:70/api/products");
               // var response = await client.GetAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(result);
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }


        public async Task<Response> Delete(
            string urlBase,
            string servicePrefix,
            string controller,
            int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{servicePrefix}{controller}/{id}";
                var response = await client.DeleteAsync(url);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = result,
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }



        //    public async Task<Response> GetList<T>(string urlBase, string prefix, string controller)
        //    {
        //        try
        //        {
        //            var client = new HttpClient();
        //            client.BaseAddress = new Uri(urlBase);
        //            var url =  $"{prefix}{controller}";
        //            var response = await client.GetAsync(url);
        //            var answer = await response.Content.ReadAsStringAsync();
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                return new Response
        //                {
        //                    IsSuccess = false,
        //                    Message = answer,
        //                };
        //            }

        //            var list = JsonConvert.DeserializeObject<List<T>>(answer);
        //            return new Response
        //            {
        //                IsSuccess = true,
        //                Result = list,
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            return new Response
        //            {
        //                IsSuccess = false,
        //                Message = ex.Message,
        //            };
        //        }
        //    }
    }
}