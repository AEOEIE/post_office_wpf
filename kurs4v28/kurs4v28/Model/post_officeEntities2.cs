using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kurs4v28.Model
{
    public class DbContextWrapper : IDbContext
    {
        private readonly post_officeEntities2 _context;
        public DbContextWrapper(post_officeEntities2 context)
        {
            _context = context;
        }
        public IQueryable<services> services => _context.services;
        public IQueryable<employee> employee => _context.employee;
        public IQueryable<Client> Client => _context.Client;
        public IQueryable<Role> Role => _context.Role;
        public IQueryable<Sposob_otpravki> Sposob_otpravki => _context.Sposob_otpravki;
        public IQueryable<post_otpravlenie> post_otpravlenie => _context.post_otpravlenie;
        public IQueryable<popitki_vhoda> popitki_vhoda => _context.popitki_vhoda;
        public IQueryable<report> report => _context.report;

        public void Add<T>(T entity) where T : class => _context.Set<T>().Add(entity);
        public void Remove<T>(T entity) where T : class => _context.Set<T>().Remove(entity);
        public int SaveChanges() => _context.SaveChanges();
    }
}
