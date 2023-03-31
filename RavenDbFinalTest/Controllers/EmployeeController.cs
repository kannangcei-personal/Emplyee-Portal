using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace RavenDbFinalTest.Controllers
{
    public class EmployeeController : Controller
    {

        private readonly IDocumentStore _session;

        public EmployeeController(IDocumentStore session)
        {
            _session = session;
        }


        // GET: Employee
        public IActionResult Index()
        {
            List<Company> model = new List<Company>();
            var certificate = new X509Certificate2("ClientCertificate.pfx", "Theophilus");

            using (var store = new DocumentStore
            {
                Urls = new[] { " https://a.theophilus.ravendb.community" },
                Database = "TestEmployee",
                Certificate = certificate
            })
            {
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    /*
                      session.Store(new Company
                      {
                         Name = "Vani HP",
                         ExternalId = "Trainee",
                        Phone = "+91 7856654357",
                       EmailId= "vanihp@ceiamerica.com",
                    });
                    */
                    session.SaveChanges();
                    model = session.Query<Company>(collectionName:"Companies").ToList();
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Company model)
        {
            var certificate = new X509Certificate2("ClientCertificate.pfx", "Theophilus");
            //var certificate = new X509Certificate2("certificate.pfx", "password");

            using (var store = new DocumentStore
            {
                Urls = new[] { " https://a.theophilus.ravendb.community" },
                Database = "TestEmployee",
                Certificate = certificate
            })
            {
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    session.Store(model);
                    session.SaveChanges();
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UploadImage()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Please select an image file to upload.");
            }

            // Read the image data into a byte array
            byte[] imageData;
            using (var stream = new MemoryStream())
            {
                await imageFile.CopyToAsync(stream);
                imageData = stream.ToArray();
            }

            // Store the image data in RavenDB
            using (var session = _session.OpenSession())
            {
                var imageDoc = new ImageDocument
                {
                    ImageData = imageData,
                    ContentType = imageFile.ContentType
                };

                session.Store(imageDoc);
                session.SaveChanges();
            }

            return RedirectToAction("AccessDenied","Home");
        }


        [HttpGet]
        public ActionResult Details(string Id)
        {
            String FormatedId = Id.Replace("%2F", "/");
            Company model = new Company();
            var certificate = new X509Certificate2("ClientCertificate.pfx", "Theophilus");

            using (var store = new DocumentStore
            {
                Urls = new[] { " https://a.theophilus.ravendb.community" },
                Database = "TestEmployee",
                Certificate = certificate
            })
            {
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    model = session.Load<Company>(FormatedId);
                }
            }
            return View(model);

        }
    }
}
