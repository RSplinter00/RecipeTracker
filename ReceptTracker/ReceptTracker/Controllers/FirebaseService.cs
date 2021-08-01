using Firebase.Database;
using Firebase.Database.Query;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ReceptTracker.Controllers
{
    class FirebaseService : IDatabaseService
    {
        private readonly GoogleAuthenticationService AuthService = new GoogleAuthenticationService();
        private string ChildName { get => AuthService.GetUser().Id; }

        private readonly FirebaseClient Firebase;

        public FirebaseService()
        {
            Firebase = new FirebaseClient(AppSettingsManager.Settings["FirebaseDatabasePath"]);
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                var result = (await Firebase
                    .Child(ChildName)
                    .OnceAsync<Recipe>()).Select(item => FirebaseObjectToRecipe(item)).ToList();

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

                await Firebase
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

                    await Firebase
                        .Child(ChildName)
                        .PostAsync(recipe.ToString());
                }
                else
                {
                    var toUpdateRecipe = (await Firebase
                        .Child(ChildName)
                        .OnceAsync<Recipe>()).FirstOrDefault(a => a.Object.Id == recipe.Id);

                    await Firebase
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
                var toDeleteRecipe = (await Firebase
                    .Child(ChildName)
                    .OnceAsync<Recipe>()).FirstOrDefault(a => a.Object.Id == id);

                await Firebase
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

        private Recipe FirebaseObjectToRecipe(FirebaseObject<Recipe> item)
        {
            try
            {
                return new Recipe
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
                };
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new Recipe();
            }
        }

        public Task SyncRecipes()
        {
            throw new NotImplementedException();
        }
    }
}
