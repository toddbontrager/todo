using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace ToDo.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private CustomWebApplicationFactory _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void GivenARequestToTheController()
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public async Task PingTestOk()
        {
            var result = await _client.GetAsync("/api/todos/ping");
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task PingTestString()
        {
            var response = await _client.GetAsync("/api/todos/ping");
            var resultString = await response.Content.ReadAsStringAsync();
            Assert.That(resultString, Is.EqualTo("ping"));
        }

        [Test]
        public async Task ToDosController_GetTodos_StatusCode()
        {
            var response = await _client.GetAsync("api/todos");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task ToDosController_GetTodos_Length()
        {
            var response = await _client.GetAsync("api/todos");
            var stringResponse = await response.Content.ReadAsStringAsync();
            var results = JsonConvert.DeserializeObject<IEnumerable<ToDoObj>>(stringResponse);
            var length = results.Count();

            Assert.That(length, Is.EqualTo(3));
        }

        [Test]
        public async Task ToDosController_GetToDo_IdIsCorrect()
        {
            var response = await _client.GetAsync("api/todos/2");
            var stringResponse = await response.Content.ReadAsStringAsync();
            var responseObj = JsonConvert.DeserializeObject<ToDoObj>(stringResponse);
            var resopnseId = responseObj.id;

            Assert.That(resopnseId, Is.EqualTo(2));
        }

        [Test]
        public async Task ToDosController_CreateToDo_ToDoWasCreated()
        {
            var toDoToAdd = new ToDoObj
            {
                task = "testing",
                completed = false,
                suitableForChild = true
            };
            var content = JsonConvert.SerializeObject(toDoToAdd);
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/todos", stringContent);

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var toDo = JsonConvert.DeserializeObject<ToDoObj>(responseString);
            Assert.That(toDo.task, Is.EqualTo("testing"));
            Assert.That(toDo.completed, Is.EqualTo(false));
        }

        [Test]
        public async Task ToDosController_DeleteToDo_ToDoWasProperlyDeleted()
        {
            var response1 = await _client.GetAsync("api/todos");
            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var results1 = JsonConvert.DeserializeObject<IEnumerable<ToDoObj>>(stringResponse1);
            var lengthBeforeDeleting = results1.Count();

            var deleteResponse = await _client.DeleteAsync("api/todos/4");

            // TODO combine this repeated code into a common method
            var response2 = await _client.GetAsync("api/todos");
            var stringResponse2 = await response2.Content.ReadAsStringAsync();
            var results2 = JsonConvert.DeserializeObject<IEnumerable<ToDoObj>>(stringResponse2);
            var lengthAfterDeleting = results2.Count();

            Assert.That(lengthAfterDeleting, Is.EqualTo(lengthBeforeDeleting - 1));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
    public class ToDoObj
    {
        public int id { get; set; }
        public string task { get; set; }
        public bool completed { get; set; }
        public bool suitableForChild { get; set; }
    }
}
