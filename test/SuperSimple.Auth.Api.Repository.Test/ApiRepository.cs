namespace SuperSimple.Auth.Api.Repository.Test
{
    using NUnit.Framework;
    using Manager.Repository;
    using System;

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

        [Test()]
        public void Authenticate()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");
            Assert.AreNotEqual(Guid.Empty,user.AuthToken);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test ()]
        public void Disable()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");
            bool result = apiRepository.Disable(user.AuthToken,domain.Key);

            Assert.True(result);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test ()]
        public void Validate()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");

            user = apiRepository.Validate(user.AuthToken,domain.Key);

            Assert.NotNull(user);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void ValidateDomainKey()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var result = apiRepository.ValidateDomainKey("test", domain.Key);

            Assert.True(result);
            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void End ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");

            bool result = apiRepository.End(domain.Key,user.AuthToken);
            Assert.True(result);
            user = apiRepository.Validate(user.AuthToken,domain.Key);
            Assert.IsNull(user);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void EmailExists ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            var result = apiRepository
                .EmailExists(domain.Key,"test@authenticate.technology");

            Assert.True(result);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void UsernameExists ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            apiRepository.CreateUser(domain.Key, "test","test",
                                     "test@authenticate.technology");

            var result = apiRepository
                .UsernameExists(domain.Key,"test");

            Assert.True(result);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void IpAllowed ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var result = apiRepository.IpAllowed(domain.Key,"127.0.0.1");

            Assert.True(result);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void Forgot ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            apiRepository.CreateUser(domain.Key, "test","test",
                                     "test@authenticate.technology");

            var result = apiRepository
                .Forgot(domain.Key,"test@authenticate.technology");

            Assert.True(result.Length > 0);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }
        [Test()]
        public void ChangePassword ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");

            var result = apiRepository
                .ChangePassword(domain.Key,user.AuthToken,"test1");
            
            Assert.True(result);

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void ChangeUserName ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");

            var result = apiRepository
                .ChangeUserName(domain.Key,user.AuthToken,"test1");

            Assert.True(result);

            user = apiRepository.Validate(user.AuthToken,domain.Key);

            Assert.AreEqual(user.UserName,"test1");

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }

        [Test()]
        public void ChangeEmail ()
        {
            var apiRepository = new ApiMongoRepository(CONNECTION);
            var repository = new MongoRepository(CONNECTION, apiRepository);
            var manager = repository.CreateManager("test", "test");

            var domain = repository.CreateDomain("test", manager);

            var user = apiRepository.CreateUser(domain.Key, "test","test",
                                                "test@authenticate.technology");

            user = apiRepository.Authenticate(domain.Key,"test","test");

            var result = apiRepository
                .ChangeEmail(domain.Key,user.AuthToken,
                             "test@authenticate.dogdun");

            Assert.True(result);

            user = apiRepository.Validate(user.AuthToken,domain.Key);

            Assert.AreEqual(user.Email,"test@authenticate.dogdun");

            repository.DeleteDomain(domain.Id);
            repository.DeleteManager(manager.Id, "test");
        }
    }
}
