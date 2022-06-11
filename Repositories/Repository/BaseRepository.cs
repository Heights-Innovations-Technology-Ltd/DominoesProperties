using Models.Context;
namespace Repositories.Repository
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseRepository
    {
        /// <summary>
        /// The context
        /// </summary>
        protected dominoespropertiesContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepository" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public BaseRepository(dominoespropertiesContext context)
        {
            _context = context;
        }
    }
}
