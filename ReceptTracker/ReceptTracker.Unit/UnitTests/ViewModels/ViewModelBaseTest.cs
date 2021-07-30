using AutoFixture;
using Moq;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using System;
using System.Collections.Generic;

namespace ReceptTracker.Unit.UnitTests.ViewModels
{
    class ViewModelBaseTest
    {
        protected MockRepository MockRepository { get; }
        protected Fixture Fixture { get; }
        protected Mock<INavigationService> NavigationServiceMock { get; }
        protected Mock<IPageDialogService> PageDialogServiceMock { get; }
        //protected Mock<IRecipeController> RecipeControllerMock { get; }
        protected Mock<IAuthenticationService> AuthServiceMock { get; }
        protected Mock<IFirebaseService> FirebaseServiceMock { get; }

        public ViewModelBaseTest()
        {
            MockRepository = new MockRepository(MockBehavior.Loose);
            Fixture = new Fixture();

            NavigationServiceMock = MockRepository.Create<INavigationService>();
            PageDialogServiceMock = MockRepository.Create<IPageDialogService>();
            //RecipeControllerMock = MockRepository.Create<IRecipeController>();
            AuthServiceMock = MockRepository.Create<IAuthenticationService>();
            FirebaseServiceMock = MockRepository.Create<IFirebaseService>();
        }
    }
}
