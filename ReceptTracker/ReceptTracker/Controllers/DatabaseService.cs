using Plugin.Connectivity;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReceptTracker.Controllers
{
    public interface IDatabaseService
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeAsync(Guid id);
        Task<bool> SaveRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(Guid id);
        Task SyncRecipes();
    }

    public class DatabaseService : IDatabaseService
    {
        private readonly GoogleAuthenticationService AuthService;
        private readonly RecipeController RecipeController;
        private readonly FirebaseService FirebaseService;

        private bool IsConnected { get => CrossConnectivity.Current.IsConnected; }
        private bool IsLoggedIn { get => AuthService.GetUser() != null; }

        public DatabaseService()
        {
            AuthService = new GoogleAuthenticationService();
            RecipeController = new RecipeController();
            FirebaseService = new FirebaseService();
        }

        public Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.GetRecipesAsync();
                else return RecipeController.GetRecipesAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RecipeController.GetRecipesAsync();
            }
        }

        public Task<Recipe> GetRecipeAsync(Guid id)
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.GetRecipeAsync(id);
                else return RecipeController.GetRecipeAsync(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RecipeController.GetRecipeAsync(id);
            }
        }

        public Task<bool> SaveRecipeAsync(Recipe recipe)
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.SaveRecipeAsync(recipe);
                else return RecipeController.SaveRecipeAsync(recipe);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RecipeController.SaveRecipeAsync(recipe);
            }
        }

        public Task<bool> DeleteRecipeAsync(Guid id)
        {
            try
            {
                if (IsConnected && IsLoggedIn) return FirebaseService.DeleteRecipeAsync(id);
                else return RecipeController.DeleteRecipeAsync(id);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return RecipeController.DeleteRecipeAsync(id);
            }
        }

        public async Task SyncRecipes()
        {
            try
            {
                if (IsConnected && IsLoggedIn)
                {
                    var localRecipes = await RecipeController.GetRecipesAsync();

                    if (localRecipes.Count > 0)
                    {
                        foreach (Recipe recipe in localRecipes)
                        {
                            var id = recipe.Id;
                            recipe.Id = Guid.Empty;

                            var response = await FirebaseService.SaveRecipeAsync(recipe);
                            if (response) await RecipeController.DeleteRecipeAsync(id);
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
