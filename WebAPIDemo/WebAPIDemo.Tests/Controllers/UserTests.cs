using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using WebAPIDemo.Controllers;
using WebAPIDemo.Models;

namespace WebAPIDemo.Tests.Controllers
{
    [TestClass]
    public class UserTests
    {
        private IList<IAppUser> collection = new List<IAppUser>();

        [TestInitialize]
        public void Initialize()
        {
            collection = new List<IAppUser>();
            collection.Add(new AppUserBase { Name = "User1", Email = "user1@demo.com", Id = 1 });
            collection.Add(new AppUserBase { Name = "User2", Email = "user2@demo.com", Id = 2 });
            collection.Add(new AppUserBase { Name = "User3", Email = "user3@demo.com", Id = 3 });

            UserController.DataSource = collection;
        }

        [TestMethod]
        public void TestGet()
        {
            UserController controller = new UserController();

            IEnumerable<IAppUser> result = controller.Get();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestGetCount()
        {
            UserController controller = new UserController();

            IList<IAppUser> result = controller.Get();

            Assert.AreEqual(result.Count, collection.Count);
        }

        [TestMethod]
        public void TestGetById()
        {
            UserController controller = new UserController();

            IAppUser result = controller.Get(1);

            Assert.AreEqual(result.Id, collection[0].Id);
        }

        [TestMethod]
        public void TestGetByIdNotFound()
        {
            UserController controller = new UserController();

            IAppUser result = controller.Get(10);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestCreateAppUser()
        {
            UserController controller = new UserController();
            int currentCout = collection.Count;

            IAppUser newUser = new AppUserBase { Name = "User4", Email = "user4@demo.com", Id = 4 };
            IAppUser result = controller.Post(newUser);

            Assert.AreEqual(result, newUser);
            Assert.AreNotEqual(currentCout, collection.Count);
        }

        [TestMethod]
        public void TestUpdateAppUser()
        {
            UserController controller = new UserController();

            IAppUser user = controller.Get(1);
            user.Name = "user1Updated";

            IAppUser userUpdated = controller.Put(user);

            Assert.AreEqual(user.Name, userUpdated.Name);
        }

        [TestMethod]
        public void TestDeleteAppUser()
        {
            UserController controller = new UserController();
            int currentCout = collection.Count;
            IList<IAppUser> result = controller.Delete(1);
            Assert.AreNotEqual(currentCout, result.Count);
        }

        [TestMethod]
        public void TestGetHttpResponseMessageOK()
        {
            UserController controller = new UserController();
            controller = SetupRequest(controller) as UserController;
            var result = controller.GetAsync().Result;

            Assert.IsInstanceOfType(result, typeof(System.Web.Http.Results.OkNegotiatedContentResult<IList<IAppUser>>));
        }

        [TestMethod]
        public void TestGetHttpResponseMessageNotFound()
        {
            UserController controller = new UserController();
            controller = SetupRequest(controller) as UserController;

            var result = controller.GetAsync(10).Result;

            Assert.IsInstanceOfType(result, typeof(System.Web.Http.Results.NotFoundResult));
        }

        [TestMethod]
        public void TestGetHttpResponseMessageRawOK()
        {
            UserController controller = new UserController();
            controller = SetupRequest(controller) as UserController;

            var result = controller.GetRawOKAsync().Result;

            Assert.AreEqual(result.StatusCode, HttpStatusCode.OK);
        }

        [TestMethod]
        public void TestGetHttpResponseMessageRawNotFound()
        {
            UserController controller = new UserController();
            controller = SetupRequest(controller) as UserController;

            var result = controller.GetRawNotFoundAsync(10).Result;

            Assert.AreEqual(result.StatusCode, HttpStatusCode.NotFound);
        }



        private ApiController SetupRequest(ApiController controller)
        {
            controller.Configuration = new HttpConfiguration();
            var route = controller.Configuration.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional });
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "Issues" } });

            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://test.com/issues");
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, controller.Configuration);
            controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, routeData);

            return controller;
        }
    }
}