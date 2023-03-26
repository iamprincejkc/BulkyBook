using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CoverTypeRepository(ApplicationDbContext db) : base(db)
        {
            _dbContext = db;
        }

        public void Update(CoverType coverType)
        {
            var objFromDB = _dbContext.CoverTypes.FirstOrDefault(s=>s.CTypeId == coverType.CTypeId);
            if (objFromDB != null)
            {
                objFromDB.Name = coverType.Name;
            }
        }
    }
}
