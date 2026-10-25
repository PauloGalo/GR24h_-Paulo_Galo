using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GR24h_Repos
{
    internal interface IRepository <TEntity>
    {
        TEntity Create(TEntity entity);
        TEntity? Retrieve(int id);
        List<TEntity> RetrieveAll();
        void Update(TEntity entity);
        void Delete(int id);
    }
}
