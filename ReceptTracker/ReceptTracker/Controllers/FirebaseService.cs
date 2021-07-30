using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceptTracker.Controllers
{
    public interface IFirebaseService
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeAsync(Guid id);
        Task<bool> SaveRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(Guid id);
    }

    public class FirebaseService : IFirebaseService
    {
        private GoogleAuthenticationService authService = new GoogleAuthenticationService();
        private string ChildName { get => authService.GetUser().Id; }

        private readonly FirebaseClient firebase;

        public FirebaseService()
        {
            firebase = new FirebaseClient(AppSettingsManager.Settings["FirebaseDatabasePath"]);
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                var test = authService.GetUser();

                var result = (await firebase
                    .Child(ChildName)
                    .OnceAsync<Recipe>()).Select(item => new Recipe
                    {
                        Id = item.Object.Id,
                        Name = item.Object.Name,
                        Category = item.Object.Category,
                        PrepTime = item.Object.PrepTime,
                        CookingTime = item.Object.CookingTime,
                        RestTime = item.Object.RestTime,
                        Method = item.Object.Method,
                        NumPortions = item.Object.NumPortions,
                        OriginalRecipe = item.Object.OriginalRecipe,
                        Description = item.Object.Description,
                        Ingredients = item.Object.Ingredients,
                        Requirements = item.Object.Requirements,
                        Steps = item.Object.Steps,
                        ServeTips = item.Object.ServeTips
                    }).ToList();

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Recipe>();
            }
        }

        public async Task<Recipe> GetRecipeAsync(Guid id)
        {
            try
            {
                var allPersons = await GetRecipesAsync();

                await firebase
                    .Child(ChildName)
                    .OnceAsync<Recipe>();

                var result = allPersons.FirstOrDefault(a => a.Id == id);

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<bool> SaveRecipeAsync(Recipe recipe)
        {
            try
            {
                if (recipe.Id == Guid.Empty)
                {
                    recipe.Id = Guid.NewGuid();

                    await firebase
                        .Child(ChildName)
                        .PostAsync(recipe.ToString());
                }
                else
                {
                    var toUpdateRecipe = (await firebase
                        .Child(ChildName)
                        .OnceAsync<Recipe>()).FirstOrDefault(a => a.Object.Id == recipe.Id);

                    await firebase
                        .Child(ChildName)
                        .Child(toUpdateRecipe.Key)
                        .PutAsync(recipe);
                }
             
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRecipeAsync(Guid id)
        {
            try
            {
                var toDeleteRecipe = (await firebase
                    .Child(ChildName)
                    .OnceAsync<Recipe>()).FirstOrDefault(a => a.Object.Id == id);

                await firebase
                    .Child(ChildName)
                    .Child(toDeleteRecipe.Key)
                    .DeleteAsync();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
