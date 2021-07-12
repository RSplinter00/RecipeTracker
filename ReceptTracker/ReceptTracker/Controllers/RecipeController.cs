using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ReceptTracker.Models;
using SQLite;

namespace ReceptTracker.Controllers
{
    public interface IRecipeController
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeAsync(int id);
        Task<int> SaveRecipeAsync(Recipe recipe);
        Task<int> DeleteRecipeAsync(Recipe Recipe);
    }

    public class RecipeController : IRecipeController
    {
        private readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Recipes.db3");

        private readonly SQLiteAsyncConnection recipeDatabase;

        public RecipeController()
        {
            recipeDatabase = new SQLiteAsyncConnection(DatabasePath);
            recipeDatabase.CreateTableAsync<Recipe>().Wait();
        }

        public Task<List<Recipe>> GetRecipesAsync() => recipeDatabase.Table<Recipe>().ToListAsync();

        public Task<Recipe> GetRecipeAsync(int id) => recipeDatabase.Table<Recipe>().Where(i => i.ID == id).FirstOrDefaultAsync();
        
        public Task<int> SaveRecipeAsync(Recipe recipe)
        {
            if (recipe.ID != 0) return recipeDatabase.UpdateAsync(recipe);
            else return recipeDatabase.InsertAsync(recipe);
        }

        public async Task<int> DeleteRecipeAsync(Recipe recipe)
        {
            var response = await recipeDatabase.DeleteAsync(recipe);
            await ResetID();
            return response;
        }

        private async Task ResetID()
        {
            try
            {
                await recipeDatabase.ExecuteAsync("DELETE FROM sqlite_sequence where name = 'Recipe'");
            }
            catch (SQLiteException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
