using AutoFixture;
using Moq;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Services;
using NUnit.Framework;
using Plugin.GoogleClient.Shared;

namespace ReceptTracker.Unit.UnitTests.ViewModels
{
    public class ViewModelBaseTest
    {
        protected MockRepository MockRepository { get; }
        protected Fixture Fixture { get; }
        protected Mock<INavigationService> NavigationServiceMock { get; }
        protected Mock<IPageDialogService> PageDialogServiceMock { get; }
        protected Mock<IAuthenticationService> AuthServiceMock { get; }
        protected Mock<IDatabaseService> DatabaseServiceMock { get; }
        protected GoogleUser CurrentUser { get; }

        public ViewModelBaseTest()
        {
            MockRepository = new MockRepository(MockBehavior.Loose);
            Fixture = new Fixture();

            NavigationServiceMock = MockRepository.Create<INavigationService>();
            PageDialogServiceMock = MockRepository.Create<IPageDialogService>();
            AuthServiceMock = MockRepository.Create<IAuthenticationService>();
            DatabaseServiceMock = MockRepository.Create<IDatabaseService>();

            CurrentUser = Fixture.Create<GoogleUser>();
        }

        [SetUp]
        public void Setup()
        {
            NavigationServiceMock.Reset();
            PageDialogServiceMock.Reset();
            AuthServiceMock.Reset();
            DatabaseServiceMock.Reset();
        }
    }
}
