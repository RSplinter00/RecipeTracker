using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    /// <summary>
    /// Class <c>FirebaseService</c> handles all operations between the application and Google Firebase.
    /// </summary>
    public class FirebaseService : IDatabaseService
    {
        private string ChildName { get => $"recipes/{ GoogleAuthenticationService.UserID }"; }

        private readonly GoogleAuthenticationService AuthService;
        private FirebaseClient Firebase { get => AuthService.Firebase; }

        public FirebaseService()
        {
            AuthService = new GoogleAuthenticationService();
        }

        /// <summary>
        /// Retrieves all recipes from Firebase.
        /// 
        /// <para>
        ///     For the offline operation, see <seealso cref="CachingService.GetRecipesAsync"/>.
        /// </para>
        /// </summary>
        /// <returns>All recipes saved to the cloud.</returns>
        public async Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                return (await Firebase
                    .Child(ChildName)
                    .OnceAsync<Recipe>()).Select(item => FirebaseObjectToRecipe(item)).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return new List<Recipe>();
            }
        }

        /// <summary>
        /// Returns a specific recipe from Firebase.
        /// 
        /// <para>
        ///     For the offline operation, see <seealso cref="CachingService.GetRecipeAsync(Guid)"/>.
        /// </para>
        /// </summary>
        /// <param name="id">Id of the recipe to be retrieved</param>
        /// <returns>The recipe with the given id.</returns>
        public async Task<Recipe> GetRecipeAsync(Guid id)
        {
            try
            {
                // Retrieve all recipes and filter out the requested recipe.
                var recipes = await GetRecipesAsync();

                return recipes.FirstOrDefault(a => a.Id == id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Saves the given recipe to Firebase.
        /// 
        /// <para>
        ///     For the offline operation, see <seealso cref="CachingService.SaveRecipeAsync(Recipe)"/>.
        /// </para>
        /// </summary>
        /// <param name="recipe">Recipe to be saved to Firebase.</param>
        /// <returns>If the operation was successful.</returns>
        public async Task<bool> SaveRecipeAsync(Recipe recipe)
        {
            try
            {
                if (recipe.Id == Guid.Empty)
                {
                    // If the recipe doesn't have a valid id, it is a new recipe.
                    recipe.Id = Guid.NewGuid();

                    await Firebase
                        .Child(ChildName)
                        .PostAsync(JsonConvert.SerializeObject(recipe));
                }
                else
                {
                    // If the recipe does have a valid id, it is an editted recipe.
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

        /// <summary>
        /// Deletes the recipe with the given id from Firebase.
        /// 
        /// <para>
        ///     For the offline operation, see <seealso cref="CachingService.DeleteRecipeAsync(Guid)"/>.
        /// </para>
        /// </summary>
        /// <param name="id">Id of the recipe to be deleted.</param>
        /// <returns>If the operation was successful.</returns>
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

        /// <summary>
        /// Converts a Firebase object of type Recipe to a recipe Object.
        /// </summary>
        /// <param name="item">Firebase object to be converted to a recipe.</param>
        /// <returns>The Recipe value of hte Firebase object.</returns>
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

        /// <summary>
        /// Class <c>FirebaseService</c> does not implement method <c>SyncRecipesAsync</c>.
        /// 
        /// <para>
        ///     For the implementation, see <seealso cref="DatabaseService.SyncRecipesAsync"/>.
        /// </para>
        /// 
        /// <para>
        ///     <exception cref="NotImplementedException"><c>NotImplementedException</c> thrown because this method is not implemented by this class.</exception>
        /// </para>
        /// </summary>
        public Task SyncRecipesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
