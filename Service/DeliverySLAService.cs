using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DeliverySLAService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<DeliverySLA> GetAll(string includeProperties)
        {
            return _contextUow.DeliverySLARepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

    }
}
