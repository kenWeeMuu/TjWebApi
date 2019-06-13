using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ErpDb.Entitys;
using ServiceStack;
using ServiceStack.Text;

namespace erpDbtest
{
    class Program
    {
        static void Main(string[] args) {
            using (var db = new ErpDbContext())
            {
                //var res = db.Permissions.ToList();
                //db.Users
                //    .Include(i=>i.Roles)

                //    .FirstOrDefault(f=> f.UserId==4)
                //.Roles
                //.ToList()
                //.ForEach(r=> r.PrintDump());
            var re =    from u in db.Users
                    from r in u.Roles
                    from p in r.Permissions
                    join s in db.Permissions.DefaultIfEmpty() on p.PermissionId equals s.PermissionId 
                    where u.UserId == 4
                    select new
                    {
                        r,
                        p
                    };
                 
                re.ToList().ForEach(r=>r.PrintDump()); 
                //res.ForEach(r=> r.PrintDump());
            }

            Console.ReadLine();
        }
    }
}
