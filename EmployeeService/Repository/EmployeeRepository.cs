using AutoMapper;
using EmployeeService.DTO;
using EmployeeService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;

namespace EmployeeService.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ILogger<EmployeeRepository> logger;

        public EmployeeRepository(IConfiguration configuration, IMapper mapper,
            ILogger<EmployeeRepository> logger)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <summary>
        /// Get employee list
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> GetEmployee(string apiUrl, string token)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = client.GetAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// creates a new employee
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> CreateEmployee<T>(string apiUrl, string token, T model) where T : class
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = client.PostAsJsonAsync(apiUrl, model);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates emplyees details
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> UpdateEmployee<T>(string apiUrl, string token, T model) where T : class
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(900);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = client.PutAsJsonAsync(apiUrl, model);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete employee's record
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> DeleteEmployee(string apiUrl, string token)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.Timeout = TimeSpan.FromSeconds(1999);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var response = client.DeleteAsync(apiUrl);
                    response.Wait();
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<EmployeeDTO>> GetAllEmployees()
        {
            var employees = new List<Employee>();

            using (var client = new HttpClient())
            {
                GetDefaultHeaders(client);

                using (HttpResponseMessage httpResponseMessage = await client.GetAsync("users"))
                {
                    try
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        employees = JsonConvert.DeserializeObject<List<Employee>>(responseContent);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                        return null;
                    }
                }
            }
            return mapper.Map<List<EmployeeDTO>>(employees);
        }

        [HttpGet]
        public async Task<EmployeeDTO> GetEmployeeById(int id)
        {
            var employees = new Employee();
            using (var client = new HttpClient())
            {
                GetDefaultHeaders(client);

                using (HttpResponseMessage httpResponseMessage = await client.GetAsync($"users/{id}"))
                {
                    try
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        employees = JsonConvert.DeserializeObject<Employee>(responseContent);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex.ToString());
                        return null;
                    }
                }
            }
            return mapper.Map<EmployeeDTO>(employees);
        }

        [HttpPut]
        public async Task<EmployeeDTO> UpdateEmployee(UpdateRequestDTO updateRequestDTO)
        {
            var employees = new EmployeeDTO();
            using (var client = new HttpClient())
            {
                var URL = configuration["GoRest:URL"];
                var Token = configuration["GoRest:Token"];
                client.BaseAddress = new Uri(URL);
                using (HttpResponseMessage httpResponseMessage = await client.PutAsJsonAsync($"users/{updateRequestDTO.id}", updateRequestDTO))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                    try
                    {
                        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        employees = JsonConvert.DeserializeObject<EmployeeDTO>(responseContent);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            }
            return employees;
        }

        private void GetDefaultHeaders(HttpClient client)
        {
            var URL = configuration["GoRest:URL"];
            var Token = configuration["GoRest:Token"];


            client.BaseAddress = new Uri(URL);

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Token);
        }
    }
}
