using System.Data.Entity;
using CenterMMibDbViewer.Domain;

namespace CenterMMibDbViewer
{
    internal class MainService
    {
        private MainService()
        {
        }

        public MibDbContext MibDbContext { get; } = new MibDbContext();

        public Database Database => MibDbContext.Database;

        public static MainService Instance { get; } = new MainService();

        public MibDbContext GetNewDbContext() => new MibDbContext();
    }
}
