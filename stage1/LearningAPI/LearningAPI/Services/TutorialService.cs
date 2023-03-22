using LearningAPI.Models;
using LearningAPI;

namespace LearningAPI.Services
{
    public class TutorialService
    {
        private readonly MyDbContext _context;

        public TutorialService(MyDbContext context)
        {
            _context = context;
        }

        public void Add(Tutorial tutorial)
        {
            _context.Tutorials.Add(tutorial);
            _context.SaveChanges();
        }

        public List<Tutorial> GetAll()
        {
            return _context.Tutorials.OrderByDescending(o => o.CreatedAt).ToList();
        }

        public Tutorial? Get(int id)
        {
            return _context.Tutorials.FirstOrDefault(o => o.Id == id);
        }

        public void Update(Tutorial tutorial)
        {
            _context.Tutorials.Update(tutorial);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Tutorial? tutorial = _context.Tutorials.FirstOrDefault(o => o.Id == id);
            if (tutorial != null)
            {
                _context.Tutorials.Remove(tutorial);
                _context.SaveChanges();
            }
        }
    }
}