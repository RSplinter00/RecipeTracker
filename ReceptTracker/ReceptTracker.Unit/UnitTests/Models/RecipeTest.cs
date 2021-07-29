﻿using NUnit.Framework;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;

namespace ReceptTracker.Unit.UnitTests.Models
{
    [TestFixture]
    class RecipeTest
    {
        private TimeSpan PrepTime = new TimeSpan(1, 0, 0);
        private readonly TimeSpan CookingTime = new TimeSpan(4, 0, 0);
        private readonly TimeSpan RestTime = new TimeSpan(0, 30, 0);

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

        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, true)]
        public void TotalDuration_CalculatesSumOfTimeValues(bool prepTime, bool cookingTime, bool restTime)
        {
            // Arrange
            Recipe recipe = new Recipe();
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
        public void EnToNlTranslation_WithCorrectValue_ReturnsNlTranslation()
        {
            // Arrange
            Recipe recipe = new Recipe();

            // Act
            List<string> actualTranslations = new List<string>();

            foreach (string propertyName in PropertyNamesEn) actualTranslations.Add(recipe.EnToNlTranslation(propertyName));

            // Assert
            for (int i = 0; i < actualTranslations.Count; i++) Assert.AreEqual(PropertyNamesNl[i], actualTranslations[i], "English property incorrectly translated to Dutch.");
        }

        [TestCase("IncorrectProperty")]
        [TestCase("")]
        [TestCase(null)]
        public void EnToNlTranslation_WithIncorrectValue_ReturnsEmptyString(string input)
        {
            // Arrange
            Recipe recipe = new Recipe();
            string expectedResult = "";

            // Act
            string result = recipe.EnToNlTranslation(input);

            // Assert
            Assert.AreEqual(expectedResult, result, "Incorrect value does not return empty string");
        }

        [Test]
        public void NlToEnTranslation_WithCorrectValue_ReturnsNlTranslation()
        {
            // Arrange
            Recipe recipe = new Recipe();

            // Act
            List<string> actualTranslations = new List<string>();

            foreach (string propertyName in PropertyNamesNl) actualTranslations.Add(recipe.NlToEnTranslation(propertyName));

            // Assert
            for (int i = 0; i < actualTranslations.Count; i++) Assert.AreEqual(PropertyNamesEn[i], actualTranslations[i], "Dutch property incorrectly translated to English.");
        }

        [TestCase("IncorrectProperty")]
        [TestCase("")]
        [TestCase(null)]
        public void NlToEnTranslation_WithIncorrectValue_ReturnsEmptyString(string input)
        {
            // Arrange
            Recipe recipe = new Recipe();
            string expectedResult = "";

            // Act
            string result = recipe.NlToEnTranslation(input);

            // Assert
            Assert.AreEqual(expectedResult, result, "Incorrect value does not return empty string");
        }
    }
}
