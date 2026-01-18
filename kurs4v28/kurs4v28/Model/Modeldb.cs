using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace kurs4v28.Model
{
    public static class ModelDB
    {
        public static post_officeEntities2 _context;
        public static post_officeEntities2 GetContext()
        {
            if (_context == null)
            {
                _context = new post_officeEntities2();
            }
            return _context;
        }

    }
}
