using Plugin.Connectivity;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ReceptTracker.Services
{
    public interface IDatabaseService
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeAsync(Guid id);
        Task<bool> SaveRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(Guid id);
        Task SyncRecipesAsync();
    }

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

        public async Task SyncRecipesAsync()
        {
            try
            {
                if (IsConnected && IsLoggedIn)
                {
                    var localRecipes = await CachingService.GetRecipesAsync();

                    if (localRecipes.Count > 0)
                    {
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
