using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ReceptTracker.Models;
using SQLite;

namespace ReceptTracker.Services
{
    class CachingService : IDatabaseService
    {
        private readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Recipes.db3");

        private readonly SQLiteAsyncConnection CachingDatabase;

        public CachingService()
        {
            CachingDatabase = new SQLiteAsyncConnection(DatabasePath);
            CachingDatabase.CreateTableAsync<Recipe>().Wait();
        }

        public Task<List<Recipe>> GetRecipesAsync()
        {
            try
            {
                return CachingDatabase.Table<Recipe>().ToListAsync();
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
                return CachingDatabase.Table<Recipe>().Where(i => i.Id == id).FirstOrDefaultAsync();
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
                    response = await CachingDatabase.UpdateAsync(recipe);
                }
                else
                {
                    recipe.Id = Guid.NewGuid();
                    response = await CachingDatabase.InsertAsync(recipe);
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
                response = await CachingDatabase.ExecuteAsync($"DELETE FROM Recipe WHERE Id LIKE '{id}'");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return response > 0;
        }

        public Task SyncRecipesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
