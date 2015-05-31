using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebAPIDemo.Models;

namespace WebAPIDemo.Controllers
{
    public class UserController : ApiController
    {
        private static IList<IAppUser> _dataSource = new List<IAppUser>();

        public static IList<IAppUser> DataSource
        {
            get { return UserController._dataSource; }
            set { UserController._dataSource = value; }
        }

        public UserController()
        {
            _dataSource = new List<IAppUser>();
            _dataSource.Add(new AppUserBase { Name = "User1", Email = "user1@demo.com", Id = 1 });
            _dataSource.Add(new AppUserBase { Name = "User2", Email = "user2@demo.com", Id = 2 });
            _dataSource.Add(new AppUserBase { Name = "User3", Email = "user3@demo.com", Id = 3 });

            UserController.DataSource = _dataSource;
        }

        // GET: api/User
        public IList<IAppUser> Get()
        {
            return _dataSource;
        }

        // GET: api/User/5
        public IAppUser Get(int id)
        {
            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();
            return user;
        }

        // POST: api/User
        public IAppUser Post(IAppUser user)
        {
            _dataSource.Add(user);

            return user;
        }

        // PUT: api/User/5
        public IAppUser Put(IAppUser user)
        {
            return user;
        }

        // DELETE: api/User/5
        public IList<IAppUser> Delete(int id)
        {
            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();
            _dataSource.Remove(user);

            return _dataSource;
        }

        [HttpGet]
        public async Task<IHttpActionResult> All()
        {
            await Task.Delay(1000);

            return Ok(_dataSource);
        }

        [HttpGet]
        public async Task<IHttpActionResult> ById(int id)
        {
            await Task.Delay(1000);

            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        public async Task<HttpResponseMessage> GetRawOKAsync()
        {
            await Task.Delay(1000);

            var result = Request.CreateResponse(HttpStatusCode.OK, _dataSource);

            return result;
        }

        public async Task<HttpResponseMessage> GetRawNotFoundAsync(int id)
        {
            await Task.Delay(1000);

            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();
            var result = Request.CreateResponse(HttpStatusCode.OK, _dataSource);

            if (user == null)
            {
                result = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Empty),
                    ReasonPhrase = "Nenhum Item encontrado"
                };
            }

            return result;
        }

        public async Task<HttpResponseMessage> GetRawExceptionAsync()
        {
            await Task.Delay(1000);

            try
            {
                throw new Exception("Sample exception");
            }
            catch (System.Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}