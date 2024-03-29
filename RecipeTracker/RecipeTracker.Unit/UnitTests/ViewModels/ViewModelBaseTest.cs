﻿using AutoFixture;
using Moq;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using NUnit.Framework;
using Plugin.GoogleClient.Shared;

namespace RecipeTracker.Unit.UnitTests.ViewModels
{
    /// <summary>
    /// Class <c>ViewModelBaseTest</c> is the base class used for every viewmodel test classes.
    /// </summary>
    public class ViewModelBaseTest
    {
        protected MockRepository MockRepository { get; }
        protected Fixture Fixture { get; }
        protected Mock<INavigationService> NavigationServiceMock { get; }
        protected Mock<IPageDialogService> PageDialogServiceMock { get; }
        protected Mock<IAuthenticationService> AuthServiceMock { get; }
        protected Mock<IDatabaseService> DatabaseServiceMock { get; }
        protected GoogleUser CurrentUser { get; private set; }

        public ViewModelBaseTest()
        {
            MockRepository = new MockRepository(MockBehavior.Loose);
            Fixture = new Fixture();

            // Create mocks for all services.
            NavigationServiceMock = MockRepository.Create<INavigationService>();
            PageDialogServiceMock = MockRepository.Create<IPageDialogService>();
            AuthServiceMock = MockRepository.Create<IAuthenticationService>();
            DatabaseServiceMock = MockRepository.Create<IDatabaseService>();
        }

        [SetUp]
        public void Setup()
        {
            CurrentUser = Fixture.Create<GoogleUser>();

            // Reset any calls made to the mocks.
            NavigationServiceMock.Reset();
            PageDialogServiceMock.Reset();
            AuthServiceMock.Reset();
            DatabaseServiceMock.Reset();
        }
    }
}
