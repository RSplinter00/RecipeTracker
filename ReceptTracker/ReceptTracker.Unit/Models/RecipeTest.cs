using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;

namespace ReceptTracker.Unit.Models
{
    [TestClass]
    public class RecipeTest
    {
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

        [TestMethod]
        public void FailTest()
        {
            Assert.AreEqual(0, 1, "This should prevent the commit from being pushed.");
        }

        [TestMethod]
        public void TotalDuration_CalculatesSumOfTimeValues()
        {
            // Arrange
            int numTestRecipes = 6;
            TimeSpan prepTime = new TimeSpan(1, 0, 0);
            TimeSpan cookingTime = new TimeSpan(4, 0, 0);
            TimeSpan restTime = new TimeSpan(0, 30, 0);
            
            List<Recipe> recipes = new List<Recipe>();
            for (int i = 0; i < numTestRecipes; i++) recipes.Add(new Recipe());

            List<TimeSpan> expectedDurations = new List<TimeSpan>
            {
                new TimeSpan(),
                prepTime,
                prepTime + cookingTime,
                cookingTime + restTime,
                prepTime + restTime,
                prepTime + cookingTime + restTime,
            };

            // Act
            recipes[1].PrepTime = prepTime;
            recipes[2].PrepTime = prepTime;
            recipes[2].CookingTime = cookingTime;
            recipes[3].CookingTime = cookingTime;
            recipes[3].RestTime = restTime;
            recipes[4].PrepTime = prepTime;
            recipes[4].RestTime = restTime;
            recipes[5].PrepTime = prepTime;
            recipes[5].CookingTime = cookingTime;
            recipes[5].RestTime = restTime;

            // Assert
            for (int i = 0; i < recipes.Count; i++) Assert.AreEqual(recipes[i].TotalDuration, expectedDurations[i], "Total Duration incorrectly calculated.");
        }

        [TestMethod]
        public void EnToNlTranslation_WithCorrectValue_ReturnsNlTranslation()
        {
            // Arrange
            Recipe recipe = new Recipe();

            // Act
            List<string> actualTranslations = new List<string>();

            foreach (string propertyName in PropertyNamesEn)
            {
                actualTranslations.Add(recipe.EnToNlTranslation(propertyName));
            }

            // Assert
            for (int i = 0; i < actualTranslations.Count; i++)
            {
                Assert.AreEqual(PropertyNamesNl[i], actualTranslations[i], "English property incorrectly translated to Dutch.");
            }
        }

        [TestMethod]
        public void EnToNlTranslation_WithIncorrectValue_ReturnsEmptyString()
        {
            // Arrange
            Recipe recipe = new Recipe();
            string expectedResult = "";

            // Act
            string result_IncorrectValue = recipe.EnToNlTranslation("IncorrectProperty");
            string result_EmptyString = recipe.EnToNlTranslation("");
            string result_Null = recipe.EnToNlTranslation(null);

            // Assert
            Assert.AreEqual(expectedResult, result_IncorrectValue, "Incorrect property name does not return empty string");
            Assert.AreEqual(expectedResult, result_EmptyString, "Empty string input does not return empty string");
            Assert.AreEqual(expectedResult, result_Null, "Null does not return empty string");
        }

        [TestMethod]
        public void NlToEnTranslation_WithCorrectValue_ReturnsNlTranslation()
        {
            // Arrange
            Recipe recipe = new Recipe();

            // Act
            List<string> actualTranslations = new List<string>();

            foreach (string propertyName in PropertyNamesNl)
            {
                actualTranslations.Add(recipe.NlToEnTranslation(propertyName));
            }

            // Assert
            for (int i = 0; i < actualTranslations.Count; i++)
            {
                Assert.AreEqual(PropertyNamesEn[i], actualTranslations[i], "Dutch property incorrectly translated to English.");
            }
        }

        [TestMethod]
        public void NlToEnTranslation_WithIncorrectValue_ReturnsEmptyString()
        {
            // Arrange
            Recipe recipe = new Recipe();
            string expectedResult = "";

            // Act
            string result_IncorrectValue = recipe.NlToEnTranslation("UnknownProperty");
            string result_EmptyString = recipe.NlToEnTranslation("");
            string result_Null = recipe.NlToEnTranslation(null);

            // Assert
            Assert.AreEqual(expectedResult, result_IncorrectValue, "Incorrect property name does not return empty string");
            Assert.AreEqual(expectedResult, result_EmptyString, "Empty string input does not return empty string");
            Assert.AreEqual(expectedResult, result_Null, "Null does not return empty string");
        }
    }
}
