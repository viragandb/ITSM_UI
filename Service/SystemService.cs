using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SystemService
    {


        public static String GetRequestStatusColor(RequestStatusEnum status)
        {
            var color = "default";
            if (status == RequestStatusEnum.Pending)
                color = "primary";
            else if (status == RequestStatusEnum.Approved )
                color = "success";
            else if (status == RequestStatusEnum.Rejected)
                color = "danger";
            //else if (status == RequestStatusEnum.Analysed)
            //    color = "warning";
            return color;
        }


        public static String GetPriorityColor(PriorityEnum status)
        {
            var color = "default";
            if (status == PriorityEnum.VeryLow || status == PriorityEnum.Low)
                color = "info";
            else if (status == PriorityEnum.Medium)
                color = "success";
            else if (status == PriorityEnum.High)
                color = "warning";
            else if (status == PriorityEnum.VeryHigh)
                color = "danger";
            return color;
        }


        public static IEnumerable<Category> GetCategories()
        {
            CategoryService _catService = new CategoryService();
            return _catService.GetAll("");

        }

        public static IEnumerable<Team> GetTeams()
        {
            TeamService _service = new TeamService();
            return _service.GetAllAsNoTracking("");

        }
    }
}
