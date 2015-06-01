using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.Results;
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
            controller = SetupRequest(controller) as UserController;
            var result = controller.Get().Result;

            Assert.IsInstanceOfType(result, typeof(System.Web.Http.Results.OkNegotiatedContentResult<IList<IAppUser>>));
        }

        [TestMethod]
        public void TestGetById()
        {
            UserController controller = new UserController();
            controller = SetupRequest(controller) as UserController;
            var result = controller.Get(1).Result;
            IAppUser user = ((OkNegotiatedContentResult<IAppUser>)result).Content as IAppUser;

            Assert.AreEqual(user.Id, collection[0].Id);
        }

        [TestMethod]
        public void TestCreateAppUser()
        {
            UserController controller = new UserController();
            int currentCout = collection.Count;

            IAppUser newUser = new AppUserBase { Name = "User4", Email = "user4@demo.com", Id = 4 };
            var result = controller.Post(newUser).Result as CreatedNegotiatedContentResult<IAppUser>;
            IAppUser user = result.Content as IAppUser;

            Assert.AreEqual(user, newUser);
            Assert.AreEqual("api/User/1", result.Location.OriginalString);
            Assert.AreNotEqual(currentCout, UserController.DataSource);
        }

        [TestMethod]
        public void TestUpdateAppUser()
        {
            UserController controller = new UserController();
            controller = SetupRequest(controller) as UserController;
            var result = controller.Get(1).Result;
            IAppUser user = ((OkNegotiatedContentResult<IAppUser>)result).Content as IAppUser;
            user.Name = "user1NameUpdated";

            var resultUpdated = controller.Put(user).Result;
            IAppUser userUpdated = ((OkNegotiatedContentResult<IAppUser>)result).Content as IAppUser;

            Assert.AreEqual(user.Name, userUpdated.Name);
            Assert.IsInstanceOfType(result, typeof(System.Web.Http.Results.OkNegotiatedContentResult<IAppUser>));
        }

        [TestMethod]
        public void TestDeleteAppUser()
        {
            UserController controller = new UserController();
            int currentCout = collection.Count;
            var result = controller.Delete(1).Result;

            IList<IAppUser> col = ((OkNegotiatedContentResult<IList<IAppUser>>)result).Content as IList<IAppUser>;
            Assert.AreNotEqual(currentCout, col.Count);
        }

        [TestMethod]
        public void TestUserNotFound()
        {
            UserController controller = new UserController();
            int currentCout = collection.Count;
            var result = controller.Get(10).Result;

            Assert.IsInstanceOfType(result, typeof(System.Web.Http.Results.NotFoundResult));
        }

        [TestMethod]
        public void TestForBadRequest()
        {
            UserController controller = new UserController();
            int currentCout = collection.Count;
            var result = controller.BadRequest(11).Result as BadRequestErrorMessageResult;
            Assert.IsInstanceOfType(result, typeof(System.Web.Http.Results.BadRequestErrorMessageResult));

            string message = string.Format("Something went wrong on the request for user {0}", 11);
            Assert.AreEqual(result.Message, message);
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