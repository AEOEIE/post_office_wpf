using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kurs4v28.Model
{
    public interface IDbContext
    {
        IQueryable<services> services { get; }
        IQueryable<employee> employee { get; }
        IQueryable<Client> Client { get; }
        IQueryable<Role> Role { get; }
        IQueryable<Sposob_otpravki> Sposob_otpravki { get; }
        IQueryable<post_otpravlenie> post_otpravlenie { get; }
        IQueryable<popitki_vhoda> popitki_vhoda { get; }
        IQueryable<report> report { get; }
        void Add<T>(T entity) where T : class;
        void Remove<T>(T entity) where T : class;
        int SaveChanges();

    }
}
