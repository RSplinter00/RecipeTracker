using NUnit.Framework;
using ReceptTracker.ViewModels;
using System;

namespace ReceptTracker.Unit.UnitTests.ViewModels
{
    [TestFixture]
    class MainPageViewModelTest : ViewModelBaseTest
    {
        private MainPageViewModel MainPageViewModel { get; set; }

        [SetUp]
        public void SetUp()
        {
            MainPageViewModel = new MainPageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, RecipeControllerMock.Object);
        }
    }
}
