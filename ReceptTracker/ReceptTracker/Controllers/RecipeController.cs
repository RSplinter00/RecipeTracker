using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ReceptTracker.Models;
using SQLite;

namespace ReceptTracker.Controllers
{
    class RecipeController : IDatabaseService
    {
        private readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Recipes.db3");

        private readonly SQLiteAsyncConnection RecipeDatabase;

        public RecipeController()
        {
            RecipeDatabase = new SQLiteAsyncConnection(DatabasePath);
            RecipeDatabase.CreateTableAsync<Recipe>().Wait();
        }

        public Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                return RecipeDatabase.Table<Recipe>().ToListAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return Task.Run(() => new List<Recipe>());
            }
        }

        public Task<Recipe> GetRecipeAsync(Guid id)
        {
            try
            {
                return RecipeDatabase.Table<Recipe>().Where(i => i.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        
        public async Task<bool> SaveRecipeAsync(Recipe recipe)
        {
            var response = 0;

            try
            {
                if (recipe.Id != Guid.Empty)
                {
                    response = await RecipeDatabase.UpdateAsync(recipe);
                }
                else
                {
                    recipe.Id = Guid.NewGuid();
                    response = await RecipeDatabase.InsertAsync(recipe);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return response > 0;
        }

        public async Task<bool> DeleteRecipeAsync(Guid id)
        {
            var response = 0;

            try
            {
                response = await RecipeDatabase.ExecuteAsync($"DELETE FROM Recipe WHERE Id LIKE '{id}'");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return response > 0;
        }

        public Task SyncRecipes()
        {
            throw new NotImplementedException();
        }
    }
}
