using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.RA;
using Domain;

namespace Web.Models
{
    public class RemoteAccessTeamsVM
    {

        public IEnumerable<Team> Teams { get; set; }
        public long SelectedTeamId { get; set; }
        public IEnumerable<RemoteAccessRequest> Requests
        {
            get; set;

        }
    }
}