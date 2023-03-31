using Raven.Client.Documents;
using RavenDbFinalTest.Models;
using Microsoft.AspNetCore.Mvc;
using RavenDbFinalTest.Models;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Microsoft.AspNetCore.Http;
namespace RavenDbFinalTest.Graphql
{
    public class QueryResolver
    {
        private readonly IDocumentStore _documentStore;
        private readonly HttpContextAccessor _contextAccessor;

        public QueryResolver(IDocumentStore documentStore,HttpContextAccessor httpContextAccessor)
        {
            _documentStore = documentStore;
            _contextAccessor = httpContextAccessor;
        }
        
        public bool EmailExists(string email)
        {
            using (var session = _documentStore.OpenSession())
            {
                Console.WriteLine(session.Query<Company>().Any(e => e.EmailId == email));
                return session.Query<Company>().Any(e => e.EmailId == email);
            }
        }
        [GraphQLName("getemployee")]
        public Company GetEmployee(string email)
        {
            
            using (var session =_documentStore.OpenSession())
            {

                var user=session.Query<Company>().FirstOrDefault(e => e.EmailId == email);
                if(user== null)
                {
                    return null;
                }
                else
                {
                    return user;
                }
            }
        }
        [GraphQLName("logactivity")]
        public string log(string email) {
            
            using (var session= _documentStore.OpenSession())
            {
                

                var user = session.Query<Company>().FirstOrDefault(e=>e.EmailId==email);
                if (user == null)
                {
                    return "error";
                }
                else
                {
                    try
                    {
                        var login = new Login()
                        {
                            EmailAddress = email,
                            LoginTime = DateTime.UtcNow,
                            

                        };
                        session.Store(login, login.Id);
                        session.SaveChanges();
                        return "savedchanges";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return "errorfromcacth";
                    }
                }
                
            }
            
           
        }

        public  async Task<Profile?>  GetProfile(string ID)
        {

            //string loggedname = _contextAccessor.HttpContext.Session.GetString("Name");
            //var profile = new Profile();
            using (var session = _documentStore.OpenSession())
            {
                var profile = session.Query<Profile>(collectionName:"Profile").FirstOrDefault(e => e.ExternalId == ID);

                if (profile == null)
                {
                   return new Profile { statusmsg = "Profile Not found" };

                    //return "not found";
                }
                else
                {
                    return profile;
                  //  return profile.Name;
                }
               
            }
        }



    }

}
