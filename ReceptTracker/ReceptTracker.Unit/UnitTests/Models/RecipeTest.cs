using AutoFixture;
using NUnit.Framework;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;

namespace ReceptTracker.Unit.UnitTests.Models
{
    [TestFixture]
    public class RecipeTest
    {
        private Recipe recipe;
        private readonly TimeSpan PrepTime = new TimeSpan(1, 0, 0);
        private readonly TimeSpan CookingTime = new TimeSpan(4, 0, 0);
        private readonly TimeSpan RestTime = new TimeSpan(0, 30, 0);
        private readonly Fixture Fixture = new Fixture();

        private readonly List<string> PropertyNamesEn = new List<string>
        {
            "Name",
            "Category",
            "PrepTime",
            "CookingTime",
            "RestTime",
            "TotalDuration",
            "Method",
            "NumPortions",
            "OriginalRecipe",
            "Description",
            "Ingredients",
            "Requirements",
            "Steps",
            "ServeTips"
        };

        private readonly List<string> PropertyNamesNl = new List<string>
        {
            "Naam",
            "Categorie",
            "Voorbereidingstijd",
            "Bereidingstijd",
            "Rusttijd",
            "Totale tijd",
            "Methode",
            "Aantal Porties",
            "Recept",
            "Beschrijving",
            "Ingrediënten",
            "Benodigdheden",
            "Bereidingswijze",
            "Serveertips"
        };

        [SetUp]
        public void SetUp()
        {
            recipe = new Recipe();
        }

        private Recipe CreateRandomizedRecipe()
        {
            return Fixture.Build<Recipe>().Without(i => i.Id).Do(i => i.Id = Fixture.Create<Guid>()).Create();
        }

        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        public void TotalDuration_ShouldCalculateSumOfTimeValues(bool prepTime, bool cookingTime, bool restTime)
        {
            // Arrange
            TimeSpan expectedDuration = new TimeSpan();

            // Act
            if (prepTime)
            {
                recipe.PrepTime = PrepTime;
                expectedDuration += PrepTime;
            }
            if (cookingTime)
            {
                recipe.CookingTime = CookingTime;
                expectedDuration += CookingTime;
            }
            if (restTime)
            {
                recipe.RestTime = RestTime;
                expectedDuration += RestTime;
            }

            // Assert
            Assert.AreEqual(recipe.TotalDuration, expectedDuration, "Total Duration incorrectly calculated.");
        }

        [Test]
        public void EnToNlTranslation_WithCorrectValue_ShouldReturnNlTranslation()
        {
            // Arrange
            List<string> actualTranslations = new List<string>();

            // Act
            foreach (string propertyName in PropertyNamesEn) actualTranslations.Add(recipe.EnToNlTranslation(propertyName));

            // Assert
            for (int i = 0; i < actualTranslations.Count; i++) Assert.AreEqual(PropertyNamesNl[i], actualTranslations[i], "English property incorrectly translated to Dutch.");
        }

        [TestCase("IncorrectProperty")]
        [TestCase("")]
        [TestCase(null)]
        public void EnToNlTranslation_WithIncorrectValue_ShouldReturnEmptyString(string input)
        {
            // Arrange
            string expectedResult = "";

            // Act
            string result = recipe.EnToNlTranslation(input);

            // Assert
            Assert.AreEqual(expectedResult, result, "Incorrect value does not return empty string");
        }

        [Test]
        public void NlToEnTranslation_WithCorrectValue_ShouldReturnNlTranslation()
        {
            // Arrange
            List<string> actualTranslations = new List<string>();

            // Act
            foreach (string propertyName in PropertyNamesNl) actualTranslations.Add(recipe.NlToEnTranslation(propertyName));

            // Assert
            for (int i = 0; i < actualTranslations.Count; i++) Assert.AreEqual(PropertyNamesEn[i], actualTranslations[i], "Dutch property incorrectly translated to English.");
        }

        [TestCase("IncorrectProperty")]
        [TestCase("")]
        [TestCase(null)]
        public void NlToEnTranslation_WithIncorrectValue_ShouldReturnEmptyString(string input)
        {
            // Arrange
            string expectedResult = "";

            // Act
            string result = recipe.NlToEnTranslation(input);

            // Assert
            Assert.AreEqual(expectedResult, result, "Incorrect value does not return empty string");
        }
        
        [Test]
        public void Equals_WithSameRecipe_ShouldReturnTrue()
        {
            // Arrange
            recipe = CreateRandomizedRecipe();

            // Act
            var result = recipe.Equals(recipe);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_WithSameIdDifferentProperties_ShouldReturnTrue()
        {
            // Arrange
            var id = Fixture.Create<Guid>();
            var desc = Fixture.Create<string>();
            var time = Fixture.Create<TimeSpan>();
            recipe = new Recipe
            {
                Id = id,
                Description = desc,
                PrepTime = time,
                NumPortions = 1
            };
            var recipe2 = new Recipe
            {
                Id = id,
                Description = desc,
                PrepTime = time,
                NumPortions = 2
            };

            // Act
            var result = recipe.Equals(recipe2);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void Equals_WithDifferentId_ShouldReturnFalse()
        {
            // Arrange
            recipe = CreateRandomizedRecipe();
            var recipe2 = CreateRandomizedRecipe();

            // Act
            var result = recipe.Equals(recipe2);

            // Arrange
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_WithDifferentIdSameProperties_ShouldReturnFalse()
        {
            // Arrange
            var desc = Fixture.Create<string>();
            var time = Fixture.Create<TimeSpan>();
            recipe = new Recipe
            {
                Id = Fixture.Create<Guid>(),
                Description = desc,
                PrepTime = time
            };
            var recipe2 = new Recipe
            {
                Id = Fixture.Create<Guid>(),
                Description = desc,
                PrepTime = time
            };

            // Act
            var result = recipe.Equals(recipe2);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Equals_WithDifferentObject_ShouldReturnFalse()
        {
            // Arrange
            recipe = CreateRandomizedRecipe();
            var object2 = Fixture.Create<TimeSpan>();

            // Act
            var result = recipe.Equals(object2);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
