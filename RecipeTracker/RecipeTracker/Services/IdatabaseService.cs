using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    /// <summary>
    /// Interface <c>IDatabaseService</c> is the template used for all services that communicate with the database.
    /// 
    /// <para>
    ///     For the general database service, see <seealso cref="DatabaseService"/>.<br/>
    ///     For the Firebase service, see <seealso cref="FirebaseService"/>.<br/>
    ///     For the caching service, see <seealso cref="CachingService"/>.
    /// </para>
    /// </summary>
    public interface IDatabaseService
    {
        Task<List<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeAsync(Guid id);
        Task<bool> SaveRecipeAsync(Recipe recipe);
        Task<bool> DeleteRecipeAsync(Guid id);
        Task SyncRecipesAsync();
    }
}
