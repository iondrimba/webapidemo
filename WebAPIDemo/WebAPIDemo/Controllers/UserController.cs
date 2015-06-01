using System.Collections.Generic;
using System.Linq;
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

        // POST: api/User
        public async Task<IHttpActionResult> Post(IAppUser user)
        {
            await Task.Delay(1000);
            string location = "api/User/1";
            return Created(location, user);
        }

        // PUT: api/User/5
        public async Task<IHttpActionResult> Put(IAppUser user)
        {
            await Task.Delay(1000);

            return Ok(user);
        }

        // DELETE: api/User/5
        public async Task<IHttpActionResult> Delete(int id)
        {
            await Task.Delay(1000);

            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();
            _dataSource.Remove(user);

            return Ok(_dataSource);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            await Task.Delay(1000);

            return Ok(_dataSource);
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            await Task.Delay(1000);

            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        public async Task<IHttpActionResult> BadRequest(int id)
        {
            await Task.Delay(1000);

            IAppUser user = _dataSource.Where(p => p.Id == id).FirstOrDefault();

            if (user == null)
            {
                string message = string.Format("Something went wrong on the request for user {0}", id);
                return BadRequest(message);
            }

            return Ok(user);
        }
    }
}