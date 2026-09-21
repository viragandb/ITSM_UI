using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class GroupService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public bool InsertGroup(Group group)
        {

            try
            {
                _contextUow.GroupRepository.Insert(group);
                _contextUow.Save();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public bool UpdateGroup(Group group)
        {
          

            try
            {
                _contextUow.GroupRepository.Update(group);
                _contextUow.Save();
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public bool DeleteGroup(Group group)
        {
            try
            {
                group.IsDeleted = true;
                return UpdateGroup(group);
            }
            catch (Exception exception)
            {
                return false;
            }
        }

        public IEnumerable<Group> GetGroups(string includeProperties)
        {
     

            var groups = _contextUow.GroupRepository.GetAsNoTracking(e => e.IsDeleted == false,
                includeProperties: includeProperties);
            return groups;
        }

        public Group GetGroup(long? id, string includeProperties)
        {
     

            var group = _contextUow.GroupRepository.Get(e => e.GroupId == id && e.IsDeleted == false,
                includeProperties: includeProperties).FirstOrDefault();
            return group;
        }

        public bool IsExistingGroup(string groupName)
        {
     
            return
                _contextUow.GroupRepository.Get(e => e.IsDeleted == false && e.GroupName.Equals(groupName))
                    .Any();
        }

        public bool IsExistingGroup(long accountId, long groupId, string groupName)
        {
      

            return
                _contextUow.GroupRepository.Get(e => e.IsDeleted == false && e.GroupId != groupId && e.GroupName.Equals(groupName))
                    .Any();
        }


    }
}
