using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RecipeTracker.Models;
using SQLite;

namespace RecipeTracker.Services
{
    /// <summary>
    /// Class <c>CachingService</c> manages the caching of recipes if the user does not have an internet connection or is not logged in.
    /// </summary>
    public class CachingService : IDatabaseService
    {
        private readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Recipes.db3");

        private readonly SQLiteAsyncConnection CachingDatabase;

        public CachingService()
        {
            CachingDatabase = new SQLiteAsyncConnection(DatabasePath);
            CachingDatabase.CreateTableAsync<Recipe>().Wait();
        }

        /// <summary>
        /// Retrieves all locally stored recipes.
        /// 
        /// <para>
        ///     For the online operation, see <seealso cref="FirebaseService.GetRecipesAsync"/>.
        /// </para>
        /// </summary>
        /// <returns>All cached recipes.</returns>
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

        /// <summary>
        /// Retrieves the recipe with the given id.
        /// 
        /// <para>
        ///     For the online operation, see <seealso cref="FirebaseService.GetRecipeAsync(Guid)"/>.
        /// </para>
        /// </summary>
        /// <param name="id">Id of the recipe to be retrieved.</param>
        /// <returns>The recipe with the given id.</returns>
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

        /// <summary>
        /// Caches the given recipe locally on the device.
        /// 
        /// <para>
        ///     For the online operation, see <seealso cref="FirebaseService.SaveRecipeAsync(Recipe)"/>.
        /// </para>
        /// </summary>
        /// <param name="recipe">The recipe to be cached.</param>
        /// <returns>If the operation was successful.</returns>
        public async Task<bool> SaveRecipeAsync(Recipe recipe)
        {
            var response = 0;

            try
            {
                if (recipe.Id != Guid.Empty)
                {
                    // If the id is valid, it is an editted recipe and the update operation can be executed.
                    response = await CachingDatabase.UpdateAsync(recipe);
                }
                else
                {
                    // If the id is not valid, it is a new recipe and the insert operation can be executed.
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

        /// <summary>
        /// Deletes the cached recipe with the given id.
        /// 
        /// <para>
        ///     For the online operation, see <seealso cref="FirebaseService.DeleteRecipeAsync(Guid)"/>
        /// </para>
        /// </summary>
        /// <param name="id">Id of the recipe to be deleted.</param>
        /// <returns>If the recipe was succcessful.</returns>
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

        /// <summary>
        /// Class <c>CachingService</c> does not implement method <c>SyncRecipesAsync</c>.
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
