using Plugin.Connectivity;
using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    /// <summary>
    /// Class <c>DatabaseService</c> is the main service that handles the communication with the databases.
    /// If the user is connected and logged in, it will use the cloud for its operations, else it will use the cache.
    /// </summary>
    public class DatabaseService : IDatabaseService
    {
        private readonly IAuthenticationService AuthService;
        private readonly CachingService CachingService;
        private readonly FirebaseService FirebaseService;

        private bool IsConnected { get => CrossConnectivity.Current.IsConnected; }
        private bool IsLoggedIn { get => AuthService.GetUser() != null; }

        public DatabaseService(IAuthenticationService authService)
        {
            AuthService = authService;
            CachingService = new CachingService();
            FirebaseService = new FirebaseService();
        }

        /// <summary>
        /// Retrieves all recipes of the user from the database.
        /// </summary>
        /// <returns>All recipes saved by the user.</returns>
        public Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.GetRecipesAsync();
                else return CachingService.GetRecipesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return CachingService.GetRecipesAsync();
            }
        }

        /// <summary>
        /// Retrieves a specific recipe.
        /// </summary>
        /// <param name="id">Id of the recipe to be retrieved.</param>
        /// <returns>The recipe with the given id.</returns>
        public Task<Recipe> GetRecipeAsync(Guid id)
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.GetRecipeAsync(id);
                else return CachingService.GetRecipeAsync(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return CachingService.GetRecipeAsync(id);
            }
        }

        /// <summary>
        /// Saves the given recipe to the database.
        /// </summary>
        /// <param name="recipe">The updated/new recipe that should be saved.</param>
        /// <returns>If the operation was successful.</returns>
        public Task<bool> SaveRecipeAsync(Recipe recipe)
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.SaveRecipeAsync(recipe);
                else return CachingService.SaveRecipeAsync(recipe);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return CachingService.SaveRecipeAsync(recipe);
            }
        }

        /// <summary>
        /// Deletes the recipe with the given id from the database.
        /// </summary>
        /// <param name="id">The id of the recipe to be deleted</param>
        /// <returns>If the operation was successful.</returns>
        public Task<bool> DeleteRecipeAsync(Guid id)
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.DeleteRecipeAsync(id);
                else return CachingService.DeleteRecipeAsync(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return CachingService.DeleteRecipeAsync(id);
            }
        }

        /// <summary>
        /// Synchronizes locally saved recipes with the cloud.
        /// </summary>
        public async Task SyncRecipesAsync()
        {
            try
            {
                if (IsConnected && IsLoggedIn)
                {
                    // If the user is connected and logged in, retrieve all cached recipes.
                    var localRecipes = await CachingService.GetRecipesAsync();

                    if (localRecipes.Count > 0)
                    {
                        // If the user has cached any recipes, save them to the cloud and delete them locally.
                        foreach (Recipe recipe in localRecipes)
                        {
                            var id = recipe.Id;
                            recipe.Id = Guid.Empty;

                            var response = await FirebaseService.SaveRecipeAsync(recipe);
                            if (response) await CachingService.DeleteRecipeAsync(id);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
