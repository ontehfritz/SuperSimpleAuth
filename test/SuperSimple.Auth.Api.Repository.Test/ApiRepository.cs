namespace SuperSimple.Auth.Api.Repository.Test
{
    using NUnit.Framework;
    using Manager.Repository;

    [TestFixture ()]
    public class ApiRepositoryTest
    {
        private const string CONNECTION = "mongodb://localhost";

        [Test ()]
        public void CreateUser ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                     "test@authenticate.technology");

            Assert.AreEqual("test@authenticate.technology",user.Email);
            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }
    }
}
